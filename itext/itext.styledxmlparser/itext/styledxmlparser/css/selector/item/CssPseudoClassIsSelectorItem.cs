using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Selector;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    internal class CssPseudoClassIsSelectorItem : CssPseudoClassForgivingSelectorListSelectorItem {
//\cond DO_NOT_DOCUMENT
        internal CssPseudoClassIsSelectorItem(IList<ICssSelector> selectorList, String argumentsString)
            : base(CommonCssConstants.IS, selectorList, argumentsString) {
        }
//\endcond

        public override int GetSpecificity() {
            int max = 0;
            foreach (ICssSelector sel in selectorList) {
                if (sel != null) {
                    max = Math.Max(max, sel.CalculateSpecificity());
                }
            }
            return max;
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassIsSelectorItem CreateIsSelectorItem(String
             arguments) {
            IList<ICssSelector> selectors = ParseSelectorListWithoutPseudoElements(arguments, true);
            if (selectors == null) {
                return null;
            }
            return new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassIsSelectorItem(selectors, arguments);
        }
    }
//\endcond
}
