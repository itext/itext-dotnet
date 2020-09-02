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
        private readonly String backgroundProperty;

        /// <summary>
        /// Creates a new
        /// <see cref="CssNumericValueValidator"/>
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
            if (CommonCssConstants.INITIAL.Equals(objectString) || CommonCssConstants.INHERIT.Equals(objectString) || 
                CommonCssConstants.UNSET.Equals(objectString)) {
                return true;
            }
            // Actually it's not shorthand but extractShorthandProperties method works exactly as needed in this case
            IList<IList<String>> extractedProperties = CssUtils.ExtractShorthandProperties(objectString);
            foreach (IList<String> propertyValues in extractedProperties) {
                if (propertyValues.IsEmpty()) {
                    return false;
                }
                foreach (String propertyValue in propertyValues) {
                    if (!IsValidProperty(propertyValue, propertyValues)) {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValidProperty(String propertyValue, IList<String> propertyValues) {
            if (IsPropertyValueCorrespondsPropertyType(propertyValue)) {
                if (propertyValues.Count > 1) {
                    if (IsMultiValueAllowedForThisType() && IsMultiValueAllowedForThisValue(propertyValue)) {
                        // TODO DEVSIX-2106 Some extra validations for currently not supported properties.
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsMultiValueAllowedForThisType() {
            return !CommonCssConstants.BACKGROUND_ORIGIN.Equals(backgroundProperty) && !CommonCssConstants.BACKGROUND_CLIP
                .Equals(backgroundProperty) && !CommonCssConstants.BACKGROUND_IMAGE.Equals(backgroundProperty) && !CommonCssConstants
                .BACKGROUND_ATTACHMENT.Equals(backgroundProperty);
        }

        private static bool IsMultiValueAllowedForThisValue(String value) {
            return !CommonCssConstants.REPEAT_X.Equals(value) && !CommonCssConstants.REPEAT_Y.Equals(value) && !CommonCssConstants
                .COVER.Equals(value) && !CommonCssConstants.CONTAIN.Equals(value);
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
            if (propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN_OR_CLIP && (CommonCssConstants
                .BACKGROUND_CLIP.Equals(backgroundProperty) || CommonCssConstants.BACKGROUND_ORIGIN.Equals(backgroundProperty
                ))) {
                return true;
            }
            return propertyType == CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_OR_SIZE && (CommonCssConstants
                .BACKGROUND_POSITION.Equals(backgroundProperty) || CommonCssConstants.BACKGROUND_SIZE.Equals(backgroundProperty
                ));
        }
    }
}
