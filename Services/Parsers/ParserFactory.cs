namespace RAG_Code_Base.Services.Parsers
{
    public class ParserFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ParserFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFileParser? GetParser(string fileType)
        {
            return fileType switch
            {
                "Text" => _serviceProvider.GetService<TextFileParser>(),
                "Markdown" => _serviceProvider.GetService<MarkdownParser>(),
                "CSharp" => _serviceProvider.GetService<CSharpParser>(),
                "Python" => _serviceProvider.GetService<PythonTreeSitterParser>(),
                "JavaScript" => _serviceProvider.GetService<JavaScriptTreeSitterParser>(),
                _ => null
            };
        }
    }
}
