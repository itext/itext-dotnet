//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.XMP;

namespace iText.Kernel.XMP.Impl {
    /// <summary>Utility functions for the XMPToolkit implementation.</summary>
    /// <since>06.06.2006</since>
    public sealed class Utils : XMPConst {
        /// <summary>segments of a UUID</summary>
        public const int UUID_SEGMENT_COUNT = 4;

        /// <summary>length of a UUID</summary>
        public const int UUID_LENGTH = 32 + UUID_SEGMENT_COUNT;

        /// <summary>table of XML name start chars (&lt;= 0xFF)</summary>
        private static bool[] xmlNameStartChars;

        /// <summary>table of XML name chars (&lt;= 0xFF)</summary>
        private static bool[] xmlNameChars;

        static Utils() {
            InitCharTables();
        }

        /// <summary>Private constructor</summary>
        private Utils() {
        }

        // EMPTY
        /// <summary>
        /// Normalize an xml:lang value so that comparisons are effectively case
        /// insensitive as required by RFC 3066 (which superceeds RFC 1766).
        /// </summary>
        /// <remarks>
        /// Normalize an xml:lang value so that comparisons are effectively case
        /// insensitive as required by RFC 3066 (which superceeds RFC 1766). The
        /// normalization rules:
        /// <list type="bullet">
        /// <item><description> The primary subtag is lower case, the suggested practice of ISO 639.
        /// </description></item>
        /// <item><description> All 2 letter secondary subtags are upper case, the suggested
        /// practice of ISO 3166.
        /// </description></item>
        /// <item><description> All other subtags are lower case.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="value">raw value</param>
        /// <returns>Returns the normalized value.</returns>
        public static String NormalizeLangValue(String value) {
            // don't normalize x-default
            if (XMPConst.X_DEFAULT.Equals(value)) {
                return value;
            }
            int subTag = 1;
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < value.Length; i++) {
                switch (value[i]) {
                    case '-':
                    case '_': {
                        // move to next subtag and convert underscore to hyphen
                        buffer.Append('-');
                        subTag++;
                        break;
                    }

                    case ' ': {
                        // remove spaces
                        break;
                    }

                    default: {
                        // convert second subtag to uppercase, all other to lowercase
                        if (subTag != 2) {
                            buffer.Append(char.ToLower(value[i]));
                        }
                        else {
                            buffer.Append(char.ToUpper(value[i]));
                        }
                        break;
                    }
                }
            }
            return buffer.ToString();
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Split the name and value parts for field and qualifier selectors.</summary>
        /// <remarks>
        /// Split the name and value parts for field and qualifier selectors. Following selectors are taken into account:
        /// <list type="bullet">
        /// <item><description>[qualName="value"] - An element in an array of structs, chosen by a
        /// field value.
        /// </description></item>
        /// <item><description>[?qualName="value"] - An element in an array, chosen by a qualifier
        /// value.
        /// </description></item>
        /// </list>
        /// The value portion is a string quoted by ''' or '"'. The value may contain
        /// any character including a doubled quoting character. The value may be
        /// empty. <em>Note:</em> It is assumed that the expression is formal
        /// correct
        /// </remarks>
        /// <param name="selector">the selector</param>
        /// <returns>
        /// Returns an array where the first entry contains the name and the
        /// second the value.
        /// </returns>
        internal static String[] SplitNameAndValue(String selector) {
            // get the name
            int eq = selector.IndexOf('=');
            int pos = 1;
            if (selector[pos] == '?') {
                pos++;
            }
            String name = selector.JSubstring(pos, eq);
            // get the value
            pos = eq + 1;
            char quote = selector[pos];
            pos++;
            int end = selector.Length - 2;
            // quote and ]
            StringBuilder value = new StringBuilder(end - eq);
            while (pos < end) {
                value.Append(selector[pos]);
                pos++;
                if (selector[pos] == quote) {
                    // skip one quote in value
                    pos++;
                }
            }
            return new String[] { name, value.ToString() };
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <param name="schema">a schema namespace</param>
        /// <param name="prop">an XMP Property</param>
        /// <returns>
        /// Returns true if the property is defined as &amp;quot;Internal
        /// Property&amp;quot;, see XMP Specification.
        /// </returns>
        internal static bool IsInternalProperty(String schema, String prop) {
            bool isInternal = false;
            if (NS_DC.Equals(schema)) {
                if ("dc:format".Equals(prop) || "dc:language".Equals(prop)) {
                    isInternal = true;
                }
            }
            else {
                if (NS_XMP.Equals(schema)) {
                    if ("xmp:BaseURL".Equals(prop) || "xmp:CreatorTool".Equals(prop) || "xmp:Format".Equals(prop) || "xmp:Locale"
                        .Equals(prop) || "xmp:MetadataDate".Equals(prop) || "xmp:ModifyDate".Equals(prop)) {
                        isInternal = true;
                    }
                }
                else {
                    if (NS_PDF.Equals(schema)) {
                        if ("pdf:BaseURL".Equals(prop) || "pdf:Creator".Equals(prop) || "pdf:ModDate".Equals(prop) || "pdf:PDFVersion"
                            .Equals(prop) || "pdf:Producer".Equals(prop)) {
                            isInternal = true;
                        }
                    }
                    else {
                        if (NS_TIFF.Equals(schema)) {
                            isInternal = true;
                            if ("tiff:ImageDescription".Equals(prop) || "tiff:Artist".Equals(prop) || "tiff:Copyright".Equals(prop)) {
                                isInternal = false;
                            }
                        }
                        else {
                            if (NS_EXIF.Equals(schema)) {
                                isInternal = true;
                                if ("exif:UserComment".Equals(prop)) {
                                    isInternal = false;
                                }
                            }
                            else {
                                if (NS_EXIF_AUX.Equals(schema)) {
                                    isInternal = true;
                                }
                                else {
                                    if (NS_PHOTOSHOP.Equals(schema)) {
                                        if ("photoshop:ICCProfile".Equals(prop)) {
                                            isInternal = true;
                                        }
                                    }
                                    else {
                                        if (NS_CAMERARAW.Equals(schema)) {
                                            if ("crs:Version".Equals(prop) || "crs:RawFileName".Equals(prop) || "crs:ToneCurveName".Equals(prop)) {
                                                isInternal = true;
                                            }
                                        }
                                        else {
                                            if (NS_ADOBESTOCKPHOTO.Equals(schema)) {
                                                isInternal = true;
                                            }
                                            else {
                                                if (NS_XMP_MM.Equals(schema)) {
                                                    isInternal = true;
                                                }
                                                else {
                                                    if (TYPE_TEXT.Equals(schema)) {
                                                        isInternal = true;
                                                    }
                                                    else {
                                                        if (TYPE_PAGEDFILE.Equals(schema)) {
                                                            isInternal = true;
                                                        }
                                                        else {
                                                            if (TYPE_GRAPHICS.Equals(schema)) {
                                                                isInternal = true;
                                                            }
                                                            else {
                                                                if (TYPE_IMAGE.Equals(schema)) {
                                                                    isInternal = true;
                                                                }
                                                                else {
                                                                    if (TYPE_FONT.Equals(schema)) {
                                                                        isInternal = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return isInternal;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Check some requirements for an UUID:
        /// <list type="bullet">
        /// <item><description>Length of the UUID is 32
        /// </description></item>
        /// <item><description>The Delimiter count is 4 and all the 4 delimiter are on their right
        /// position (8,13,18,23)
        /// </description></item>
        /// </list>
        /// </summary>
        /// <param name="uuid">uuid to test</param>
        /// <returns>true - this is a well formed UUID, false - UUID has not the expected format</returns>
        internal static bool CheckUUIDFormat(String uuid) {
            bool result = true;
            int delimCnt = 0;
            int delimPos = 0;
            if (uuid == null) {
                return false;
            }
            for (delimPos = 0; delimPos < uuid.Length; delimPos++) {
                if (uuid[delimPos] == '-') {
                    delimCnt++;
                    result = result && (delimPos == 8 || delimPos == 13 || delimPos == 18 || delimPos == 23);
                }
            }
            return result && UUID_SEGMENT_COUNT == delimCnt && UUID_LENGTH == delimPos;
        }
//\endcond

        /// <summary>Simple check for valid XMLNames.</summary>
        /// <remarks>
        /// Simple check for valid XMLNames. Within ASCII range<br />
        /// ":" | [A-Z] | "_" | [a-z] | [#xC0-#xD6] | [#xD8-#xF6]<br />
        /// are accepted, above all characters (which is not entirely
        /// correct according to the XML Spec.
        /// </remarks>
        /// <param name="name">an XML Name</param>
        /// <returns>Return <c>true</c> if the name is correct.</returns>
        public static bool IsXMLName(String name) {
            if (name.Length > 0 && !IsNameStartChar(name[0])) {
                return false;
            }
            for (int i = 1; i < name.Length; i++) {
                if (!IsNameChar(name[i])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the value is a legal "unqualified" XML name, as
        /// defined in the XML Namespaces proposed recommendation.
        /// </summary>
        /// <remarks>
        /// Checks if the value is a legal "unqualified" XML name, as
        /// defined in the XML Namespaces proposed recommendation.
        /// These are XML names, except that they must not contain a colon.
        /// </remarks>
        /// <param name="name">the value to check</param>
        /// <returns>Returns true if the name is a valid "unqualified" XML name.</returns>
        public static bool IsXMLNameNS(String name) {
            if (name.Length > 0 && (!IsNameStartChar(name[0]) || name[0] == ':')) {
                return false;
            }
            for (int i = 1; i < name.Length; i++) {
                if (!IsNameChar(name[i]) || name[i] == ':') {
                    return false;
                }
            }
            return true;
        }

//\cond DO_NOT_DOCUMENT
        /// <param name="c">a char</param>
        /// <returns>Returns true if the char is an ASCII control char.</returns>
        internal static bool IsControlChar(char c) {
            return (c <= 0x1F || c == 0x7F) && c != 0x09 && c != 0x0A && c != 0x0D;
        }
//\endcond

        /// <summary>Serializes the node value in XML encoding.</summary>
        /// <remarks>
        /// Serializes the node value in XML encoding. Its used for tag bodies and
        /// attributes.<br />
        /// <em>Note:</em> The attribute is always limited by quotes,
        /// thats why <c>&amp;apos;</c> is never serialized.<br />
        /// <em>Note:</em> Control chars are written unescaped, but if the user uses others than tab, LF
        /// and CR the resulting XML will become invalid.
        /// </remarks>
        /// <param name="value">a string</param>
        /// <param name="forAttribute">flag if string is attribute value (need to additional escape quotes)</param>
        /// <param name="escapeWhitespaces">Decides if LF, CR and TAB are escaped.</param>
        /// <returns>Returns the value ready for XML output.</returns>
        public static String EscapeXML(String value, bool forAttribute, bool escapeWhitespaces) {
            // quick check if character are contained that need special treatment
            bool needsEscaping = false;
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                if (c == '<' || c == '>' || c == '&' || 
                                // XML chars
                                (escapeWhitespaces && (c == '\t' || c == '\n' || c == '\r')) || (forAttribute && c == '"')) {
                    needsEscaping = true;
                    break;
                }
            }
            if (!needsEscaping) {
                // fast path
                return value;
            }
            else {
                // slow path with escaping
                StringBuilder buffer = new StringBuilder(value.Length * 4 / 3);
                for (int i = 0; i < value.Length; i++) {
                    char c = value[i];
                    if (!(escapeWhitespaces && (c == '\t' || c == '\n' || c == '\r'))) {
                        switch (c) {
                            // we do what "Canonical XML" expects
                            // AUDIT: &apos; not serialized as only outer qoutes are used
                            case '<': {
                                buffer.Append("&lt;");
                                continue;
                            }

                            case '>': {
                                buffer.Append("&gt;");
                                continue;
                            }

                            case '&': {
                                buffer.Append("&amp;");
                                continue;
                            }

                            case '"': {
                                buffer.Append(forAttribute ? "&quot;" : "\"");
                                continue;
                            }

                            default: {
                                buffer.Append(c);
                                continue;
                            }
                        }
                    }
                    else {
                        // write control chars escaped,
                        // if there are others than tab, LF and CR the xml will become invalid.
                        buffer.Append("&#x");
                        buffer.Append(JavaUtil.IntegerToHexString(c).ToUpperInvariant());
                        buffer.Append(';');
                    }
                }
                return buffer.ToString();
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Replaces the ASCII control chars with a space.</summary>
        /// <param name="value">a node value</param>
        /// <returns>Returns the cleaned up value</returns>
        internal static String RemoveControlChars(String value) {
            StringBuilder buffer = new StringBuilder(value);
            for (int i = 0; i < buffer.Length; i++) {
                if (IsControlChar(buffer[i])) {
                    buffer.SetCharAt(i, ' ');
                }
            }
            return buffer.ToString();
        }
//\endcond

        /// <summary>Simple check if a character is a valid XML start name char.</summary>
        /// <remarks>
        /// Simple check if a character is a valid XML start name char.
        /// All characters according to the XML Spec 1.1 are accepted:
        /// http://www.w3.org/TR/xml11/#NT-NameStartChar
        /// </remarks>
        /// <param name="ch">a character</param>
        /// <returns>Returns true if the character is a valid first char of an XML name.</returns>
        private static bool IsNameStartChar(char ch) {
            return (ch <= 0xFF && xmlNameStartChars[ch]) || (ch >= 0x100 && ch <= 0x2FF) || (ch >= 0x370 && ch <= 0x37D
                ) || (ch >= 0x37F && ch <= 0x1FFF) || (ch >= 0x200C && ch <= 0x200D) || (ch >= 0x2070 && ch <= 0x218F)
                 || (ch >= 0x2C00 && ch <= 0x2FEF) || (ch >= 0x3001 && ch <= 0xD7FF) || (ch >= 0xF900 && ch <= 0xFDCF)
                 || (ch >= 0xFDF0 && ch <= 0xFFFD) || (ch >= 0x10000 && ch <= 0xEFFFF);
        }

        /// <summary>
        /// Simple check if a character is a valid XML name char
        /// (every char except the first one), according to the XML Spec 1.1:
        /// http://www.w3.org/TR/xml11/#NT-NameChar
        /// </summary>
        /// <param name="ch">a character</param>
        /// <returns>Returns true if the character is a valid char of an XML name.</returns>
        private static bool IsNameChar(char ch) {
            return (ch <= 0xFF && xmlNameChars[ch]) || IsNameStartChar(ch) || (ch >= 0x300 && ch <= 0x36F) || (ch >= 0x203F
                 && ch <= 0x2040);
        }

        /// <summary>
        /// Initializes the char tables for the chars 0x00-0xFF for later use,
        /// according to the XML 1.1 specification
        /// http://www.w3.org/TR/xml11
        /// </summary>
        private static void InitCharTables() {
            xmlNameChars = new bool[0x0100];
            xmlNameStartChars = new bool[0x0100];
            for (int i = 0; i < xmlNameChars.Length; i++) {
                char ch = (char)i;
                xmlNameStartChars[ch] = ch == ':' || ('A' <= ch && ch <= 'Z') || ch == '_' || ('a' <= ch && ch <= 'z') || 
                    (0xC0 <= ch && ch <= 0xD6) || (0xD8 <= ch && ch <= 0xF6) || (0xF8 <= ch && ch <= 0xFF);
                xmlNameChars[ch] = xmlNameStartChars[ch] || ch == '-' || ch == '.' || ('0' <= ch && ch <= '9') || ch == 0xB7;
            }
        }
    }
}
