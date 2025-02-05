/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Layout.Font;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css {
    /// <summary>Class to store a CSS font face At rule.</summary>
    public class CssFontFaceRule : CssNestedAtRule {
        /// <summary>Properties in the form of a list of CSS declarations.</summary>
        private IList<CssDeclaration> properties;

        /// <summary>Instantiates a new CSS font face rule.</summary>
        public CssFontFaceRule()
            : base(CssRuleName.FONT_FACE, "") {
        }

        /// <summary>Gets the properties.</summary>
        /// <returns>the properties</returns>
        public virtual IList<CssDeclaration> GetProperties() {
            if (properties == null) {
                return new List<CssDeclaration>();
            }
            return new List<CssDeclaration>(properties);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssNestedAtRule#addBodyCssDeclarations(java.util.List)
        */
        public override void AddBodyCssDeclarations(IList<CssDeclaration> cssDeclarations) {
            properties = new List<CssDeclaration>(cssDeclarations);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssNestedAtRule#toString()
        */
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("@").Append(GetRuleName()).Append(" {").Append("\n");
            foreach (CssDeclaration declaration in GetProperties()) {
                sb.Append("    ");
                sb.Append(declaration);
                sb.Append(";\n");
            }
            sb.Append("}");
            return sb.ToString();
        }

        public virtual Range ResolveUnicodeRange() {
            Range range = null;
            foreach (CssDeclaration descriptor in GetProperties()) {
                if ("unicode-range".Equals(descriptor.GetProperty())) {
                    range = CssUtils.ParseUnicodeRange(descriptor.GetExpression());
                }
            }
            return range;
        }
    }
}
