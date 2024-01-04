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
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Page {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.CssNestedAtRule"/>
    /// implementation for page rules.
    /// </summary>
    public class CssPageRule : CssNestedAtRule {
        /// <summary>The page selectors.</summary>
        private IList<ICssSelector> pageSelectors;

        /// <summary>
        /// Creates a new
        /// <see cref="CssPageRule"/>
        /// instance.
        /// </summary>
        /// <param name="ruleParameters">the rule parameters</param>
        public CssPageRule(String ruleParameters)
            : base(CssRuleName.PAGE, ruleParameters) {
            pageSelectors = new List<ICssSelector>();
            String[] selectors = iText.Commons.Utils.StringUtil.Split(ruleParameters, ",");
            for (int i = 0; i < selectors.Length; i++) {
                selectors[i] = CssUtils.RemoveDoubleSpacesAndTrim(selectors[i]);
            }
            foreach (String currentSelectorStr in selectors) {
                pageSelectors.Add(new CssPageSelector(currentSelectorStr));
            }
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssNestedAtRule#addBodyCssDeclarations(java.util.List)
        */
        public override void AddBodyCssDeclarations(IList<CssDeclaration> cssDeclarations) {
            // TODO DEVSIX-6364 Fix the body declarations duplication for each pageSelector part
            //      Due to this for-loop, on toString method call for the CssPageRule instance
            //      all the body declarations will be duplicated for each pageSelector part.
            //      This potentially could lead to a nasty behaviour when declarations will double
            //      for each read-write iteration of the same css-file (however, this use case seems
            //      to be unlikely to happen).
            //      Possible solution would be to split single page rule with compound selector into
            //      several page rules with simple selectors on addition of the page rule to it's parent.
            //
            //      Also, the same concerns this method implementation in CssMarginRule class.
            //
            //      See CssStyleSheetParserTest#test11 test.
            foreach (ICssSelector pageSelector in pageSelectors) {
                this.body.Add(new CssNonStandardRuleSet(pageSelector, cssDeclarations));
            }
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssNestedAtRule#addStatementToBody(com.itextpdf.styledxmlparser.css.CssStatement)
        */
        public override void AddStatementToBody(CssStatement statement) {
            if (statement is CssMarginRule) {
                ((CssMarginRule)statement).SetPageSelectors(pageSelectors);
            }
            this.body.Add(statement);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssNestedAtRule#addStatementsToBody(java.util.Collection)
        */
        public override void AddStatementsToBody(ICollection<CssStatement> statements) {
            foreach (CssStatement statement in statements) {
                AddStatementToBody(statement);
            }
        }
    }
}
