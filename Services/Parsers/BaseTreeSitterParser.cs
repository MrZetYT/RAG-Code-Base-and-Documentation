using RAG_Code_Base.Models;
using TreeSitter;

namespace RAG_Code_Base.Services.Parsers
{
    public abstract class BaseTreeSitterParser : IFileParser
    {
        protected abstract Language GetLanguage();
        protected abstract string[] GetFunctionNodeTypes();
        protected abstract string[] GetClassNodeTypes();
        protected abstract string[] GetInterfaceNodeTypes();
        protected abstract string[] GetEnumNodeTypes();

        protected abstract string? ExtractFunctionName(Node node);
        protected abstract string? ExtractClassName(Node node);

        public List<InfoBlock> Parse(FileItem fileItem)
        {
            string code = File.ReadAllText(fileItem.FilePath);

            using var parser = new Parser();
            parser.Language = GetLanguage();

            using var tree = parser.Parse(code);

            var blocks = new List<InfoBlock>();
            CollectNodes(tree.Root, fileItem.Id, blocks);

            return blocks;
        }

        private void CollectNodes(Node node, Guid fileItemId, List<InfoBlock> blocks)
        {
            string nodeKind = node.Kind;
            if (GetFunctionNodeTypes().Contains(nodeKind))
            {
                blocks.Add(CreateFunctionBlock(node, fileItemId));
            }
            else if (GetClassNodeTypes().Contains(nodeKind))
            {
                blocks.Add(CreateClassBlock(node, fileItemId));
            }
            else if (GetInterfaceNodeTypes().Contains(nodeKind))
            {
                blocks.Add(CreateInterfaceBlock(node, fileItemId));
            }
            else if (GetEnumNodeTypes().Contains(nodeKind))
            {
                blocks.Add(CreateEnumBlock(node, fileItemId));
            }

            foreach(var child in node.NamedChildren)
            {
                CollectNodes(child, fileItemId, blocks);
            }
        }

        private string GetNodeText(Node node)
        {
            return node.ToString();
        }

        private InfoBlock CreateFunctionBlock(Node node, Guid fileItemId)
        {
            return new InfoBlock()
            {
                FileItemId = fileItemId,
                Content = node.ToString(),
                BlockType = "Method",
                MethodName = ExtractFunctionName(node),
                ClassName = FindParentClassName(node),
                StartLine = node.StartPosition.Row + 1,
                EndLine = node.EndPosition.Row + 1,
                CreatedAt = DateTime.UtcNow
            };
        }
        private InfoBlock CreateClassBlock(Node node, Guid fileItemId)
        {
            return new InfoBlock
            {
                FileItemId = fileItemId,
                Content = node.ToString(),
                BlockType = "Class",
                ClassName = ExtractClassName(node),
                StartLine = node.StartPosition.Row + 1,
                EndLine = node.EndPosition.Row + 1,
                CreatedAt = DateTime.UtcNow
            };
        }

        private InfoBlock CreateInterfaceBlock(Node node, Guid fileItemId)
        {
            return new InfoBlock
            {
                FileItemId = fileItemId,
                Content = node.ToString(),
                BlockType = "Interface",
                ClassName = ExtractClassName(node),
                StartLine = node.StartPosition.Row + 1,
                EndLine = node.EndPosition.Row + 1,
                CreatedAt = DateTime.UtcNow
            };
        }

        private InfoBlock CreateEnumBlock(Node node, Guid fileItemId)
        {
            return new InfoBlock
            {
                FileItemId = fileItemId,
                Content = node.ToString(),
                BlockType = "Enum",
                ClassName = ExtractClassName(node),
                StartLine = node.StartPosition.Row + 1,
                EndLine = node.EndPosition.Row + 1,
                CreatedAt = DateTime.UtcNow
            };
        }

        private string? FindParentClassName(Node node)
        {
            var parent = node.Parent;

            while (parent != null)
            {
                if (GetClassNodeTypes().Contains(parent.Kind))
                {
                    return ExtractClassName(parent);
                }

                parent = parent.Parent;
            }

            return null;
        }
    }
}
