using Microsoft.EntityFrameworkCore;
using RAG_Code_Base.Database;
using RAG_Code_Base.Services.DataLoader;
using RAG_Code_Base.Services.Parsers;
using RAG_Code_Base.Services.Vectorization;
using RAG_Code_Base.Services.VectorStorage;
using Hangfire;
using Hangfire.PostgreSql;

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

// Add parsers
builder.Services.AddScoped<TextFileParser>();
builder.Services.AddScoped<MarkdownParser>();
builder.Services.AddScoped<CSharpParser>();
builder.Services.AddScoped<PythonTreeSitterParser>();
builder.Services.AddScoped<JavaScriptTreeSitterParser>();

// Add services to the container.
builder.Services.AddScoped<ParserFactory>();
builder.Services.AddScoped<VectorStorageService>();
builder.Services.AddSingleton<FileValidator>();
builder.Services.AddSingleton<VectorizationService>();

builder.Services.AddRazorPages();


builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
});


builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    client.BaseAddress = new Uri("http://localhost:5275");
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




var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

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
app.UseCors("AllowBlazor");

app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
