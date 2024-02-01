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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Declaration {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDeclarationValidator"/>
    /// implementation in case multiple types have to be checked.
    /// </summary>
    public class MultiTypeDeclarationValidator : ICssDeclarationValidator {
        /// <summary>The allowed data types.</summary>
        private IList<ICssDataTypeValidator> allowedTypes;

        /// <summary>
        /// Creates a new
        /// <see cref="MultiTypeDeclarationValidator"/>
        /// instance.
        /// </summary>
        /// <param name="allowedTypes">the allowed types</param>
        public MultiTypeDeclarationValidator(params ICssDataTypeValidator[] allowedTypes) {
            this.allowedTypes = JavaUtil.ArraysAsList(allowedTypes);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.validate.ICssDeclarationValidator#isValid(com.itextpdf.styledxmlparser.css.CssDeclaration)
        */
        public virtual bool IsValid(CssDeclaration cssDeclaration) {
            foreach (ICssDataTypeValidator dTypeValidator in allowedTypes) {
                if (dTypeValidator.IsValid(cssDeclaration.GetExpression())) {
                    return true;
                }
            }
            return false;
        }
    }
}
