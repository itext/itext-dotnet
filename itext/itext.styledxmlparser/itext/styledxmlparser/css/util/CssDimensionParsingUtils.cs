/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class for CSS dimension parsing operations.</summary>
    public sealed class CssDimensionParsingUtils {
        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="CssDimensionParsingUtils"/>
        /// instance.
        /// </summary>
        private CssDimensionParsingUtils() {
        }

        // Empty constructor
        /// <summary>Parses an integer without throwing an exception if something goes wrong.</summary>
        /// <param name="str">a string that might be an integer value</param>
        /// <returns>the integer value, or null if something went wrong</returns>
        public static int? ParseInteger(String str) {
            if (str == null) {
                return null;
            }
            try {
                return Convert.ToInt32(str, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                return null;
            }
        }

        /// <summary>Parses a float without throwing an exception if something goes wrong.</summary>
        /// <param name="str">a string that might be a float value</param>
        /// <returns>the float value, or null if something went wrong</returns>
        public static float? ParseFloat(String str) {
            if (str == null) {
                return null;
            }
            try {
                return float.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                return null;
            }
        }

        /// <summary>Parses a double without throwing an exception if something goes wrong.</summary>
        /// <param name="str">a string that might be a double value</param>
        /// <returns>the double value, or null if something went wrong</returns>
        public static double? ParseDouble(String str) {
            if (str == null) {
                return null;
            }
            try {
                return Double.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException) {
                return null;
            }
        }

        /// <summary>
        /// Parses an angle with an allowed metric unit (deg, grad, rad) or numeric value (e.g. 123, 1.23,
        /// .123) to rad.
        /// </summary>
        /// <param name="angle">String containing the angle to parse</param>
        /// <param name="defaultMetric">default metric to use in case the input string does not specify a metric</param>
        /// <returns>the angle in radians</returns>
        public static float ParseAngle(String angle, String defaultMetric) {
            int pos = iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.DeterminePositionBetweenValueAndUnit(angle
                );
            if (pos == 0) {
                if (angle == null) {
                    angle = "null";
                }
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.NAN, angle));
            }
            float floatValue = float.Parse(angle.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture
                );
            String unit = angle.Substring(pos);
            // Degrees
            if (unit.StartsWith(CommonCssConstants.DEG) || unit.Equals("") && CommonCssConstants.DEG.Equals(defaultMetric
                )) {
                return (float)Math.PI * floatValue / 180f;
            }
            // Grads
            if (unit.StartsWith(CommonCssConstants.GRAD) || unit.Equals("") && CommonCssConstants.GRAD.Equals(defaultMetric
                )) {
                return (float)Math.PI * floatValue / 200f;
            }
            // Radians
            if (unit.StartsWith(CommonCssConstants.RAD) || unit.Equals("") && CommonCssConstants.RAD.Equals(defaultMetric
                )) {
                return floatValue;
            }
            logger.LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_METRIC_ANGLE_PARSED
                , unit.Equals("") ? defaultMetric : unit));
            return floatValue;
        }

        /// <summary>
        /// Parses a angle with an allowed metric unit (deg, grad, rad) or numeric value (e.g. 123, 1.23,
        /// .123) to rad.
        /// </summary>
        /// <remarks>
        /// Parses a angle with an allowed metric unit (deg, grad, rad) or numeric value (e.g. 123, 1.23,
        /// .123) to rad. Default metric is degrees
        /// </remarks>
        /// <param name="angle">String containing the angle to parse</param>
        /// <returns>the angle in radians</returns>
        public static float ParseAngle(String angle) {
            return ParseAngle(angle, CommonCssConstants.DEG);
        }

        /// <summary>Parses an aspect ratio into an array with two integers.</summary>
        /// <param name="str">a string that might contain two integer values</param>
        /// <returns>the aspect ratio as an array of two integer values</returns>
        public static int[] ParseAspectRatio(String str) {
            int indexOfSlash = str.IndexOf('/');
            try {
                int first = Convert.ToInt32(str.JSubstring(0, indexOfSlash), System.Globalization.CultureInfo.InvariantCulture
                    );
                int second = Convert.ToInt32(str.Substring(indexOfSlash + 1), System.Globalization.CultureInfo.InvariantCulture
                    );
                return new int[] { first, second };
            }
            catch (FormatException) {
                return null;
            }
            catch (NullReferenceException) {
                return null;
            }
        }

        /// <summary>
        /// Parses a length with an allowed metric unit (px, pt, in, cm, mm, pc, q) or numeric value (e.g. 123, 1.23,
        /// .123) to pt.<br />
        /// A numeric value (without px, pt, etc in the given length string) is considered to be in the default metric that
        /// was given.
        /// </summary>
        /// <param name="length">the string containing the length</param>
        /// <param name="defaultMetric">
        /// the string containing the metric if it is possible that the length string does not contain
        /// one. If null the length is considered to be in px as is default in HTML/CSS
        /// </param>
        /// <returns>parsed value</returns>
        public static float ParseAbsoluteLength(String length, String defaultMetric) {
            int pos = iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.DeterminePositionBetweenValueAndUnit(length
                );
            if (pos == 0) {
                if (length == null) {
                    length = "null";
                }
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.NAN, length));
            }
            // Use double type locally to have better precision of the result after applying arithmetic operations
            double f = Double.Parse(length.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture);
            String unit = length.Substring(pos);
            //points
            if (unit.StartsWith(CommonCssConstants.PT) || unit.Equals("") && defaultMetric.Equals(CommonCssConstants.PT
                )) {
                return (float)f;
            }
            // inches
            if (unit.StartsWith(CommonCssConstants.IN) || (unit.Equals("") && defaultMetric.Equals(CommonCssConstants.
                IN))) {
                return (float)(f * 72);
            }
            else {
                // centimeters
                if (unit.StartsWith(CommonCssConstants.CM) || (unit.Equals("") && defaultMetric.Equals(CommonCssConstants.
                    CM))) {
                    return (float)((f / 2.54) * 72);
                }
                else {
                    // quarter of a millimeter (1/40th of a centimeter).
                    if (unit.StartsWith(CommonCssConstants.Q) || (unit.Equals("") && defaultMetric.Equals(CommonCssConstants.Q
                        ))) {
                        return (float)((f / 2.54) * 72 / 40);
                    }
                    else {
                        // millimeters
                        if (unit.StartsWith(CommonCssConstants.MM) || (unit.Equals("") && defaultMetric.Equals(CommonCssConstants.
                            MM))) {
                            return (float)((f / 25.4) * 72);
                        }
                        else {
                            // picas
                            if (unit.StartsWith(CommonCssConstants.PC) || (unit.Equals("") && defaultMetric.Equals(CommonCssConstants.
                                PC))) {
                                return (float)(f * 12);
                            }
                            else {
                                // pixels (1px = 0.75pt).
                                if (unit.StartsWith(CommonCssConstants.PX) || (unit.Equals("") && defaultMetric.Equals(CommonCssConstants.
                                    PX))) {
                                    return (float)(f * 0.75);
                                }
                            }
                        }
                    }
                }
            }
            logger.LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
                , unit.Equals("") ? defaultMetric : unit));
            return (float)f;
        }

        /// <summary>Parses the absolute length.</summary>
        /// <param name="length">the length as a string</param>
        /// <returns>the length as a float</returns>
        public static float ParseAbsoluteLength(String length) {
            return ParseAbsoluteLength(length, CommonCssConstants.PX);
        }

        /// <summary>
        /// Parses an relative value based on the base value that was given, in the metric unit of the base value.<br />
        /// (e.g. margin=10% should be based on the page width, so if an A4 is used, the margin = 0.10*595.0 = 59.5f)
        /// </summary>
        /// <param name="relativeValue">in %, em or ex</param>
        /// <param name="baseValue">the value the returned float is based on</param>
        /// <returns>the parsed float in the metric unit of the base value</returns>
        public static float ParseRelativeValue(String relativeValue, float baseValue) {
            int pos = iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.DeterminePositionBetweenValueAndUnit(relativeValue
                );
            if (pos == 0) {
                return 0f;
            }
            // Use double type locally to have better precision of the result after applying arithmetic operations
            double f = Double.Parse(relativeValue.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture
                );
            String unit = relativeValue.Substring(pos);
            if (unit.StartsWith(CommonCssConstants.PERCENTAGE)) {
                f = baseValue * f / 100;
            }
            else {
                if (unit.StartsWith(CommonCssConstants.EM) || unit.StartsWith(CommonCssConstants.REM)) {
                    f = baseValue * f;
                }
                else {
                    if (unit.StartsWith(CommonCssConstants.EX)) {
                        f = baseValue * f / 2;
                    }
                }
            }
            return (float)f;
        }

        /// <summary>Convenience method for parsing a value to pt.</summary>
        /// <remarks>
        /// Convenience method for parsing a value to pt. Possible values are: <list type="bullet">
        /// <item><description>a numeric value in pixels (e.g. 123, 1.23, .123),
        /// </description></item>
        /// <item><description>a value with a metric unit (px, in, cm, mm, pc or pt) attached to it,
        /// </description></item>
        /// <item><description>or a value with a relative value (%, em, ex).
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="value">the value</param>
        /// <param name="emValue">the em value</param>
        /// <param name="remValue">the root em value</param>
        /// <returns>the unit value</returns>
        public static UnitValue ParseLengthValueToPt(String value, float emValue, float remValue) {
            // TODO (DEVSIX-3596) Add support of 'lh' 'ch' units and viewport-relative units
            if (CssTypesValidationUtils.IsMetricValue(value) || CssTypesValidationUtils.IsNumber(value)) {
                return new UnitValue(UnitValue.POINT, ParseAbsoluteLength(value));
            }
            else {
                if (value != null && value.EndsWith(CommonCssConstants.PERCENTAGE)) {
                    return new UnitValue(UnitValue.PERCENT, float.Parse(value.JSubstring(0, value.Length - 1), System.Globalization.CultureInfo.InvariantCulture
                        ));
                }
                else {
                    if (CssTypesValidationUtils.IsRemValue(value)) {
                        return new UnitValue(UnitValue.POINT, ParseRelativeValue(value, remValue));
                    }
                    else {
                        if (CssTypesValidationUtils.IsRelativeValue(value)) {
                            return new UnitValue(UnitValue.POINT, ParseRelativeValue(value, emValue));
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>Parses a flex value "xfr" to x.</summary>
        /// <param name="value">String containing the flex value to parse</param>
        /// <returns>the flex value as a float</returns>
        public static float? ParseFlex(String value) {
            if (value == null) {
                return null;
            }
            value = value.Trim();
            if (value.EndsWith(CommonCssConstants.FR)) {
                value = value.JSubstring(0, value.Length - CommonCssConstants.FR.Length);
                if (CssTypesValidationUtils.IsNumber(value)) {
                    return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return null;
        }

        /// <summary>Parse length attributes.</summary>
        /// <param name="length">
        /// 
        /// <see cref="System.String"/>
        /// for parsing
        /// </param>
        /// <param name="percentBaseValue">the value on which percent length is based on</param>
        /// <param name="defaultValue">default value if length is not recognized</param>
        /// <param name="fontSize">font size of the current element</param>
        /// <param name="rootFontSize">root element font size</param>
        /// <returns>absolute value in points</returns>
        public static float ParseLength(String length, float percentBaseValue, float defaultValue, float fontSize, 
            float rootFontSize) {
            if (CssTypesValidationUtils.IsPercentageValue(length)) {
                return iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.ParseRelativeValue(length, percentBaseValue
                    );
            }
            else {
                UnitValue unitValue = iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.ParseLengthValueToPt(length, 
                    fontSize, rootFontSize);
                if (unitValue != null && unitValue.IsPointValue()) {
                    return unitValue.GetValue();
                }
                else {
                    return defaultValue;
                }
            }
        }

        /// <summary>Parses the absolute font size.</summary>
        /// <remarks>
        /// Parses the absolute font size.
        /// <para />
        /// A numeric value (without px, pt, etc in the given length string) is considered to be in the default metric that
        /// was given.
        /// </remarks>
        /// <param name="fontSizeValue">
        /// the font size value as a
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="defaultMetric">
        /// the string containing the metric if it is possible that the length string does not contain
        /// one. If null the length is considered to be in px as is default in HTML/CSS.
        /// </param>
        /// <returns>
        /// the font size value as a
        /// <c>float</c>
        /// </returns>
        public static float ParseAbsoluteFontSize(String fontSizeValue, String defaultMetric) {
            if (null != fontSizeValue && CommonCssConstants.FONT_ABSOLUTE_SIZE_KEYWORDS_VALUES.ContainsKey(fontSizeValue
                )) {
                fontSizeValue = CommonCssConstants.FONT_ABSOLUTE_SIZE_KEYWORDS_VALUES.Get(fontSizeValue);
            }
            try {
                /* Styled XML Parser will throw an exception when it can't parse the given value
                but in html2pdf, we want to fall back to the default value of 0
                */
                return iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.ParseAbsoluteLength(fontSizeValue, defaultMetric
                    );
            }
            catch (StyledXMLParserException) {
                return 0f;
            }
        }

        /// <summary>Parses the absolute font size.</summary>
        /// <remarks>
        /// Parses the absolute font size.
        /// <para />
        /// A numeric value (without px, pt, etc in the given length string) is considered to be in the px.
        /// </remarks>
        /// <param name="fontSizeValue">
        /// the font size value as a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the font size value as a
        /// <c>float</c>
        /// </returns>
        public static float ParseAbsoluteFontSize(String fontSizeValue) {
            return ParseAbsoluteFontSize(fontSizeValue, CommonCssConstants.PX);
        }

        /// <summary>Parses the relative font size.</summary>
        /// <param name="relativeFontSizeValue">
        /// the relative font size value as a
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="baseValue">the base value</param>
        /// <returns>
        /// the relative font size value as a
        /// <c>float</c>
        /// </returns>
        public static float ParseRelativeFontSize(String relativeFontSizeValue, float baseValue) {
            if (CommonCssConstants.SMALLER.Equals(relativeFontSizeValue)) {
                return (float)(baseValue / 1.2);
            }
            else {
                if (CommonCssConstants.LARGER.Equals(relativeFontSizeValue)) {
                    return (float)(baseValue * 1.2);
                }
            }
            return iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.ParseRelativeValue(relativeFontSizeValue, baseValue
                );
        }

        /// <summary>Parses the border radius of specific corner.</summary>
        /// <param name="specificBorderRadius">string that defines the border radius of specific corner</param>
        /// <param name="emValue">the em value</param>
        /// <param name="remValue">the root em value</param>
        /// <returns>
        /// an array of
        /// <see cref="iText.Layout.Properties.UnitValue">UnitValues</see>
        /// that define horizontal and vertical border radius values
        /// </returns>
        public static UnitValue[] ParseSpecificCornerBorderRadius(String specificBorderRadius, float emValue, float
             remValue) {
            if (null == specificBorderRadius) {
                return null;
            }
            UnitValue[] cornerRadii = new UnitValue[2];
            String[] props = iText.Commons.Utils.StringUtil.Split(specificBorderRadius, "\\s+");
            cornerRadii[0] = ParseLengthValueToPt(props[0], emValue, remValue);
            cornerRadii[1] = 2 == props.Length ? ParseLengthValueToPt(props[1], emValue, remValue) : cornerRadii[0];
            return cornerRadii;
        }

        /// <summary>Parses the resolution.</summary>
        /// <param name="resolutionStr">the resolution as a string</param>
        /// <returns>a value in dpi</returns>
        public static float ParseResolution(String resolutionStr) {
            int pos = iText.StyledXmlParser.Css.Util.CssDimensionParsingUtils.DeterminePositionBetweenValueAndUnit(resolutionStr
                );
            if (pos == 0) {
                return 0f;
            }
            double f = Double.Parse(resolutionStr.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture
                );
            String unit = resolutionStr.Substring(pos);
            if (unit.StartsWith(CommonCssConstants.DPCM)) {
                f *= 2.54;
            }
            else {
                if (unit.StartsWith(CommonCssConstants.DPPX)) {
                    f *= 96;
                }
                else {
                    if (!unit.StartsWith(CommonCssConstants.DPI)) {
                        throw new StyledXMLParserException(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INCORRECT_RESOLUTION_UNIT_VALUE
                            );
                    }
                }
            }
            return (float)f;
        }

        /// <summary>Parses either RGBA or CMYK color.</summary>
        /// <param name="colorValue">the color value</param>
        /// <returns>an RGBA or CMYK value expressed as an array with four float values</returns>
        public static TransparentColor ParseColor(String colorValue) {
            Color device = null;
            float opacity = 1;
            float[] color = WebColors.GetRGBAColor(colorValue);
            if (color == null) {
                color = WebColors.GetCMYKArray(colorValue);
            }
            else {
                device = new DeviceRgb(color[0], color[1], color[2]);
                if (color.Length == 4) {
                    opacity = color[3];
                }
            }
            if (color == null) {
                color = new float[] { 0, 0, 0, 1 };
                device = new DeviceRgb(0, 0, 0);
            }
            else {
                if (device == null) {
                    device = new DeviceCmyk(color[0], color[1], color[2], color[3]);
                    if (color.Length == 5) {
                        opacity = color[4];
                    }
                }
            }
            return new TransparentColor(device, opacity);
        }

        /// <summary>Parses the RGBA color.</summary>
        /// <param name="colorValue">the color value</param>
        /// <returns>an RGBA value expressed as an array with four float values</returns>
        public static float[] ParseRgbaColor(String colorValue) {
            float[] rgbaColor = WebColors.GetRGBAColor(colorValue);
            if (rgbaColor == null) {
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.COLOR_NOT_PARSED, colorValue));
                rgbaColor = new float[] { 0, 0, 0, 1 };
            }
            return rgbaColor;
        }

        /// <summary>
        /// Method used in preparation of splitting a string containing a numeric value with a metric unit (e.g. 18px, 9pt,
        /// 6cm, etc).<br /><br />
        /// Determines the position between digits and affiliated characters ('+','-','0-9' and '.') and all other
        /// characters.<br />
        /// e.g. string "16px" will return 2, string "0.5em" will return 3 and string '-8.5mm' will return 4.
        /// </summary>
        /// <param name="string">containing a numeric value with a metric unit</param>
        /// <returns>
        /// int position between the numeric value and unit or 0 if string is null or string started with a
        /// non-numeric value.
        /// </returns>
        public static int DeterminePositionBetweenValueAndUnit(String @string) {
            if (@string == null) {
                return 0;
            }
            int pos = 0;
            while (pos < @string.Length) {
                if (@string[pos] == '+' || @string[pos] == '-' || @string[pos] == '.' || IsDigit(@string[pos]) || IsExponentNotation
                    (@string, pos)) {
                    pos++;
                }
                else {
                    break;
                }
            }
            return pos;
        }

        private static bool IsDigit(char ch) {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsExponentNotation(String s, int index) {
            return index < s.Length && char.ToLower(s[index]) == 'e' && 
                        // e.g. 12e5
                        (index + 1 < s.Length && IsDigit(s[index + 1]) || 
                        // e.g. 12e-5, 12e+5
                        index + 2 < s.Length && (s[index + 1] == '-' || s[index + 1] == '+') && IsDigit(s[index + 2]));
        }
    }
}
