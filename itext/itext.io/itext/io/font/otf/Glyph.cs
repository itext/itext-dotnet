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
using iText.Commons.Utils;

namespace iText.IO.Font.Otf {
    public class Glyph {
        private const char REPLACEMENT_CHARACTER = '\ufffd';

        private static readonly char[] REPLACEMENT_CHARACTERS = new char[] { REPLACEMENT_CHARACTER };

        private static readonly String REPLACEMENT_CHARACTER_STRING = REPLACEMENT_CHARACTER.ToString();

        // The <i>code</i> or <i>id</i> by which this is represented in the Font File.
        private readonly int code;

        // The normalized width of this Glyph.
        private readonly int width;

        // The normalized bbox of this Glyph.
        private int[] bbox = null;

        // utf-32 representation of glyph if appears. Correct value is > -1
        private int unicode;

        // The Unicode text represented by this Glyph
        private char[] chars;

        // true, if this Glyph is Mark
        private readonly bool isMark;

        // placement offset
        internal short xPlacement = 0;

        internal short yPlacement = 0;

        // advance offset
        internal short xAdvance = 0;

        internal short yAdvance = 0;

        // Index delta to base glyph. If after a glyph there are several anchored glyphs we should know we to find base glyph.
        internal short anchorDelta = 0;

        /// <summary>Construct a non-mark Glyph, retrieving characters from unicode.</summary>
        /// <param name="code">code representation of the glyph in the font file</param>
        /// <param name="width">normalized width of the glyph</param>
        /// <param name="unicode">utf-32 representation of glyph if appears. Correct value is &gt; -1</param>
        public Glyph(int code, int width, int unicode)
            : this(code, width, unicode, null, false) {
        }

        /// <summary>Construct a non-mark Glyph, using the codepoint of the characters as unicode point.</summary>
        /// <param name="code">code representation of the glyph in the font file</param>
        /// <param name="width">normalized width of the glyph</param>
        /// <param name="chars">The Unicode text represented by this Glyph.</param>
        public Glyph(int code, int width, char[] chars)
            : this(code, width, CodePoint(chars), chars, false) {
        }

        /// <summary>Construct a non-mark Glyph, retrieving characters from unicode.</summary>
        /// <param name="code">code representation of the glyph in the font file</param>
        /// <param name="width">normalized width of the glyph</param>
        /// <param name="unicode">utf-32 representation of glyph if appears. Correct value is &gt; -1</param>
        /// <param name="bbox">The normalized bounding box of this Glyph.</param>
        public Glyph(int code, int width, int unicode, int[] bbox)
            : this(code, width, unicode, null, false) {
            this.bbox = bbox;
        }

        /// <summary>Construct a non-mark Glyph object with id -1 and characters retrieved from unicode.</summary>
        /// <param name="width">normalized width of the glyph</param>
        /// <param name="unicode">utf-32 representation of glyph if appears. Correct value is &gt; -1</param>
        public Glyph(int width, int unicode)
            : this(-1, width, unicode, GetChars(unicode), false) {
        }

        /// <summary>Construct a glyph object form the passed arguments.</summary>
        /// <param name="code">code representation of the glyph in the font file</param>
        /// <param name="width">normalized width of the glyph</param>
        /// <param name="unicode">utf-32 representation of glyph if appears. Correct value is &gt; -1</param>
        /// <param name="chars">
        /// The Unicode text represented by this Glyph.
        /// if null is passed, the unicode value is used to retrieve the chars.
        /// </param>
        /// <param name="IsMark">True if the glyph is a Mark</param>
        public Glyph(int code, int width, int unicode, char[] chars, bool IsMark) {
            this.code = code;
            this.width = width;
            this.unicode = unicode;
            this.isMark = IsMark;
            this.chars = chars != null ? chars : GetChars(unicode);
        }

        /// <summary>Copy a Glyph.</summary>
        /// <param name="glyph">Glyph to copy</param>
        public Glyph(iText.IO.Font.Otf.Glyph glyph) {
            this.code = glyph.code;
            this.width = glyph.width;
            this.chars = glyph.chars;
            this.unicode = glyph.unicode;
            this.isMark = glyph.isMark;
            this.bbox = glyph.bbox;
            this.xPlacement = glyph.xPlacement;
            this.yPlacement = glyph.yPlacement;
            this.xAdvance = glyph.xAdvance;
            this.yAdvance = glyph.yAdvance;
            this.anchorDelta = glyph.anchorDelta;
        }

        /// <summary>Copy a Glyph and assign new placement and advance offsets and a new index delta to base glyph</summary>
        /// <param name="glyph">Glyph to copy</param>
        /// <param name="xPlacement">x - placement offset</param>
        /// <param name="yPlacement">y - placement offset</param>
        /// <param name="xAdvance">x - advance offset</param>
        /// <param name="yAdvance">y - advance offset</param>
        /// <param name="anchorDelta">Index delta to base glyph. If after a glyph there are several anchored glyphs we should know we to find base glyph.
        ///     </param>
        public Glyph(iText.IO.Font.Otf.Glyph glyph, int xPlacement, int yPlacement, int xAdvance, int yAdvance, int
             anchorDelta)
            : this(glyph) {
            this.xPlacement = (short)xPlacement;
            this.yPlacement = (short)yPlacement;
            this.xAdvance = (short)xAdvance;
            this.yAdvance = (short)yAdvance;
            this.anchorDelta = (short)anchorDelta;
        }

