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
using System.Text.RegularExpressions;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class for CSS types validating operations.</summary>
    public sealed class CssTypesValidationUtils {
        // TODO (DEVSIX-3595) The list of the angle measurements is not full. Required to
        //  add 'turn' units to array and move this array to the CommonCssConstants
        private static readonly String[] ANGLE_MEASUREMENTS_VALUES = new String[] { CommonCssConstants.DEG, CommonCssConstants
            .GRAD, CommonCssConstants.RAD };

        // TODO (DEVSIX-3596) The list of the relative measurements is not full.
        //  Add new relative units to array and move this array to the CommonCssConstants
        private static readonly String[] RELATIVE_MEASUREMENTS_VALUES = new String[] { CommonCssConstants.PERCENTAGE
            , CommonCssConstants.EM, CommonCssConstants.EX, CommonCssConstants.REM };

        private static readonly Regex BASE64_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("^data:[^\\s]+;base64,"
            );

        /// <summary>
        /// Creates a new
        /// <see cref="CssTypesValidationUtils"/>
        /// instance.
        /// </summary>
        private CssTypesValidationUtils() {
        }

        // Empty constructor
        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; rad, deg and grad.</summary>
        /// <param name="valueArgument">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed angle value</returns>
        public static bool IsAngleValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            foreach (String metricPostfix in ANGLE_MEASUREMENTS_VALUES) {
                if (value.EndsWith(metricPostfix) && IsNumber(value.JSubstring(0, value.Length - metricPostfix.Length))) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks if a data is base 64 encoded.</summary>
        /// <param name="data">the data</param>
        /// <returns>true, if the data is base 64 encoded</returns>
        public static bool IsBase64Data(String data) {
            return iText.Commons.Utils.Matcher.Match(BASE64_PATTERN, data).Find();
        }

        /// <summary>Checks if a value is a color property.</summary>
        /// <param name="value">the value</param>
        /// <returns>true, if the value contains a color property</returns>
        public static bool IsColorProperty(String value) {
            return CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants.COLOR, value)
                );
        }

        /// <summary>Checks whether a string contains an allowed value relative to parent value.</summary>
        /// <param name="valueArgument">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a em value</returns>
        public static bool IsEmValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            return value.EndsWith(CommonCssConstants.EM) && IsNumber(value.JSubstring(0, value.Length - CommonCssConstants
                .EM.Length));
        }

        /// <summary>Checks whether a string contains an allowed value relative to element font height.</summary>
        /// <param name="valueArgument">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a ex value</returns>
        public static bool IsExValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            return value != null && value.EndsWith(CommonCssConstants.EX) && IsNumber(value.JSubstring(0, value.Length
                 - CommonCssConstants.EX.Length));
        }

        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; px, in, cm, mm, pc, Q or pt.
        ///     </summary>
        /// <param name="valueArgument">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        public static bool IsMetricValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            foreach (String metricPostfix in CommonCssConstants.METRIC_MEASUREMENTS_VALUES) {
                if (value.EndsWith(metricPostfix) && IsNumber(value.JSubstring(0, value.Length - metricPostfix.Length))) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks whether a string matches a negative value (e.g. -123, -2em, -0.123).</summary>
        /// <remarks>
        /// Checks whether a string matches a negative value (e.g. -123, -2em, -0.123).
        /// All these metric values are allowed in HTML/CSS.
        /// </remarks>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>true if value is negative</returns>
        public static bool IsNegativeValue(String value) {
            if (value == null) {
                return false;
            }
            if (IsNumber(value) || IsRelativeValue(value) || IsMetricValue(value)) {
                return value.StartsWith("-");
            }
            return false;
        }

        /// <summary>Checks whether a string matches a numeric value (e.g. 123, 1.23, .123).</summary>
        /// <remarks>
        /// Checks whether a string matches a numeric value (e.g. 123, 1.23, .123). All these metric values are allowed in
        /// HTML/CSS.
        /// </remarks>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        public static bool IsNumber(String value) {
            return value != null && (value.Matches("^[-+]?\\d\\d*\\.\\d*$") || value.Matches("^[-+]?\\d\\d*$") || value
                .Matches("^[-+]?\\.\\d\\d*$"));
        }

        /// <summary>Checks whether a string contains a percentage value</summary>
        /// <param name="valueArgument">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed percentage value</returns>
        public static bool IsPercentageValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            return value.EndsWith(CommonCssConstants.PERCENTAGE) && IsNumber(value.JSubstring(0, value.Length - CommonCssConstants
                .PERCENTAGE.Length));
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set value.</summary>
        /// <param name="valueArgument">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        public static bool IsRelativeValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            foreach (String relativePostfix in RELATIVE_MEASUREMENTS_VALUES) {
                if (value.EndsWith(relativePostfix) && IsNumber(value.JSubstring(0, value.Length - relativePostfix.Length)
                    )) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set root value.</summary>
        /// <param name="valueArgument">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a rem value</returns>
        public static bool IsRemValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            return value != null && value.EndsWith(CommonCssConstants.REM) && IsNumber(value.JSubstring(0, value.Length
                 - CommonCssConstants.REM.Length));
        }

        /// <summary>Checks if a string is in a valid format.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value is in a valid format</returns>
        public static bool IsValidNumericValue(String value) {
            if (value == null || value.Contains(" ")) {
                return false;
            }
            return IsRelativeValue(value) || IsMetricValue(value) || IsNumber(value);
        }

        /// <summary>Checks if value is initial, inherit or unset.</summary>
        /// <param name="value">value to check</param>
        /// <returns>true if value is initial, inherit or unset. false otherwise</returns>
        public static bool IsInitialOrInheritOrUnset(String value) {
            return CommonCssConstants.INITIAL.Equals(value) || CommonCssConstants.INHERIT.Equals(value) || CommonCssConstants
                .UNSET.Equals(value);
        }

        /// <summary>Checks if value contains initial, inherit or unset.</summary>
        /// <param name="value">value to check</param>
        /// <returns>true if value contains initial, inherit or unset. False otherwise</returns>
        public static bool ContainsInitialOrInheritOrUnset(String value) {
            if (value == null) {
                return false;
            }
            return value.Contains(CommonCssConstants.INITIAL) || value.Contains(CommonCssConstants.INHERIT) || value.Contains
                (CommonCssConstants.UNSET);
        }

        /// <summary>Checks whether a string contains a zero.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a zero</returns>
        public static bool IsZero(String value) {
            return IsNumericZeroValue(value) || IsMetricZeroValue(value) || IsRelativeZeroValue(value);
        }

        internal static bool IsMetricZeroValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            foreach (String metricPostfix in CommonCssConstants.METRIC_MEASUREMENTS_VALUES) {
                if (value.EndsWith(metricPostfix) && IsNumericZeroValue(value.JSubstring(0, value.Length - metricPostfix.Length
                    ))) {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsNumericZeroValue(String value) {
            return value != null && (value.Matches("^[-+]?0$") || value.Matches("^[-+]?\\.0$"));
        }

        internal static bool IsRelativeZeroValue(String valueArgument) {
            String value = valueArgument;
            if (value == null) {
                return false;
            }
            else {
                value = value.Trim();
            }
            foreach (String relativePostfix in RELATIVE_MEASUREMENTS_VALUES) {
                if (value.EndsWith(relativePostfix) && IsNumericZeroValue(value.JSubstring(0, value.Length - relativePostfix
                    .Length))) {
                    return true;
                }
            }
            return false;
        }
    }
}
