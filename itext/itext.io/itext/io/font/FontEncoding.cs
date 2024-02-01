/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.IO.Font {
    public class FontEncoding {
        private static readonly byte[] emptyBytes = new byte[0];

        public const String FONT_SPECIFIC = "FontSpecific";

        /// <summary>A not defined character in a custom PDF encoding.</summary>
        public const String NOTDEF = ".notdef";

        /// <summary>Base font encoding.</summary>
        protected internal String baseEncoding;

        /// <summary>
        /// <see langword="true"/>
        /// if the font must use its built in encoding.
        /// </summary>
        /// <remarks>
        /// <see langword="true"/>
        /// if the font must use its built in encoding. In that case
        /// the
        /// <c>encoding</c>
        /// is only used to map a char to the position inside the font, not to the expected char name.
        /// </remarks>
        protected internal bool fontSpecific;

        /// <summary>Mapping map from unicode to simple code according to the encoding.</summary>
        protected internal IntHashtable unicodeToCode;

        protected internal int[] codeToUnicode;

        /// <summary>Encoding names.</summary>
        protected internal String[] differences;

        /// <summary>Encodings unicode differences</summary>
        protected internal IntHashtable unicodeDifferences;

        protected internal FontEncoding() {
            unicodeToCode = new IntHashtable(256);
            codeToUnicode = ArrayUtil.FillWithValue(new int[256], -1);
            unicodeDifferences = new IntHashtable(256);
            fontSpecific = false;
        }

        public static iText.IO.Font.FontEncoding CreateFontEncoding(String baseEncoding) {
            iText.IO.Font.FontEncoding encoding = new iText.IO.Font.FontEncoding();
            encoding.baseEncoding = NormalizeEncoding(baseEncoding);
            if (encoding.baseEncoding.StartsWith("#")) {
                encoding.FillCustomEncoding();
            }
            else {
                encoding.FillNamedEncoding();
            }
            return encoding;
        }

        public static iText.IO.Font.FontEncoding CreateEmptyFontEncoding() {
            iText.IO.Font.FontEncoding encoding = new iText.IO.Font.FontEncoding();
            encoding.baseEncoding = null;
            encoding.fontSpecific = false;
            encoding.differences = new String[256];
            for (int ch = 0; ch < 256; ch++) {
                encoding.unicodeDifferences.Put(ch, ch);
            }
            return encoding;
        }

        /// <summary>This encoding will base on font encoding (FontSpecific encoding in Type 1 terminology)</summary>
        /// <returns>created font specific encoding</returns>
        public static iText.IO.Font.FontEncoding CreateFontSpecificEncoding() {
            iText.IO.Font.FontEncoding encoding = new iText.IO.Font.FontEncoding();
            encoding.fontSpecific = true;
            iText.IO.Font.FontEncoding.FillFontEncoding(encoding);
            return encoding;
        }

        /// <summary>
        /// Fill
        /// <see cref="FontEncoding"/>
        /// object with default data.
        /// </summary>
        /// <param name="encoding">
        /// 
        /// <see cref="FontEncoding"/>
        /// to fill.
        /// </param>
        public static void FillFontEncoding(iText.IO.Font.FontEncoding encoding) {
            for (int ch = 0; ch < 256; ch++) {
                encoding.unicodeToCode.Put(ch, ch);
                encoding.codeToUnicode[ch] = ch;
                encoding.unicodeDifferences.Put(ch, ch);
            }
        }

        public virtual String GetBaseEncoding() {
            return baseEncoding;
        }

        public virtual bool IsFontSpecific() {
            return fontSpecific;
        }

        public virtual bool AddSymbol(int code, int unicode) {
            if (code < 0 || code > 255) {
                return false;
            }
            String glyphName = AdobeGlyphList.UnicodeToName(unicode);
            if (glyphName != null) {
                unicodeToCode.Put(unicode, code);
                codeToUnicode[code] = unicode;
                differences[code] = glyphName;
                unicodeDifferences.Put(unicode, unicode);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>Gets unicode value for corresponding font's char code.</summary>
        /// <param name="index">font's char code</param>
        /// <returns>-1, if the char code unsupported or valid unicode.</returns>
        public virtual int GetUnicode(int index) {
            return codeToUnicode[index];
        }

        public virtual int GetUnicodeDifference(int index) {
            return unicodeDifferences.Get(index);
        }

        public virtual bool HasDifferences() {
            return differences != null;
        }

        public virtual String GetDifference(int index) {
            return differences != null ? differences[index] : null;
        }

        /// <summary>Sets a new value in the differences array.</summary>
        /// <remarks>
        /// Sets a new value in the differences array.
        /// See
        /// <see cref="differences"/>.
        /// </remarks>
        /// <param name="index">position to replace</param>
        /// <param name="difference">new difference value</param>
        public virtual void SetDifference(int index, String difference) {
            if (index >= 0 && differences != null && index < differences.Length) {
                differences[index] = difference;
            }
        }

        /// <summary>
        /// Converts a
        /// <c>String</c>
        /// to a
        /// <c>byte</c>
        /// array according to the encoding.
        /// </summary>
        /// <remarks>
        /// Converts a
        /// <c>String</c>
        /// to a
        /// <c>byte</c>
        /// array according to the encoding.
        /// String could contain a unicode symbols or font specific codes.
        /// </remarks>
        /// <param name="text">
        /// the
        /// <c>String</c>
        /// to be converted.
        /// </param>
        /// <returns>
        /// an array of
        /// <c>byte</c>
        /// representing the conversion according to the encoding
        /// </returns>
        public virtual byte[] ConvertToBytes(String text) {
            if (text == null || text.Length == 0) {
                return emptyBytes;
            }
            int ptr = 0;
            byte[] bytes = new byte[text.Length];
            for (int i = 0; i < text.Length; i++) {
                if (unicodeToCode.ContainsKey(text[i])) {
                    bytes[ptr++] = (byte)ConvertToByte(text[i]);
                }
            }
            return ArrayUtil.ShortenArray(bytes, ptr);
        }

        /// <summary>
        /// Converts a unicode symbol or font specific code
        /// to
        /// <c>byte</c>
        /// according to the encoding.
        /// </summary>
        /// <param name="unicode">a unicode symbol or FontSpecif code to be converted.</param>
        /// <returns>
        /// a
        /// <c>byte</c>
        /// representing the conversion according to the encoding
        /// </returns>
        public virtual int ConvertToByte(int unicode) {
            return unicodeToCode.Get(unicode);
        }

        /// <summary>
        /// Check whether a unicode symbol or font specific code can be converted
        /// to
        /// <c>byte</c>
        /// according to the encoding.
        /// </summary>
        /// <param name="unicode">a unicode symbol or font specific code to be checked.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if
        /// <c>ch</c>
        /// could be encoded.
        /// </returns>
        public virtual bool CanEncode(int unicode) {
            return unicodeToCode.ContainsKey(unicode) || iText.IO.Util.TextUtil.IsNonPrintable(unicode) || iText.IO.Util.TextUtil
                .IsNewLine(unicode);
        }

        /// <summary>
        /// Check whether a
        /// <c>byte</c>
        /// code can be converted
        /// to unicode symbol according to the encoding.
        /// </summary>
        /// <param name="code">a byte code to be checked.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if
        /// <paramref name="code"/>
        /// could be decoded.
        /// </returns>
        public virtual bool CanDecode(int code) {
            return codeToUnicode[code] > -1;
        }

        /// <summary>
        /// Checks whether the
        /// <see cref="FontEncoding"/>
        /// was built with corresponding encoding.
        /// </summary>
        /// <param name="encoding">an encoding</param>
        /// <returns>true, if the FontEncoding was built with the encoding. Otherwise false.</returns>
        public virtual bool IsBuiltWith(String encoding) {
            return Object.Equals(NormalizeEncoding(encoding), baseEncoding);
        }

        protected internal virtual void FillCustomEncoding() {
            differences = new String[256];
            StringTokenizer tok = new StringTokenizer(baseEncoding.Substring(1), " ,\t\n\r\f");
            if (tok.NextToken().Equals("full")) {
                while (tok.HasMoreTokens()) {
                    String order = tok.NextToken();
                    String name = tok.NextToken();
                    char uni = (char)Convert.ToInt32(tok.NextToken(), 16);
                    int uniName = AdobeGlyphList.NameToUnicode(name);
                    int orderK;
                    if (order.StartsWith("'")) {
                        orderK = order[1];
                    }
                    else {
                        orderK = Convert.ToInt32(order, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    orderK %= 256;
                    unicodeToCode.Put(uni, orderK);
                    codeToUnicode[orderK] = (int)uni;
                    differences[orderK] = name;
                    unicodeDifferences.Put(uni, uniName);
                }
            }
            else {
                int k = 0;
                if (tok.HasMoreTokens()) {
                    k = Convert.ToInt32(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                }
                while (tok.HasMoreTokens() && k < 256) {
                    String hex = tok.NextToken();
                    int uni = Convert.ToInt32(hex, 16) % 0x10000;
                    String name = AdobeGlyphList.UnicodeToName(uni);
                    if (name == null) {
                        name = "uni" + hex;
                    }
                    unicodeToCode.Put(uni, k);
                    codeToUnicode[k] = uni;
                    differences[k] = name;
                    unicodeDifferences.Put(uni, uni);
                    k++;
                }
            }
            for (int k = 0; k < 256; k++) {
                if (differences[k] == null) {
                    differences[k] = NOTDEF;
                }
            }
        }

        protected internal virtual void FillNamedEncoding() {
            // check if the encoding exists
            PdfEncodings.ConvertToBytes(" ", baseEncoding);
            bool stdEncoding = PdfEncodings.WINANSI.Equals(baseEncoding) || PdfEncodings.MACROMAN.Equals(baseEncoding);
            if (!stdEncoding && differences == null) {
                differences = new String[256];
            }
            byte[] b = new byte[256];
            for (int k = 0; k < 256; ++k) {
                b[k] = (byte)k;
            }
            String str = PdfEncodings.ConvertToString(b, baseEncoding);
            char[] encoded = str.ToCharArray();
            for (int ch = 0; ch < 256; ++ch) {
                char uni = encoded[ch];
                String name = AdobeGlyphList.UnicodeToName(uni);
                if (name == null) {
                    name = NOTDEF;
                }
                else {
                    unicodeToCode.Put(uni, ch);
                    codeToUnicode[ch] = (int)uni;
                    unicodeDifferences.Put(uni, uni);
                }
                if (differences != null) {
                    differences[ch] = name;
                }
            }
        }

        protected internal virtual void FillStandardEncoding() {
            int[] encoded = PdfEncodings.standardEncoding;
            for (int ch = 0; ch < 256; ++ch) {
                int uni = encoded[ch];
                String name = AdobeGlyphList.UnicodeToName(uni);
                if (name == null) {
                    name = NOTDEF;
                }
                else {
                    unicodeToCode.Put(uni, ch);
                    codeToUnicode[ch] = uni;
                    unicodeDifferences.Put(uni, uni);
                }
                if (differences != null) {
                    differences[ch] = name;
                }
            }
        }

        /// <summary>Normalize the encoding names.</summary>
        /// <remarks>
        /// Normalize the encoding names. "winansi" is changed to "Cp1252" and
        /// "macroman" is changed to "MacRoman".
        /// </remarks>
        /// <param name="enc">the encoding to be normalized</param>
        /// <returns>the normalized encoding</returns>
        protected internal static String NormalizeEncoding(String enc) {
            String tmp = enc == null ? "" : enc.ToLowerInvariant();
            switch (tmp) {
                case "":
                case "winansi":
                case "winansiencoding": {
                    return PdfEncodings.WINANSI;
                }

                case "macroman":
                case "macromanencoding": {
                    return PdfEncodings.MACROMAN;
                }

                case "zapfdingbatsencoding": {
                    return PdfEncodings.ZAPFDINGBATS;
                }

                default: {
                    return enc;
                }
            }
        }
    }
}
