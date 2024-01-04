/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.StyledXmlParser.Css.Selector;

namespace iText.StyledXmlParser.Css.Page {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.CssNestedAtRule"/>
    /// implementation for margins.
    /// </summary>
    public class CssMarginRule : CssNestedAtRule {
        /// <summary>The page selectors.</summary>
        private IList<ICssSelector> pageSelectors;

        /// <summary>
        /// Creates a new
        /// <see cref="CssMarginRule"/>
        /// instance.
        /// </summary>
        /// <param name="ruleName">the rule name</param>
        public CssMarginRule(String ruleName)
            : base(ruleName, "") {
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssNestedAtRule#addBodyCssDeclarations(java.util.List)
        */
        public override void AddBodyCssDeclarations(IList<CssDeclaration> cssDeclarations) {
            // TODO DEVSIX-6364 Fix the body declarations duplication for each pageSelector part
            foreach (ICssSelector pageSelector in pageSelectors) {
                this.body.Add(new CssNonStandardRuleSet(new CssPageMarginBoxSelector(GetRuleName(), pageSelector), cssDeclarations
                    ));
            }
        }

        /// <summary>Sets the page selectors.</summary>
        /// <param name="pageSelectors">the new page selectors</param>
        internal virtual void SetPageSelectors(IList<ICssSelector> pageSelectors) {
            this.pageSelectors = new List<ICssSelector>(pageSelectors);
        }
    }
}
