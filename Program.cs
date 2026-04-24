using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using RAG_Code_Base.Database;
using RAG_Code_Base.Services.DataLoader;
using RAG_Code_Base.Services.Explanation;
using RAG_Code_Base.Services.Parsers;
using RAG_Code_Base.Services.Parsers.TreeSitterParsers;
using RAG_Code_Base.Services.Vectorization;
using RAG_Code_Base.Services.VectorStorage;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(Hangfire.CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();


builder.Services.AddScoped<FileLoaderService>();
builder.Services.AddSingleton(sp =>
{
    var vectorizationService = sp.GetRequiredService<VectorizationService>();
    var vectorStorageService = sp.GetRequiredService<VectorStorageService>();
    var logger = sp.GetRequiredService<ILogger<ExplanationService>>();
    return new ExplanationService(vectorizationService, vectorStorageService, logger);
});

// Add parsers
builder.Services.AddScoped<TextFileParser>();
builder.Services.AddScoped<MarkdownParser>();
builder.Services.AddScoped<CSharpParser>();
builder.Services.AddScoped<PythonTreeSitterParser>();
builder.Services.AddScoped<JavaScriptTreeSitterParser>();
builder.Services.AddScoped<TypeScriptTreeSitterParser>();
builder.Services.AddScoped<JavaTreeSitterParser>();
builder.Services.AddScoped<CppTreeSitterParser>();
builder.Services.AddScoped<CTreeSitterParser>();
builder.Services.AddScoped<RustTreeSitterParser>();
builder.Services.AddScoped<PHPTreeSitterParser>();
builder.Services.AddScoped<HTMLTreeSitterParser>();
builder.Services.AddScoped<CSSTreeSitterParser>();
builder.Services.AddScoped<PdfParser>();
builder.Services.AddScoped<DocxParser>();



// Add services to the container.
builder.Services.AddScoped<ParserFactory>();

//именно так и никак иначе
builder.Services.AddSingleton<VectorStorageService>();


builder.Services.AddScoped<FileValidator>();


builder.Services.AddSingleton<VectorizationService>();

builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});



builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5275";
    client.BaseAddress = new Uri(baseUrl);
    return client;
});





builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql")
    .AddUrlGroup(
    new Uri(builder.Configuration["HealthChecks:QdrantUrl"]
        ?? "http://localhost:6333/health"),
    name: "qdrant");


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Применение миграций базы данных...");
        await context.Database.MigrateAsync();  // ← ЭТО СОЗДАСТ ТАБЛИЦЫ!
        logger.LogInformation("Миграции успешно применены");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка при применении миграций");
    }

    var vectorStorage = services.GetRequiredService<VectorStorageService>();
}


app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = Array.Empty<IDashboardAuthorizationFilter>()  // ← Разрешить все запросы
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Используем статические файлы (для Blazor)
app.UseStaticFiles();


app.UseRouting();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds.ToString("F0") + "ms"
            })
        }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        await context.Response.WriteAsync(result);
    }
});
app.UseCors("AllowBlazor");

app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
