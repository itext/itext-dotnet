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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Datatype {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDataTypeValidator"/>
    /// implementation for background properties.
    /// </summary>
    /// <remarks>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDataTypeValidator"/>
    /// implementation for background properties.
    /// This validator should not be used with non-background properties.
    /// </remarks>
    public class CssBackgroundValidator : ICssDataTypeValidator {
        private const int MAX_AMOUNT_OF_VALUES = 2;

        private readonly String backgroundProperty;

        /// <summary>
        /// Creates a new
        /// <see cref="CssBackgroundValidator"/>
        /// instance.
        /// </summary>
        /// <param name="backgroundProperty">is background property corresponding to current validator</param>
        public CssBackgroundValidator(String backgroundProperty) {
            this.backgroundProperty = backgroundProperty;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsValid(String objectString) {
            if (objectString == null) {
                return false;
            }
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(objectString)) {
                return true;
            }
            // Actually it's not shorthand but extractShorthandProperties method works exactly as needed in this case
            IList<IList<String>> extractedProperties = CssUtils.ExtractShorthandProperties(objectString);
            foreach (IList<String> propertyValues in extractedProperties) {
                if (propertyValues.IsEmpty() || propertyValues.Count > MAX_AMOUNT_OF_VALUES) {
                    return false;
                }
                for (int i = 0; i < propertyValues.Count; i++) {
                    if (!IsValidProperty(propertyValues, i)) {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValidProperty(IList<String> propertyValues, int index) {
            if (IsPropertyValueCorrespondsPropertyType(propertyValues[index])) {
                if (propertyValues.Count == MAX_AMOUNT_OF_VALUES) {
                    if (IsMultiValueAllowedForThisType() && IsMultiValueAllowedForThisValue(propertyValues[index])) {
                        return CheckMultiValuePositionXY(propertyValues, index);
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool CheckMultiValuePositionXY(IList<String> propertyValues, int index) {
            if (CommonCssConstants.BACKGROUND_POSITION_X.Equals(backgroundProperty) || CommonCssConstants.BACKGROUND_POSITION_Y
                .Equals(backgroundProperty)) {
                if (CommonCssConstants.BACKGROUND_POSITION_VALUES.Contains(propertyValues[index]) && index == 1) {
                    return false;
                }
                return CommonCssConstants.BACKGROUND_POSITION_VALUES.Contains(propertyValues[index]) || index == 1;
            }
            return true;
        }

        private bool IsMultiValueAllowedForThisType() {
            return !CommonCssConstants.BACKGROUND_ORIGIN.Equals(backgroundProperty) && !CommonCssConstants.BACKGROUND_CLIP
                .Equals(backgroundProperty) && !CommonCssConstants.BACKGROUND_IMAGE.Equals(backgroundProperty) && !CommonCssConstants
                .BACKGROUND_ATTACHMENT.Equals(backgroundProperty);
        }

        private static bool IsMultiValueAllowedForThisValue(String value) {
            return !CommonCssConstants.REPEAT_X.Equals(value) && !CommonCssConstants.REPEAT_Y.Equals(value) && !CommonCssConstants
                .COVER.Equals(value) && !CommonCssConstants.CONTAIN.Equals(value) && !CommonCssConstants.CENTER.Equals
                (value);
        }

        private bool IsPropertyValueCorrespondsPropertyType(String value) {
            CssBackgroundUtils.BackgroundPropertyType propertyType = CssBackgroundUtils.ResolveBackgroundPropertyType(
                value);
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.UNDEFINED) {
                return false;
            }
            if (CssBackgroundUtils.GetBackgroundPropertyNameFromType(propertyType).Equals(backgroundProperty)) {
                return true;
            }
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION && (CommonCssConstants.BACKGROUND_POSITION_X
                .Equals(backgroundProperty) || CommonCssConstants.BACKGROUND_POSITION_Y.Equals(backgroundProperty))) {
                return true;
            }
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN_OR_CLIP && (CommonCssConstants
                .BACKGROUND_CLIP.Equals(backgroundProperty) || CommonCssConstants.BACKGROUND_ORIGIN.Equals(backgroundProperty
                ))) {
                return true;
            }
            return propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_OR_SIZE && (CommonCssConstants
                .BACKGROUND_POSITION_X.Equals(backgroundProperty) || CommonCssConstants.BACKGROUND_POSITION_Y.Equals(backgroundProperty
                ) || CommonCssConstants.BACKGROUND_SIZE.Equals(backgroundProperty));
        }
    }
}
