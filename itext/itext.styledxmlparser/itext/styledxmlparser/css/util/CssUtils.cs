/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Text;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class for CSS operations.</summary>
    public class CssUtils {
        // TODO (DEVSIX-3595) The list of the angle measurements is not full. Required to
        //  add 'turn' units to array and move this array to the CommonCssConstants
        private static readonly String[] ANGLE_MEASUREMENTS_VALUES = new String[] { CommonCssConstants.DEG, CommonCssConstants
            .GRAD, CommonCssConstants.RAD };

        // TODO (DEVSIX-3596) The list of the relative measurements is not full.
        //  Add new relative units to array and move this array to the CommonCssConstants
        private static readonly String[] RELATIVE_MEASUREMENTS_VALUES = new String[] { CommonCssConstants.PERCENTAGE
            , CommonCssConstants.EM, CommonCssConstants.EX, CommonCssConstants.REM };

        // TODO (DEVSIX-3596) The list of the font-relative measurements is not full.
        //  Add 'ch' units to array and move this array to the CommonCssConstants
        private static readonly String[] FONT_RELATIVE_MEASUREMENTS_VALUES = new String[] { CommonCssConstants.EM, 
            CommonCssConstants.EX, CommonCssConstants.REM };

        private const float EPSILON = 1e-6f;

        private static readonly ILog logger = LogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Util.CssUtils)
            );

        /// <summary>
        /// Creates a new
        /// <see cref="CssUtils"/>
        /// instance.
        /// </summary>
        private CssUtils() {
        }

        /// <summary>Normalizes a CSS property.</summary>
        /// <param name="str">the property</param>
        /// <returns>the normalized property</returns>
        public static String NormalizeCssProperty(String str) {
            return str == null ? null : CssPropertyNormalizer.Normalize(str);
        }

        /// <summary>Removes double spaces and trims a string.</summary>
        /// <param name="str">the string</param>
        /// <returns>the string without the unnecessary spaces</returns>
        public static String RemoveDoubleSpacesAndTrim(String str) {
            String[] parts = iText.IO.Util.StringUtil.Split(str, "\\s");
            StringBuilder sb = new StringBuilder();
            foreach (String part in parts) {
                if (part.Length > 0) {
                    if (sb.Length != 0) {
                        sb.Append(" ");
                    }
                    sb.Append(part);
                }
            }
            return sb.ToString();
        }

        /// <summary>Parses an integer without throwing an exception if something goes wrong.</summary>
        /// <param name="str">a string that might be an integer value</param>
        /// <returns>the integer value, or null if something went wrong</returns>
        public static int? ParseInteger(String str) {
            if (str == null) {
                return null;
            }
            try {
                return Convert.ToInt32(str);
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
            int pos = DeterminePositionBetweenValueAndUnit(angle);
            if (pos == 0) {
                if (angle == null) {
                    angle = "null";
                }
                throw new StyledXMLParserException(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.NAN, 
                    angle));
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
            logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_METRIC_ANGLE_PARSED
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
                int first = Convert.ToInt32(str.JSubstring(0, indexOfSlash));
                int second = Convert.ToInt32(str.Substring(indexOfSlash + 1));
                return new int[] { first, second };
            }
            catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// Parses a length with an allowed metric unit (px, pt, in, cm, mm, pc, q) or numeric value (e.g. 123, 1.23,
        /// .123) to pt.<br />
        /// A numeric value (without px, pt, etc in the given length string) is considered to be in the default metric that
        /// was given.
        /// </summary>
        /// <param name="length">the string containing the length.</param>
        /// <param name="defaultMetric">
        /// the string containing the metric if it is possible that the length string does not contain
        /// one. If null the length is considered to be in px as is default in HTML/CSS.
        /// </param>
        /// <returns>parsed value</returns>
        public static float ParseAbsoluteLength(String length, String defaultMetric) {
            int pos = DeterminePositionBetweenValueAndUnit(length);
            if (pos == 0) {
                if (length == null) {
                    length = "null";
                }
                throw new StyledXMLParserException(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.NAN, 
                    length));
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
            logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
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
        /// <param name="relativeValue">in %, em or ex.</param>
        /// <param name="baseValue">the value the returned float is based on.</param>
        /// <returns>the parsed float in the metric unit of the base value.</returns>
        public static float ParseRelativeValue(String relativeValue, float baseValue) {
            int pos = DeterminePositionBetweenValueAndUnit(relativeValue);
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
            if (IsMetricValue(value) || IsNumericValue(value)) {
                return new UnitValue(UnitValue.POINT, ParseAbsoluteLength(value));
            }
            else {
                if (value != null && value.EndsWith(CommonCssConstants.PERCENTAGE)) {
                    return new UnitValue(UnitValue.PERCENT, float.Parse(value.JSubstring(0, value.Length - 1), System.Globalization.CultureInfo.InvariantCulture
                        ));
                }
                else {
                    if (IsRemValue(value)) {
                        return new UnitValue(UnitValue.POINT, ParseRelativeValue(value, remValue));
                    }
                    else {
                        if (IsRelativeValue(value)) {
                            return new UnitValue(UnitValue.POINT, ParseRelativeValue(value, emValue));
                        }
                    }
                }
            }
            return null;
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
                return iText.StyledXmlParser.Css.Util.CssUtils.ParseAbsoluteLength(fontSizeValue, defaultMetric);
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
            return iText.StyledXmlParser.Css.Util.CssUtils.ParseRelativeValue(relativeFontSizeValue, baseValue);
        }

        /// <summary>Parses the border radius of specific corner.</summary>
        /// <param name="specificBorderRadius">string that defines the border radius of specific corner.</param>
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
            String[] props = iText.IO.Util.StringUtil.Split(specificBorderRadius, "\\s+");
            cornerRadii[0] = ParseLengthValueToPt(props[0], emValue, remValue);
            cornerRadii[1] = 2 == props.Length ? ParseLengthValueToPt(props[1], emValue, remValue) : cornerRadii[0];
            return cornerRadii;
        }

        /// <summary>Parses the resolution.</summary>
        /// <param name="resolutionStr">the resolution as a string</param>
        /// <returns>a value in dpi (currently)</returns>
        public static float ParseResolution(String resolutionStr) {
            // TODO change default units? If so, change MediaDeviceDescription#resolutoin as well
            int pos = DeterminePositionBetweenValueAndUnit(resolutionStr);
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
            }
            return (float)f;
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
        private static int DeterminePositionBetweenValueAndUnit(String @string) {
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

        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; px, in, cm, mm, pc, Q or pt.
        ///     </summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains an allowed metric value.</returns>
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

        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; rad, deg and grad.</summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains an allowed angle value.</returns>
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

        /// <summary>Checks whether a string contains an allowed value relative to previously set value.</summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains an allowed metric value.</returns>
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

        /// <summary>Checks whether a string contains an allowed value relative to font.</summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains an allowed font relative value.</returns>
        public static bool IsFontRelativeValue(String value) {
            if (value == null) {
                return false;
            }
            foreach (String relativePostfix in FONT_RELATIVE_MEASUREMENTS_VALUES) {
                if (value.EndsWith(relativePostfix) && IsNumericValue(value.JSubstring(0, value.Length - relativePostfix.Length
                    ).Trim())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks whether a string contains a percentage value</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed percentage value</returns>
        public static bool IsPercentageValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.PERCENTAGE) && IsNumericValue(value.JSubstring(0
                , value.Length - CommonCssConstants.PERCENTAGE.Length).Trim());
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set root value.</summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains a rem value.</returns>
        public static bool IsRemValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.REM) && IsNumericValue(value.JSubstring(0, value
                .Length - CommonCssConstants.REM.Length).Trim());
        }

        /// <summary>Checks whether a string contains an allowed value relative to parent value.</summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains a em value.</returns>
        public static bool IsEmValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.EM) && IsNumericValue(value.JSubstring(0, value.
                Length - CommonCssConstants.EM.Length).Trim());
        }

        /// <summary>Checks whether a string contains an allowed value relative to element font height.</summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains a ex value.</returns>
        public static bool IsExValue(String value) {
            return value != null && value.EndsWith(CommonCssConstants.EX) && IsNumericValue(value.JSubstring(0, value.
                Length - CommonCssConstants.EX.Length).Trim());
        }

        /// <summary>Checks whether a string matches a numeric value (e.g. 123, 1.23, .123).</summary>
        /// <remarks>
        /// Checks whether a string matches a numeric value (e.g. 123, 1.23, .123). All these metric values are allowed in
        /// HTML/CSS.
        /// </remarks>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains an allowed metric value.</returns>
        public static bool IsNumericValue(String value) {
            return value != null && (value.Matches("^[-+]?\\d\\d*\\.\\d*$") || value.Matches("^[-+]?\\d\\d*$") || value
                .Matches("^[-+]?\\.\\d\\d*$"));
        }

        /// <summary>
        /// Parses
        /// <c>url("file.jpg")</c>
        /// to
        /// <c>file.jpg</c>.
        /// </summary>
        /// <param name="url">the url attribute to parse</param>
        /// <returns>the parsed url. Or original url if not wrappend in url()</returns>
        public static String ExtractUrl(String url) {
            String str = null;
            if (url.StartsWith("url")) {
                String urlString = url.Substring(3).Trim().Replace("(", "").Replace(")", "").Trim();
                if (urlString.StartsWith("'") && urlString.EndsWith("'")) {
                    str = urlString.JSubstring(urlString.IndexOf("'", StringComparison.Ordinal) + 1, urlString.LastIndexOf("'"
                        ));
                }
                else {
                    if (urlString.StartsWith("\"") && urlString.EndsWith("\"")) {
                        str = urlString.JSubstring(urlString.IndexOf('"') + 1, urlString.LastIndexOf('"'));
                    }
                    else {
                        str = urlString;
                    }
                }
            }
            else {
                // assume it's an url without wrapping in "url()"
                str = url;
            }
            return str;
        }

        /// <summary>Checks if a data is base 64 encoded.</summary>
        /// <param name="data">the data</param>
        /// <returns>true, if the data is base 64 encoded</returns>
        public static bool IsBase64Data(String data) {
            return data.Matches("^data:([^\\s]*);base64,([^\\s]*)");
        }

        /// <summary>Find the next unescaped character.</summary>
        /// <param name="source">a source</param>
        /// <param name="ch">the character to look for</param>
        /// <param name="startIndex">where to start looking</param>
        /// <returns>the position of the next unescaped character</returns>
        public static int FindNextUnescapedChar(String source, char ch, int startIndex) {
            int symbolPos = source.IndexOf(ch, startIndex);
            if (symbolPos == -1) {
                return -1;
            }
            int afterNoneEscapePos = symbolPos;
            while (afterNoneEscapePos > 0 && source[afterNoneEscapePos - 1] == '\\') {
                --afterNoneEscapePos;
            }
            return (symbolPos - afterNoneEscapePos) % 2 == 0 ? symbolPos : FindNextUnescapedChar(source, ch, symbolPos
                 + 1);
        }

        /// <summary>Checks if a value is a color property.</summary>
        /// <param name="value">the value</param>
        /// <returns>true, if the value contains a color property</returns>
        public static bool IsColorProperty(String value) {
            return value.Contains("rgb(") || value.Contains("rgba(") || value.Contains("#") || WebColors.NAMES.Contains
                (value.ToLowerInvariant()) || CommonCssConstants.TRANSPARENT.Equals(value);
        }

        /// <summary>Helper method for comparing floating point numbers</summary>
        /// <param name="d1">first float to compare</param>
        /// <param name="d2">second float to compare</param>
        /// <returns>True if both floats are equal within a Epsilon defined in this class, false otherwise</returns>
        public static bool CompareFloats(double d1, double d2) {
            return (Math.Abs(d1 - d2) < EPSILON);
        }

        /// <summary>Helper method for comparing floating point numbers</summary>
        /// <param name="f1">first float to compare</param>
        /// <param name="f2">second float to compare</param>
        /// <returns>True if both floats are equal within a Epsilon defined in this class, false otherwise</returns>
        public static bool CompareFloats(float f1, float f2) {
            return (Math.Abs(f1 - f2) < EPSILON);
        }

        /// <summary>Parses the RGBA color.</summary>
        /// <param name="colorValue">the color value</param>
        /// <returns>an RGBA value expressed as an array with four float values</returns>
        public static float[] ParseRgbaColor(String colorValue) {
            float[] rgbaColor = WebColors.GetRGBAColor(colorValue);
            if (rgbaColor == null) {
                logger.Error(MessageFormatUtil.Format(iText.IO.LogMessageConstant.COLOR_NOT_PARSED, colorValue));
                rgbaColor = new float[] { 0, 0, 0, 1 };
            }
            return rgbaColor;
        }

        /// <summary>Parses the unicode range.</summary>
        /// <param name="unicodeRange">the string which stores the unicode range</param>
        /// <returns>
        /// the unicode range as a
        /// <see cref="iText.Layout.Font.Range"/>
        /// object
        /// </returns>
        public static Range ParseUnicodeRange(String unicodeRange) {
            String[] ranges = iText.IO.Util.StringUtil.Split(unicodeRange, ",");
            RangeBuilder builder = new RangeBuilder();
            foreach (String range in ranges) {
                if (!AddRange(builder, range)) {
                    return null;
                }
            }
            return builder.Create();
        }

        private static bool AddRange(RangeBuilder builder, String range) {
            range = range.Trim();
            if (range.Matches("[uU]\\+[0-9a-fA-F?]{1,6}(-[0-9a-fA-F]{1,6})?")) {
                String[] parts = iText.IO.Util.StringUtil.Split(range.JSubstring(2, range.Length), "-");
                if (1 == parts.Length) {
                    if (parts[0].Contains("?")) {
                        return AddRange(builder, parts[0].Replace('?', '0'), parts[0].Replace('?', 'F'));
                    }
                    else {
                        return AddRange(builder, parts[0], parts[0]);
                    }
                }
                else {
                    return AddRange(builder, parts[0], parts[1]);
                }
            }
            return false;
        }

        private static bool AddRange(RangeBuilder builder, String left, String right) {
            int l = Convert.ToInt32(left, 16);
            int r = Convert.ToInt32(right, 16);
            if (l > r || r > 1114111) {
                // Although Firefox follows the spec (and therefore the second condition), it seems it's ignored in Chrome or Edge
                return false;
            }
            builder.AddRange(l, r);
            return true;
        }

        private static bool IsDigit(char ch) {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsExponentNotation(String s, int index) {
            return index < s.Length && s[index] == 'e' && 
                        // e.g. 12e5
                        (index + 1 < s.Length && IsDigit(s[index + 1]) || 
                        // e.g. 12e-5, 12e+5
                        index + 2 < s.Length && (s[index + 1] == '-' || s[index + 1] == '+') && IsDigit(s[index + 2]));
        }
    }
}
