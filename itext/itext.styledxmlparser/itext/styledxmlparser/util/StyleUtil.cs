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
using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Util {
    /// <summary>Utility class for resolving parent-inheritance of style and attribute declarations.</summary>
    public sealed class StyleUtil {
        private StyleUtil() {
        }

        /// <summary>List to store the properties whose value can depend on parent or element font-size</summary>
        private static readonly IList<String> fontSizeDependentPercentage = new List<String>(2);

        static StyleUtil() {
            fontSizeDependentPercentage.Add(CommonCssConstants.FONT_SIZE);
            fontSizeDependentPercentage.Add(CommonCssConstants.LINE_HEIGHT);
        }

        /// <summary>Merge parent CSS declarations.</summary>
        /// <param name="styles">the styles map</param>
        /// <param name="styleProperty">the CSS property</param>
        /// <param name="parentPropValue">the parent properties value</param>
        /// <param name="parentFontSizeString">is a font size of parent element</param>
        /// <param name="inheritanceRules">set of inheritance rules</param>
        /// <returns>a map of updated styles after merging parent and child style declarations</returns>
        public static IDictionary<String, String> MergeParentStyleDeclaration(IDictionary<String, String> styles, 
            String styleProperty, String parentPropValue, String parentFontSizeString, ICollection<IStyleInheritance
            > inheritanceRules) {
            String childPropValue = styles.Get(styleProperty);
            if ((childPropValue == null && CheckInheritance(styleProperty, inheritanceRules)) || CommonCssConstants.INHERIT
                .Equals(childPropValue)) {
                if (ValueIsOfMeasurement(parentPropValue, CommonCssConstants.EM) || ValueIsOfMeasurement(parentPropValue, 
                    CommonCssConstants.EX) || ValueIsOfMeasurement(parentPropValue, CommonCssConstants.PERCENTAGE) && fontSizeDependentPercentage
                    .Contains(styleProperty)) {
                    float absoluteParentFontSize = CssDimensionParsingUtils.ParseAbsoluteLength(parentFontSizeString);
                    // Format to 4 decimal places to prevent differences between Java and C#
                    styles.Put(styleProperty, DecimalFormatUtil.FormatNumber(CssDimensionParsingUtils.ParseRelativeValue(parentPropValue
                        , absoluteParentFontSize), "0.####") + CommonCssConstants.PT);
                }
                else {
                    styles.Put(styleProperty, parentPropValue);
                }
            }
            return styles;
        }

        /// <summary>Check all inheritance rule-sets to see if the passed property is inheritable</summary>
        /// <param name="styleProperty">property identifier to check</param>
        /// <param name="inheritanceRules">a set of inheritance rules</param>
        /// <returns>
        /// True if the property is inheritable by one of the rule-sets,
        /// false if it is not marked as inheritable in all rule-sets
        /// </returns>
        private static bool CheckInheritance(String styleProperty, ICollection<IStyleInheritance> inheritanceRules
            ) {
            foreach (IStyleInheritance inheritanceRule in inheritanceRules) {
                if (inheritanceRule.IsInheritable(styleProperty)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Check to see if the passed value is a measurement of the type based on the passed measurement symbol string
        ///     </summary>
        /// <param name="value">string containing value to check</param>
        /// <param name="measurement">measurement symbol (e.g. % for relative, px for pixels)</param>
        /// <returns>True if the value is numerical and ends with the measurement symbol, false otherwise</returns>
        private static bool ValueIsOfMeasurement(String value, String measurement) {
            if (value == null) {
                return false;
            }
            return value.EndsWith(measurement) && CssTypesValidationUtils.IsNumber(value.JSubstring(0, value.Length - 
                measurement.Length).Trim());
        }
    }
}
