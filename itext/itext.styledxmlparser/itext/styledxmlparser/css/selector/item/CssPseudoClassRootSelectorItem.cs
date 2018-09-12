using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassRootSelectorItem : CssPseudoClassSelectorItem {
        private static readonly iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassRootSelectorItem instance = 
            new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassRootSelectorItem();

        private CssPseudoClassRootSelectorItem()
            : base(CommonCssConstants.ROOT) {
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassRootSelectorItem GetInstance() {
            return instance;
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            return node.ParentNode() is IDocumentNode;
        }
    }
}
