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
