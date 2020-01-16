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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using iText.StyledXmlParser.Jsoup;

namespace iText.StyledXmlParser.Jsoup.Helper {
    /// <summary>A minimal String utility class.</summary>
    /// <remarks>A minimal String utility class. Designed for internal jsoup use only.</remarks>
    public sealed class StringUtil {
        // memoised padding up to 10
        /// <summary>Join a collection of strings by a seperator</summary>
        /// <param name="strings">collection of string objects</param>
        /// <param name="sep">string to place between strings</param>
        /// <returns>joined string</returns>
        private static readonly String[] padding = new String[] { "", " ", "  ", "   ", "    ", "     ", "      ", 
            "       ", "        ", "         ", "          " };


        /// <summary>Join a collection of strings by a seperator</summary>
        /// <param name="strings">iterator of string objects</param>
        /// <param name="sep">string to place between strings</param>
        /// <returns>joined string</returns>
        //public static String Join(IEnumerable strings, String sep) {
        //    return Join(strings.GetEnumerator(), sep);
        //}

        public static String Join<T>(IEnumerable<T> strings, String sep) {
            return Join(strings.GetEnumerator(), sep);
        }

        public static String Join(IEnumerator strings, String sep) {
            if (!strings.MoveNext()) {
                return "";
            }
            String start = strings.Current.ToString();
            if (!strings.MoveNext()) {
                return start;
            }
            StringBuilder sb = new StringBuilder(64).Append(start).Append(sep).Append(strings.Current);
            while (strings.MoveNext()) {
                sb.Append(sep);
                sb.Append(strings.Current);
            }
            return sb.ToString();
        }

        /// <summary>Returns space padding</summary>
        /// <param name="width">amount of padding desired</param>
        /// <returns>string of spaces * width</returns>
        public static String Padding(int width) {
            if (width < 0) {
                throw new ArgumentException("width must be > 0");
            }
            if (width < padding.Length) {
                return padding[width];
            }
            char[] @out = new char[width];
            for (int i = 0; i < width; i++) {
                @out[i] = ' ';
            }
            return new String(@out);
        }

        /// <summary>Tests if a string is blank: null, emtpy, or only whitespace (" ", \r\n, \t, etc)</summary>
        /// <param name="string">string to test</param>
        /// <returns>if string is blank</returns>
        public static bool IsBlank(String @string) {
            if (@string == null || @string.Length == 0) {
                return true;
            }
            int l = @string.Length;
            for (int i = 0; i < l; i++) {
                if (!iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace(@string.CodePointAt(i))) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Tests if a string is numeric, i.e.</summary>
        /// <remarks>Tests if a string is numeric, i.e. contains only digit characters</remarks>
        /// <param name="string">string to test</param>
        /// <returns>true if only digit chars, false if empty or null or contains non-digit chrs</returns>
        public static bool IsNumeric(String @string) {
            if (@string == null || @string.Length == 0) {
                return false;
            }
            int l = @string.Length;
            for (int i = 0; i < l; i++) {
                if (!char.IsDigit(@string[i])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Tests if a code point is "whitespace" as defined in the HTML spec.</summary>
        /// <param name="c">code point to test</param>
        /// <returns>true if code point is whitespace, false otherwise</returns>
        public static bool IsWhitespace(int c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\f' || c == '\r';
        }

        /// <summary>
        /// Normalise the whitespace within this string; multiple spaces collapse to a single, and all whitespace characters
        /// (e.g.
        /// </summary>
        /// <remarks>
        /// Normalise the whitespace within this string; multiple spaces collapse to a single, and all whitespace characters
        /// (e.g. newline, tab) convert to a simple space
        /// </remarks>
        /// <param name="string">content to normalise</param>
        /// <returns>normalised string</returns>
        public static String NormaliseWhitespace(String @string) {
            StringBuilder sb = new StringBuilder(@string.Length);
            AppendNormalisedWhitespace(sb, @string, false);
            return sb.ToString();
        }

        /// <summary>After normalizing the whitespace within a string, appends it to a string builder.</summary>
        /// <param name="accum">builder to append to</param>
        /// <param name="string">string to normalize whitespace within</param>
        /// <param name="stripLeading">set to true if you wish to remove any leading whitespace</param>
        public static void AppendNormalisedWhitespace(StringBuilder accum, String @string, bool stripLeading) {
            bool lastWasWhite = false;
            bool reachedNonWhite = false;
            int len = @string.Length;
            int c;
            for (int i = 0; i < len; i += iText.StyledXmlParser.Jsoup.PortUtil.CharCount(c)) {
                c = @string.CodePointAt(i);
                if (IsWhitespace(c)) {
                    if ((stripLeading && !reachedNonWhite) || lastWasWhite) {
                        continue;
                    }
                    accum.Append(' ');
                    lastWasWhite = true;
                }
                else {
                    accum.AppendCodePoint(c);
                    lastWasWhite = false;
                    reachedNonWhite = true;
                }
            }
        }

        public static bool In(String needle, params String[] haystack) {
            foreach (String hay in haystack) {
                if (hay.Equals(needle)) {
                    return true;
                }
            }
            return false;
        }

        public static bool InSorted(String needle, String[] haystack) {
            return iText.IO.Util.JavaUtil.ArraysBinarySearch(haystack, needle) >= 0;
        }

        /// <summary>Create a new absolute URL, from a provided existing absolute URL and a relative URL component.</summary>
        /// <param name="base">the existing absolulte base URL</param>
        /// <param name="relUrl">the relative URL to resolve. (If it's already absolute, it will be returned)</param>
        /// <returns>the resolved absolute URL</returns>
        public static Uri Resolve(Uri @base, String relUrl) {
            Uri result;
            if (!TryResolve(@base, relUrl, out result)) {
                throw new UriFormatException();
            }
            return result;
        }

        /// <summary>Create a new absolute URL, from a provided existing absolute URL and a relative URL component.</summary>
        /// <param name="baseUrl">the existing absolute base URL</param>
        /// <param name="relUrl">the relative URL to resolve. (If it's already absolute, it will be returned)</param>
        /// <returns>an absolute URL if one was able to be generated, or the empty string if not</returns>
        public static String Resolve(String baseUrl, String relUrl) {
            Uri @base, result;
            if (Uri.TryCreate(baseUrl, UriKind.Absolute, out @base)) {
                if (!TryResolve(@base, relUrl, out result)) {
                    return "";
                }
            } else if (!Uri.TryCreate(relUrl, UriKind.Absolute, out result)) {
                return "";
            }
            return result.ToExternalForm();
        }

        private static bool TryResolve(Uri @base, String relUrl, out Uri result) {
            // workaround: java resolves '//path/file + ?foo' to '//path/?foo', not '//path/file?foo' as desired
            if (relUrl.StartsWith("?"))
            {
                relUrl = @base.AbsolutePath + relUrl;
            }
            // workaround: //example.com + ./foo = //example.com/./foo, not //example.com/foo
            // seems fine in C#
            //if (relUrl.IndexOf('.') == 0 && @base.PathAndQuery.IndexOf('/') != 0) {
            //    var builder = new UriBuilder(@base.Scheme, @base.Host, @base.Port, "/" + @base.PathAndQuery);
            //}
            return Uri.TryCreate(@base, relUrl, out result);
        }
    }
}
