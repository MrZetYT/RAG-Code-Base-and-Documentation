using RAG_Code_Base.Services.Parsers.TreeSitterParsers;

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
                "TypeScript" => _serviceProvider.GetService<TypeScriptTreeSitterParser>(),
                "Java"=> _serviceProvider.GetService<JavaTreeSitterParser>(),
                "Go" => _serviceProvider.GetService<GoTreeSitterParser>(),
                "C++" => _serviceProvider.GetService<CppTreeSitterParser>(),
                //TODO: Протестить парсеры после Python
                //TODO: Допилить парсеры на  C, Rust, PHP, Swift
                _ => null
            };
        }
    }
}
