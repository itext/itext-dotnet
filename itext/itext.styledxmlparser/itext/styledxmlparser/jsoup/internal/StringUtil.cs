/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Internal {
    /// <summary>A minimal String utility class.</summary>
    /// <remarks>
    /// A minimal String utility class. Designed for <b>internal</b> jsoup use only - the API and outcome may change without
    /// notice.
    /// </remarks>
    public sealed class StringUtil {
        // memoised padding up to 21
        internal static readonly String[] padding = new String[] { "", " ", "  ", "   ", "    ", "     ", "      "
            , "       ", "        ", "         ", "          ", "           ", "            ", "             ", "              "
            , "               ", "                ", "                 ", "                  ", "                   "
            , "                    " };

        private const int maxPaddingWidth = 30;

        // so very deeply nested nodes don't get insane padding amounts
        /// <summary>Join a collection of strings by a separator</summary>
        /// <param name="strings">collection of string objects</param>
        /// <param name="sep">string to place between strings</param>
        /// <returns>joined string</returns>
        public static String Join<_T0>(ICollection<_T0> strings, String sep) {
            return Join(strings.GetEnumerator(), sep);
        }

        /// <summary>Join a collection of strings by a separator</summary>
        /// <param name="strings">iterator of string objects</param>
        /// <param name="sep">string to place between strings</param>
        /// <returns>joined string</returns>
        public static String Join<_T0>(IEnumerator<_T0> strings, String sep) {
            if (!strings.MoveNext()) {
                return "";
            }
            String start = strings.Current.ToString();
            if (!strings.MoveNext()) {
                // only one, avoid builder
                return start;
            }
            StringUtil.StringJoiner j = new StringUtil.StringJoiner(sep);
            j.Add(start);
            j.Add(strings.Current);
            while (strings.MoveNext()) {
                j.Add(strings.Current);
            }
            return j.Complete();
        }

        /// <summary>Join an array of strings by a separator</summary>
        /// <param name="strings">collection of string objects</param>
        /// <param name="sep">string to place between strings</param>
        /// <returns>joined string</returns>
        public static String Join(String[] strings, String sep) {
            return Join(JavaUtil.ArraysAsList(strings), sep);
        }

        /// <summary>A StringJoiner allows incremental / filtered joining of a set of stringable objects.</summary>
        /// <since>1.14.1</since>
        public class StringJoiner {
            internal StringBuilder sb = BorrowBuilder();

            // sets null on builder release so can't accidentally be reused
            internal readonly String separator;

            internal bool first = true;

            /// <summary>Create a new joiner, that uses the specified separator.</summary>
            /// <remarks>
            /// Create a new joiner, that uses the specified separator. MUST call
            /// <see cref="Complete()"/>
            /// or will leak a thread
            /// local string builder.
            /// </remarks>
            /// <param name="separator">the token to insert between strings</param>
            public StringJoiner(String separator) {
                this.separator = separator;
            }

            /// <summary>Add another item to the joiner, will be separated</summary>
            public virtual StringUtil.StringJoiner Add(Object stringy) {
                Validate.NotNull(sb);
                // don't reuse
                if (!first) {
                    sb.Append(separator);
                }
                sb.Append(stringy);
                first = false;
                return this;
            }

            /// <summary>Append content to the current item; not separated</summary>
            public virtual StringUtil.StringJoiner Append(Object stringy) {
                Validate.NotNull(sb);
                // don't reuse
                sb.Append(stringy);
                return this;
            }

            /// <summary>Return the joined string, and release the builder back to the pool.</summary>
            /// <remarks>Return the joined string, and release the builder back to the pool. This joiner cannot be reused.
            ///     </remarks>
            public virtual String Complete() {
                String @string = ReleaseBuilder(sb);
                sb = null;
                return @string;
            }
        }

        /// <summary>Returns space padding (up to a max of 30).</summary>
        /// <param name="width">amount of padding desired</param>
        /// <returns>string of spaces * width</returns>
        public static String Padding(int width) {
            if (width < 0) {
                throw new ArgumentException("width must be > 0");
            }
            if (width < padding.Length) {
                return padding[width];
            }
            width = Math.Min(width, maxPaddingWidth);
            char[] @out = new char[width];
            for (int i = 0; i < width; i++) {
                @out[i] = ' ';
            }
            return JavaUtil.GetStringForChars(@out);
        }

        /// <summary>Tests if a string is blank: null, empty, or only whitespace (" ", \r\n, \t, etc)</summary>
        /// <param name="string">string to test</param>
        /// <returns>if string is blank</returns>
        public static bool IsBlank(String @string) {
            if (@string == null || @string.Length == 0) {
                return true;
            }
            int l = @string.Length;
            for (int i = 0; i < l; i++) {
                if (!StringUtil.IsWhitespace(@string.CodePointAt(i))) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Tests if a string is numeric, i.e. contains only digit characters</summary>
        /// <param name="string">string to test</param>
        /// <returns>true if only digit chars, false if empty or null or contains non-digit chars</returns>
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
        /// <remarks>Tests if a code point is "whitespace" as defined in the HTML spec. Used for output HTML.</remarks>
        /// <param name="c">code point to test</param>
        /// <returns>true if code point is whitespace, false otherwise</returns>
        /// <seealso cref="IsActuallyWhitespace(int)"/>
        public static bool IsWhitespace(int c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\f' || c == '\r';
        }

        /// <summary>Tests if a code point is "whitespace" as defined by what it looks like.</summary>
        /// <remarks>Tests if a code point is "whitespace" as defined by what it looks like. Used for Element.text etc.
        ///     </remarks>
        /// <param name="c">code point to test</param>
        /// <returns>true if code point is whitespace, false otherwise</returns>
        public static bool IsActuallyWhitespace(int c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\f' || c == '\r' || c == 160;
        }

        // 160 is &nbsp; (non-breaking space). Not in the spec but expected.
        public static bool IsInvisibleChar(int c) {
            return c == 8203 || c == 173;
        }

        // zero width sp, soft hyphen
        // previously also included zw non join, zw join - but removing those breaks semantic meaning of text
        /// <summary>
        /// Normalise the whitespace within this string; multiple spaces collapse to a single, and all whitespace characters
        /// (e.g. newline, tab) convert to a simple space
        /// </summary>
        /// <param name="string">content to normalise</param>
        /// <returns>normalised string</returns>
        public static String NormaliseWhitespace(String @string) {
            StringBuilder sb = StringUtil.BorrowBuilder();
            AppendNormalisedWhitespace(sb, @string, false);
            return StringUtil.ReleaseBuilder(sb);
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
            for (int i = 0; i < len; i += iText.IO.Util.TextUtil.CharCount(c)) {
                c = @string.CodePointAt(i);
                if (IsActuallyWhitespace(c)) {
                    if ((stripLeading && !reachedNonWhite) || lastWasWhite) {
                        continue;
                    }
                    accum.Append(' ');
                    lastWasWhite = true;
                }
                else {
                    if (!IsInvisibleChar(c)) {
                        accum.AppendCodePoint(c);
                        lastWasWhite = false;
                        reachedNonWhite = true;
                    }
                }
            }
        }

        public static bool In(String needle, params String[] haystack) {
            int len = haystack.Length;
            for (int i = 0; i < len; i++) {
                if (haystack[i].Equals(needle)) {
                    return true;
                }
            }
            return false;
        }

        public static bool InSorted(String needle, String[] haystack) {
            return JavaUtil.ArraysBinarySearch(haystack, needle) >= 0;
        }

        /// <summary>Tests that a String contains only ASCII characters.</summary>
        /// <param name="string">scanned string</param>
        /// <returns>true if all characters are in range 0 - 127</returns>
        public static bool IsAscii(String @string) {
            Validate.NotNull(@string);
            for (int i = 0; i < @string.Length; i++) {
                int c = @string[i];
                if (c > 127) {
                    // ascii range
                    return false;
                }
            }
            return true;
        }

        private static Regex extraDotSegmentsPattern = iText.Commons.Utils.StringUtil.RegexCompile("^/((\\.{1,2}/)+)");

        /// <summary>Create a new absolute URL, from a provided existing absolute URL and a relative URL component.</summary>
        /// <param name="base">the existing absolute base URL</param>
        /// <param name="relUrl">the relative URL to resolve. (If it's already absolute, it will be returned)</param>
        /// <returns>the resolved absolute URL</returns>
        public static Uri Resolve(Uri @base, String relUrl) {
            // workaround: java resolves '//path/file + ?foo' to '//path/?foo', not '//path/file?foo' as desired
            if (relUrl.StartsWith("?")) {
                relUrl = @base.AbsolutePath + relUrl;
            }
            // workaround: //example.com + ./foo = //example.com/./foo, not //example.com/foo
            // seems fine in C#
            return new Uri(@base, relUrl);
        }

        /// <summary>Create a new absolute URL, from a provided existing absolute URL and a relative URL component.</summary>
        /// <param name="baseUrl">the existing absolute base URL</param>
        /// <param name="relUrl">the relative URL to resolve. (If it's already absolute, it will be returned)</param>
        /// <returns>an absolute URL if one was able to be generated, or the empty string if not</returns>
        public static String Resolve(String baseUrl, String relUrl) {
            Uri @base;
            try {
                try {
                    @base = new Uri(baseUrl);
                }
                catch (UriFormatException) {
                    // the base is unsuitable, but the attribute/rel may be abs on its own, so try that
                    Uri abs = new Uri(relUrl);
                    return abs.ToExternalForm();
                }
                return Resolve(@base, relUrl).ToExternalForm();
            }
            catch (UriFormatException) {
                return "";
            }
        }

        private static readonly ThreadLocal<Stack<StringBuilder>> threadLocalBuilders =
            new ThreadLocal<Stack<StringBuilder>>(() => new Stack<StringBuilder>());

        /// <summary>Maintains cached StringBuilders in a flyweight pattern, to minimize new StringBuilder GCs.</summary>
        /// <remarks>
        /// Maintains cached StringBuilders in a flyweight pattern, to minimize new StringBuilder GCs. The StringBuilder is
        /// prevented from growing too large.
        /// <para />
        /// Care must be taken to release the builder once its work has been completed, with
        /// <see cref="ReleaseBuilder(System.Text.StringBuilder)"/>
        /// </remarks>
        /// <returns>an empty StringBuilder</returns>
        public static StringBuilder BorrowBuilder() {
            Stack<StringBuilder> builders = threadLocalBuilders.Value;
            return builders.IsEmpty() ? new StringBuilder(MaxCachedBuilderSize) : builders.Pop();
        }

        /// <summary>Release a borrowed builder.</summary>
        /// <remarks>
        /// Release a borrowed builder. Care must be taken not to use the builder after it has been returned, as its
        /// contents may be changed by this method, or by a concurrent thread.
        /// </remarks>
        /// <param name="sb">the StringBuilder to release.</param>
        /// <returns>the string value of the released String Builder (as an incentive to release it!).</returns>
        public static String ReleaseBuilder(StringBuilder sb) {
            Validate.NotNull(sb);
            String @string = sb.ToString();
            if (sb.Length > MaxCachedBuilderSize) {
                sb = new StringBuilder(MaxCachedBuilderSize);
            }
            else {
                // make sure it hasn't grown too big
                sb.Delete(0, sb.Length);
            }
            // make sure it's emptied on release
            Stack<StringBuilder> builders = threadLocalBuilders.Value;
            builders.Push(sb);
            while (builders.Count > MaxIdleBuilders) {
                builders.Pop();
            }
            return @string;
        }

        private const int MaxCachedBuilderSize = 8 * 1024;

        private const int MaxIdleBuilders = 8;
    }
}
