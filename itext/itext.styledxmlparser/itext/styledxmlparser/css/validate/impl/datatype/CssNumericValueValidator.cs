/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Datatype {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDataTypeValidator"/>
    /// implementation for numeric elements.
    /// </summary>
    public class CssNumericValueValidator : ICssDataTypeValidator {
        private readonly bool allowedPercent;

        private readonly bool allowedNormal;

        /// <summary>
        /// Creates a new
        /// <see cref="CssNumericValueValidator"/>
        /// instance.
        /// </summary>
        /// <param name="allowedPercent">is percent value allowed</param>
        /// <param name="allowedNormal">is 'normal' value allowed</param>
        public CssNumericValueValidator(bool allowedPercent, bool allowedNormal) {
            this.allowedPercent = allowedPercent;
            this.allowedNormal = allowedNormal;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsValid(String objectString) {
            if (objectString == null) {
                return false;
            }
            if (CommonCssConstants.INITIAL.Equals(objectString) || CommonCssConstants.INHERIT.Equals(objectString) || 
                CommonCssConstants.UNSET.Equals(objectString)) {
                return true;
            }
            if (CommonCssConstants.NORMAL.Equals(objectString)) {
                return this.allowedNormal;
            }
            if (!CssUtils.IsValidNumericValue(objectString)) {
                return false;
            }
            if (CssUtils.IsPercentageValue(objectString)) {
                return this.allowedPercent;
            }
            return true;
        }
    }
}
