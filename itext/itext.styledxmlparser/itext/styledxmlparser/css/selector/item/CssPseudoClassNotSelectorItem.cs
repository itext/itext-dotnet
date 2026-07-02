/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    internal class CssPseudoClassNotSelectorItem : CssPseudoClassSelectorItem {
        protected internal readonly IList<ICssSelector> selectorList;

//\cond DO_NOT_DOCUMENT
        internal CssPseudoClassNotSelectorItem(IList<ICssSelector> selectorList, String argumentsString)
            : base(CommonCssConstants.NOT, argumentsString) {
            this.selectorList = selectorList;
        }
//\endcond

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassNotSelectorItem CreateNotSelectorItem(
            String arguments) {
            IList<ICssSelector> selectors = ParseSelectorListWithoutPseudoElements(arguments, false);
            if (selectors == null) {
                return null;
            }
            return new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassNotSelectorItem(selectors, arguments);
        }

        public override int GetSpecificity() {
            int max = 0;
            foreach (ICssSelector sel in selectorList) {
                if (sel != null) {
                    max = Math.Max(max, sel.CalculateSpecificity());
                }
            }
            return max;
        }

        public virtual IList<ICssSelectorItem> GetArgumentsSelector() {
            return CssSelectorParser.ParseSelectorItems(arguments);
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            foreach (ICssSelector selector in selectorList) {
                if (selector.Matches(node)) {
                    return false;
                }
            }
            return true;
        }
    }
//\endcond
}
