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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css {
    /// <summary>Class to store a CSS declaration.</summary>
    public class CssDeclaration {
        /// <summary>The property.</summary>
        private String property;

        /// <summary>The expression.</summary>
        private String expression;

        /// <summary>Instantiates a new CSS declaration.</summary>
        /// <param name="property">the property</param>
        /// <param name="expression">the expression</param>
        public CssDeclaration(String property, String expression) {
            this.property = ResolveAlias(CssUtils.NormalizeCssProperty(property));
            this.expression = CssUtils.NormalizeCssProperty(expression);
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            return MessageFormatUtil.Format("{0}: {1}", property, expression);
        }

        /// <summary>Gets the property.</summary>
        /// <returns>the property</returns>
        public virtual String GetProperty() {
            return property;
        }

        /// <summary>Gets the expression.</summary>
        /// <returns>the expression</returns>
        public virtual String GetExpression() {
            return expression;
        }

        /// <summary>Sets the expression.</summary>
        /// <param name="expression">the new expression</param>
        public virtual void SetExpression(String expression) {
            this.expression = expression;
        }

        /// <summary>Resolves css property aliases.</summary>
        /// <remarks>
        /// Resolves css property aliases.
        /// For example, word-wrap is an alias for overflow-wrap property.
        /// </remarks>
        /// <param name="normalizedCssProperty">css property to be resolved as alias</param>
        /// <returns>resolved property if the provided property was an alias, otherwise original provided property.</returns>
        internal virtual String ResolveAlias(String normalizedCssProperty) {
            if (CommonCssConstants.WORDWRAP.Equals(normalizedCssProperty)) {
                return CommonCssConstants.OVERFLOW_WRAP;
            }
            return normalizedCssProperty;
        }
    }
}
