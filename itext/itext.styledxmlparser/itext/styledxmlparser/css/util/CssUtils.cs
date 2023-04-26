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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class for CSS operations.</summary>
    public class CssUtils {
        private const float EPSILON = 1e-6f;

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Util.CssUtils
            ));

        private const int QUANTITY_OF_PARAMS_WITH_FALLBACK_OR_TYPE = 2;

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
            String[] parts = iText.Commons.Utils.StringUtil.Split(str, "\\s");
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

        /// <summary>Parses string and return attribute value.</summary>
        /// <param name="attrStr">the string contains attr() to extract attribute value</param>
        /// <param name="element">the parentNode from which we extract information</param>
        /// <returns>the value of attribute</returns>
        public static String ExtractAttributeValue(String attrStr, IElementNode element) {
            String attrValue = null;
            if (attrStr.StartsWith(CommonCssConstants.ATTRIBUTE + '(') && attrStr.Length > CommonCssConstants.ATTRIBUTE
                .Length + 2 && attrStr.EndsWith(")")) {
                String fallback = null;
                String typeOfAttribute = null;
                String stringToSplit = attrStr.JSubstring(5, attrStr.Length - 1);
                IList<String> paramsWithFallback = SplitString(stringToSplit, ',', new EscapeGroup('\"'), new EscapeGroup(
                    '\''));
                if (paramsWithFallback.Count > QUANTITY_OF_PARAMS_WITH_FALLBACK_OR_TYPE) {
                    return null;
                }
                if (paramsWithFallback.Count == QUANTITY_OF_PARAMS_WITH_FALLBACK_OR_TYPE) {
                    fallback = ExtractFallback(paramsWithFallback[1]);
                }
                IList<String> @params = SplitString(paramsWithFallback[0], ' ');
                if (@params.Count > QUANTITY_OF_PARAMS_WITH_FALLBACK_OR_TYPE) {
                    return null;
                }
                if (@params.Count == QUANTITY_OF_PARAMS_WITH_FALLBACK_OR_TYPE) {
                    typeOfAttribute = ExtractTypeOfAttribute(@params[1]);
                    if (typeOfAttribute == null) {
                        return null;
                    }
                }
                String attributeName = @params[0];
                if (IsAttributeNameValid(attributeName)) {
                    attrValue = GetAttributeValue(attributeName, typeOfAttribute, fallback, element);
                }
            }
            return attrValue;
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

        /// <summary>Parses the unicode range.</summary>
        /// <param name="unicodeRange">the string which stores the unicode range</param>
        /// <returns>
        /// the unicode range as a
        /// <see cref="iText.Layout.Font.Range"/>
        /// object
        /// </returns>
        public static Range ParseUnicodeRange(String unicodeRange) {
            String[] ranges = iText.Commons.Utils.StringUtil.Split(unicodeRange, ",");
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

        private static bool AddRange(RangeBuilder builder, String range) {
            range = range.Trim();
            if (range.Matches("[uU]\\+[0-9a-fA-F?]{1,6}(-[0-9a-fA-F]{1,6})?")) {
                String[] parts = iText.Commons.Utils.StringUtil.Split(range.JSubstring(2, range.Length), "-");
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

        private static bool IsAttributeNameValid(String attributeName) {
            return !(attributeName.Contains("'") || attributeName.Contains("\"") || attributeName.Contains("(") || attributeName
                .Contains(")"));
        }

        private static String ExtractFallback(String fallbackString) {
            String tmpString;
            if ((fallbackString.StartsWith("'") && fallbackString.EndsWith("'")) || (fallbackString.StartsWith("\"") &&
                 fallbackString.EndsWith("\""))) {
                tmpString = fallbackString.JSubstring(1, fallbackString.Length - 1);
            }
            else {
                tmpString = fallbackString;
            }
            return ExtractUrl(tmpString);
        }

        private static String ExtractTypeOfAttribute(String typeString) {
            if (typeString.Equals(CommonCssConstants.URL) || typeString.Equals(CommonCssConstants.STRING)) {
                return typeString;
            }
            return null;
        }

        private static String GetAttributeValue(String attributeName, String typeOfAttribute, String fallback, IElementNode
             elementNode) {
            String returnString = elementNode.GetAttribute(attributeName);
            if (CommonCssConstants.URL.Equals(typeOfAttribute)) {
                returnString = returnString == null ? null : ExtractUrl(returnString);
            }
            else {
                returnString = returnString == null ? "" : returnString;
            }
            if (fallback != null && (returnString == null || String.IsNullOrEmpty(returnString))) {
                returnString = fallback;
            }
            return returnString;
        }
    }
}
