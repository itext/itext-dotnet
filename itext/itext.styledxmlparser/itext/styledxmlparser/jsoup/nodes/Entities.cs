/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Parser;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>HTML entities, and escape routines.</summary>
    /// <remarks>
    /// HTML entities, and escape routines. Source: <a href="http://www.w3.org/tr/html5/named-character-references.html#named-character-references">W3C
    /// HTML named character references</a>.
    /// </remarks>
    public class Entities {
        private const int empty = -1;

        private const String emptyName = "";

        internal const int codepointRadix = 36;

        private static readonly char[] codeDelims = new char[] { ',', ';' };

        private static readonly Dictionary<String, String> multipoints = new Dictionary<String, String>();

        // name -> multiple character references
        private static readonly OutputSettings DefaultOutput = new OutputSettings();

        public class EscapeMode {
            /// <summary>Restricted entities suitable for XHTML output: lt, gt, amp, and quot only.</summary>
            public static Entities.EscapeMode xhtml = new Entities.EscapeMode(EntitiesData.xmlPoints, 4);

            /// <summary>Default HTML output entities.</summary>
            public static Entities.EscapeMode @base = new Entities.EscapeMode(EntitiesData.basePoints, 106);

            /// <summary>Complete HTML entities.</summary>
            public static Entities.EscapeMode extended = new Entities.EscapeMode(EntitiesData.fullPoints, 2125);

            // table of named references to their codepoints. sorted so we can binary search. built by BuildEntities.
            internal String[] nameKeys;

            internal int[] codeVals;

            // limitation is the few references with multiple characters; those go into multipoints.
            // table of codepoints to named entities.
            internal int[] codeKeys;

            // we don't support multicodepoints to single named value currently
            internal String[] nameVals;

            internal EscapeMode(String file, int size) {
                Load(this, file, size);
            }

            internal virtual int CodepointForName(String name) {
                int index = Array.IndexOf(nameKeys, name);
                return index >= 0 ? codeVals[index] : empty;
            }

            internal virtual String NameForCodepoint(int codepoint) {
                int index = JavaUtil.ArraysBinarySearch(codeKeys, codepoint);
                if (index >= 0) {
                    // the results are ordered so lower case versions of same codepoint come after uppercase, and we prefer to emit lower
                    // (and binary search for same item with multi results is undefined
                    return (index < nameVals.Length - 1 && codeKeys[index + 1] == codepoint) ? nameVals[index + 1] : nameVals[
                        index];
                }
                return emptyName;
            }

            private int Size() {
                return nameKeys.Length;
            }
        }

        private Entities() {
        }

        /// <summary>Check if the input is a known named entity</summary>
        /// <param name="name">the possible entity name (e.g. "lt" or "amp")</param>
        /// <returns>true if a known named entity</returns>
        public static bool IsNamedEntity(String name) {
            return Entities.EscapeMode.extended.CodepointForName(name) != empty;
        }

        /// <summary>Check if the input is a known named entity in the base entity set.</summary>
        /// <param name="name">the possible entity name (e.g. "lt" or "amp")</param>
        /// <returns>true if a known named entity in the base set</returns>
        /// <seealso cref="IsNamedEntity(System.String)"/>
        public static bool IsBaseNamedEntity(String name) {
            return Entities.EscapeMode.@base.CodepointForName(name) != empty;
        }

        /// <summary>Get the character(s) represented by the named entity</summary>
        /// <param name="name">entity (e.g. "lt" or "amp")</param>
        /// <returns>the string value of the character(s) represented by this entity, or "" if not defined</returns>
        public static String GetByName(String name) {
            String val = multipoints.Get(name);
            if (val != null) {
                return val;
            }
            int codepoint = Entities.EscapeMode.extended.CodepointForName(name);
            if (codepoint != empty) {
                return new String(new char[] { (char)codepoint }, 0, 1);
            }
            return emptyName;
        }

        public static int CodepointsForName(String name, int[] codepoints) {
            String val = multipoints.Get(name);
            if (val != null) {
                codepoints[0] = val.CodePointAt(0);
                codepoints[1] = val.CodePointAt(1);
                return 2;
            }
            int codepoint = Entities.EscapeMode.extended.CodepointForName(name);
            if (codepoint != empty) {
                codepoints[0] = codepoint;
                return 1;
            }
            return 0;
        }

        /// <summary>HTML escape an input string.</summary>
        /// <remarks>
        /// HTML escape an input string. That is,
        /// <c>&lt;</c>
        /// is returned as
        /// <c>&amp;lt;</c>
        /// </remarks>
        /// <param name="string">the un-escaped string to escape</param>
        /// <param name="out">the output settings to use</param>
        /// <returns>the escaped string</returns>
        public static String Escape(String @string, OutputSettings @out) {
            if (@string == null) {
                return "";
            }
            StringBuilder accum = iText.StyledXmlParser.Jsoup.Internal.StringUtil.BorrowBuilder();
            try {
                Escape(accum, @string, @out, false, false, false);
            }
            catch (System.IO.IOException e) {
                throw new SerializationException(e);
            }
            // doesn't happen
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.ReleaseBuilder(accum);
        }

        /// <summary>HTML escape an input string, using the default settings (UTF-8, base entities).</summary>
        /// <remarks>
        /// HTML escape an input string, using the default settings (UTF-8, base entities). That is,
        /// <c>&lt;</c>
        /// is returned as
        /// <c>&amp;lt;</c>
        /// </remarks>
        /// <param name="string">the un-escaped string to escape</param>
        /// <returns>the escaped string</returns>
        public static String Escape(String @string) {
            return Escape(@string, DefaultOutput);
        }

        // this method is ugly, and does a lot. but other breakups cause rescanning and stringbuilder generations
        internal static void Escape(StringBuilder accum, String str, OutputSettings @out, bool inAttribute, bool normaliseWhite
            , bool stripLeadingWhite) {
            bool lastWasWhite = false;
            bool reachedNonWhite = false;
            Entities.EscapeMode escapeMode = @out.EscapeMode();
            System.Text.Encoding encoder = @out.Encoder();
            Entities.CoreCharset coreCharset = @out.coreCharset;
            // init in out.prepareEncoder()
            int length = str.Length;
            int codePoint;
            for (int offset = 0; offset < length; offset += iText.IO.Util.TextUtil.CharCount(codePoint)) {
                codePoint = str.CodePointAt(offset);
                if (normaliseWhite) {
                    if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.IsWhitespace(codePoint)) {
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
                            // escape when in character data or when in a xml attribute val or XML syntax; not needed in html attr val
                            if (!inAttribute || escapeMode == Entities.EscapeMode.xhtml || @out.Syntax() == iText.StyledXmlParser.Jsoup.Nodes.Syntax
                                .xml) {
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
                                AppendEncoded(accum, escapeMode, codePoint);
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
                        AppendEncoded(accum, escapeMode, codePoint);
                    }
                }
            }
        }

        private static void AppendEncoded(StringBuilder accum, Entities.EscapeMode escapeMode, int codePoint) {
            String name = escapeMode.NameForCodepoint(codePoint);
            if (!emptyName.Equals(name)) {
                // ok for identity check
                accum.Append('&').Append(name).Append(';');
            }
            else {
                accum.Append("&#x").Append(JavaUtil.IntegerToHexString(codePoint)).Append(';');
            }
        }

        /// <summary>Un-escape an HTML escaped string.</summary>
        /// <remarks>
        /// Un-escape an HTML escaped string. That is,
        /// <c>&amp;lt;</c>
        /// is returned as
        /// <c>&lt;</c>.
        /// </remarks>
        /// <param name="string">the HTML string to un-escape</param>
        /// <returns>the unescaped string</returns>
        public static String Unescape(String @string) {
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

        internal enum CoreCharset {
            ascii,
            utf,
            fallback
        }

        internal static Entities.CoreCharset GetCoreCharsetByName(String name) {
            if (name.Equals("US-ASCII")) {
                return Entities.CoreCharset.ascii;
            }
            if (name.StartsWith("UTF-")) {
                // covers UTF-8, UTF-16, et al
                return Entities.CoreCharset.utf;
            }
            return Entities.CoreCharset.fallback;
        }

        private static void Load(Entities.EscapeMode e, String pointsData, int size) {
            e.nameKeys = new String[size];
            e.codeVals = new int[size];
            e.codeKeys = new int[size];
            e.nameVals = new String[size];
            int i = 0;
            CharacterReader reader = new CharacterReader(pointsData);
            while (!reader.IsEmpty()) {
                // NotNestedLessLess=10913,824;1887&
                String name = reader.ConsumeTo('=');
                reader.Advance();
                int cp1 = PortUtil.ToInt32(reader.ConsumeToAny(codeDelims), codepointRadix);
                char codeDelim = reader.Current();
                reader.Advance();
                int cp2;
                if (codeDelim == ',') {
                    cp2 = PortUtil.ToInt32(reader.ConsumeTo(';'), codepointRadix);
                    reader.Advance();
                }
                else {
                    cp2 = empty;
                }
                String indexS = reader.ConsumeTo('&');
                int index = PortUtil.ToInt32(indexS, codepointRadix);
                reader.Advance();
                e.nameKeys[i] = name;
                e.codeVals[i] = cp1;
                e.codeKeys[index] = cp1;
                e.nameVals[index] = name;
                if (cp2 != empty) {
                    multipoints.Put(name, new String(new char[] { (char)cp1, (char)cp2 }, 0, 2));
                }
                i++;
            }
            Validate.IsTrue(i == size, "Unexpected count of entities loaded");
        }
    }
}
