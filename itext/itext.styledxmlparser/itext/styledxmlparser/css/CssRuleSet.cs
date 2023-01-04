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
