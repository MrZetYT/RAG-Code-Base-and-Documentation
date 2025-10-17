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
                "Markdown" => _serviceProvider.GetService<TextFileParser>(),
                _ => null
            };
        }
    }
}
