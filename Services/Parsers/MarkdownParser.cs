using Markdig;
using Markdig.Syntax;
using RAG_Code_Base.Models;

namespace RAG_Code_Base.Services.Parsers
{
    public class MarkdownParser: IFileParser
    {
        public List<InfoBlock> Parse(FileItem fileItem)
        {
            string content = File.ReadAllText(fileItem.FilePath);

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var document = Markdown.Parse(content, pipeline);

            var blocks = new List<InfoBlock>();
            string currentHeader = null;

            foreach(var block in document)
            {
                var infoBlock = new InfoBlock
                {
                    FileItemId = fileItem.Id,
                    CreatedAt = DateTime.UtcNow,
                    StartLine = block.Line +1,
                    EndLine = CountLines(content,0,block.Span.End)
                };

                switch (block)
                {
                    case HeadingBlock heading:
                        infoBlock.BlockType = "Header";
                        infoBlock.Content = string.Concat(heading.Inline?.Select(x => x.ToString()));
                        currentHeader = infoBlock.Content;
                        break;

                    case ParagraphBlock paragraph:
                        infoBlock.BlockType = "Paragraph";
                        infoBlock.Content = string.Concat(paragraph.Inline?.Select(x => x.ToString()));
                        infoBlock.HeaderSection = currentHeader;
                        break;

                    case ListBlock list:
                        infoBlock.BlockType = "List";
                        var items = new List<string>();

                        foreach (ListItemBlock item in list)
                        {
                            foreach (var subBlock in item)
                            {
                                if (subBlock is ParagraphBlock paragraph)
                                {
                                    var text = string.Concat(paragraph.Inline?.Select(x => x.ToString()));
                                    items.Add(text);
                                }
                            }
                        }

                        infoBlock.Content = string.Join("\n", items);
                        infoBlock.HeaderSection = currentHeader;
                        break;

                    case CodeBlock code:
                        infoBlock.BlockType = "Code";
                        infoBlock.Content = string.Join("\n", code.Lines.Lines.Select(l=>l.ToString()).ToArray()).Trim();
                        infoBlock.HeaderSection = currentHeader;
                        break;

                    default:
                        continue;
                }

                blocks.Add(infoBlock);
            }

            return blocks;
        }

        private int CountLines(string content, int startIndex, int charPosition)
        {
            int lineCount = 1;
            for (int i = startIndex; i < charPosition && i < content.Length; i++)
            {
                if (content[i] == '\n') lineCount++;
            }
            return lineCount;
        }
    }
}
