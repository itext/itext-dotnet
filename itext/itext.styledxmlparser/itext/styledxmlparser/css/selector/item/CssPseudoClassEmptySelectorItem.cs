using System;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassEmptySelectorItem : CssPseudoClassSelectorItem {
        private static readonly iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassEmptySelectorItem instance = 
            new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassEmptySelectorItem();

        private CssPseudoClassEmptySelectorItem()
            : base(CommonCssConstants.EMPTY) {
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassEmptySelectorItem GetInstance() {
            return instance;
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            if (node.ChildNodes().IsEmpty()) {
                return true;
            }
            foreach (INode childNode in node.ChildNodes()) {
                if (!(childNode is ITextNode) || !String.IsNullOrEmpty(((ITextNode)childNode).WholeText())) {
                    return false;
                }
            }
            return true;
        }
    }
}
