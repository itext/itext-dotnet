/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.IO.Util;

namespace iText.IO.Font.Otf
{
	public class Glyph
	{
		private readonly int code;

		private readonly int width;

		private int[] bbox = null;

		private int unicode;

		private char[] chars;

		private readonly bool isMark;

		internal short xPlacement = 0;

		internal short yPlacement = 0;

		internal short xAdvance = 0;

		internal short yAdvance = 0;

		internal short anchorDelta = 0;

		public Glyph(int code, int width, int unicode)
			: this(code, width, unicode, null, false)
		{
		}

		public Glyph(int code, int width, char[] chars)
			: this(code, width, CodePoint(chars), chars, false)
		{
		}

		public Glyph(int code, int width, int unicode, int[] bbox)
			: this(code, width, unicode, null, false)
		{
			// The <i>code</i> or <i>id</i> by which this is represented in the Font File.
			// The normalized width of this Glyph.
			// The normalized bbox of this Glyph.
			// utf-32 representation of glyph if appears. Correct value is > -1
			// The Unicode text represented by this Glyph
			// ture, if this Glyph is Mark
			// placement offset
			// advance offset
			// Index delta to base glyph. If after a glyph there are several anchored glyphs we should know we to find base glyph.
			this.bbox = bbox;
		}

		public Glyph(int width, int unicode)
			: this(-1, width, unicode, GetChars(unicode), false)
		{
		}

		public Glyph(int code, int width, int unicode, char[] chars, bool IsMark)
		{
			this.code = code;
			this.width = width;
			this.unicode = unicode;
			this.isMark = IsMark;
			this.chars = chars != null ? chars : GetChars(unicode);
		}

		public Glyph(iText.IO.Font.Otf.Glyph glyph)
		{
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

		public Glyph(iText.IO.Font.Otf.Glyph glyph, int xPlacement, int yPlacement, 
			int xAdvance, int yAdvance, int anchorDelta)
			: this(glyph)
		{
			this.xPlacement = (short)xPlacement;
			this.yPlacement = (short)yPlacement;
			this.xAdvance = (short)xAdvance;
			this.yAdvance = (short)yAdvance;
			this.anchorDelta = (short)anchorDelta;
		}

		public Glyph(iText.IO.Font.Otf.Glyph glyph, int unicode)
			: this(glyph.code, glyph.width, unicode, GetChars(unicode), glyph.IsMark())
		{
		}

		public virtual int GetCode()
		{
			return code;
		}

		public virtual int GetWidth()
		{
			return width;
		}

		public virtual int[] GetBbox()
		{
			return bbox;
		}

		public virtual bool HasValidUnicode()
		{
			return unicode > -1;
		}

		public virtual int GetUnicode()
		{
			return unicode;
		}

		public virtual void SetUnicode(int unicode)
		{
			this.unicode = unicode;
			this.chars = GetChars(unicode);
		}

		public virtual char[] GetChars()
		{
			return chars;
		}

		public virtual void SetChars(char[] chars)
		{
			this.chars = chars;
		}

		public virtual bool IsMark()
		{
			return isMark;
		}

		public virtual short GetXPlacement()
		{
			return xPlacement;
		}

		public virtual void SetXPlacement(short xPlacement)
		{
			this.xPlacement = xPlacement;
		}

		public virtual short GetYPlacement()
		{
			return yPlacement;
		}

		public virtual void SetYPlacement(short yPlacement)
		{
			this.yPlacement = yPlacement;
		}

		public virtual short GetXAdvance()
		{
			return xAdvance;
		}

		public virtual void SetXAdvance(short xAdvance)
		{
			this.xAdvance = xAdvance;
		}

		public virtual short GetYAdvance()
		{
			return yAdvance;
		}

		public virtual void SetYAdvance(short yAdvance)
		{
			this.yAdvance = yAdvance;
		}

		public virtual short GetAnchorDelta()
		{
			return anchorDelta;
        }

        public void SetAnchorDelta(short anchorDelta) {
            this.anchorDelta = anchorDelta;
        }

        public virtual bool HasOffsets()
		{
			return xPlacement != 0 || yPlacement != 0 || xAdvance != 0 || yAdvance != 0;
		}

		public virtual bool HasPlacement()
		{
			return xPlacement != 0 || yPlacement != 0;
		}

		public virtual bool HasAdvance()
		{
			return xAdvance != 0 || yAdvance != 0;
		}

		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + ((chars == null) ? 0 : iText.IO.Util.JavaUtil.ArraysHashCode
				(chars));
			result = prime * result + code;
			result = prime * result + width;
			return result;
		}

		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is iText.IO.Font.Otf.Glyph))
			{
				return false;
			}
			iText.IO.Font.Otf.Glyph other = (iText.IO.Font.Otf.Glyph)obj;
			return iText.IO.Util.JavaUtil.ArraysEquals(chars, other.chars) && code == other
				.code && width == other.width;
        }

        /// <summary>
        /// Gets a Unicode string corresponding to this glyph. In general case it might consist of many characters.
        /// If this glyph does not have a valid unicode (<see cref="HasValidUnicode"/>, then a string consisting of a special  
        /// Unicode '\ufffd' character is returned.
        /// </summary>
        /// <returns>the Unicode string that corresponds to this glyph</returns>
        public virtual String GetUnicodeString() {
            if (chars != null) {
                return new String(chars);
            } else {
                return '\ufffd'.ToString();
            }
        }

        public override String ToString()
		{
			return String.Format("[id={0}, chars={1}, uni={2}, width={3}]", ToHex(code), chars != null
				 ? iText.IO.Util.JavaUtil.ArraysToString(chars) : "null", ToHex(unicode), width);
        }

        private static String ToHex(int ch) {
            String s = "0000" + iText.IO.Util.JavaUtil.IntegerToHexString(ch);
            return s.Substring(Math.Min(4, s.Length - 4));
        }

        private static int CodePoint(char[] a)
		{
			if (a != null)
			{
				if (a.Length == 1 && iText.IO.Util.JavaUtil.IsValidCodePoint(a[0]))
				{
					return a[0];
				}
				else
				{
					if (a.Length == 2 && char.IsHighSurrogate(a[0]) && char.IsLowSurrogate(a[1]))
					{
						return iText.IO.Util.JavaUtil.ToCodePoint(a[0], a[1]);
					}
				}
			}
			return -1;
		}

		private static char[] GetChars(int unicode)
		{
			return unicode > -1 ? TextUtil.ConvertFromUtf32(unicode) : null;
		}
	}
}
