using TreeSitter;
using TreeSitter.Python;

namespace RAG_Code_Base.Services.Parsers
{
    public class PythonTreeSitterParser : BaseTreeSitterParser
    {
        protected override Language GetLanguage()
        {
            return PythonLanguage.Create();
        }

        protected override string[] GetFunctionNodeTypes()
        {
            return new[] { "function_definition" };
        }

        protected override string[] GetClassNodeTypes()
        {
            return new[] { "class_definition" };
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
            return null;
        }
    }
}
