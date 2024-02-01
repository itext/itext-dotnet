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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate.Impl;

namespace iText.StyledXmlParser.Css.Validate {
    /// <summary>Class that holds CSS declaration validator.</summary>
    public class CssDeclarationValidationMaster {
        /// <summary>A validator containing all the CSS declaration validators.</summary>
        private static ICssDeclarationValidator VALIDATOR = new CssDefaultValidator();

        /// <summary>
        /// Creates a new
        /// <c>CssDeclarationValidationMaster</c>
        /// instance.
        /// </summary>
        private CssDeclarationValidationMaster() {
        }

        /// <summary>Checks a CSS declaration.</summary>
        /// <param name="declaration">the CSS declaration</param>
        /// <returns>true, if the validation was successful</returns>
        public static bool CheckDeclaration(CssDeclaration declaration) {
            return VALIDATOR.IsValid(declaration);
        }

        /// <summary>Sets new validator for CSS declarations.</summary>
        /// <param name="validator">
        /// validator for CSS declarations:
        /// use
        /// <see cref="iText.StyledXmlParser.Css.Validate.Impl.CssDefaultValidator"/>
        /// instance to
        /// use default validation,
        /// use
        /// <see cref="iText.StyledXmlParser.Css.Validate.Impl.CssDeviceCmykAwareValidator"/>
        /// instance to support device-cmyk feature
        /// </param>
        public static void SetValidator(ICssDeclarationValidator validator) {
            VALIDATOR = validator;
        }
    }
}
