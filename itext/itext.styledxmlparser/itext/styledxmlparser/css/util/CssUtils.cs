/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class for CSS operations.</summary>
    public class CssUtils {
        private static readonly String[] METRIC_MEASUREMENTS = new String[] { CssConstants.PX, CssConstants.IN, CssConstants
            .CM, CssConstants.MM, CssConstants.PC, CssConstants.PT };

        private static readonly String[] RELATIVE_MEASUREMENTS = new String[] { CssConstants.PERCENTAGE, CssConstants
            .EM, CssConstants.EX, CssConstants.REM };

        private const float EPSILON = 0.000000000000001f;

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

        /// <summary>Parses a length with an allowed metric unit (px, pt, in, cm, mm, pc, q) or numeric value (e.g.</summary>
        /// <remarks>
        /// Parses a length with an allowed metric unit (px, pt, in, cm, mm, pc, q) or numeric value (e.g. 123, 1.23,
        /// .123) to pt.<br />
        /// A numeric value (without px, pt, etc in the given length string) is considered to be in the default metric that
        /// was given.
        /// </remarks>
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
            float f = float.Parse(length.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture);
            String unit = length.Substring(pos);
            //points
            if (unit.StartsWith(CssConstants.PT) || unit.Equals("") && defaultMetric.Equals(CssConstants.PT)) {
                return f;
            }
            // inches
            if (unit.StartsWith(CssConstants.IN) || (unit.Equals("") && defaultMetric.Equals(CssConstants.IN))) {
                return f * 72f;
            }
            else {
                // centimeters
                if (unit.StartsWith(CssConstants.CM) || (unit.Equals("") && defaultMetric.Equals(CssConstants.CM))) {
                    return (f / 2.54f) * 72f;
                }
                else {
                    // quarter of a millimeter (1/40th of a centimeter).
                    if (unit.StartsWith(CssConstants.Q) || (unit.Equals("") && defaultMetric.Equals(CssConstants.Q))) {
                        return (f / 2.54f) * 72f / 40;
                    }
                    else {
                        // millimeters
                        if (unit.StartsWith(CssConstants.MM) || (unit.Equals("") && defaultMetric.Equals(CssConstants.MM))) {
                            return (f / 25.4f) * 72f;
                        }
                        else {
                            // picas
                            if (unit.StartsWith(CssConstants.PC) || (unit.Equals("") && defaultMetric.Equals(CssConstants.PC))) {
                                return f * 12f;
                            }
                            else {
                                // pixels (1px = 0.75pt).
                                if (unit.StartsWith(CssConstants.PX) || (unit.Equals("") && defaultMetric.Equals(CssConstants.PX))) {
                                    return f * 0.75f;
                                }
                            }
                        }
                    }
                }
            }
            ILog logger = LogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Util.CssUtils));
            logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
                , unit.Equals("") ? defaultMetric : unit));
            return f;
        }

        /// <summary>Parses the absolute length.</summary>
        /// <param name="length">the length as a string</param>
        /// <returns>the length as a float</returns>
        public static float ParseAbsoluteLength(String length) {
            return ParseAbsoluteLength(length, CssConstants.PX);
        }

        /// <summary>
        /// Parses an relative value based on the base value that was given, in the metric unit of the base value.<br />
        /// (e.g.
        /// </summary>
        /// <remarks>
        /// Parses an relative value based on the base value that was given, in the metric unit of the base value.<br />
        /// (e.g. margin=10% should be based on the page width, so if an A4 is used, the margin = 0.10*595.0 = 59.5f)
        /// </remarks>
        /// <param name="relativeValue">in %, em or ex.</param>
        /// <param name="baseValue">the value the returned float is based on.</param>
        /// <returns>the parsed float in the metric unit of the base value.</returns>
        public static float ParseRelativeValue(String relativeValue, float baseValue) {
            int pos = DeterminePositionBetweenValueAndUnit(relativeValue);
            if (pos == 0) {
                return 0f;
            }
            double f = Double.Parse(relativeValue.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture
                );
            String unit = relativeValue.Substring(pos);
            if (unit.StartsWith(CssConstants.PERCENTAGE)) {
                f = baseValue * f / 100;
            }
            else {
                if (unit.StartsWith(CssConstants.EM) || unit.StartsWith(CssConstants.REM)) {
                    f = baseValue * f;
                }
                else {
                    if (unit.StartsWith(CssConstants.EX)) {
                        f = baseValue * f / 2;
                    }
                }
            }
            return (float)f;
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
            float f = float.Parse(resolutionStr.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture);
            String unit = resolutionStr.Substring(pos);
            if (unit.StartsWith(CssConstants.DPCM)) {
                f *= 2.54f;
            }
            else {
                if (unit.StartsWith(CssConstants.DPPX)) {
                    f *= 96;
                }
            }
            return f;
        }

        /// <summary>Method used in preparation of splitting a string containing a numeric value with a metric unit (e.g.
        ///     </summary>
        /// <remarks>
        /// Method used in preparation of splitting a string containing a numeric value with a metric unit (e.g. 18px, 9pt, 6cm, etc).<br /><br />
        /// Determines the position between digits and affiliated characters ('+','-','0-9' and '.') and all other characters.<br />
        /// e.g. string "16px" will return 2, string "0.5em" will return 3 and string '-8.5mm' will return 4.
        /// </remarks>
        /// <param name="string">containing a numeric value with a metric unit</param>
        /// <returns>int position between the numeric value and unit or 0 if string is null or string started with a non-numeric value.
        ///     </returns>
        private static int DeterminePositionBetweenValueAndUnit(String @string) {
            if (@string == null) {
                return 0;
            }
            int pos = 0;
            while (pos < @string.Length) {
                if (@string[pos] == '+' || @string[pos] == '-' || @string[pos] == '.' || @string[pos] >= '0' && @string[pos
                    ] <= '9') {
                    pos++;
                }
                else {
                    break;
                }
            }
            return pos;
        }

        /// <summary>
        /// /
        /// Checks whether a string contains an allowed metric unit in HTML/CSS; px, in, cm, mm, pc or pt.
        /// </summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains an allowed metric value.</returns>
        public static bool IsMetricValue(String value) {
            if (value == null) {
                return false;
            }
            foreach (String metricPostfix in METRIC_MEASUREMENTS) {
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
            foreach (String relativePostfix in RELATIVE_MEASUREMENTS) {
                if (value.EndsWith(relativePostfix) && IsNumericValue(value.JSubstring(0, value.Length - relativePostfix.Length
                    ).Trim())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set root value.</summary>
        /// <param name="value">the string that needs to be checked.</param>
        /// <returns>boolean true if value contains an allowed metric value.</returns>
        public static bool IsRemValue(String value) {
            return value != null && value.EndsWith(CssConstants.REM) && IsNumericValue(value.JSubstring(0, value.Length
                 - CssConstants.REM.Length).Trim());
        }

        /// <summary>Checks whether a string matches a numeric value (e.g.</summary>
        /// <remarks>Checks whether a string matches a numeric value (e.g. 123, 1.23, .123). All these metric values are allowed in HTML/CSS.
        ///     </remarks>
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
        /// <c>file.jpg</c>
        /// .
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
            /*
            return value.contains("rgb(") || value.contains("rgba(") || value.contains("#")
            || WebColors.NAMES.containsKey(value.toLowerCase()) || CssConstants.TRANSPARENT.equals(value);
            */
            //TODO re-add Webcolors by either creating a dependency on kernel or moving webcolors to io
            return value.Contains("rgb(") || value.Contains("rgba(") || value.Contains("#") || CssConstants.TRANSPARENT
                .Equals(value);
        }

        /// <summary>Helper method for comparing floating point numbers</summary>
        /// <returns>true if both floating point numbers are close enough to be considered equal</returns>
        public static bool CompareFloats(double f1, double f2) {
            return (Math.Abs(f1 - f2) < EPSILON);
        }
    }
}
