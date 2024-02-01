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
using System.Text;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css {
    /// <summary>Class to store a CSS rule set.</summary>
    public class CssRuleSet : CssStatement {
        /// <summary>Pattern to match "important" in a rule declaration.</summary>
        private static readonly Regex IMPORTANT_MATCHER = iText.Commons.Utils.StringUtil.RegexCompile(".*!\\s*important$"
            );

        /// <summary>The CSS selector.</summary>
        private ICssSelector selector;

        /// <summary>The normal CSS declarations.</summary>
        private IList<CssDeclaration> normalDeclarations;

        /// <summary>The important CSS declarations.</summary>
        private IList<CssDeclaration> importantDeclarations;

        /// <summary>
        /// Creates a new
        /// <see cref="CssRuleSet"/>
        /// from selector and raw list of declarations.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="CssRuleSet"/>
        /// from selector and raw list of declarations.
        /// The declarations are split into normal and important under the hood.
        /// To construct the
        /// <see cref="CssRuleSet"/>
        /// instance from normal and important declarations, see
        /// <see cref="CssRuleSet(iText.StyledXmlParser.Css.Selector.ICssSelector, System.Collections.Generic.IList{E}, System.Collections.Generic.IList{E})
        ///     "/>
        /// </remarks>
        /// <param name="selector">the CSS selector</param>
        /// <param name="declarations">the CSS declarations</param>
        public CssRuleSet(ICssSelector selector, IList<CssDeclaration> declarations) {
            this.selector = selector;
            this.normalDeclarations = new List<CssDeclaration>();
            this.importantDeclarations = new List<CssDeclaration>();
            SplitDeclarationsIntoNormalAndImportant(declarations, normalDeclarations, importantDeclarations);
        }

        public CssRuleSet(ICssSelector selector, IList<CssDeclaration> normalDeclarations, IList<CssDeclaration> importantDeclarations
            ) {
            this.selector = selector;
            this.normalDeclarations = normalDeclarations;
            this.importantDeclarations = importantDeclarations;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssStatement#getCssRuleSets(com.itextpdf.styledxmlparser.html.node.INode, com.itextpdf.styledxmlparser.css.media.MediaDeviceDescription)
        */
        public override IList<iText.StyledXmlParser.Css.CssRuleSet> GetCssRuleSets(INode element, MediaDeviceDescription
             deviceDescription) {
            if (selector.Matches(element)) {
                return JavaCollectionsUtil.SingletonList(this);
            }
            else {
                return base.GetCssRuleSets(element, deviceDescription);
            }
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(selector.ToString());
            sb.Append(" {\n");
            for (int i = 0; i < normalDeclarations.Count; i++) {
                if (i > 0) {
                    sb.Append(";").Append("\n");
                }
                CssDeclaration declaration = normalDeclarations[i];
                sb.Append("    ").Append(declaration.ToString());
            }
            for (int i = 0; i < importantDeclarations.Count; i++) {
                if (i > 0 || normalDeclarations.Count > 0) {
                    sb.Append(";").Append("\n");
                }
                CssDeclaration declaration = importantDeclarations[i];
                sb.Append("    ").Append(declaration.ToString()).Append(" !important");
            }
            sb.Append("\n}");
            return sb.ToString();
        }

        /// <summary>Gets the CSS selector.</summary>
        /// <returns>the CSS selector</returns>
        public virtual ICssSelector GetSelector() {
            return selector;
        }

        /// <summary>Gets the normal CSS declarations.</summary>
        /// <returns>the normal declarations</returns>
        public virtual IList<CssDeclaration> GetNormalDeclarations() {
            return normalDeclarations;
        }

        /// <summary>Gets the important CSS declarations.</summary>
        /// <returns>the important declarations</returns>
        public virtual IList<CssDeclaration> GetImportantDeclarations() {
            return importantDeclarations;
        }

        /// <summary>Split CSS declarations into normal and important CSS declarations.</summary>
        /// <param name="declarations">the declarations</param>
        private static void SplitDeclarationsIntoNormalAndImportant(IList<CssDeclaration> declarations, IList<CssDeclaration
            > normalDeclarations, IList<CssDeclaration> importantDeclarations) {
            foreach (CssDeclaration declaration in declarations) {
                int exclIndex = declaration.GetExpression().IndexOf('!');
                if (exclIndex > 0 && iText.Commons.Utils.Matcher.Match(IMPORTANT_MATCHER, declaration.GetExpression()).Matches
                    ()) {
                    importantDeclarations.Add(new CssDeclaration(declaration.GetProperty(), declaration.GetExpression().JSubstring
                        (0, exclIndex).Trim()));
                }
                else {
                    normalDeclarations.Add(declaration);
                }
            }
        }
    }
}
