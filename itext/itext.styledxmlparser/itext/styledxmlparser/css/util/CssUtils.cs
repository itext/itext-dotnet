/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class for CSS operations.</summary>
    public class CssUtils {
        // TODO (DEVSIX-3596) The list of the font-relative measurements is not full.
        //  Add 'ch' units to array and move this array to the CommonCssConstants
        [Obsolete]
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

        // Empty constructor
        /// <summary>
        /// Splits the provided
        /// <see cref="System.String"/>
        /// by comma with respect of brackets.
        /// </summary>
        /// <param name="value">to split</param>
        /// <returns>
        /// the
        /// <see cref="System.Collections.IList{E}"/>
        /// of split result
        /// </returns>
        public static IList<String> SplitStringWithComma(String value) {
            return SplitString(value, ',', new EscapeGroup('(', ')'));
        }

        /// <summary>
        /// Splits the provided
        /// <see cref="System.String"/>
        /// by split character with respect of escape characters.
        /// </summary>
        /// <param name="value">value to split</param>
        /// <param name="splitChar">character to split the String</param>
        /// <param name="escapeCharacters">escape characters</param>
        /// <returns>
        /// the
        /// <see cref="System.Collections.IList{E}"/>
        /// of split result
        /// </returns>
        public static IList<String> SplitString(String value, char splitChar, params EscapeGroup[] escapeCharacters
            ) {
            if (value == null) {
                return new List<String>();
            }
            IList<String> resultList = new List<String>();
            int lastSplitChar = 0;
            for (int i = 0; i < value.Length; ++i) {
                char currentChar = value[i];
                bool isEscaped = false;
                foreach (EscapeGroup character in escapeCharacters) {
                    if (currentChar == splitChar) {
                        isEscaped = isEscaped || character.IsEscaped();
                    }
                    else {
                        character.ProcessCharacter(currentChar);
                    }
                }
                if (currentChar == splitChar && !isEscaped) {
                    resultList.Add(value.JSubstring(lastSplitChar, i));
                    lastSplitChar = i + 1;
                }
            }
            String lastToken = value.Substring(lastSplitChar);
            if (!String.IsNullOrEmpty(lastToken)) {
                resultList.Add(lastToken);
            }
            return resultList;
        }

        /// <summary>Parses the given css blend mode value.</summary>
        /// <remarks>
        /// Parses the given css blend mode value. If the argument is
        /// <see langword="null"/>
        /// or an unknown blend
        /// mode, then the default css
        /// <see cref="iText.Layout.Properties.BlendMode.NORMAL"/>
        /// value would be returned.
        /// </remarks>
        /// <param name="cssValue">the value to parse</param>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Properties.BlendMode"/>
        /// instance representing the parsed value
        /// </returns>
        public static BlendMode ParseBlendMode(String cssValue) {
            if (cssValue == null) {
                return BlendMode.NORMAL;
            }
            switch (cssValue) {
                case CommonCssConstants.MULTIPLY: {
                    return BlendMode.MULTIPLY;
                }

                case CommonCssConstants.SCREEN: {
                    return BlendMode.SCREEN;
                }

                case CommonCssConstants.OVERLAY: {
                    return BlendMode.OVERLAY;
                }

                case CommonCssConstants.DARKEN: {
                    return BlendMode.DARKEN;
                }

                case CommonCssConstants.LIGHTEN: {
                    return BlendMode.LIGHTEN;
                }

                case CommonCssConstants.COLOR_DODGE: {
                    return BlendMode.COLOR_DODGE;
                }

                case CommonCssConstants.COLOR_BURN: {
                    return BlendMode.COLOR_BURN;
                }

                case CommonCssConstants.HARD_LIGHT: {
                    return BlendMode.HARD_LIGHT;
                }

                case CommonCssConstants.SOFT_LIGHT: {
                    return BlendMode.SOFT_LIGHT;
                }

                case CommonCssConstants.DIFFERENCE: {
                    return BlendMode.DIFFERENCE;
                }

                case CommonCssConstants.EXCLUSION: {
                    return BlendMode.EXCLUSION;
                }

                case CommonCssConstants.HUE: {
                    return BlendMode.HUE;
                }

                case CommonCssConstants.SATURATION: {
                    return BlendMode.SATURATION;
                }

                case CommonCssConstants.COLOR: {
                    return BlendMode.COLOR;
                }

                case CommonCssConstants.LUMINOSITY: {
                    return BlendMode.LUMINOSITY;
                }

                case CommonCssConstants.NORMAL:
                default: {
                    return BlendMode.NORMAL;
                }
            }
        }

        /// <summary>
        /// Extracts shorthand properties as list of string lists from a string, where the top level
        /// list is shorthand property and the lower level list is properties included in shorthand property.
        /// </summary>
        /// <param name="str">the source string with shorthand properties</param>
        /// <returns>the list of string lists</returns>
        public static IList<IList<String>> ExtractShorthandProperties(String str) {
            IList<IList<String>> result = new List<IList<String>>();
            IList<String> currentLayer = new List<String>();
            CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(str);
            CssDeclarationValueTokenizer.Token currentToken = tokenizer.GetNextValidToken();
            while (currentToken != null) {
                if (currentToken.GetType() == CssDeclarationValueTokenizer.TokenType.COMMA) {
                    result.Add(currentLayer);
                    currentLayer = new List<String>();
                }
                else {
                    currentLayer.Add(currentToken.GetValue());
                }
                currentToken = tokenizer.GetNextValidToken();
            }
            result.Add(currentLayer);
            return result;
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseInteger(System.String) instead"
            )]
        public static int? ParseInteger(String str) {
            return CssDimensionParsingUtils.ParseInteger(str);
        }

        /// <summary>Parses a float without throwing an exception if something goes wrong.</summary>
        /// <param name="str">a string that might be a float value</param>
        /// <returns>the float value, or null if something went wrong</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseFloat(System.String) instead"
            )]
        public static float? ParseFloat(String str) {
            return CssDimensionParsingUtils.ParseFloat(str);
        }

        /// <summary>Parses a double without throwing an exception if something goes wrong.</summary>
        /// <param name="str">a string that might be a double value</param>
        /// <returns>the double value, or null if something went wrong</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseDouble(System.String) instead"
            )]
        public static double? ParseDouble(String str) {
            return CssDimensionParsingUtils.ParseDouble(str);
        }

        /// <summary>
        /// Parses an angle with an allowed metric unit (deg, grad, rad) or numeric value (e.g. 123, 1.23,
        /// .123) to rad.
        /// </summary>
        /// <param name="angle">String containing the angle to parse</param>
        /// <param name="defaultMetric">default metric to use in case the input string does not specify a metric</param>
        /// <returns>the angle in radians</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseAngle(System.String, System.String) instead"
            )]
        public static float ParseAngle(String angle, String defaultMetric) {
            return CssDimensionParsingUtils.ParseAngle(angle, defaultMetric);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseAngle(System.String) instead"
            )]
        public static float ParseAngle(String angle) {
            return CssDimensionParsingUtils.ParseAngle(angle);
        }

        /// <summary>Parses an aspect ratio into an array with two integers.</summary>
        /// <param name="str">a string that might contain two integer values</param>
        /// <returns>the aspect ratio as an array of two integer values</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseAspectRatio(System.String) instead"
            )]
        public static int[] ParseAspectRatio(String str) {
            return CssDimensionParsingUtils.ParseAspectRatio(str);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseAbsoluteLength(System.String, System.String) instead"
            )]
        public static float ParseAbsoluteLength(String length, String defaultMetric) {
            return CssDimensionParsingUtils.ParseAbsoluteLength(length, defaultMetric);
        }

        /// <summary>Parses the absolute length.</summary>
        /// <param name="length">the length as a string</param>
        /// <returns>the length as a float</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseAbsoluteLength(System.String) instead"
            )]
        public static float ParseAbsoluteLength(String length) {
            return CssDimensionParsingUtils.ParseAbsoluteLength(length);
        }

        /// <summary>
        /// Parses an relative value based on the base value that was given, in the metric unit of the base value.<br />
        /// (e.g. margin=10% should be based on the page width, so if an A4 is used, the margin = 0.10*595.0 = 59.5f)
        /// </summary>
        /// <param name="relativeValue">in %, em or ex</param>
        /// <param name="baseValue">the value the returned float is based on</param>
        /// <returns>the parsed float in the metric unit of the base value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseRelativeValue(System.String, float) instead"
            )]
        public static float ParseRelativeValue(String relativeValue, float baseValue) {
            return CssDimensionParsingUtils.ParseRelativeValue(relativeValue, baseValue);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseLengthValueToPt(System.String, float, float) instead"
            )]
        public static UnitValue ParseLengthValueToPt(String value, float emValue, float remValue) {
            return CssDimensionParsingUtils.ParseLengthValueToPt(value, emValue, remValue);
        }

        /// <summary>Checks if a string is in a valid format.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value is in a valid format</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsValidNumericValue(System.String) instead"
            )]
        public static bool IsValidNumericValue(String value) {
            return CssTypesValidationUtils.IsValidNumericValue(value);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseAbsoluteFontSize(System.String, System.String) instead"
            )]
        public static float ParseAbsoluteFontSize(String fontSizeValue, String defaultMetric) {
            return CssDimensionParsingUtils.ParseAbsoluteFontSize(fontSizeValue, defaultMetric);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseAbsoluteFontSize(System.String) instead"
            )]
        public static float ParseAbsoluteFontSize(String fontSizeValue) {
            return CssDimensionParsingUtils.ParseAbsoluteFontSize(fontSizeValue);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseRelativeFontSize(System.String, float) instead"
            )]
        public static float ParseRelativeFontSize(String relativeFontSizeValue, float baseValue) {
            return CssDimensionParsingUtils.ParseRelativeFontSize(relativeFontSizeValue, baseValue);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, useCssDimensionParsingUtils.ParseSpecificCornerBorderRadius(System.String, float, float) instead"
            )]
        public static UnitValue[] ParseSpecificCornerBorderRadius(String specificBorderRadius, float emValue, float
             remValue) {
            return CssDimensionParsingUtils.ParseSpecificCornerBorderRadius(specificBorderRadius, emValue, remValue);
        }

        /// <summary>Parses the resolution.</summary>
        /// <param name="resolutionStr">the resolution as a string</param>
        /// <returns>a value in dpi</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseResolution(System.String) instead"
            )]
        public static float ParseResolution(String resolutionStr) {
            return CssDimensionParsingUtils.ParseResolution(resolutionStr);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, useCssDimensionParsingUtils.DeterminePositionBetweenValueAndUnit(System.String) instead"
            )]
        public static int DeterminePositionBetweenValueAndUnit(String @string) {
            return CssDimensionParsingUtils.DeterminePositionBetweenValueAndUnit(@string);
        }

        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; px, in, cm, mm, pc, Q or pt.
        ///     </summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsMetricValue(System.String) instead"
            )]
        public static bool IsMetricValue(String value) {
            return CssTypesValidationUtils.IsMetricValue(value);
        }

        /// <summary>Checks whether a string contains an allowed metric unit in HTML/CSS; rad, deg and grad.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed angle value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsAngleValue(System.String) instead"
            )]
        public static bool IsAngleValue(String value) {
            return CssTypesValidationUtils.IsAngleValue(value);
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set value.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsRelativeValue(System.String) instead"
            )]
        public static bool IsRelativeValue(String value) {
            return CssTypesValidationUtils.IsRelativeValue(value);
        }

        /// <summary>Checks whether a string contains an allowed value relative to font.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed font relative value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsRelativeValue(System.String) method instead"
            )]
        public static bool IsFontRelativeValue(String value) {
            if (value == null) {
                return false;
            }
            foreach (String relativePostfix in FONT_RELATIVE_MEASUREMENTS_VALUES) {
                if (value.EndsWith(relativePostfix) && CssTypesValidationUtils.IsNumericValue(value.JSubstring(0, value.Length
                     - relativePostfix.Length).Trim())) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Checks whether a string contains a percentage value</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed percentage value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsPercentageValue(System.String) method instead"
            )]
        public static bool IsPercentageValue(String value) {
            return CssTypesValidationUtils.IsPercentageValue(value);
        }

        /// <summary>Checks whether a string contains an allowed value relative to previously set root value.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a rem value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsRemValue(System.String) method instead"
            )]
        public static bool IsRemValue(String value) {
            return CssTypesValidationUtils.IsRemValue(value);
        }

        /// <summary>Checks whether a string contains an allowed value relative to parent value.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a em value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsEmValue(System.String) method instead"
            )]
        public static bool IsEmValue(String value) {
            return CssTypesValidationUtils.IsEmValue(value);
        }

        /// <summary>Checks whether a string contains an allowed value relative to element font height.</summary>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains a ex value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsExValue(System.String) method instead"
            )]
        public static bool IsExValue(String value) {
            return CssTypesValidationUtils.IsExValue(value);
        }

        /// <summary>Checks whether a string matches a numeric value (e.g. 123, 1.23, .123).</summary>
        /// <remarks>
        /// Checks whether a string matches a numeric value (e.g. 123, 1.23, .123). All these metric values are allowed in
        /// HTML/CSS.
        /// </remarks>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>boolean true if value contains an allowed metric value</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsNumericValue(System.String) method instead"
            )]
        public static bool IsNumericValue(String value) {
            return CssTypesValidationUtils.IsNumericValue(value);
        }

        /// <summary>Checks whether a string matches a negative value (e.g. -123, -2em, -0.123).</summary>
        /// <remarks>
        /// Checks whether a string matches a negative value (e.g. -123, -2em, -0.123).
        /// All these metric values are allowed in HTML/CSS.
        /// </remarks>
        /// <param name="value">the string that needs to be checked</param>
        /// <returns>true if value is negative</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsNegativeValue(System.String) method instead"
            )]
        public static bool IsNegativeValue(String value) {
            return CssTypesValidationUtils.IsNegativeValue(value);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsBase64Data(System.String) method instead"
            )]
        public static bool IsBase64Data(String data) {
            return CssTypesValidationUtils.IsBase64Data(data);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssTypesValidationUtils.IsColorProperty(System.String) method instead"
            )]
        public static bool IsColorProperty(String value) {
            return CssTypesValidationUtils.IsColorProperty(value);
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
        [System.ObsoleteAttribute(@"will be removed in 7.2, use CssDimensionParsingUtils.ParseRgbaColor(System.String) method instead"
            )]
        public static float[] ParseRgbaColor(String colorValue) {
            return CssDimensionParsingUtils.ParseRgbaColor(colorValue);
        }

        /// <summary>Parses the unicode range.</summary>
        /// <param name="unicodeRange">the string which stores the unicode range</param>
        /// <returns>
        /// the unicode range as a
        /// <see cref="iText.Layout.Font.Range"/>
        /// object
        /// </returns>
        public static iText.Layout.Font.Range ParseUnicodeRange(String unicodeRange) {
            String[] ranges = iText.IO.Util.StringUtil.Split(unicodeRange, ",");
            RangeBuilder builder = new RangeBuilder();
            foreach (String range in ranges) {
                if (!AddRange(builder, range)) {
                    return null;
                }
            }
            return builder.Create();
        }

        /// <summary>Convert given point value to a pixel value.</summary>
        /// <remarks>Convert given point value to a pixel value. 1 px is 0.75 pts.</remarks>
        /// <param name="pts">float value to be converted to pixels</param>
        /// <returns>float converted value pts/0.75f</returns>
        public static float ConvertPtsToPx(float pts) {
            return pts / 0.75f;
        }

        /// <summary>Convert given point value to a pixel value.</summary>
        /// <remarks>Convert given point value to a pixel value. 1 px is 0.75 pts.</remarks>
        /// <param name="pts">double value to be converted to pixels</param>
        /// <returns>double converted value pts/0.75</returns>
        public static double ConvertPtsToPx(double pts) {
            return pts / 0.75;
        }

        /// <summary>Convert given point value to a point value.</summary>
        /// <remarks>Convert given point value to a point value. 1 px is 0.75 pts.</remarks>
        /// <param name="px">float value to be converted to pixels</param>
        /// <returns>float converted value px*0.75</returns>
        public static float ConvertPxToPts(float px) {
            return px * 0.75f;
        }

        /// <summary>Convert given point value to a point value.</summary>
        /// <remarks>Convert given point value to a point value. 1 px is 0.75 pts.</remarks>
        /// <param name="px">double value to be converted to pixels</param>
        /// <returns>double converted value px*0.75</returns>
        public static double ConvertPxToPts(double px) {
            return px * 0.75;
        }

        /// <summary>
        /// Checks if an
        /// <see cref="iText.StyledXmlParser.Node.IElementNode"/>
        /// represents a style sheet link.
        /// </summary>
        /// <param name="headChildElement">the head child element</param>
        /// <returns>true, if the element node represents a style sheet link</returns>
        public static bool IsStyleSheetLink(IElementNode headChildElement) {
            return CommonCssConstants.LINK.Equals(headChildElement.Name()) && CommonAttributeConstants.STYLESHEET.Equals
                (headChildElement.GetAttribute(CommonAttributeConstants.REL));
        }

        /// <summary>Checks if value is initial, inherit or unset.</summary>
        /// <param name="value">value to check</param>
        /// <returns>true if value is initial, inherit or unset. false otherwise</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2, useCssTypesValidationUtils.IsInitialOrInheritOrUnset(System.String) method instead"
            )]
        public static bool IsInitialOrInheritOrUnset(String value) {
            return CssTypesValidationUtils.IsInitialOrInheritOrUnset(value);
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
    }
}