        /// <summary>Copy a glyph and assign the copied glyph a new unicode point and characters</summary>
        /// <param name="glyph">glyph to copy</param>
        /// <param name="unicode">new unicode point</param>
        public Glyph(iText.IO.Font.Otf.Glyph glyph, int unicode)
            : this(glyph.code, glyph.width, unicode, GetChars(unicode), glyph.IsMark()) {
        }

        public virtual int GetCode() {
            return code;
        }

        public virtual int GetWidth() {
            return width;
        }

        public virtual int[] GetBbox() {
            return bbox;
        }

        public virtual bool HasValidUnicode() {
            return unicode > -1;
        }

        public virtual int GetUnicode() {
            return unicode;
        }

        public virtual void SetUnicode(int unicode) {
            this.unicode = unicode;
            this.chars = GetChars(unicode);
        }

        public virtual char[] GetChars() {
            return chars;
        }

        public virtual void SetChars(char[] chars) {
            this.chars = chars;
        }

        public virtual bool IsMark() {
            return isMark;
        }

        public virtual short GetXPlacement() {
            return xPlacement;
        }

        public virtual void SetXPlacement(short xPlacement) {
            this.xPlacement = xPlacement;
        }

        public virtual short GetYPlacement() {
            return yPlacement;
        }

        public virtual void SetYPlacement(short yPlacement) {
            this.yPlacement = yPlacement;
        }

        public virtual short GetXAdvance() {
            return xAdvance;
        }

        public virtual void SetXAdvance(short xAdvance) {
            this.xAdvance = xAdvance;
        }

        public virtual short GetYAdvance() {
            return yAdvance;
        }

        public virtual void SetYAdvance(short yAdvance) {
            this.yAdvance = yAdvance;
        }

        public virtual short GetAnchorDelta() {
            return anchorDelta;
        }

        public virtual void SetAnchorDelta(short anchorDelta) {
            this.anchorDelta = anchorDelta;
        }

        public virtual bool HasOffsets() {
            return HasAdvance() || HasPlacement();
        }

        // In case some of placement values are not zero we always expect anchorDelta to be non-zero
        public virtual bool HasPlacement() {
            return anchorDelta != 0;
        }

        public virtual bool HasAdvance() {
            return xAdvance != 0 || yAdvance != 0;
        }

        public override int GetHashCode() {
            int prime = 31;
            int result = 1;
            result = prime * result + ((chars == null) ? 0 : JavaUtil.ArraysHashCode(chars));
            result = prime * result + code;
            result = prime * result + width;
            return result;
        }

        /// <summary>Two Glyphs are equal if their unicode characters, code and normalized width are equal.</summary>
        /// <param name="obj">The object</param>
        /// <returns>True if this equals obj cast to Glyph, false otherwise.</returns>
        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            iText.IO.Font.Otf.Glyph other = (iText.IO.Font.Otf.Glyph)obj;
            return JavaUtil.ArraysEquals(chars, other.chars) && code == other.code && width == other.width;
        }

        /// <summary>Gets a Unicode string corresponding to this glyph.</summary>
        /// <remarks>
        /// Gets a Unicode string corresponding to this glyph. In general case it might consist of many characters.
        /// If this glyph does not have a valid unicode (
        /// <see cref="HasValidUnicode()"/>
        /// ), then a string consisting of a special
        /// Unicode '\ufffd' character is returned.
        /// </remarks>
        /// <returns>the Unicode string that corresponds to this glyph</returns>
        public virtual String GetUnicodeString() {
            if (chars != null) {
                return JavaUtil.GetStringForChars(chars);
            }
            else {
                return REPLACEMENT_CHARACTER_STRING;
            }
        }

        /// <summary>Gets Unicode char sequence corresponding to this glyph.</summary>
        /// <remarks>
        /// Gets Unicode char sequence corresponding to this glyph. In general case it might consist of many characters.
        /// If this glyph does not have a valid unicode (
        /// <see cref="HasValidUnicode()"/>
        /// ), then a special
        /// Unicode '\ufffd' character is returned.
        /// </remarks>
        /// <returns>the Unicode char sequence that corresponds to this glyph</returns>
        public virtual char[] GetUnicodeChars() {
            if (chars != null) {
                return chars;
            }
            else {
                return REPLACEMENT_CHARACTERS;
            }
        }

        public override String ToString() {
            return MessageFormatUtil.Format("[id={0}, chars={1}, uni={2}, width={3}]", ToHex(code), chars != null ? JavaUtil.ArraysToString
                (chars) : "null", ToHex(unicode), width);
        }

        private static String ToHex(int ch) {
            String s = "0000" + JavaUtil.IntegerToHexString(ch);
            return s.Substring(Math.Min(4, s.Length - 4));
        }

        private static int CodePoint(char[] a) {
            if (a != null) {
                if (a.Length == 1 && JavaUtil.IsValidCodePoint(a[0])) {
                    return a[0];
                }
                else {
                    if (a.Length == 2 && Char.IsHighSurrogate(a[0]) && Char.IsLowSurrogate(a[1])) {
                        return JavaUtil.ToCodePoint(a[0], a[1]);
                    }
                }
            }
            return -1;
        }

        private static char[] GetChars(int unicode) {
            return unicode > -1 ? iText.IO.Util.TextUtil.ConvertFromUtf32(unicode) : null;
        }
    }
}
