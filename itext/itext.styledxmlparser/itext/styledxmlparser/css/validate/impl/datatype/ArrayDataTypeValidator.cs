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
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Datatype {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDeclarationValidator"/>
    /// implementation to validate an array of specified element types.
    /// </summary>
    public class ArrayDataTypeValidator : ICssDataTypeValidator {
        /// <summary>The data type validator.</summary>
        private readonly ICssDataTypeValidator dataTypeValidator;

        /// <summary>
        /// Creates a new
        /// <see cref="ArrayDataTypeValidator"/>
        /// instance.
        /// </summary>
        /// <param name="dataTypeValidator">the data type validator for each array element</param>
        public ArrayDataTypeValidator(ICssDataTypeValidator dataTypeValidator) {
            this.dataTypeValidator = dataTypeValidator;
        }

        public virtual bool IsValid(String objectString) {
            if (objectString == null) {
                return false;
            }
            IList<String> values = CssUtils.SplitStringWithComma(objectString);
            foreach (String value in values) {
                if (!dataTypeValidator.IsValid(value.Trim())) {
                    return false;
                }
            }
            return true;
        }
    }
}
