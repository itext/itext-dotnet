/*
$Id: ea941d20672b625436bad12975fd7e47c544a35c $

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
using com.itextpdf.io;
using com.itextpdf.io.font.otf;
using com.itextpdf.io.log;
using com.itextpdf.io.util;

namespace com.itextpdf.io.font
{
	public class TrueTypeFont : FontProgram
	{
		private const long serialVersionUID = -2232044646577669268L;

		private OpenTypeParser fontParser;

		protected internal int[][] bBoxes;

		protected internal bool isVertical;

		private GlyphSubstitutionTableReader gsubTable;

		private GlyphPositioningTableReader gposTable;

		private OpenTypeGdefTableReader gdefTable;

		/// <summary>The map containing the kerning information.</summary>
		/// <remarks>
		/// The map containing the kerning information. It represents the content of
		/// table 'kern'. The key is an <CODE>Integer</CODE> where the top 16 bits
		/// are the glyph number for the first character and the lower 16 bits are the
		/// glyph number for the second character. The value is the amount of kerning in
		/// normalized 1000 units as an <CODE>Integer</CODE>. This value is usually negative.
		/// </remarks>
		protected internal IntHashtable kerning = new IntHashtable();

		private byte[] fontStreamBytes;

		protected internal TrueTypeFont()
		{
		}

		/// <exception cref="System.IO.IOException"/>
		public TrueTypeFont(String path)
		{
			CheckFilePath(path);
			fontParser = new OpenTypeParser(path);
			InitializeFontProperties();
		}

		/// <exception cref="System.IO.IOException"/>
		public TrueTypeFont(byte[] ttf)
		{
			fontParser = new OpenTypeParser(ttf);
			InitializeFontProperties();
		}

		/// <exception cref="System.IO.IOException"/>
		internal TrueTypeFont(String ttcPath, int ttcIndex)
		{
			CheckFilePath(ttcPath);
			fontParser = new OpenTypeParser(ttcPath, ttcIndex);
			InitializeFontProperties();
		}

		/// <exception cref="System.IO.IOException"/>
		internal TrueTypeFont(byte[] ttc, int ttcIndex)
		{
			fontParser = new OpenTypeParser(ttc, ttcIndex);
			InitializeFontProperties();
		}

		public override bool HasKernPairs()
		{
			return kerning.Size() > 0;
		}

		/// <summary>Gets the kerning between two glyphs.</summary>
		/// <param name="first">the first glyph</param>
		/// <param name="second">the second glyph</param>
		/// <returns>the kerning to be applied</returns>
		public override int GetKerning(Glyph first, Glyph second)
		{
			if (first == null || second == null)
			{
				return 0;
			}
			return kerning.Get((first.GetCode() << 16) + second.GetCode());
		}

		public virtual bool IsCff()
		{
			return fontParser.IsCff();
		}

		public virtual IDictionary<int, int[]> GetActiveCmap()
		{
			OpenTypeParser.CmapTable cmaps = fontParser.GetCmapTable();
			if (cmaps.cmapExt != null)
			{
				return cmaps.cmapExt;
			}
			else
			{
				if (!cmaps.fontSpecific && cmaps.cmap31 != null)
				{
					return cmaps.cmap31;
				}
				else
				{
					if (cmaps.fontSpecific && cmaps.cmap10 != null)
					{
						return cmaps.cmap10;
					}
					else
					{
						if (cmaps.cmap31 != null)
						{
							return cmaps.cmap31;
						}
						else
						{
							return cmaps.cmap10;
						}
					}
				}
			}
		}

		public virtual byte[] GetFontStreamBytes()
		{
			if (fontStreamBytes != null)
			{
				return fontStreamBytes;
			}
			try
			{
				if (fontParser.IsCff())
				{
					fontStreamBytes = fontParser.ReadCffFont();
				}
				else
				{
					fontStreamBytes = fontParser.GetFullFont();
				}
			}
			catch (System.IO.IOException e)
			{
				fontStreamBytes = null;
				throw new IOException(IOException.IoException, e);
			}
			return fontStreamBytes;
		}

		public override int GetPdfFontFlags()
		{
			int flags = 0;
			if (fontMetrics.IsFixedPitch())
			{
				flags |= 1;
			}
			flags |= IsFontSpecific() ? 4 : 32;
			if (fontNames.IsItalic())
			{
				flags |= 64;
			}
			if (fontNames.IsBold() || fontNames.GetFontWeight() > 500)
			{
				flags |= 262144;
			}
			return flags;
		}

		/// <summary>The offset from the start of the file to the table directory.</summary>
		/// <remarks>
		/// The offset from the start of the file to the table directory.
		/// It is 0 for TTF and may vary for TTC depending on the chosen font.
		/// </remarks>
		public virtual int GetDirectoryOffset()
		{
			return fontParser.directoryOffset;
		}

		public virtual GlyphSubstitutionTableReader GetGsubTable()
		{
			return gsubTable;
		}

		public virtual GlyphPositioningTableReader GetGposTable()
		{
			return gposTable;
		}

		public virtual byte[] GetSubset(ICollection<int> glyphs, bool subset)
		{
			try
			{
				return fontParser.GetSubset(glyphs, subset);
			}
			catch (System.IO.IOException e)
			{
				throw new IOException(IOException.IoException, e);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal virtual void ReadGdefTable()
		{
			int[] gdef = fontParser.tables["GDEF"];
			if (gdef != null)
			{
				gdefTable = new OpenTypeGdefTableReader(fontParser.raf, gdef[0]);
			}
			else
			{
				gdefTable = new OpenTypeGdefTableReader(fontParser.raf, 0);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal virtual void ReadGsubTable()
		{
			int[] gsub = fontParser.tables["GSUB"];
			if (gsub != null)
			{
				gsubTable = new GlyphSubstitutionTableReader(fontParser.raf, gsub[0], gdefTable, 
					codeToGlyph, fontMetrics.GetUnitsPerEm());
			}
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal virtual void ReadGposTable()
		{
			int[] gpos = fontParser.tables["GPOS"];
			if (gpos != null)
			{
				gposTable = new GlyphPositioningTableReader(fontParser.raf, gpos[0], gdefTable, codeToGlyph
					, fontMetrics.GetUnitsPerEm());
			}
		}

		/// <exception cref="System.IO.IOException"/>
		private void InitializeFontProperties()
		{
			// initialize sfnt tables
			OpenTypeParser.HeaderTable head = fontParser.GetHeadTable();
			OpenTypeParser.HorizontalHeader hhea = fontParser.GetHheaTable();
			OpenTypeParser.WindowsMetrics os_2 = fontParser.GetOs_2Table();
			OpenTypeParser.PostTable post = fontParser.GetPostTable();
			isFontSpecific = fontParser.GetCmapTable().fontSpecific;
			kerning = fontParser.ReadKerning(head.unitsPerEm);
			bBoxes = fontParser.ReadBbox(head.unitsPerEm);
			// font names group
			fontNames.SetAllNames(fontParser.GetAllNameEntries());
			fontNames.SetFontName(fontParser.GetPsFontName());
			fontNames.SetFullName(fontNames.GetNames(4));
			String[][] otfFamilyName = fontNames.GetNames(16);
			if (otfFamilyName != null)
			{
				fontNames.SetFamilyName(otfFamilyName);
			}
			else
			{
				fontNames.SetFamilyName(fontNames.GetNames(1));
			}
			String[][] subfamily = fontNames.GetNames(2);
			if (subfamily != null)
			{
				fontNames.SetStyle(subfamily[0][3]);
			}
			String[][] otfSubFamily = fontNames.GetNames(17);
			if (otfFamilyName != null)
			{
				fontNames.SetSubfamily(otfSubFamily);
			}
			else
			{
				fontNames.SetSubfamily(subfamily);
			}
			String[][] cidName = fontNames.GetNames(20);
			if (cidName != null)
			{
				fontNames.SetCidFontName(cidName[0][3]);
			}
			fontNames.SetWeight(os_2.usWeightClass);
			fontNames.SetWidth(os_2.usWidthClass);
			fontNames.SetMacStyle(head.macStyle);
			fontNames.SetAllowEmbedding(os_2.fsType != 2);
			// font metrics group
			fontMetrics.SetUnitsPerEm(head.unitsPerEm);
			fontMetrics.UpdateBbox(head.xMin, head.yMin, head.xMax, head.yMax);
			fontMetrics.SetMaxGlyphId(fontParser.ReadMaxGlyphId());
			fontMetrics.SetGlyphWidths(fontParser.GetGlyphWidthsByIndex());
			fontMetrics.SetTypoAscender(os_2.sTypoAscender);
			fontMetrics.SetTypoDescender(os_2.sTypoDescender);
			fontMetrics.SetCapHeight(os_2.sCapHeight);
			fontMetrics.SetXHeight(os_2.sxHeight);
			fontMetrics.SetItalicAngle(post.italicAngle);
			fontMetrics.SetAscender(hhea.Ascender);
			fontMetrics.SetDescender(hhea.Descender);
			fontMetrics.SetLineGap(hhea.LineGap);
			fontMetrics.SetWinAscender(os_2.usWinAscent);
			fontMetrics.SetWinDescender(os_2.usWinDescent);
			fontMetrics.SetAdvanceWidthMax(hhea.advanceWidthMax);
			fontMetrics.SetUnderlinePosition((post.underlinePosition - post.underlineThickness
				) / 2);
			fontMetrics.SetUnderlineThickness(post.underlineThickness);
			fontMetrics.SetStrikeoutPosition(os_2.yStrikeoutPosition);
			fontMetrics.SetStrikeoutSize(os_2.yStrikeoutSize);
			fontMetrics.SetSubscriptOffset(-os_2.ySubscriptYOffset);
			fontMetrics.SetSubscriptSize(os_2.ySubscriptYSize);
			fontMetrics.SetSuperscriptOffset(os_2.ySuperscriptYOffset);
			fontMetrics.SetSuperscriptSize(os_2.ySuperscriptYSize);
			fontMetrics.SetIsFixedPitch(post.isFixedPitch);
			// font identification group
			String[][] ttfVersion = fontNames.GetNames(5);
			if (ttfVersion != null)
			{
				fontIdentification.SetTtfVersion(ttfVersion[0][3]);
			}
			String[][] ttfUniqueId = fontNames.GetNames(3);
			if (ttfUniqueId != null)
			{
				fontIdentification.SetTtfVersion(ttfUniqueId[0][3]);
			}
			fontIdentification.SetPanose(os_2.panose);
			IDictionary<int, int[]> cmap = GetActiveCmap();
			int[] glyphWidths = fontParser.GetGlyphWidthsByIndex();
			unicodeToGlyph = new LinkedDictionary<int, Glyph>(cmap.Count);
			codeToGlyph = new LinkedDictionary<int, Glyph>(glyphWidths.Length);
			avgWidth = 0;
			foreach (int charCode in cmap.Keys)
			{
				int index = cmap[charCode][0];
				if (index >= glyphWidths.Length)
				{
					Logger LOGGER = LoggerFactory.GetLogger(typeof(com.itextpdf.io.font.TrueTypeFont)
						);
					LOGGER.Warn(String.Format(LogMessageConstant.FONT_HAS_INVALID_GLYPH, GetFontNames
						().GetFontName(), index));
					continue;
				}
				Glyph glyph = new Glyph(index, glyphWidths[index], charCode, bBoxes != null ? bBoxes
					[index] : null);
				unicodeToGlyph[charCode] = glyph;
				codeToGlyph[index] = glyph;
				avgWidth += glyph.GetWidth();
			}
			FixSpaceIssue();
			for (int index_1 = 0; index_1 < glyphWidths.Length; index_1++)
			{
				if (codeToGlyph.ContainsKey(index_1))
				{
					continue;
				}
				Glyph glyph = new Glyph(index_1, glyphWidths[index_1], -1);
				codeToGlyph[index_1] = glyph;
				avgWidth += glyph.GetWidth();
			}
			if (codeToGlyph.Count != 0)
			{
				avgWidth /= codeToGlyph.Count;
			}
			ReadGdefTable();
			ReadGsubTable();
			ReadGposTable();
			isVertical = false;
		}
	}
}
