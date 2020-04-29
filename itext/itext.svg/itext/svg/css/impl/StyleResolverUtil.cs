/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Css.Util;

namespace iText.Svg.Css.Impl {
    /// <summary>Utility class for resolving parent-inheritance of style and attribute declarations</summary>
    public class StyleResolverUtil {
        private ICollection<IStyleInheritance> inheritanceRules;

        /// <summary>List to store the properties whose value can depend on parent or element font-size</summary>
        private static readonly IList<String> fontSizeDependentPercentage = new List<String>(2);

        static StyleResolverUtil() {
            fontSizeDependentPercentage.Add(CommonCssConstants.FONT_SIZE);
            fontSizeDependentPercentage.Add(CommonCssConstants.LINE_HEIGHT);
        }

        public StyleResolverUtil() {
            this.inheritanceRules = new HashSet<IStyleInheritance>();
            inheritanceRules.Add(new CssInheritance());
            inheritanceRules.Add(new SvgAttributeInheritance());
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
                    float absoluteParentFontSize = CssUtils.ParseAbsoluteLength(parentFontSizeString);
                    // Format to 4 decimal places to prevent differences between Java and C#
                    styles.Put(styleProperty, DecimalFormatUtil.FormatNumber(CssUtils.ParseRelativeValue(parentPropValue, absoluteParentFontSize
                        ), "0.####") + CommonCssConstants.PT);
                }
                else {
                    //Property is inherited, add to element style declarations
                    styles.Put(styleProperty, parentPropValue);
                }
            }
            else {
                if (CommonCssConstants.TEXT_DECORATION.Equals(styleProperty) && !CommonCssConstants.INLINE_BLOCK.Equals(styles
                    .Get(CommonCssConstants.DISPLAY))) {
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
            if (value.EndsWith(measurement) && CssUtils.IsNumericValue(value.JSubstring(0, value.Length - measurement.
                Length).Trim())) {
                return true;
            }
            return false;
        }
    }
}
