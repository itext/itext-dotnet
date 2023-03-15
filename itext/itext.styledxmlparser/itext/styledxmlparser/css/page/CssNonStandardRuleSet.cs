/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Selector;

namespace iText.StyledXmlParser.Css.Page {
    /// <summary>
    /// Class for a non standard
    /// <see cref="iText.StyledXmlParser.Css.CssRuleSet"/>.
    /// </summary>
    internal class CssNonStandardRuleSet : CssRuleSet {
        /// <summary>
        /// Creates a new
        /// <see cref="CssNonStandardRuleSet"/>
        /// instance.
        /// </summary>
        /// <param name="selector">the selector</param>
        /// <param name="declarations">the declarations</param>
        public CssNonStandardRuleSet(ICssSelector selector, IList<CssDeclaration> declarations)
            : base(selector, declarations) {
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssRuleSet#toString()
        */
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < GetNormalDeclarations().Count; i++) {
                if (i > 0) {
                    sb.Append(";").Append("\n");
                }
                CssDeclaration declaration = GetNormalDeclarations()[i];
                sb.Append(declaration.ToString());
            }
            for (int i = 0; i < GetImportantDeclarations().Count; i++) {
                if (i > 0 || GetNormalDeclarations().Count > 0) {
                    sb.Append(";").Append("\n");
                }
                CssDeclaration declaration = GetImportantDeclarations()[i];
                sb.Append(declaration.ToString()).Append(" !important");
            }
            return sb.ToString();
        }
    }
}
