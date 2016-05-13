/*
$Id$

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
using System.Collections.Generic;
using System.Text;
using iTextSharp.IO.Font;
using iTextSharp.IO.Font.Otf;
using iTextSharp.IO.Util;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Font
{
	public abstract class PdfFont : PdfObjectWrapper<PdfDictionary>
	{
		private const long serialVersionUID = -7661159455613720321L;

		protected internal FontProgram fontProgram;

		protected internal static readonly byte[] emptyBytes = new byte[0];

		protected internal IDictionary<int, Glyph> notdefGlyphs = new Dictionary<int, Glyph
			>();

		/// <summary>false, if the font comes from PdfDocument.</summary>
		protected internal bool newFont = true;

		/// <summary>true if the font is to be embedded in the PDF.</summary>
		protected internal bool embedded = false;

		/// <summary>Indicates if all the glyphs and widths for that particular encoding should be included in the document.
		/// 	</summary>
		protected internal bool subset = true;

		protected internal IList<int[]> subsetRanges;

		protected internal PdfFont(PdfDictionary fontDictionary)
			: base(fontDictionary)
		{
			GetPdfObject().Put(PdfName.Type, PdfName.Font);
		}

		protected internal PdfFont()
			: base(new PdfDictionary())
		{
			MarkObjectAsIndirect(GetPdfObject());
			GetPdfObject().Put(PdfName.Type, PdfName.Font);
		}

		public abstract Glyph GetGlyph(int unicode);

		public virtual bool ContainsGlyph(char unicode)
		{
			Glyph glyph = GetGlyph(unicode);
			if (glyph != null)
			{
				if (GetFontProgram() != null && GetFontProgram().IsFontSpecific())
				{
					//if current is symbolic, zero code is valid value
					return glyph.GetCode() > -1;
				}
				else
				{
					return glyph.GetCode() > 0;
				}
			}
			else
			{
				return false;
			}
		}

		public abstract GlyphLine CreateGlyphLine(String content);

		/// <summary>Converts the text into bytes to be placed in the document.</summary>
		/// <remarks>
		/// Converts the text into bytes to be placed in the document.
		/// The conversion is done according to the font and the encoding and the characters
		/// used are stored.
		/// </remarks>
		/// <param name="text">the text to convert</param>
		/// <returns>the conversion</returns>
		public abstract byte[] ConvertToBytes(String text);

		public abstract byte[] ConvertToBytes(GlyphLine glyphLine);

		public abstract String Decode(PdfString content);

		public abstract float GetContentWidth(PdfString content);

		public abstract byte[] ConvertToBytes(Glyph glyph);

		public abstract void WriteText(GlyphLine text, int from, int to, PdfOutputStream 
			stream);

		public abstract void WriteText(String text, PdfOutputStream stream);

		public virtual void WriteText(GlyphLine text, PdfOutputStream stream)
		{
			WriteText(text, 0, text.Size() - 1, stream);
		}

		public virtual double[] GetFontMatrix()
		{
			return FontConstants.DefaultFontMatrix;
		}

		/// <summary>Returns the width of a certain character of this font in 1000 normalized units.
		/// 	</summary>
		/// <param name="unicode">a certain character.</param>
		/// <returns>a width in Text Space.</returns>
		public virtual int GetWidth(int unicode)
		{
			Glyph glyph = GetGlyph(unicode);
			return glyph != null ? glyph.GetWidth() : 0;
		}

		/// <summary>Returns the width of a certain character of this font in points.</summary>
		/// <param name="unicode">a certain character.</param>
		/// <param name="fontSize">the font size.</param>
		/// <returns>a width in points.</returns>
		public virtual float GetWidth(int unicode, float fontSize)
		{
			return GetWidth(unicode) * fontSize / FontProgram.UNITS_NORMALIZATION;
		}

		/// <summary>Returns the width of a string of this font in 1000 normalized units.</summary>
		/// <param name="text">a string content.</param>
		/// <returns>a width of string in Text Space.</returns>
		public virtual int GetWidth(String text)
		{
			int total = 0;
			for (int i = 0; i < text.Length; i++)
			{
				int ch;
				if (TextUtil.IsSurrogatePair(text, i))
				{
					ch = TextUtil.ConvertToUtf32(text, i);
					i++;
				}
				else
				{
					ch = text[i];
				}
				Glyph glyph = GetGlyph(ch);
				if (glyph != null)
				{
					total += glyph.GetWidth();
				}
			}
			return total;
		}

		/// <summary>
		/// Gets the width of a
		/// <c>String</c>
		/// in points.
		/// </summary>
		/// <param name="text">
		/// the
		/// <c>String</c>
		/// to get the width of
		/// </param>
		/// <param name="fontSize">the font size</param>
		/// <returns>the width in points</returns>
		public virtual float GetWidth(String text, float fontSize)
		{
			return GetWidth(text) * fontSize / FontProgram.UNITS_NORMALIZATION;
		}

		/// <summary>
		/// Gets the descent of a
		/// <c>String</c>
		/// in points. The descent will always be
		/// less than or equal to zero even if all the characters have an higher descent.
		/// </summary>
		/// <param name="text">
		/// the
		/// <c>String</c>
		/// to get the descent of
		/// </param>
		/// <param name="fontSize">the font size</param>
		/// <returns>the descent in points</returns>
		public virtual int GetDescent(String text, float fontSize)
		{
			int min = 0;
			for (int k = 0; k < text.Length; ++k)
			{
				int ch;
				if (TextUtil.IsSurrogatePair(text, k))
				{
					ch = TextUtil.ConvertToUtf32(text, k);
					k++;
				}
				else
				{
					ch = text[k];
				}
				int[] bbox = GetGlyph(ch).GetBbox();
				if (bbox != null && bbox[1] < min)
				{
					min = bbox[1];
				}
				else
				{
					if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoDescender() < min)
					{
						min = GetFontProgram().GetFontMetrics().GetTypoDescender();
					}
				}
			}
			return (int)(min * fontSize / FontProgram.UNITS_NORMALIZATION);
		}

		/// <summary>Gets the descent of a char code in points.</summary>
		/// <remarks>
		/// Gets the descent of a char code in points. The descent will always be
		/// less than or equal to zero even if all the characters have an higher descent.
		/// </remarks>
		/// <param name="unicode">the char code to get the descent of</param>
		/// <param name="fontSize">the font size</param>
		/// <returns>the descent in points</returns>
		public virtual int GetDescent(int unicode, float fontSize)
		{
			int min = 0;
			int[] bbox = GetGlyph(unicode).GetBbox();
			if (bbox != null && bbox[1] < min)
			{
				min = bbox[1];
			}
			else
			{
				if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoDescender() < min)
				{
					min = GetFontProgram().GetFontMetrics().GetTypoDescender();
				}
			}
			return (int)(min * fontSize / FontProgram.UNITS_NORMALIZATION);
		}

		/// <summary>
		/// Gets the ascent of a
		/// <c>String</c>
		/// in points. The ascent will always be
		/// greater than or equal to zero even if all the characters have a lower ascent.
		/// </summary>
		/// <param name="text">
		/// the
		/// <c>String</c>
		/// to get the ascent of
		/// </param>
		/// <param name="fontSize">the font size</param>
		/// <returns>the ascent in points</returns>
		public virtual int GetAscent(String text, float fontSize)
		{
			int max = 0;
			for (int k = 0; k < text.Length; ++k)
			{
				int ch;
				if (TextUtil.IsSurrogatePair(text, k))
				{
					ch = TextUtil.ConvertToUtf32(text, k);
					k++;
				}
				else
				{
					ch = text[k];
				}
				int[] bbox = GetGlyph(ch).GetBbox();
				if (bbox != null && bbox[3] > max)
				{
					max = bbox[3];
				}
				else
				{
					if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoAscender() > max)
					{
						max = GetFontProgram().GetFontMetrics().GetTypoAscender();
					}
				}
			}
			return (int)(max * fontSize / FontProgram.UNITS_NORMALIZATION);
		}

		/// <summary>Gets the ascent of a char code in normalized 1000 units.</summary>
		/// <remarks>
		/// Gets the ascent of a char code in normalized 1000 units. The ascent will always be
		/// greater than or equal to zero even if all the characters have a lower ascent.
		/// </remarks>
		/// <param name="unicode">the char code to get the ascent of</param>
		/// <param name="fontSize">the font size</param>
		/// <returns>the ascent in points</returns>
		public virtual int GetAscent(int unicode, float fontSize)
		{
			int max = 0;
			int[] bbox = GetGlyph(unicode).GetBbox();
			if (bbox != null && bbox[3] > max)
			{
				max = bbox[3];
			}
			else
			{
				if (bbox == null && GetFontProgram().GetFontMetrics().GetTypoAscender() > max)
				{
					max = GetFontProgram().GetFontMetrics().GetTypoAscender();
				}
			}
			return (int)(max * fontSize / FontProgram.UNITS_NORMALIZATION);
		}

		public virtual FontProgram GetFontProgram()
		{
			return fontProgram;
		}

		public virtual bool IsEmbedded()
		{
			return embedded;
		}

		/// <summary>
		/// Indicates if all the glyphs and widths for that particular
		/// encoding should be included in the document.
		/// </summary>
		/// <returns><CODE>false</CODE> to include all the glyphs and widths.</returns>
		public virtual bool IsSubset()
		{
			return subset;
		}

		/// <summary>
		/// Indicates if all the glyphs and widths for that particular
		/// encoding should be included in the document.
		/// </summary>
		/// <remarks>
		/// Indicates if all the glyphs and widths for that particular
		/// encoding should be included in the document. When set to <CODE>true</CODE>
		/// only the glyphs used will be included in the font. When set to <CODE>false</CODE>
		/// and
		/// <see cref="AddSubsetRange(int[])"/>
		/// was not called the full font will be included
		/// otherwise just the characters ranges will be included.
		/// </remarks>
		/// <param name="subset">new value of property subset</param>
		public virtual void SetSubset(bool subset)
		{
			this.subset = subset;
		}

		/// <summary>Adds a character range when subsetting.</summary>
		/// <remarks>
		/// Adds a character range when subsetting. The range is an <CODE>int</CODE> array
		/// where the first element is the start range inclusive and the second element is the
		/// end range inclusive. Several ranges are allowed in the same array.
		/// </remarks>
		/// <param name="range">the character range</param>
		public virtual void AddSubsetRange(int[] range)
		{
			if (subsetRanges == null)
			{
				subsetRanges = new List<int[]>();
			}
			subsetRanges.Add(range);
		}

		public virtual IList<String> SplitString(String text, int fontSize, float maxWidth
			)
		{
			IList<String> resultString = new List<String>();
			int lastWhiteSpace = 0;
			int startPos = 0;
			float tokenLength = 0;
			for (int i = 0; i < text.Length; i++)
			{
				char ch = text[i];
				if (char.IsWhiteSpace(ch))
				{
					lastWhiteSpace = i;
				}
				tokenLength += GetWidth(ch, fontSize);
				if (tokenLength >= maxWidth || ch == '\n')
				{
					if (startPos < lastWhiteSpace)
					{
						resultString.Add(text.JSubstring(startPos, lastWhiteSpace));
						startPos = lastWhiteSpace + 1;
						tokenLength = 0;
						i = lastWhiteSpace;
					}
					else
					{
						resultString.Add(text.JSubstring(startPos, i + 1));
						startPos = i + 1;
						tokenLength = 0;
						i = i + 1;
					}
				}
			}
			resultString.Add(text.Substring(startPos));
			return resultString;
		}

		protected internal abstract PdfDictionary GetFontDescriptor(String fontName);

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return true;
		}

		protected internal virtual bool CheckFontDictionary(PdfDictionary fontDic, PdfName
			 fontType)
		{
			return PdfFontFactory.CheckFontDictionary(fontDic, fontType, true);
		}

		protected internal virtual bool CheckTrueTypeFontDictionary(PdfDictionary fontDic
			)
		{
			return CheckTrueTypeFontDictionary(fontDic, true);
		}

		protected internal virtual bool CheckTrueTypeFontDictionary(PdfDictionary fontDic
			, bool isException)
		{
			if (fontDic == null || fontDic.Get(PdfName.Subtype) == null || !(fontDic.Get(PdfName
				.Subtype).Equals(PdfName.TrueType) || fontDic.Get(PdfName.Subtype).Equals(PdfName
				.Type1)))
			{
				if (isException)
				{
					throw new PdfException(PdfException.DictionaryNotContainFontData).SetMessageParams
						(PdfName.TrueType.GetValue());
				}
				return false;
			}
			return true;
		}

		/// <summary>Creates a unique subset prefix to be added to the font name when the font is embedded and subset.
		/// 	</summary>
		/// <returns>the subset prefix</returns>
		protected internal static String CreateSubsetPrefix()
		{
			StringBuilder s = new StringBuilder("");
			for (int k = 0; k < 6; ++k)
			{
				s.Append((char)(iTextSharp.IO.Util.JavaUtil.Random() * 26 + 'A'));
			}
			return s + "+";
		}

		/// <summary>
		/// Create
		/// <c>PdfStream</c>
		/// based on
		/// <paramref name="fontStreamBytes"/>
		/// .
		/// </summary>
		/// <param name="fontStreamBytes">original font data, must be not null.</param>
		/// <param name="fontStreamLengths">
		/// array to generate
		/// <c>Length*</c>
		/// keys, must be not null.
		/// </param>
		/// <returns>
		/// the PdfStream containing the font or
		/// <see langword="null"/>
		/// , if there is an error reading the font.
		/// </returns>
		/// <exception>
		/// PdfException
		/// Method will throw exception if
		/// <paramref name="fontStreamBytes"/>
		/// is
		/// <see langword="null"/>
		/// .
		/// </exception>
		protected internal virtual PdfStream GetPdfFontStream(byte[] fontStreamBytes, int
			[] fontStreamLengths)
		{
			if (fontStreamBytes == null)
			{
				throw new PdfException(PdfException.FontEmbeddingIssue);
			}
			PdfStream fontStream = new PdfStream(fontStreamBytes);
			for (int k = 0; k < fontStreamLengths.Length; ++k)
			{
				fontStream.Put(new PdfName("Length" + (k + 1)), new PdfNumber(fontStreamLengths[k
					]));
			}
			return fontStream;
		}

		protected internal static int[] CompactRanges(IList<int[]> ranges)
		{
			IList<int[]> simp = new List<int[]>();
			foreach (int[] range in ranges)
			{
				for (int j = 0; j < range.Length; j += 2)
				{
					simp.Add(new int[] { Math.Max(0, Math.Min(range[j], range[j + 1])), Math.Min(0xffff
						, Math.Max(range[j], range[j + 1])) });
				}
			}
			for (int k1 = 0; k1 < simp.Count - 1; ++k1)
			{
				for (int k2 = k1 + 1; k2 < simp.Count; ++k2)
				{
					int[] r1 = simp[k1];
					int[] r2 = simp[k2];
					if (r1[0] >= r2[0] && r1[0] <= r2[1] || r1[1] >= r2[0] && r1[0] <= r2[1])
					{
						r1[0] = Math.Min(r1[0], r2[0]);
						r1[1] = Math.Max(r1[1], r2[1]);
						simp.RemoveAt(k2);
						--k2;
					}
				}
			}
			int[] s = new int[simp.Count * 2];
			for (int k = 0; k < simp.Count; ++k)
			{
				int[] r = simp[k];
				s[k * 2] = r[0];
				s[k * 2 + 1] = r[1];
			}
			return s;
		}
	}
}
