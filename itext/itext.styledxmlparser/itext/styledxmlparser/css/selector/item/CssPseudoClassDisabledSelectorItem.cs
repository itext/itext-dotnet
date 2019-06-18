using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassDisabledSelectorItem : CssPseudoClassSelectorItem {
        private static readonly iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassDisabledSelectorItem instance
             = new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassDisabledSelectorItem();

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassDisabledSelectorItem GetInstance() {
            return instance;
        }

        private CssPseudoClassDisabledSelectorItem()
            : base(CommonCssConstants.DISABLED) {
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            return null != ((IElementNode)node).GetAttribute(CommonCssConstants.DISABLED);
        }
    }
}
