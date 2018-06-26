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

        public StyleResolverUtil(ICollection<IStyleInheritance> inheritanceRules) {
            this.inheritanceRules = inheritanceRules;
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
                    // TODO Note! This property is formally not inherited, but the browsers behave very similar to inheritance here.
                    /* Text decorations on inline boxes are drawn across the entire element,
                    going across any descendant elements without paying any attention to their presence. */
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
