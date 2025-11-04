using TreeSitter;
using TreeSitter.JavaScript;

namespace RAG_Code_Base.Services.Parsers
{
    public class JavaScriptTreeSitterParser : BaseTreeSitterParser
    {
        protected override Language GetLanguage()
        {
            return JavaScriptLanguage.Create();
        }

        protected override string[] GetFunctionNodeTypes()
        {
            return new[] { "function_declaration", "arrow_function", "function" };
        }

        protected override string[] GetClassNodeTypes()
        {
            return new[] { "class_declaration" };
        }

        protected override string[] GetInterfaceNodeTypes()
        {
            return Array.Empty<string>();
        }

        protected override string[] GetEnumNodeTypes()
        {
            return Array.Empty<string>();
        }

        protected override string? ExtractClassName(Node node)
        {
            var nameNode = node.ChildByFieldName("name");
            if (nameNode != null)
            {
                return nameNode.ToString();
            }
            return null;
        }

        protected override string? ExtractFunctionName(Node node)
        {
            var nameNode = node.ChildByFieldName("name");
            if (nameNode != null)
            {
                return nameNode.ToString();
            }

            return $"<{node.Kind}_line_{node.StartPosition.Row + 1}>";
        }
    }
}
