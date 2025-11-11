using RAG_Code_Base.Services.Parsers.TreeSitterParsers;
using TreeSitter;

namespace RAG_Code_Base.Services.Parsers.TreeSitterParsers
{
    public class GoTreeSitterParser : BaseTreeSitterParser
    {
        protected override string GetLanguageName()
        {
            return "Go";
        }

        protected override string[] GetFunctionNodeTypes()
        {
            return new[]
            {
                "function_declaration",
                "method_declaration"
            };
        }

        protected override string[] GetClassNodeTypes()
        {
            return new[] { "type_declaration" };
        }

        protected override string[] GetInterfaceNodeTypes()
        {
            return new[] { "interface_type" };
        }

        protected override string[] GetEnumNodeTypes()
        {
            return Array.Empty<string>();
        }

        protected override string? ExtractClassName(Node node)
        {
            var nameNode = node.GetChildForField("name");
            return nameNode?.Text;
        }

        protected override string? ExtractFunctionName(Node node)
        {
            var nameNode = node.GetChildForField("name");
            return nameNode?.Text;
        }
    }
}