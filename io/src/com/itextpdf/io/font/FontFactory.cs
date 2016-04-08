/*
$Id: 8af601b3603f8794c202337201ff2f4cbbc3dd3d $

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
using com.itextpdf.io.util;

namespace com.itextpdf.io.font
{
	/// <summary>Provides methods for creating various types of fonts.</summary>
	public class FontFactory
	{
		private static FontRegisterProvider fontRegisterProvider = new FontRegisterProvider
			();

		/// <summary>Creates a new font.</summary>
		/// <remarks>
		/// Creates a new font. This will always be the default Helvetica font (not embedded).
		/// This method is introduced because Helvetica is used in many examples.
		/// </remarks>
		/// <returns>a BaseFont object (Helvetica, Winansi, not embedded)</returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont()
		{
			return CreateFont(FontConstants.HELVETICA);
		}

		/// <summary>Creates a new font.</summary>
		/// <remarks>
		/// Creates a new font. This font can be one of the 14 built in types,
		/// a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
		/// Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
		/// appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
		/// example would be "STSong-Light,Bold". Note that this modifiers do not work if
		/// the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
		/// This would get the second font (indexes start at 0), in this case "MS PGothic".
		/// <p/>
		/// The fonts are cached and if they already exist they are extracted from the cache,
		/// not parsed again.
		/// <p/>
		/// Besides the common encodings described by name, custom encodings
		/// can also be made. These encodings will only work for the single byte fonts
		/// Type1 and TrueType. The encoding string starts with a '#'
		/// followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
		/// of hex values representing the Unicode codes that compose that encoding.<br />
		/// The "simple" encoding is recommended for TrueType fonts
		/// as the "full" encoding risks not matching the character with the right glyph
		/// if not done with care.<br />
		/// The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
		/// described by non standard names like the Tex math fonts. Each group of three elements
		/// compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
		/// used to access the glyph. The space must be assigned to character position 32 otherwise
		/// text justification will not work.
		/// <p/>
		/// Example for a "simple" encoding that includes the Unicode
		/// character space, A, B and ecyrillic:
		/// <PRE>
		/// "# simple 32 0020 0041 0042 0454"
		/// </PRE>
		/// <p/>
		/// Example for a "full" encoding for a Type1 Tex font:
		/// <PRE>
		/// "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
		/// </PRE>
		/// <p/>
		/// This method calls:<br />
		/// <PRE>
		/// createFont(name, null, true);
		/// </PRE>
		/// </remarks>
		/// <param name="name">the name of the font or its location on file</param>
		/// <returns>returns a new font. This font may come from the cache</returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont(String name)
		{
			return CreateFont(name, null, true);
		}

		/// <summary>Creates a new font.</summary>
		/// <remarks>
		/// Creates a new font. This font can be one of the 14 built in types,
		/// a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
		/// Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
		/// appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
		/// example would be "STSong-Light,Bold". Note that this modifiers do not work if
		/// the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
		/// This would get the second font (indexes start at 0), in this case "MS PGothic".
		/// <p/>
		/// The fonts are cached and if they already exist they are extracted from the cache,
		/// not parsed again.
		/// <p/>
		/// Besides the common encodings described by name, custom encodings
		/// can also be made. These encodings will only work for the single byte fonts
		/// Type1 and TrueType. The encoding string starts with a '#'
		/// followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
		/// of hex values representing the Unicode codes that compose that encoding.<br />
		/// The "simple" encoding is recommended for TrueType fonts
		/// as the "full" encoding risks not matching the character with the right glyph
		/// if not done with care.<br />
		/// The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
		/// described by non standard names like the Tex math fonts. Each group of three elements
		/// compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
		/// used to access the glyph. The space must be assigned to character position 32 otherwise
		/// text justification will not work.
		/// <p/>
		/// Example for a "simple" encoding that includes the Unicode
		/// character space, A, B and ecyrillic:
		/// <PRE>
		/// "# simple 32 0020 0041 0042 0454"
		/// </PRE>
		/// <p/>
		/// Example for a "full" encoding for a Type1 Tex font:
		/// <PRE>
		/// "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
		/// </PRE>
		/// <p/>
		/// This method calls:<br />
		/// <PRE>
		/// createFont(name, encoding, embedded, true, null, null);
		/// </PRE>
		/// </remarks>
		/// <param name="font">the name of the font or its location on file</param>
		/// <param name="cached">
		/// ttrue if the font comes from the cache or is added to
		/// the cache if new, false if the font is always created new
		/// </param>
		/// <returns>returns a new font. This font may come from the cache</returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont(String font, bool cached)
		{
			return CreateFont(font, null, cached);
		}

		/// <summary>Creates a new font.</summary>
		/// <remarks>
		/// Creates a new font. This font can be one of the 14 built in types,
		/// a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
		/// Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
		/// appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
		/// example would be "STSong-Light,Bold". Note that this modifiers do not work if
		/// the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
		/// This would get the second font (indexes start at 0), in this case "MS PGothic".
		/// <p/>
		/// Besides the common encodings described by name, custom encodings
		/// can also be made. These encodings will only work for the single byte fonts
		/// Type1 and TrueType. The encoding string starts with a '#'
		/// followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
		/// of hex values representing the Unicode codes that compose that encoding.<br />
		/// The "simple" encoding is recommended for TrueType fonts
		/// as the "full" encoding risks not matching the character with the right glyph
		/// if not done with care.<br />
		/// The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
		/// described by non standard names like the Tex math fonts. Each group of three elements
		/// compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
		/// used to access the glyph. The space must be assigned to character position 32 otherwise
		/// text justification will not work.
		/// <p/>
		/// Example for a "simple" encoding that includes the Unicode
		/// character space, A, B and ecyrillic:
		/// <PRE>
		/// "# simple 32 0020 0041 0042 0454"
		/// </PRE>
		/// <p/>
		/// Example for a "full" encoding for a Type1 Tex font:
		/// <PRE>
		/// "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
		/// </PRE>
		/// </remarks>
		/// <param name="font">
		/// the true type font or the afm in a byte array
		/// an exception if the font is not recognized. Note that even if true an exception may be thrown in some circumstances.
		/// This parameter is useful for FontFactory that may have to check many invalid font names before finding the right one
		/// </param>
		/// <returns>
		/// returns a new font. This font may come from the cache but only if cached
		/// is true, otherwise it will always be created new
		/// </returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont(byte[] font)
		{
			return CreateFont(null, font, false);
		}

		/// <summary>Creates a new font.</summary>
		/// <remarks>
		/// Creates a new font. This font can be one of the 14 built in types,
		/// a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
		/// Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
		/// appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
		/// example would be "STSong-Light,Bold". Note that this modifiers do not work if
		/// the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
		/// This would get the second font (indexes start at 0), in this case "MS PGothic".
		/// <p/>
		/// The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
		/// If the <CODE>byte</CODE> arrays are present the font will be
		/// read from them instead of the name. A name is still required to identify
		/// the font type.
		/// <p/>
		/// Besides the common encodings described by name, custom encodings
		/// can also be made. These encodings will only work for the single byte fonts
		/// Type1 and TrueType. The encoding string starts with a '#'
		/// followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
		/// of hex values representing the Unicode codes that compose that encoding.<br />
		/// The "simple" encoding is recommended for TrueType fonts
		/// as the "full" encoding risks not matching the character with the right glyph
		/// if not done with care.<br />
		/// The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
		/// described by non standard names like the Tex math fonts. Each group of three elements
		/// compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
		/// used to access the glyph. The space must be assigned to character position 32 otherwise
		/// text justification will not work.
		/// <p/>
		/// Example for a "simple" encoding that includes the Unicode
		/// character space, A, B and ecyrillic:
		/// <PRE>
		/// "# simple 32 0020 0041 0042 0454"
		/// </PRE>
		/// <p/>
		/// Example for a "full" encoding for a Type1 Tex font:
		/// <PRE>
		/// "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
		/// </PRE>
		/// </remarks>
		/// <param name="name">the name of the font or its location on file</param>
		/// <param name="font">the true type font or the afm in a byte array</param>
		/// <param name="cached">
		/// true if the font comes from the cache or is added to
		/// the cache if new, false if the font is always created new
		/// </param>
		/// <returns>
		/// returns a new font. This font may come from the cache but only if cached
		/// is true, otherwise it will always be created new
		/// </returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont(String name, byte[] font, bool cached)
		{
			String baseName = FontProgram.GetBaseName(name);
			//yes, we trying to find built-in standard font with original name, not baseName.
			bool isBuiltinFonts14 = FontConstants.BUILTIN_FONTS_14.Contains(name);
			bool isCidFont = !isBuiltinFonts14 && FontCache.IsPredefinedCidFont(baseName);
			FontProgram fontFound;
			if (cached && name != null)
			{
				fontFound = FontCache.GetFont(name);
				if (fontFound != null)
				{
					return fontFound;
				}
			}
			if (name == null)
			{
				if (font != null)
				{
					try
					{
						return new TrueTypeFont(font);
					}
					catch (Exception)
					{
					}
					try
					{
						return new Type1Font(null, null, font, null);
					}
					catch (Exception)
					{
					}
				}
				throw new IOException(IOException.FontIsNotRecognized);
			}
			FontProgram fontBuilt;
			if (isBuiltinFonts14 || name.ToLower().EndsWith(".afm") || name.ToLower().EndsWith
				(".pfm"))
			{
				fontBuilt = new Type1Font(name, null, font, null);
			}
			else
			{
				if (baseName.ToLower().EndsWith(".ttf") || baseName.ToLower().EndsWith(".otf") ||
					 baseName.ToLower().IndexOf(".ttc,") > 0)
				{
					if (font != null)
					{
						fontBuilt = new TrueTypeFont(font);
					}
					else
					{
						fontBuilt = new TrueTypeFont(name);
					}
				}
				else
				{
					if (isCidFont)
					{
						fontBuilt = new CidFont(name, FontCache.GetCompatibleCmaps(baseName));
					}
					else
					{
						throw new IOException(IOException.Font1IsNotRecognized).SetMessageParams(name);
					}
				}
			}
			return cached ? FontCache.SaveFont(fontBuilt, name) : fontBuilt;
		}

		// todo make comment relevant to type 1 font creation
		/// <summary>Creates a new font.</summary>
		/// <remarks>
		/// Creates a new font. This font can be one of the 14 built in types,
		/// a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
		/// Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
		/// appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
		/// example would be "STSong-Light,Bold". Note that this modifiers do not work if
		/// the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
		/// This would get the second font (indexes start at 0), in this case "MS PGothic".
		/// <p/>
		/// The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
		/// If the <CODE>byte</CODE> arrays are present the font will be
		/// read from them instead of the name. A name is still required to identify
		/// the font type.
		/// <p/>
		/// Besides the common encodings described by name, custom encodings
		/// can also be made. These encodings will only work for the single byte fonts
		/// Type1 and TrueType. The encoding string starts with a '#'
		/// followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
		/// of hex values representing the Unicode codes that compose that encoding.<br />
		/// The "simple" encoding is recommended for TrueType fonts
		/// as the "full" encoding risks not matching the character with the right glyph
		/// if not done with care.<br />
		/// The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
		/// described by non standard names like the Tex math fonts. Each group of three elements
		/// compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
		/// used to access the glyph. The space must be assigned to character position 32 otherwise
		/// text justification will not work.
		/// <p/>
		/// Example for a "simple" encoding that includes the Unicode
		/// character space, A, B and ecyrillic:
		/// <PRE>
		/// "# simple 32 0020 0041 0042 0454"
		/// </PRE>
		/// <p/>
		/// Example for a "full" encoding for a Type1 Tex font:
		/// <PRE>
		/// "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
		/// </PRE>
		/// </remarks>
		/// <param name="name">the name of the font or its location on file</param>
		/// <param name="cached">
		/// true if the font comes from the cache or is added to
		/// the cache if new, false if the font is always created new
		/// </param>
		/// <param name="afm">the afm or pfm metrics file in a byte array</param>
		/// <param name="pfb">the pfb in a byte array</param>
		/// <returns>
		/// returns a new font. This font may come from the cache but only if cached
		/// is true, otherwise it will always be created new
		/// </returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateType1Font(String name, byte[] afm, byte[] pfb, bool
			 cached)
		{
			FontProgram fontProgram;
			if (cached && name != null)
			{
				fontProgram = FontCache.GetFont(name);
				if (fontProgram != null)
				{
					return fontProgram;
				}
			}
			fontProgram = new Type1Font(name, null, afm, pfb);
			return cached && name != null ? FontCache.SaveFont(fontProgram, name) : fontProgram;
		}

		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateType1Font(byte[] afm, byte[] pfb)
		{
			return CreateType1Font(null, afm, pfb, false);
		}

		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateType1Font(String metricsPath, String binaryPath, 
			bool cached)
		{
			FontProgram fontProgram;
			if (cached && metricsPath != null)
			{
				fontProgram = FontCache.GetFont(metricsPath);
				if (fontProgram != null)
				{
					return fontProgram;
				}
			}
			fontProgram = new Type1Font(metricsPath, binaryPath, null, null);
			return cached && metricsPath != null ? FontCache.SaveFont(fontProgram, metricsPath
				) : fontProgram;
		}

		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateType1Font(String metricsPath, String binaryPath)
		{
			return CreateType1Font(metricsPath, binaryPath, true);
		}

		/// <summary>
		/// Creates a new True Type font from ttc file,
		/// <p/>
		/// The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
		/// </summary>
		/// <remarks>
		/// Creates a new True Type font from ttc file,
		/// <p/>
		/// The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
		/// If the <CODE>byte</CODE> arrays are present the font will be
		/// read from them instead of the name. A name is still required to identify
		/// the font type.
		/// <p/>
		/// Besides the common encodings described by name, custom encodings
		/// can also be made. These encodings will only work for the single byte fonts
		/// Type1 and TrueType. The encoding string starts with a '#'
		/// followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
		/// of hex values representing the Unicode codes that compose that encoding.<br />
		/// The "simple" encoding is recommended for TrueType fonts
		/// as the "full" encoding risks not matching the character with the right glyph
		/// if not done with care.<br />
		/// The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
		/// described by non standard names like the Tex math fonts. Each group of three elements
		/// compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
		/// used to access the glyph. The space must be assigned to character position 32 otherwise
		/// text justification will not work.
		/// <p/>
		/// Example for a "simple" encoding that includes the Unicode
		/// character space, A, B and ecyrillic:
		/// <PRE>
		/// "# simple 32 0020 0041 0042 0454"
		/// </PRE>
		/// <p/>
		/// Example for a "full" encoding for a Type1 Tex font:
		/// <PRE>
		/// "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
		/// </PRE>
		/// </remarks>
		/// <param name="ttcPath">location  of true type collection file (*.ttc)</param>
		/// <param name="ttcIndex">the encoding to be applied to this font</param>
		/// <param name="cached">
		/// true if the font comes from the cache or is added to
		/// the cache if new, false if the font is always created new
		/// </param>
		/// <returns>
		/// returns a new font. This font may come from the cache but only if cached
		/// is true, otherwise it will always be created new
		/// </returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont(String ttcPath, int ttcIndex, bool cached)
		{
			if (cached)
			{
				FontProgram fontFound = FontCache.GetFont(ttcPath + ttcIndex);
				if (fontFound != null)
				{
					return fontFound;
				}
			}
			FontProgram fontBuilt = new TrueTypeFont(ttcPath, ttcIndex);
			return cached ? FontCache.SaveFont(fontBuilt, ttcPath + ttcIndex) : fontBuilt;
		}

		/// <summary>
		/// Creates a new True Type font from ttc bytes array,
		/// <p/>
		/// The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
		/// </summary>
		/// <remarks>
		/// Creates a new True Type font from ttc bytes array,
		/// <p/>
		/// The fonts may or may not be cached depending on the flag <CODE>cached</CODE>.
		/// If the <CODE>byte</CODE> arrays are present the font will be
		/// read from them instead of the name. A name is still required to identify
		/// the font type.
		/// <p/>
		/// Besides the common encodings described by name, custom encodings
		/// can also be made. These encodings will only work for the single byte fonts
		/// Type1 and TrueType. The encoding string starts with a '#'
		/// followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
		/// of hex values representing the Unicode codes that compose that encoding.<br />
		/// The "simple" encoding is recommended for TrueType fonts
		/// as the "full" encoding risks not matching the character with the right glyph
		/// if not done with care.<br />
		/// The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
		/// described by non standard names like the Tex math fonts. Each group of three elements
		/// compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the Unicode character
		/// used to access the glyph. The space must be assigned to character position 32 otherwise
		/// text justification will not work.
		/// <p/>
		/// Example for a "simple" encoding that includes the Unicode
		/// character space, A, B and ecyrillic:
		/// <PRE>
		/// "# simple 32 0020 0041 0042 0454"
		/// </PRE>
		/// <p/>
		/// Example for a "full" encoding for a Type1 Tex font:
		/// <PRE>
		/// "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
		/// </PRE>
		/// </remarks>
		/// <param name="ttcPath">bytes array of ttc font</param>
		/// <param name="ttcIndex">the encoding to be applied to this font</param>
		/// <returns>returns a new font.</returns>
		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont(String ttcPath, int ttcIndex)
		{
			return CreateFont(ttcPath, ttcIndex, true);
		}

		// TODO should we cache fonts based on byte array?
		/// <exception cref="System.IO.IOException"/>
		internal static FontProgram CreateFont(byte[] ttc, int ttcIndex, bool cached)
		{
			if (cached)
			{
				String ttcNameKey = String.Format("{0}{1}", ArrayUtil.HashCode(ttc), ttcIndex);
				FontProgram fontFound = FontCache.GetFont(ttcNameKey);
				if (fontFound != null)
				{
					return fontFound;
				}
			}
			FontProgram fontBuilt = new TrueTypeFont(ttc, ttcIndex);
			String ttcNameKey_1 = String.Format("{0}{1}", ArrayUtil.HashCode(ttc), ttcIndex);
			return cached ? FontCache.SaveFont(fontBuilt, ttcNameKey_1) : fontBuilt;
		}

		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateFont(byte[] ttc, int ttcIndex)
		{
			return CreateFont(ttc, ttcIndex, false);
		}

		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateRegisteredFont(String fontName, int style, bool cached
			)
		{
			return fontRegisterProvider.GetFont(fontName, style, cached);
		}

		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateRegisteredFont(String fontName, int style)
		{
			return fontRegisterProvider.GetFont(fontName, style);
		}

		/// <exception cref="System.IO.IOException"/>
		public static FontProgram CreateRegisteredFont(String fontName)
		{
			return fontRegisterProvider.GetFont(fontName, FontConstants.UNDEFINED);
		}

		/// <summary>Register a font by giving explicitly the font family and name.</summary>
		/// <param name="familyName">the font family</param>
		/// <param name="fullName">the font name</param>
		/// <param name="path">the font path</param>
		public static void RegisterFamily(String familyName, String fullName, String path
			)
		{
			fontRegisterProvider.RegisterFamily(familyName, fullName, path);
		}

		/// <summary>Register a ttf- or a ttc-file.</summary>
		/// <param name="path">the path to a ttf- or ttc-file</param>
		public static void Register(String path)
		{
			Register(path, null);
		}

		/// <summary>Register a font file and use an alias for the font contained in it.</summary>
		/// <param name="path">the path to a font file</param>
		/// <param name="alias">the alias you want to use for the font</param>
		public static void Register(String path, String alias)
		{
			fontRegisterProvider.Register(path, alias);
		}

		/// <summary>Register all the fonts in a directory.</summary>
		/// <param name="dir">the directory</param>
		/// <returns>the number of fonts registered</returns>
		public static int RegisterDirectory(String dir)
		{
			return fontRegisterProvider.RegisterDirectory(dir);
		}

		/// <summary>Register fonts in some probable directories.</summary>
		/// <remarks>
		/// Register fonts in some probable directories. It usually works in Windows,
		/// Linux and Solaris.
		/// </remarks>
		/// <returns>the number of fonts registered</returns>
		public static int RegisterSystemDirectories()
		{
			return fontRegisterProvider.RegisterSystemDirectories();
		}

		/// <summary>Gets a set of registered font names.</summary>
		/// <returns>a set of registered fonts</returns>
		public static ICollection<String> GetRegisteredFonts()
		{
			return fontRegisterProvider.GetRegisteredFonts();
		}

		/// <summary>Gets a set of registered font names.</summary>
		/// <returns>a set of registered font families</returns>
		public static ICollection<String> GetRegisteredFamilies()
		{
			return fontRegisterProvider.GetRegisteredFamilies();
		}

		/// <summary>Gets a set of registered font names.</summary>
		/// <param name="fontname">of a font that may or may not be registered</param>
		/// <returns>true if a given font is registered</returns>
		public static bool Contains(String fontname)
		{
			return fontRegisterProvider.IsRegistered(fontname);
		}

		/// <summary>Checks if a certain font is registered.</summary>
		/// <param name="fontname">the name of the font that has to be checked.</param>
		/// <returns>true if the font is found</returns>
		public static bool IsRegistered(String fontname)
		{
			return fontRegisterProvider.IsRegistered(fontname);
		}
	}
}
