using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Selector;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    internal class CssPseudoClassWhereSelectorItem : CssPseudoClassForgivingSelectorListSelectorItem {
//\cond DO_NOT_DOCUMENT
        internal CssPseudoClassWhereSelectorItem(IList<ICssSelector> selectorList, String argumentsString)
            : base(CommonCssConstants.WHERE, selectorList, argumentsString) {
        }
//\endcond

        public override int GetSpecificity() {
            // Per Selectors Level 4: :where() always contributes 0 specificity.
            return 0;
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassWhereSelectorItem CreateWhereSelectorItem
            (String arguments) {
            IList<ICssSelector> selectors = ParseForgivingSelectorListWithoutPseudoElements(arguments);
            if (selectors == null) {
                return null;
            }
            return new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassWhereSelectorItem(selectors, arguments);
        }
    }
//\endcond
}
