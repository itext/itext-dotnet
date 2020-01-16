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
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>HTML entities, and escape routines.</summary>
    /// <remarks>
    /// HTML entities, and escape routines.
    /// Source: <a href="http://www.w3.org/tr/html5/named-character-references.html#named-character-references">W3C HTML
    /// named character references</a>.
    /// </remarks>
    public class Entities {
        public class EscapeMode {
            /// <summary>Restricted entities suitable for XHTML output: lt, gt, amp, and quot only.</summary>
            public static readonly Entities.EscapeMode xhtml = new Entities.EscapeMode(xhtmlByVal, "xhtml");

            /// <summary>Default HTML output entities.</summary>
            public static readonly Entities.EscapeMode @base = new Entities.EscapeMode(baseByVal, "base");

            /// <summary>Complete HTML entities.</summary>
            public static readonly Entities.EscapeMode extended = new Entities.EscapeMode(fullByVal, "extended");

            private static IDictionary<String, Entities.EscapeMode> nameValueMap = new Dictionary<String, Entities.EscapeMode
                >();

            public static Entities.EscapeMode ValueOf(String name) {
                return nameValueMap.Get(name);
            }

            static EscapeMode() {
                nameValueMap.Put(xhtml.name, xhtml);
                nameValueMap.Put(@base.name, @base);
                nameValueMap.Put(extended.name, extended);
            }

            private IDictionary<char, String> map;

            private String name;

            private EscapeMode(IDictionary<char, String> map, String name) {
                this.map = map;
                this.name = name;
            }

            public virtual IDictionary<char, String> GetMap() {
                return map;
            }

            public virtual String Name() {
                return name;
            }
        }

        private static readonly IDictionary<String, char?> full;

        private static readonly IDictionary<char, String> xhtmlByVal;

        private static readonly IDictionary<String, char?> @base;

        private static readonly IDictionary<char, String> baseByVal;

        private static readonly IDictionary<char, String> fullByVal;

        private Entities() {
        }

        /// <summary>Check if the input is a known named entity</summary>
        /// <param name="name">the possible entity name (e.g. "lt" or "amp")</param>
        /// <returns>true if a known named entity</returns>
        public static bool IsNamedEntity(String name) {
            return full.ContainsKey(name);
        }

        /// <summary>Check if the input is a known named entity in the base entity set.</summary>
        /// <param name="name">the possible entity name (e.g. "lt" or "amp")</param>
        /// <returns>true if a known named entity in the base set</returns>
        /// <seealso cref="IsNamedEntity(System.String)"/>
        public static bool IsBaseNamedEntity(String name) {
            return @base.ContainsKey(name);
        }

        /// <summary>Get the Character value of the named entity</summary>
        /// <param name="name">named entity (e.g. "lt" or "amp")</param>
        /// <returns>
        /// the Character value of the named entity (e.g. '
        /// <literal></literal>
        /// ' or '
        /// <literal>&</literal>
        /// ')
        /// </returns>
        public static char? GetCharacterByName(String name) {
            return full.Get(name);
        }

        internal static String Escape(String @string, OutputSettings @out) {
            StringBuilder accum = new StringBuilder(@string.Length * 2);
            try {
                Escape(accum, @string, @out, false, false, false);
            }
            catch (System.IO.IOException e) {
                throw new SerializationException(e);
            }
            // doesn't happen
            return accum.ToString();
        }

        // this method is ugly, and does a lot. but other breakups cause rescanning and stringbuilder generations
        internal static void Escape(StringBuilder accum, String str, OutputSettings outputSettings, bool inAttribute
            , bool normaliseWhite, bool stripLeadingWhite) {
            bool lastWasWhite = false;
            bool reachedNonWhite = false;
            Entities.EscapeMode escapeMode = outputSettings.EscapeMode();
            System.Text.Encoding encoder = outputSettings.Charset();
            Entities.CoreCharset coreCharset = GetCoreCharsetByName(outputSettings.Charset().Name());
            IDictionary<char, String> map = escapeMode.GetMap();
            int length = str.Length;
            int codePoint;
            for (int offset = 0; offset < length; offset += iText.IO.Util.TextUtil.CharCount(codePoint)) {
                codePoint = str.CodePointAt(offset);
                if (normaliseWhite) {
                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace(codePoint)) {
                        if ((stripLeadingWhite && !reachedNonWhite) || lastWasWhite) {
                            continue;
                        }
                        accum.Append(' ');
                        lastWasWhite = true;
                        continue;
                    }
                    else {
                        lastWasWhite = false;
                        reachedNonWhite = true;
                    }
                }
                // surrogate pairs, split implementation for efficiency on single char common case (saves creating strings, char[]):
                if (codePoint < iText.IO.Util.TextUtil.CHARACTER_MIN_SUPPLEMENTARY_CODE_POINT) {
                    char c = (char)codePoint;
                    // html specific and required escapes:
                    switch (c) {
                        case '&': {
                            accum.Append("&amp;");
                            break;
                        }

                        case (char)0xA0: {
                            if (escapeMode != Entities.EscapeMode.xhtml) {
                                accum.Append("&nbsp;");
                            }
                            else {
                                accum.Append("&#xa0;");
                            }
                            break;
                        }

                        case '<': {
                            // escape when in character data or when in a xml attribue val; not needed in html attr val
                            if (!inAttribute || escapeMode == Entities.EscapeMode.xhtml) {
                                accum.Append("&lt;");
                            }
                            else {
                                accum.Append(c);
                            }
                            break;
                        }

                        case '>': {
                            if (!inAttribute) {
                                accum.Append("&gt;");
                            }
                            else {
                                accum.Append(c);
                            }
                            break;
                        }

                        case '"': {
                            if (inAttribute) {
                                accum.Append("&quot;");
                            }
                            else {
                                accum.Append(c);
                            }
                            break;
                        }

                        default: {
                            if (CanEncode(coreCharset, c, encoder)) {
                                accum.Append(c);
                            }
                            else {
                                if (map.ContainsKey(c)) {
                                    accum.Append('&').Append(map.Get(c)).Append(';');
                                }
                                else {
                                    accum.Append("&#x").Append(JavaUtil.IntegerToHexString(codePoint)).Append(';');
                                }
                            }
                            break;
                        }
                    }
                }
                else {
                    String c = new String(iText.IO.Util.TextUtil.ToChars(codePoint));
                    if (encoder.CanEncode(c)) {
                        // uses fallback encoder for simplicity
                        accum.Append(c);
                    }
                    else {
                        accum.Append("&#x").Append(JavaUtil.IntegerToHexString(codePoint)).Append(';');
                    }
                }
            }
        }

        internal static String Unescape(String @string) {
            return Unescape(@string, false);
        }

        /// <summary>Unescape the input string.</summary>
        /// <param name="string">to un-HTML-escape</param>
        /// <param name="strict">if "strict" (that is, requires trailing ';' char, otherwise that's optional)</param>
        /// <returns>unescaped string</returns>
        internal static String Unescape(String @string, bool strict) {
            return iText.StyledXmlParser.Jsoup.Parser.Parser.UnescapeEntities(@string, strict);
        }

        /*
        * Provides a fast-path for Encoder.canEncode, which drastically improves performance on Android post JellyBean.
        * After KitKat, the implementation of canEncode degrades to the point of being useless. For non ASCII or UTF,
        * performance may be bad. We can add more encoders for common character sets that are impacted by performance
        * issues on Android if required.
        *
        * Benchmarks:     *
        * OLD toHtml() impl v New (fastpath) in millis
        * Wiki: 1895, 16
        * CNN: 6378, 55
        * Alterslash: 3013, 28
        * Jsoup: 167, 2
        */
        private static bool CanEncode(Entities.CoreCharset charset, char c, System.Text.Encoding fallback) {
            // todo add more charset tests if impacted by Android's bad perf in canEncode
            switch (charset) {
                case Entities.CoreCharset.ascii: {
                    return c < 0x80;
                }

                case Entities.CoreCharset.utf: {
                    // real is:!(Character.isLowSurrogate(c) || Character.isHighSurrogate(c)); - but already check above
                    return true;
                }

                default: {
                    return fallback.CanEncode(c);
                }
            }
        }

        private enum CoreCharset {
            ascii,
            utf,
            fallback
        }

        private static Entities.CoreCharset GetCoreCharsetByName(String name) {
            if (name.Equals("US-ASCII")) {
                return Entities.CoreCharset.ascii;
            }
            if (name.StartsWith("UTF-")) {
                // covers UTF-8, UTF-16, et al
                return Entities.CoreCharset.utf;
            }
            return Entities.CoreCharset.fallback;
        }

        // xhtml has restricted entities
        private static readonly Object[][] xhtmlArray = new Object[][] { new Object[] { "quot", 0x00022 }, new Object
            [] { "amp", 0x00026 }, new Object[] { "lt", 0x0003C }, new Object[] { "gt", 0x0003E } };

        static Entities() {
            xhtmlByVal = new Dictionary<char, String>();
            @base = LoadEntities("entities-base.properties");
            // most common / default
            baseByVal = ToCharacterKey(@base);
            full = LoadEntities("entities-full.properties");
            // extended and overblown.
            fullByVal = ToCharacterKey(full);
            foreach (Object[] entity in xhtmlArray) {
                char c = (char)((int?)entity[1]).Value;
                xhtmlByVal.Put(c, ((String)entity[0]));
            }
        }

        private static IDictionary<String, char?> LoadEntities(String filename) {
            Properties properties = new Properties();
            IDictionary<String, char?> entities = new Dictionary<String, char?>();
            try {
                Stream @in = typeof(Entities).GetResourceAsStream(filename);
                properties.Load(@in);
                @in.Dispose();
            }
            catch (System.IO.IOException e) {
                throw new MissingResourceException("Error loading entities resource: " + e.Message, "Entities", filename);
            }
            foreach (Object name in properties.Keys) {
                char? val = (char)Convert.ToInt32(properties.GetProperty((String)name), 16);
                entities.Put((String)name, val);
            }
            return entities;
        }

        private static IDictionary<char, String> ToCharacterKey(IDictionary<String, char?> inMap) {
            IDictionary<char, String> outMap = new Dictionary<char, String>();
            foreach (KeyValuePair<String, char?> entry in inMap) {
                char character = (char)entry.Value;
                String name = entry.Key;
                if (outMap.ContainsKey(character)) {
                    // dupe, prefer the lower case version
                    if (name.ToLowerInvariant().Equals(name)) {
                        outMap.Put(character, name);
                    }
                }
                else {
                    outMap.Put(character, name);
                }
            }
            return outMap;
        }
    }
}
