using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassFirstChildSelectorItem : CssPseudoClassChildSelectorItem {
        private static readonly iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassFirstChildSelectorItem instance
             = new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassFirstChildSelectorItem();

        private CssPseudoClassFirstChildSelectorItem()
            : base(CommonCssConstants.FIRST_CHILD) {
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassFirstChildSelectorItem GetInstance() {
            return instance;
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            IList<INode> children = GetAllSiblings(node);
            return !children.IsEmpty() && node.Equals(children[0]);
        }
    }
}
