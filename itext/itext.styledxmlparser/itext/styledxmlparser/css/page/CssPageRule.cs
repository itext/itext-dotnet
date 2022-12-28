/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
