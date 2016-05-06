/*
$Id: b90260d10ef4b1d64684455e319ad7941d50fcf4 $

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
namespace com.itextpdf.io.font
{
	public class FontMetrics
	{
		private const long serialVersionUID = -7113134666493365588L;

		protected internal float normalizationCoef = 1f;

		private int unitsPerEm = 1000;

		private int maxGlyphId;

		private int[] glyphWidths;

		private int typoAscender = 800;

		private int typoDescender = -200;

		private int capHeight = 700;

		private int xHeight = 0;

		private float italicAngle = 0;

		private int[] bbox = new int[] { -50, -200, 1000, 900 };

		private int ascender;

		private int descender;

		private int lineGap;

		private int winAscender;

		private int winDescender;

		private int advanceWidthMax;

		private int underlinePosition = -100;

		private int underlineThickness = 50;

		private int strikeoutPosition;

		private int strikeoutSize;

		private int subscriptSize;

		private int subscriptOffset;

		private int superscriptSize;

		private int superscriptOffset;

		private int stemV = 80;

		private int stemH = 0;

		private bool isFixedPitch;

		// head.unitsPerEm
		// maxp.numGlyphs
		// hmtx
		// os_2.sTypoAscender * normalization
		// os_2.sTypoDescender * normalization
		// os_2.sCapHeight * normalization
		// os_2.sxHeight * normalization
		// post.italicAngle
		// llx: head.xMin * normalization; lly: head.yMin * normalization
		// urx: head.xMax * normalization; ury: head.yMax * normalization
		// hhea.Ascender * normalization
		// hhea.Descender * normalization
		// hhea.LineGap * normaliztion (leading)
		// os_2.winAscender * normalization
		// os_2.winDescender * normalization
		// hhea.advanceWidthMax * normalization
		// (post.underlinePosition - post.underlineThickness / 2) * normalization
		// post.underlineThickness * normalization
		// os_2.yStrikeoutPosition * normalization
		// os_2.yStrikeoutSize * normalization
		// os_2.ySubscriptYSize * normalization
		// -os_2.ySubscriptYOffset * normalization
		// os_2.ySuperscriptYSize * normalization
		// os_2.ySuperscriptYOffset * normalization
		// in type1/cff it is stdVW
		// in type1/cff it is stdHW
		// post.isFixedPitch (monospaced)
		public virtual int GetUnitsPerEm()
		{
			return unitsPerEm;
		}

		public virtual int GetMaxGlyphId()
		{
			return maxGlyphId;
		}

		public virtual int[] GetGlyphWidths()
		{
			return glyphWidths;
		}

		public virtual int GetTypoAscender()
		{
			return typoAscender;
		}

		public virtual int GetTypoDescender()
		{
			return typoDescender;
		}

		public virtual int GetCapHeight()
		{
			return capHeight;
		}

		public virtual int GetXHeight()
		{
			return xHeight;
		}

		public virtual float GetItalicAngle()
		{
			return italicAngle;
		}

		public virtual int[] GetBbox()
		{
			return bbox;
		}

		public virtual void SetBbox(int llx, int lly, int urx, int ury)
		{
			bbox[0] = llx;
			bbox[1] = lly;
			bbox[2] = urx;
			bbox[3] = ury;
		}

		public virtual int GetAscender()
		{
			return ascender;
		}

		public virtual int GetDescender()
		{
			return descender;
		}

		public virtual int GetLineGap()
		{
			return lineGap;
		}

		public virtual int GetWinAscender()
		{
			return winAscender;
		}

		public virtual int GetWinDescender()
		{
			return winDescender;
		}

		public virtual int GetAdvanceWidthMax()
		{
			return advanceWidthMax;
		}

		public virtual int GetUnderlinePosition()
		{
			return underlinePosition - underlineThickness / 2;
		}

		public virtual int GetUnderlineThickness()
		{
			return underlineThickness;
		}

		public virtual int GetStrikeoutPosition()
		{
			return strikeoutPosition;
		}

		public virtual int GetStrikeoutSize()
		{
			return strikeoutSize;
		}

		public virtual int GetSubscriptSize()
		{
			return subscriptSize;
		}

		public virtual int GetSubscriptOffset()
		{
			return subscriptOffset;
		}

		public virtual int GetSuperscriptSize()
		{
			return superscriptSize;
		}

		public virtual int GetSuperscriptOffset()
		{
			return superscriptOffset;
		}

		public virtual int GetStemV()
		{
			return stemV;
		}

		public virtual int GetStemH()
		{
			return stemH;
		}

		public virtual bool IsFixedPitch()
		{
			return isFixedPitch;
		}

		protected internal virtual void SetUnitsPerEm(int unitsPerEm)
		{
			this.unitsPerEm = unitsPerEm;
			normalizationCoef = (float)FontProgram.UNITS_NORMALIZATION / unitsPerEm;
		}

		protected internal virtual void UpdateBbox(float llx, float lly, float urx, float
			 ury)
		{
			bbox[0] = (int)(llx * normalizationCoef);
			bbox[1] = (int)(lly * normalizationCoef);
			bbox[2] = (int)(urx * normalizationCoef);
			bbox[3] = (int)(ury * normalizationCoef);
		}

		protected internal virtual void SetMaxGlyphId(int maxGlyphId)
		{
			this.maxGlyphId = maxGlyphId;
		}

		protected internal virtual void SetGlyphWidths(int[] glyphWidths)
		{
			this.glyphWidths = glyphWidths;
		}

		protected internal virtual void SetTypoAscender(int typoAscender)
		{
			this.typoAscender = (int)(typoAscender * normalizationCoef);
		}

		protected internal virtual void SetTypoDescender(int typoDesctender)
		{
			this.typoDescender = (int)(typoDesctender * normalizationCoef);
		}

		protected internal virtual void SetCapHeight(int capHeight)
		{
			this.capHeight = (int)(capHeight * normalizationCoef);
		}

		protected internal virtual void SetXHeight(int xHeight)
		{
			this.xHeight = (int)(xHeight * normalizationCoef);
		}

		protected internal virtual void SetItalicAngle(float italicAngle)
		{
			this.italicAngle = italicAngle;
		}

		protected internal virtual void SetAscender(int ascender)
		{
			this.ascender = (int)(ascender * normalizationCoef);
		}

		protected internal virtual void SetDescender(int descender)
		{
			this.descender = (int)(descender * normalizationCoef);
		}

		protected internal virtual void SetLineGap(int lineGap)
		{
			this.lineGap = (int)(lineGap * normalizationCoef);
		}

		protected internal virtual void SetWinAscender(int winAscender)
		{
			this.winAscender = (int)(winAscender * normalizationCoef);
		}

		protected internal virtual void SetWinDescender(int winDescender)
		{
			this.winDescender = (int)(winDescender * normalizationCoef);
		}

		protected internal virtual void SetAdvanceWidthMax(int advanceWidthMax)
		{
			this.advanceWidthMax = (int)(advanceWidthMax * normalizationCoef);
		}

		protected internal virtual void SetUnderlinePosition(int underlinePosition)
		{
			this.underlinePosition = (int)(underlinePosition * normalizationCoef);
		}

		protected internal virtual void SetUnderlineThickness(int underineThickness)
		{
			this.underlineThickness = underineThickness;
		}

		protected internal virtual void SetStrikeoutPosition(int strikeoutPosition)
		{
			this.strikeoutPosition = (int)(strikeoutPosition * normalizationCoef);
		}

		protected internal virtual void SetStrikeoutSize(int strikeoutSize)
		{
			this.strikeoutSize = (int)(strikeoutSize * normalizationCoef);
		}

		protected internal virtual void SetSubscriptSize(int subscriptSize)
		{
			this.subscriptSize = (int)(subscriptSize * normalizationCoef);
		}

		protected internal virtual void SetSubscriptOffset(int subscriptOffset)
		{
			this.subscriptOffset = (int)(subscriptOffset * normalizationCoef);
		}

		protected internal virtual void SetSuperscriptSize(int superscriptSize)
		{
			this.superscriptSize = superscriptSize;
		}

		protected internal virtual void SetSuperscriptOffset(int superscriptOffset)
		{
			this.superscriptOffset = (int)(superscriptOffset * normalizationCoef);
		}

		//todo change to protected!
		public virtual void SetStemV(int stemV)
		{
			this.stemV = stemV;
		}

		protected internal virtual void SetStemH(int stemH)
		{
			this.stemH = stemH;
		}

		protected internal virtual void SetIsFixedPitch(bool isFixedPitch)
		{
			this.isFixedPitch = isFixedPitch;
		}
	}
}
