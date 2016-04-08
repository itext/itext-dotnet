/*
$Id: 1f08db850650246ac3a8a6db696a66794bd3bb95 $

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

namespace com.itextpdf.io.font
{
	public class FontConstants
	{
		public static readonly ICollection<String> BUILTIN_FONTS_14 = new HashSet<String>
			();

		static FontConstants()
		{
			BUILTIN_FONTS_14.Add(FontConstants.COURIER);
			BUILTIN_FONTS_14.Add(FontConstants.COURIER_BOLD);
			BUILTIN_FONTS_14.Add(FontConstants.COURIER_BOLDOBLIQUE);
			BUILTIN_FONTS_14.Add(FontConstants.COURIER_OBLIQUE);
			BUILTIN_FONTS_14.Add(FontConstants.HELVETICA);
			BUILTIN_FONTS_14.Add(FontConstants.HELVETICA_BOLD);
			BUILTIN_FONTS_14.Add(FontConstants.HELVETICA_BOLDOBLIQUE);
			BUILTIN_FONTS_14.Add(FontConstants.HELVETICA_OBLIQUE);
			BUILTIN_FONTS_14.Add(FontConstants.SYMBOL);
			BUILTIN_FONTS_14.Add(FontConstants.TIMES_ROMAN);
			BUILTIN_FONTS_14.Add(FontConstants.TIMES_BOLD);
			BUILTIN_FONTS_14.Add(FontConstants.TIMES_BOLDITALIC);
			BUILTIN_FONTS_14.Add(FontConstants.TIMES_ITALIC);
			BUILTIN_FONTS_14.Add(FontConstants.ZAPFDINGBATS);
		}

		/// <summary>The path to the font resources.</summary>
		public const String RESOURCE_PATH = "com/itextpdf/io/font/";

		public const int UNDEFINED = -1;

		/// <summary>this is a possible style.</summary>
		public const int NORMAL = 0;

		/// <summary>this is a possible style.</summary>
		public const int BOLD = 1;

		/// <summary>this is a possible style.</summary>
		public const int ITALIC = 2;

		/// <summary>this is a possible style.</summary>
		public const int UNDERLINE = 4;

		/// <summary>this is a possible style.</summary>
		public const int STRIKETHRU = 8;

		/// <summary>this is a possible style.</summary>
		public const int BOLDITALIC = BOLD | ITALIC;

		/// <summary>Type 1 PostScript font.</summary>
		public const int Type1Font = 1;

		/// <summary>Compact Font Format PostScript font.</summary>
		public const int Type1CompactFont = 2;

		/// <summary>TrueType or OpenType with TrueType outlines font.</summary>
		public const int TrueTypeFont = 3;

		/// <summary>CIDFont Type0 (Type1 outlines).</summary>
		public const int CIDFontType0Font = 4;

		/// <summary>CIDFont Type2 (TrueType outlines).</summary>
		public const int CIDFontType2Font = 5;

		/// <summary>OpenType with Type1 outlines.</summary>
		public const int OpenTypeFont = 6;

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String COURIER = "Courier";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String COURIER_BOLD = "Courier-Bold";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String COURIER_OBLIQUE = "Courier-Oblique";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String COURIER_BOLDOBLIQUE = "Courier-BoldOblique";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String HELVETICA = "Helvetica";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String HELVETICA_BOLD = "Helvetica-Bold";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String HELVETICA_OBLIQUE = "Helvetica-Oblique";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String HELVETICA_BOLDOBLIQUE = "Helvetica-BoldOblique";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String SYMBOL = "Symbol";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String TIMES_ROMAN = "Times-Roman";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String TIMES_BOLD = "Times-Bold";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String TIMES_ITALIC = "Times-Italic";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String TIMES_BOLDITALIC = "Times-BoldItalic";

		/// <summary>This is a possible value of a base 14 type 1 font</summary>
		public const String ZAPFDINGBATS = "ZapfDingbats";

		public const String TIMES = "Times";

		/// <summary>
		/// The maximum height above the baseline reached by glyphs in this
		/// font, excluding the height of glyphs for accented characters.
		/// </summary>
		public const int ASCENT = 1;

		/// <summary>
		/// The y coordinate of the top of flat capital letters, measured from
		/// the baseline.
		/// </summary>
		public const int CAPHEIGHT = 2;

		/// <summary>
		/// The maximum depth below the baseline reached by glyphs in this
		/// font.
		/// </summary>
		/// <remarks>
		/// The maximum depth below the baseline reached by glyphs in this
		/// font. The value is a negative number.
		/// </remarks>
		public const int DESCENT = 3;

		/// <summary>
		/// The angle, expressed in degrees counterclockwise from the vertical,
		/// of the dominant vertical strokes of the font.
		/// </summary>
		/// <remarks>
		/// The angle, expressed in degrees counterclockwise from the vertical,
		/// of the dominant vertical strokes of the font. The value is
		/// negative for fonts that slope to the right, as almost all italic fonts do.
		/// </remarks>
		public const int ITALICANGLE = 4;

		/// <summary>The lower left x glyph coordinate.</summary>
		public const int BBOXLLX = 5;

		/// <summary>The lower left y glyph coordinate.</summary>
		public const int BBOXLLY = 6;

		/// <summary>The upper right x glyph coordinate.</summary>
		public const int BBOXURX = 7;

		/// <summary>The upper right y glyph coordinate.</summary>
		public const int BBOXURY = 8;

		/// <summary>AWT Font property.</summary>
		public const int AWT_ASCENT = 9;

		/// <summary>AWT Font property.</summary>
		public const int AWT_DESCENT = 10;

		/// <summary>AWT Font property.</summary>
		public const int AWT_LEADING = 11;

		/// <summary>AWT Font property.</summary>
		public const int AWT_MAXADVANCE = 12;

		/// <summary>The underline position.</summary>
		/// <remarks>The underline position. Usually a negative value.</remarks>
		public const int UNDERLINE_POSITION = 13;

		/// <summary>The underline thickness.</summary>
		public const int UNDERLINE_THICKNESS = 14;

		/// <summary>The strikethrough position.</summary>
		public const int STRIKETHROUGH_POSITION = 15;

		/// <summary>The strikethrough thickness.</summary>
		public const int STRIKETHROUGH_THICKNESS = 16;

		/// <summary>The recommended vertical size for subscripts for this font.</summary>
		public const int SUBSCRIPT_SIZE = 17;

		/// <summary>The recommended vertical offset from the baseline for subscripts for this font.
		/// 	</summary>
		/// <remarks>The recommended vertical offset from the baseline for subscripts for this font. Usually a negative value.
		/// 	</remarks>
		public const int SUBSCRIPT_OFFSET = 18;

		/// <summary>The recommended vertical size for superscripts for this font.</summary>
		public const int SUPERSCRIPT_SIZE = 19;

		/// <summary>The recommended vertical offset from the baseline for superscripts for this font.
		/// 	</summary>
		public const int SUPERSCRIPT_OFFSET = 20;

		/// <summary>The weight class of the font, as defined by the font author.</summary>
		public const int WEIGHT_CLASS = 21;

		/// <summary>The width class of the font, as defined by the font author.</summary>
		public const int WIDTH_CLASS = 22;

		/// <summary>The entry of PDF FontDescriptor dictionary.</summary>
		/// <remarks>
		/// The entry of PDF FontDescriptor dictionary.
		/// (Optional; PDF 1.5; strongly recommended for Type 3 fonts in Tagged PDF documents)
		/// The weight (thickness) component of the fully-qualified font name or font specifier.
		/// A value larger than 500 indicates bold font-weight.
		/// </remarks>
		public const int FONT_WEIGHT = 23;

		/// <summary>A not defined character in a custom PDF encoding.</summary>
		public const String notdef = ".notdef";

		public static readonly double[] DefaultFontMatrix = new double[] { 0.001, 0, 0, 0.001
			, 0, 0 };

		protected internal const int HEAD_LOCA_FORMAT_OFFSET = 51;

		/// <summary>The code pages possible for a True Type font.</summary>
		protected internal static readonly String[] TrueTypeCodePages = new String[] { "1252 Latin 1"
			, "1250 Latin 2: Eastern Europe", "1251 Cyrillic", "1253 Greek", "1254 Turkish", 
			"1255 Hebrew", "1256 Arabic", "1257 Windows Baltic", "1258 Vietnamese", null, null
			, null, null, null, null, null, "874 Thai", "932 JIS/Japan", "936 Chinese: Simplified chars--PRC and Singapore"
			, "949 Korean Wansung", "950 Chinese: Traditional chars--Taiwan and Hong Kong", 
			"1361 Korean Johab", null, null, null, null, null, null, null, "Macintosh Character Set (US Roman)"
			, "OEM Character Set", "Symbol Character Set", null, null, null, null, null, null
			, null, null, null, null, null, null, null, null, null, null, "869 IBM Greek", "866 MS-DOS Russian"
			, "865 MS-DOS Nordic", "864 Arabic", "863 MS-DOS Canadian French", "862 Hebrew", 
			"861 MS-DOS Icelandic", "860 MS-DOS Portuguese", "857 IBM Turkish", "855 IBM Cyrillic; primarily Russian"
			, "852 Latin 2", "775 MS-DOS Baltic", "737 Greek; former 437 G", "708 Arabic; ASMO 708"
			, "850 WE/Latin 1", "437 US" };
		//-Font styles------------------------------------------------------------------------------------------------------
		//-Font types-------------------------------------------------------------------------------------------------------
		//-Default fonts----------------------------------------------------------------------------------------------------
		//-Font Descriptor--------------------------------------------------------------------------------------------------
		//-Internal constants-----------------------------------------------------------------------------------------------
		//-TrueType constants-----------------------------------------------------------------------------------------------
		//-True Type Code pages---------------------------------------------------------------------------------------------
	}
}
