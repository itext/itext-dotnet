using System;
using iText.StyledXmlParser.Css;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassNthChildSelectorItem : CssPseudoClassNthSelectorItem {
        internal CssPseudoClassNthChildSelectorItem(String arguments)
            : base(CommonCssConstants.NTH_CHILD, arguments) {
        }
    }
}
