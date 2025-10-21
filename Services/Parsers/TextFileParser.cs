using RAG_Code_Base.Models;

namespace RAG_Code_Base.Services.Parsers
{
    public class TextFileParser: IFileParser
    {
        public List<InfoBlock> Parse(FileItem fileItem)
        {
            string content = File.ReadAllText(fileItem.FilePath);

            var paragraphs = content.Split(new[] { "\n\n", "\r\n\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            var blocks = new List<InfoBlock>();
            int currentLine = 1;
            foreach ( var block in paragraphs)
            {
                if(string.IsNullOrWhiteSpace(block)) continue;

                int lineCount = block.Split('\n').Length;

                blocks.Add(new InfoBlock
                {
                    FileItemId = fileItem.Id,
                    Content = block.Trim(),
                    BlockType = "Paragraph",
                    StartLine = currentLine,
                    EndLine = currentLine + lineCount - 1,
                    CreatedAt = DateTime.UtcNow
                });

                currentLine += lineCount + 1;
            }

            return blocks;
        }
    }
}
