using RAG_Code_Base.Models;

namespace RAG_Code_Base.Services.Parsers
{
    public interface IFileParser
    {
        public List<InfoBlock> Parse(FileItem fileItem);
    }
}
