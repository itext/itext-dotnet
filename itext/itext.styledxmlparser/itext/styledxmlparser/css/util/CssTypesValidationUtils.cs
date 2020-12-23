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
using iText.Kernel.Colors;
using iText.StyledXmlParser.Css;

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

        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; rad, deg and grad.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed angle value</returns>
        public static bool IsAngleValue(String value) {
            if (value == null) {
                return false;
            }
            foreach (String metricPostfix in ANGLE_MEASUREMENTS_VALUES) {
                if (value.EndsWith(metricPostfix) && IsNumericValue(value.JSubstring(0, value.Length - metricPostfix.Length
                    ).Trim())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks if a data is base 64 encoded.</summary>
        /// <param name="data">the data</param>
        /// <returns>true, if the data is base 64 encoded</returns>
        public static bool IsBase64Data(String data) {
            return data.Matches("^data:([^\\s]*);base64,([^\\s]*)");
        }

        /// <summary>Checks if a value is a color property.</summary>
        /// <param name="value">the value</param>
        /// <returns>true, if the value contains a color property</returns>
        public static bool IsColorProperty(String value) {
            return value.StartsWith("rgb(") || value.StartsWith("rgba(") || value.StartsWith("#") || WebColors.NAMES.Contains
                (value.ToLowerInvariant()) || CommonCssConstants.TRANSPARENT.Equals(value);
        }

        /// <summary>Checks whether a string contains an allowed value relative to parent value.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a em value</returns>
        public static bool IsEmValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.EM) && IsNumericValue(value.JSubstring(0, value.
                Length - CommonCssConstants.EM.Length).Trim());
        }

        /// <summary>Checks whether a string contains an allowed value relative to element font height.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a ex value</returns>
        public static bool IsExValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.EX) && IsNumericValue(value.JSubstring(0, value.
                Length - CommonCssConstants.EX.Length).Trim());
        }

        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; px, in, cm, mm, pc, Q or pt.
        ///     </summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        public static bool IsMetricValue(String value) {
            if (value == null) {
                return false;
            }
            foreach (String metricPostfix in CommonCssConstants.METRIC_MEASUREMENTS_VALUES) {
                if (value.EndsWith(metricPostfix) && IsNumericValue(value.JSubstring(0, value.Length - metricPostfix.Length
                    ).Trim())) {
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
            if (IsNumericValue(value) || IsRelativeValue(value)) {
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
        public static bool IsNumericValue(String value) {
            return value != null && (value.Matches("^[-+]?\\d\\d*\\.\\d*$") || value.Matches("^[-+]?\\d\\d*$") || value
                .Matches("^[-+]?\\.\\d\\d*$"));
        }

        /// <summary>Checks whether a string contains a percentage value</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed percentage value</returns>
        public static bool IsPercentageValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.PERCENTAGE) && IsNumericValue(value.JSubstring(0
                , value.Length - CommonCssConstants.PERCENTAGE.Length).Trim());
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set value.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        public static bool IsRelativeValue(String value) {
            if (value == null) {
                return false;
            }
            foreach (String relativePostfix in RELATIVE_MEASUREMENTS_VALUES) {
                if (value.EndsWith(relativePostfix) && IsNumericValue(value.JSubstring(0, value.Length - relativePostfix.Length
                    ).Trim())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set root value.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a rem value</returns>
        public static bool IsRemValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.REM) && IsNumericValue(value.JSubstring(0, value
                .Length - CommonCssConstants.REM.Length).Trim());
        }

        /// <summary>Checks if a string is in a valid format.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value is in a valid format</returns>
        public static bool IsValidNumericValue(String value) {
            if (value == null || value.Contains(" ")) {
                return false;
            }
            return IsRelativeValue(value) || IsMetricValue(value) || IsNumericValue(value);
        }

        /// <summary>Checks if value is initial, inherit or unset.</summary>
        /// <param name="value">value to check</param>
        /// <returns>true if value is initial, inherit or unset. false otherwise</returns>
        public static bool IsInitialOrInheritOrUnset(String value) {
            return CommonCssConstants.INITIAL.Equals(value) || CommonCssConstants.INHERIT.Equals(value) || CommonCssConstants
                .UNSET.Equals(value);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssTypesValidationUtils"/>
        /// instance.
        /// </summary>
        private CssTypesValidationUtils() {
        }
        // Empty constructor
    }
}
