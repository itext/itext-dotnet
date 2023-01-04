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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Font.Otf;

namespace iText.IO.Util {
    /// <summary>This file is a helper class for internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </remarks>
    public sealed class TextUtil {

        public const int CHARACTER_MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;

        private static HashSet<char> javaNonUnicodeCategoryWhiteSpaceChars = new HashSet<char> {
            '\t', //  U+0009 HORIZONTAL TABULATION
            '\n', // U+000A LINE FEED
            '\u000B', // U+000B VERTICAL TABULATION
            '\f', // U+000C FORM FEED
            '\r', // U+000D CARRIAGE RETURN
            '\u001C', // U+001C FILE SEPARATOR
            '\u001D', // U+001D GROUP SEPARATOR
            '\u001E', // U+001E RECORD SEPARATOR
            '\u001F', // U+001F UNIT SEPARATOR
        };

        private TextUtil() {
        }

        /// <summary>
        /// Check if the value of a character belongs to a certain interval
        /// that indicates it's the higher part of a surrogate pair.
        /// </summary>
        /// <param name="c">the character</param>
        /// <returns>true if the character belongs to the interval</returns>
        public static bool IsSurrogateHigh(char c) {
            return c >= '\ud800' && c <= '\udbff';
        }

        /// <summary>
        /// Check if the value of a character belongs to a certain interval
        /// that indicates it's the lower part of a surrogate pair.
        /// </summary>
        /// <param name="c">the character</param>
        /// <returns>true if the character belongs to the interval</returns>
        public static bool IsSurrogateLow(char c) {
            return c >= '\udc00' && c <= '\udfff';
        }

        public static char HighSurrogate(int codePoint) {
            return (char) ((int) ((uint) codePoint >> 10) + ('\uD800' - (int) ((uint) 0x010000 >> 10)));
        }

        public static char LowSurrogate(int codePoint) {
            return (char) ((codePoint & 0x3ff) + '\uDC00');
        }

