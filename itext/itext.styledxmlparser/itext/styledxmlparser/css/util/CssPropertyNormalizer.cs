/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class with functionality to normalize CSS properties.</summary>
    internal class CssPropertyNormalizer {
        private static readonly Regex URL_PATTERN = PortUtil.CreateRegexPatternWithDotMatchingNewlines("^[uU][rR][lL]\\("
            );

        /// <summary>Normalize a property.</summary>
        /// <param name="str">the property</param>
        /// <returns>the normalized property</returns>
        internal static String Normalize(String str) {
            StringBuilder sb = new StringBuilder();
            bool isWhitespace = false;
            int i = 0;
            while (i < str.Length) {
                if (str[i] == '\\') {
                    sb.Append(str[i]);
                    ++i;
                    if (i < str.Length) {
                        sb.Append(str[i]);
                        ++i;
                    }
                }
                else {
                    if (iText.IO.Util.TextUtil.IsWhiteSpace(str[i])) {
                        isWhitespace = true;
                        ++i;
                    }
                    else {
                        if (isWhitespace) {
                            if (sb.Length > 0 && !TrimSpaceAfter(sb[sb.Length - 1]) && !TrimSpaceBefore(str[i])) {
                                sb.Append(" ");
                            }
                            isWhitespace = false;
                        }
                        if (str[i] == '\'' || str[i] == '"') {
                            i = AppendQuotedString(sb, str, i);
                        }
                        else {
                            if ((str[i] == 'u' || str[i] == 'U') && iText.Commons.Utils.Matcher.Match(URL_PATTERN, str.Substring(i)).Find
                                ()) {
                                sb.Append(str.JSubstring(i, i + 4).ToLowerInvariant());
                                i = AppendUrlContent(sb, str, i + 4);
                            }
                            else {
                                sb.Append(char.ToLower(str[i]));
                                ++i;
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>Appends quoted string.</summary>
        /// <param name="buffer">the current buffer</param>
        /// <param name="source">a source</param>
        /// <param name="start">where to start in the source. Should point at quote symbol.</param>
        /// <returns>the new position in the source</returns>
        private static int AppendQuotedString(StringBuilder buffer, String source, int start) {
            char endQuoteSymbol = source[start];
            int end = CssUtils.FindNextUnescapedChar(source, endQuoteSymbol, start + 1);
            if (end == -1) {
                end = source.Length;
                ITextLogManager.GetLogger(typeof(CssPropertyNormalizer)).LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant
                    .QUOTE_IS_NOT_CLOSED_IN_CSS_EXPRESSION, source));
            }
            else {
                ++end;
            }
            buffer.JAppend(source, start, end);
            return end;
        }

        /// <summary>Appends url content and end parenthesis if url is correct.</summary>
        /// <param name="buffer">the current buffer</param>
        /// <param name="source">a source</param>
        /// <param name="start">where to start in the source. Should point at first symbol after "url(".</param>
        /// <returns>the new position in the source</returns>
        private static int AppendUrlContent(StringBuilder buffer, String source, int start) {
            while (iText.IO.Util.TextUtil.IsWhiteSpace(source[start]) && start < source.Length) {
                ++start;
            }
            if (start < source.Length) {
                int curr = start;
                if (source[curr] == '"' || source[curr] == '\'') {
                    curr = AppendQuotedString(buffer, source, curr);
                    return curr;
                }
                else {
                    curr = CssUtils.FindNextUnescapedChar(source, ')', curr);
                    if (curr == -1) {
                        ITextLogManager.GetLogger(typeof(CssPropertyNormalizer)).LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant
                            .URL_IS_NOT_CLOSED_IN_CSS_EXPRESSION, source));
                        return source.Length;
                    }
                    else {
                        buffer.Append(source.JSubstring(start, curr).Trim());
                        buffer.Append(')');
                        return curr + 1;
                    }
                }
            }
            else {
                ITextLogManager.GetLogger(typeof(CssPropertyNormalizer)).LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant
                    .URL_IS_EMPTY_IN_CSS_EXPRESSION, source));
                return source.Length;
            }
        }

        /// <summary>Checks if spaces can be trimmed after a specific character.</summary>
        /// <param name="ch">the character</param>
        /// <returns>true, if spaces can be trimmed after the character</returns>
        private static bool TrimSpaceAfter(char ch) {
            return ch == ',' || ch == '(';
        }

        /// <summary>Checks if spaces can be trimmed before a specific character.</summary>
        /// <param name="ch">the character</param>
        /// <returns>true, if spaces can be trimmed before the character</returns>
        private static bool TrimSpaceBefore(char ch) {
            return ch == ',' || ch == ')';
        }
    }
}
