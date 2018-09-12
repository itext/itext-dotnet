using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassNthOfTypeSelectorItem : CssPseudoClassNthSelectorItem {
        public CssPseudoClassNthOfTypeSelectorItem(String arguments)
            : base(CommonCssConstants.NTH_OF_TYPE, arguments) {
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            IList<INode> children = GetAllSiblingsOfNodeType(node);
            return !children.IsEmpty() && ResolveNth(node, children);
        }
    }
}