        /// <summary>
        /// Checks if two subsequent characters in a String are
        /// are the higher and the lower character in a surrogate
        /// pair (and therefore eligible for conversion to a UTF 32 character).
        /// </summary>
        /// <param name="text">the String with the high and low surrogate characters</param>
        /// <param name="idx">the index of the 'high' character in the pair</param>
        /// <returns>true if the characters are surrogate pairs</returns>
        public static bool IsSurrogatePair(String text, int idx) {
            return !(idx < 0 || idx > text.Length - 2) && IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        /// <summary>
        /// Checks if two subsequent characters in a character array are
        /// are the higher and the lower character in a surrogate
        /// pair (and therefore eligible for conversion to a UTF 32 character).
        /// </summary>
        /// <param name="text">the character array with the high and low surrogate characters</param>
        /// <param name="idx">the index of the 'high' character in the pair</param>
        /// <returns>true if the characters are surrogate pairs</returns>
        public static bool IsSurrogatePair(char[] text, int idx) {
            return !(idx < 0 || idx > text.Length - 2) && IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        /// <summary>
        /// Returns the code point of a UTF32 character corresponding with
        /// a high and a low surrogate value.
        /// </summary>
        /// <param name="highSurrogate">the high surrogate value</param>
        /// <param name="lowSurrogate">the low surrogate value</param>
        /// <returns>a code point value</returns>
        public static int ConvertToUtf32(char highSurrogate, char lowSurrogate) {
            return (highSurrogate - 0xd800) * 0x400 + lowSurrogate - 0xdc00 + 0x10000;
        }

        /// <summary>Converts a unicode character in a character array to a UTF 32 code point value.</summary>
        /// <param name="text">a character array that has the unicode character(s)</param>
        /// <param name="idx">the index of the 'high' character</param>
        /// <returns>the code point value</returns>
        public static int ConvertToUtf32(char[] text, int idx) {
            return (text[idx] - 0xd800) * 0x400 + text[idx + 1] - 0xdc00 + 0x10000;
        }

        /// <summary>Converts a unicode character in a String to a UTF32 code point value</summary>
        /// <param name="text">a String that has the unicode character(s)</param>
        /// <param name="idx">the index of the 'high' character</param>
        /// <returns>the codepoint value</returns>
        public static int ConvertToUtf32(String text, int idx) {
            return (text[idx] - 0xd800) * 0x400 + text[idx + 1] - 0xdc00 + 0x10000;
        }

        public static int[] ConvertToUtf32(String text) {
            if (text == null) {
                return null;
            }

            IList<int> charCodes = new List<int>(text.Length);
            int pos = 0;
            while (pos < text.Length) {
                if (IsSurrogatePair(text, pos)) {
                    charCodes.Add(ConvertToUtf32(text, pos));
                    pos += 2;
                } else {
                    charCodes.Add((int) text[pos]);
                    pos++;
                }
            }

            return ArrayUtil.ToIntArray(charCodes);
        }

        /// <summary>Converts a UTF32 code point value to a char array with the corresponding character(s).</summary>
        /// <param name="codePoint">a Unicode value</param>
        /// <returns>the corresponding char array</returns>
        public static char[] ConvertFromUtf32(int codePoint) {
            if (codePoint < 0x10000) {
                return new char[] {(char) codePoint};
            }

            codePoint -= 0x10000;
            return new char[] {(char) (codePoint / 0x400 + 0xd800), (char) (codePoint % 0x400 + 0xdc00)};
        }

        /// <summary>
        /// /
        /// Converts a UTF32 code point sequence to a String with the corresponding character(s).
        /// </summary>
        /// <param name="text">a Unicode text sequence</param>
        /// <param name="startPos">start position of text to convert, inclusive</param>
        /// <param name="endPos">end position of txt to convert, exclusive</param>
        /// <returns>the corresponding characters in a String</returns>
        public static String ConvertFromUtf32(int[] text, int startPos, int endPos) {
            StringBuilder sb = new StringBuilder();
            for (int i = startPos; i < endPos; i++) {
                sb.Append(ConvertFromUtf32ToCharArray(text[i]));
            }

            return sb.ToString();
        }

        /// <summary>Converts a UTF32 code point value to a char array with the corresponding character(s).</summary>
        /// <param name="codePoint">a Unicode value</param>
        /// <returns>the corresponding characters in a char array</returns>
        public static char[] ConvertFromUtf32ToCharArray(int codePoint) {
            if (codePoint < 0x10000) {
                return new char[] {(char) codePoint};
            }

            codePoint -= 0x10000;
            return new char[] {(char) (codePoint / 0x400 + 0xd800), (char) (codePoint % 0x400 + 0xdc00)};
        }

        public static bool IsWhiteSpace(char ch) {
            return IsWhiteSpace((int) ch);
        }

        public static bool IsWhiteSpace(int unicode) {
            if (unicode == '\u00A0' || unicode == '\u2007' || unicode == '\u202F') {
                // non-breaking space char
                return false;
            }

            UnicodeCategory category = GetUnicodeCategory(unicode);
            if (category == UnicodeCategory.SpaceSeparator || category == UnicodeCategory.LineSeparator ||
                category == UnicodeCategory.ParagraphSeparator) {

                return true;
            }

            return unicode <= Char.MaxValue && javaNonUnicodeCategoryWhiteSpaceChars.Contains((char) unicode);
        }


        static int[] ignorableCodePoints = new int[] {
            0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x000E, 0x000F, 0x0010,
            0x0011, 0x0012, 0x0013, 0x0014, 0x0015, 0x0016, 0x0017, 0x0018, 0x0019, 0x001A, 0x001B, 0x007F,
            0x0080, 0x0081, 0x0082, 0x0083, 0x0084, 0x0085, 0x0086, 0x0087, 0x0088, 0x0089, 0x008A, 0x008B,
            0x008C, 0x008D, 0x008E, 0x008F, 0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096, 0x0097,
            0x0098, 0x0099, 0x009A, 0x009B, 0x009C, 0x009D, 0x009E, 0x009F, 0x00AD, 0x0600, 0x0601, 0x0602,
            0x0603, 0x06DD, 0x070F, 0x17B4, 0x17B5, 0x200B, 0x200C, 0x200D, 0x200E, 0x200F, 0x202A, 0x202B,
            0x202C, 0x202D, 0x202E, 0x2060, 0x2061, 0x2062, 0x2063, 0x2064, 0x206A, 0x206B, 0x206C, 0x206D,
            0x206E, 0x206F, 0xFEFF, 0xFFF9, 0xFFFa, 0xFFFb, 0x110BD, 0x1D173, 0x1D174, 0x1D175, 0x1D176,
            0x1D177, 0x1D178, 0x1D179, 0x1D17A, 0xE0001
        };

        /// <summary>
        /// Determines if the specified character (Unicode code point) should be regarded
        /// as an ignorable character in a Java identifier or a Unicode identifier.
        /// </summary>
        public static bool IsIdentifierIgnorable(int codePoint) {
            if (codePoint >= 0xE0020) return codePoint <= 0xE007F;
            return Array.BinarySearch(ignorableCodePoints, codePoint) > -1;
        }

        /// <summary>
        /// Determines if represented Glyph is '\n' or '\r' character.
        /// </summary>
        public static bool IsNewLine(Glyph glyph) {
            int unicode = glyph.GetUnicode();
            return IsNewLine(unicode);
        }

        /// <summary>
        /// Determines if represented Glyph is '\n' or '\r' character.
        /// </summary>
        public static bool IsNewLine(char c) {
            int unicode = (int) c;
            return IsNewLine(unicode);
        }

        /// <summary>
        /// Determines if represented Glyph is '\n' or '\r' character.
        /// </summary>
        public static bool IsNewLine(int unicode) {
            return unicode == '\n' || unicode == '\r';
        }

        public static bool IsCarriageReturnFollowedByLineFeed(GlyphLine glyphLine, int carriageReturnPosition) {
            return glyphLine.Size() > 1
                   && carriageReturnPosition <= glyphLine.Size() - 2
                   && glyphLine.Get(carriageReturnPosition).GetUnicode() == '\r'
                   && glyphLine.Get(carriageReturnPosition + 1).GetUnicode() == '\n';
        }

        /// <summary>
        /// Determines if represented Glyph is space or whitespace character.
        /// </summary>
        public static bool IsSpaceOrWhitespace(Glyph glyph) {
            //\r, \n, and \t are whitespaces, but not space chars.
            //\u00a0 is SpaceChar, but not whitespace.
            return IsWhiteSpace((char) glyph.GetUnicode()) || char.IsSeparator((char) glyph.GetUnicode());
        }

        /// <summary>
        /// Determines if represented Glyph is whitespace character.
        /// </summary>
        public static bool IsWhitespace(Glyph glyph) {
            return IsWhiteSpace(glyph.GetUnicode());
        }

        public static bool IsNonBreakingHyphen(Glyph glyph) {
            return '\u2011' == glyph.GetUnicode();
        }

        /// <summary>
        /// Determines if represented Glyph is ' ' (SPACE) character.
        /// </summary>
        public static bool IsUni0020(Glyph g) {
            return g.GetUnicode() == ' ';
        }

        public static bool IsNonPrintable(int c) {
            return IsIdentifierIgnorable(c) || c == '\u00AD';
        }

        public static bool IsWhitespaceOrNonPrintable(int code) {
            return IsWhiteSpace((char) code) || IsNonPrintable(code);
        }

        public static bool IsLetterOrDigit(Glyph glyph) {
            int unicode = glyph.GetUnicode();
            UnicodeCategory category = GetUnicodeCategory(unicode);

            return category == UnicodeCategory.UppercaseLetter
                   || category == UnicodeCategory.LowercaseLetter
                   || category == UnicodeCategory.TitlecaseLetter
                   || category == UnicodeCategory.ModifierLetter
                   || category == UnicodeCategory.OtherLetter
                   || category == UnicodeCategory.DecimalDigitNumber;
        }

        public static bool IsMark(Glyph glyph) {
            int unicode = glyph.GetUnicode();
            UnicodeCategory category = GetUnicodeCategory(unicode);

            return category == UnicodeCategory.NonSpacingMark
                   || category == UnicodeCategory.SpacingCombiningMark
                   || category == UnicodeCategory.EnclosingMark;
        }

        public static char[] ToChars(int codePoint) {
            return char.ConvertFromUtf32(codePoint).ToCharArray();
        }

        public static int CharCount(int codePoint) {
            return codePoint >= CHARACTER_MIN_SUPPLEMENTARY_CODE_POINT ? 2 : 1;
        }

        public static Encoding NewEncoder(Encoding charset) {
            return charset;
        }

        public static bool CharsetIsSupported(string charset) {
            try {
                var enc = EncodingUtil.GetEncoding(charset);
                return true;
            }
            catch (ArgumentException) {
                return false;
            }
        }

        private static UnicodeCategory GetUnicodeCategory(int unicodeCodePoint) {
            UnicodeCategory category = unicodeCodePoint <= Char.MaxValue
                ? CharUnicodeInfo.GetUnicodeCategory((char) unicodeCodePoint)
                : CharUnicodeInfo.GetUnicodeCategory(new String(ConvertFromUtf32(unicodeCodePoint)), 0);
            return category;
        }
    }
}
