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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Declaration {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDeclarationValidator"/>
    /// implementation to validate a single type.
    /// </summary>
    public class SingleTypeDeclarationValidator : ICssDeclarationValidator {
        /// <summary>The data type validator.</summary>
        private ICssDataTypeValidator dataTypeValidator;

        /// <summary>
        /// Creates a new
        /// <see cref="SingleTypeDeclarationValidator"/>
        /// instance.
        /// </summary>
        /// <param name="dataTypeValidator">the data type validator</param>
        public SingleTypeDeclarationValidator(ICssDataTypeValidator dataTypeValidator) {
            this.dataTypeValidator = dataTypeValidator;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.validate.ICssDeclarationValidator#isValid(com.itextpdf.styledxmlparser.css.CssDeclaration)
        */
        public virtual bool IsValid(CssDeclaration cssDeclaration) {
            return dataTypeValidator.IsValid(cssDeclaration.GetExpression());
        }
    }
}
