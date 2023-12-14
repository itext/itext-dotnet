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
using iText.Svg;

namespace iText.Svg.Css.Impl {
    [System.ObsoleteAttribute(@"use iText.StyledXmlParser.Util.StyleUtil instead. Utility class for resolving parent-inheritance of style and attribute declarations"
        )]
    public class StyleResolverUtil {
        /// <summary>List to store the properties whose value can depend on parent or element font-size</summary>
        private static readonly IList<String> fontSizeDependentPercentage = new List<String>(2);

        static StyleResolverUtil() {
            fontSizeDependentPercentage.Add(SvgConstants.Attributes.FONT_SIZE);
            fontSizeDependentPercentage.Add(CommonCssConstants.LINE_HEIGHT);
        }

        /// <summary>Merge parent style declarations for passed styleProperty into passed style map</summary>
        /// <param name="styles">the styles map</param>
        /// <param name="styleProperty">the style property</param>
        /// <param name="parentPropValue">the parent properties value</param>
        /// <param name="parentFontSizeString">the parent font-size for resolving relative, font-dependent attributes</param>
        public virtual void MergeParentStyleDeclaration(IDictionary<String, String> styles, String styleProperty, 
            String parentPropValue, String parentFontSizeString) {
            String childPropValue = styles.Get(styleProperty);
            if ((childPropValue == null && CheckInheritance(styleProperty)) || CommonCssConstants.INHERIT.Equals(childPropValue
                )) {
                if (ValueIsOfMeasurement(parentPropValue, CommonCssConstants.EM) || ValueIsOfMeasurement(parentPropValue, 
                    CommonCssConstants.EX) || (ValueIsOfMeasurement(parentPropValue, CommonCssConstants.PERCENTAGE) && fontSizeDependentPercentage
                    .Contains(styleProperty))) {
                    float absoluteParentFontSize = CssDimensionParsingUtils.ParseAbsoluteLength(parentFontSizeString);
                    // Format to 4 decimal places to prevent differences between Java and C#
                    styles.Put(styleProperty, DecimalFormatUtil.FormatNumber(CssDimensionParsingUtils.ParseRelativeValue(parentPropValue
                        , absoluteParentFontSize), "0.####") + CommonCssConstants.PT);
                }
                else {
                    //Property is inherited, add to element style declarations
                    styles.Put(styleProperty, parentPropValue);
                }
            }
            else {
                if ((CommonCssConstants.TEXT_DECORATION_LINE.Equals(styleProperty) || CommonCssConstants.TEXT_DECORATION.Equals
                    (styleProperty)) && !CommonCssConstants.INLINE_BLOCK.Equals(styles.Get(CommonCssConstants.DISPLAY))) {
                    // Note! This property is formally not inherited, but the browsers behave very similar to inheritance here.
                    // Text decorations on inline boxes are drawn across the entire element,
                    // going across any descendant elements without paying any attention to their presence.
                    // Also, when, for example, parent element has text-decoration:underline, and the child text-decoration:overline,
                    // then the text in the child will be both overline and underline. This is why the declarations are merged
                    // See TextDecorationTest#textDecoration01Test
                    styles.Put(styleProperty, CssPropertyMerger.MergeTextDecoration(childPropValue, parentPropValue));
                }
            }
        }

        /// <summary>Check all inheritance rule-sets to see if the passed property is inheritable</summary>
        /// <param name="styleProperty">property identifier to check</param>
        /// <returns>
        /// True if the property is inheritable by one of the rule-sets,
        /// false if it is not marked as inheritable in all rule-sets
        /// </returns>
        private bool CheckInheritance(String styleProperty) {
            foreach (IStyleInheritance inheritanceRule in SvgStyleResolver.INHERITANCE_RULES) {
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
            if (value.EndsWith(measurement) && CssTypesValidationUtils.IsNumericValue(value.JSubstring(0, value.Length
                 - measurement.Length).Trim())) {
                return true;
            }
            return false;
        }
    }
}
