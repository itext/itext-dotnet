/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;

namespace iText.IO.Font {
    /// <summary>Base representation of a font program and its glyph, naming, and metric data.</summary>
    /// <remarks>
    /// Base representation of a font program and its glyph, naming, and metric data.
    /// <para />
    /// Glyph metrics exposed by this abstraction use the normalized 1000-unit PDF glyph space.
    /// </remarks>
    public abstract class FontProgram {
        public const int HORIZONTAL_SCALING_FACTOR = 100;

        public const int DEFAULT_WIDTH = 1000;

        public const int UNITS_NORMALIZATION = 1000;

        /// <summary>Converts a PDF text-space value to normalized glyph space.</summary>
        /// <param name="value">value in text space</param>
        /// <returns>
        /// value divided by
        /// <see cref="UNITS_NORMALIZATION"/>
        /// </returns>
        public static float ConvertTextSpaceToGlyphSpace(float value) {
            return value / UNITS_NORMALIZATION;
        }

        /// <summary>Converts a normalized glyph-space value to PDF text space.</summary>
        /// <param name="value">value in normalized glyph space</param>
        /// <returns>
        /// value multiplied by
        /// <see cref="UNITS_NORMALIZATION"/>
        /// </returns>
        public static float ConvertGlyphSpaceToTextSpace(float value) {
            return value * UNITS_NORMALIZATION;
        }

        /// <summary>Converts a normalized glyph-space value to PDF text space without losing double precision.</summary>
        /// <param name="value">value in normalized glyph space</param>
        /// <returns>
        /// value multiplied by
        /// <see cref="UNITS_NORMALIZATION"/>
        /// </returns>
        public static double ConvertGlyphSpaceToTextSpace(double value) {
            return value * UNITS_NORMALIZATION;
        }

        /// <summary>Converts an integral normalized glyph-space value to PDF text space.</summary>
        /// <param name="value">value in normalized glyph space</param>
        /// <returns>
        /// value multiplied by
        /// <see cref="UNITS_NORMALIZATION"/>
        /// </returns>
        public static int ConvertGlyphSpaceToTextSpace(int value) {
            return value * UNITS_NORMALIZATION;
        }

        // In case Type1: char code to glyph.
        // In case TrueType: glyph index to glyph.
        protected internal IDictionary<int, Glyph> codeToGlyph = new Dictionary<int, Glyph>();

        protected internal IDictionary<int, Glyph> unicodeToGlyph = new Dictionary<int, Glyph>();

        /// <summary>
        /// Indicates that character codes are interpreted using the font's built-in encoding rather than a
        /// Unicode-oriented encoding.
        /// </summary>
        protected internal bool isFontSpecific;

        /// <summary>Naming data extracted from the font program.</summary>
        protected internal FontNames fontNames;

        /// <summary>Metrics extracted from the font program.</summary>
        protected internal FontMetrics fontMetrics = new FontMetrics();

        protected internal FontIdentification fontIdentification = new FontIdentification();

        /// <summary>Average glyph width in normalized glyph units.</summary>
        protected internal int avgWidth;

        /// <summary>The font's encoding name.</summary>
        /// <remarks>
        /// The font's encoding name. This encoding is 'StandardEncoding' or 'AdobeStandardEncoding' for a font
        /// that can be totally encoded according to the characters names. For all other names the font is treated as
        /// symbolic.
        /// </remarks>
        protected internal String encodingScheme = FontEncoding.FONT_SPECIFIC;

        /// <summary>
        /// CID registry name associated with this font, or
        /// <see langword="null"/>
        /// for fonts without a CID registry.
        /// </summary>
        protected internal String registry;

        /// <summary>Gets the amount of glyphs in this font program.</summary>
        /// <returns>the amount of glyphs in this font program</returns>
        public virtual int CountOfGlyphs() {
            return Math.Max(codeToGlyph.Count, unicodeToGlyph.Count);
        }

        /// <summary>Gets the parsed font naming data.</summary>
        /// <returns>naming data owned by this program</returns>
        public virtual FontNames GetFontNames() {
            return fontNames;
        }

        /// <summary>Gets the parsed font metric data.</summary>
        /// <returns>metric data owned by this program</returns>
        public virtual FontMetrics GetFontMetrics() {
            return fontMetrics;
        }

        /// <summary>Gets the parsed font identification data.</summary>
        /// <returns>identification data owned by this program</returns>
        public virtual FontIdentification GetFontIdentification() {
            return fontIdentification;
        }

        /// <summary>Gets the CID registry name.</summary>
        /// <returns>
        /// CID registry, or
        /// <see langword="null"/>
        /// when none was supplied
        /// </returns>
        public virtual String GetRegistry() {
            return registry;
        }

        /// <summary>Computes the PDF font descriptor flags for this program.</summary>
        /// <returns>bit set defined by the PDF font descriptor specification</returns>
        public abstract int GetPdfFontFlags();

        /// <summary>Checks whether this program uses its built-in character encoding.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when character codes are font-specific
        /// </returns>
        public virtual bool IsFontSpecific() {
            return isFontSpecific;
        }

        /// <summary>Get glyph's width.</summary>
        /// <param name="unicode">a unicode symbol or FontSpecif code.</param>
        /// <returns>Gets width in normalized 1000 units.</returns>
        public virtual int GetWidth(int unicode) {
            Glyph glyph = GetGlyph(unicode);
            return glyph != null ? glyph.GetWidth() : 0;
        }

        /// <summary>Gets the average glyph width.</summary>
        /// <returns>average width in normalized glyph units</returns>
        public virtual int GetAvgWidth() {
            return avgWidth;
        }

        /// <summary>Get glyph's bbox.</summary>
        /// <param name="unicode">a unicode symbol or FontSpecif code.</param>
        /// <returns>Gets bbox in normalized 1000 units.</returns>
        public virtual int[] GetCharBBox(int unicode) {
            Glyph glyph = GetGlyph(unicode);
            return glyph != null ? glyph.GetBbox() : null;
        }

        /// <summary>Looks up a glyph by Unicode value.</summary>
        /// <param name="unicode">Unicode scalar value</param>
        /// <returns>
        /// matching glyph, or
        /// <see langword="null"/>
        /// when unmapped
        /// </returns>
        public virtual Glyph GetGlyph(int unicode) {
            return unicodeToGlyph.Get(unicode);
        }

        /// <summary>Looks up a glyph by its format-specific code.</summary>
        /// <param name="charCode">Type 1 character code or OpenType glyph index</param>
        /// <returns>
        /// matching glyph, or
        /// <see langword="null"/>
        /// when absent
        /// </returns>
        public virtual Glyph GetGlyphByCode(int charCode) {
            return codeToGlyph.Get(charCode);
        }

        /// <summary>Checks whether this program supplies kerning pairs.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if supplies,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool HasKernPairs() {
            return false;
        }

        /// <summary>Gets the kerning between two glyphs.</summary>
        /// <param name="first">the first unicode value</param>
        /// <param name="second">the second unicode value</param>
        /// <returns>the kerning to be applied</returns>
        public virtual int GetKerning(int first, int second) {
            return GetKerning(unicodeToGlyph.Get(first), unicodeToGlyph.Get(second));
        }

        /// <summary>Gets the kerning between two glyphs.</summary>
        /// <param name="first">the first glyph</param>
        /// <param name="second">the second glyph</param>
        /// <returns>the kerning to be applied</returns>
        public abstract int GetKerning(Glyph first, Glyph second);

        /// <summary>
        /// Checks whether the
        /// <see cref="FontProgram"/>
        /// was built with corresponding fontName.
        /// </summary>
        /// <remarks>
        /// Checks whether the
        /// <see cref="FontProgram"/>
        /// was built with corresponding fontName.
        /// Default value is false unless overridden.
        /// </remarks>
        /// <param name="fontName">a font name or path to a font program</param>
        /// <returns>true, if the FontProgram was built with the fontProgram. Otherwise false.</returns>
        public virtual bool IsBuiltWith(String fontName) {
            return false;
        }

        /// <summary>Sets the CID registry associated with this font program.</summary>
        /// <param name="registry">
        /// CID registry name, or
        /// <see langword="null"/>
        /// when unavailable
        /// </param>
        protected internal virtual void SetRegistry(String registry) {
            this.registry = registry;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the name without the modifiers Bold, Italic or BoldItalic.</summary>
        /// <param name="name">the full name of the font</param>
        /// <returns>the name without the modifiers Bold, Italic or BoldItalic</returns>
        internal static String TrimFontStyle(String name) {
            if (name == null) {
                return null;
            }
            if (name.EndsWith(",Bold")) {
                return name.JSubstring(0, name.Length - 5);
            }
            else {
                if (name.EndsWith(",Italic")) {
                    return name.JSubstring(0, name.Length - 7);
                }
                else {
                    if (name.EndsWith(",BoldItalic")) {
                        return name.JSubstring(0, name.Length - 11);
                    }
                    else {
                        return name;
                    }
                }
            }
        }
//\endcond

        /// <summary>Sets typo ascender.</summary>
        /// <remarks>
        /// Sets typo ascender. See also
        /// <see cref="FontMetrics.SetTypoAscender(int)"/>.
        /// </remarks>
        /// <param name="ascender">typo ascender value in 1000-units</param>
        protected internal virtual void SetTypoAscender(int ascender) {
            fontMetrics.SetTypoAscender(ascender);
        }

        /// <summary>Sets typo descender.</summary>
        /// <remarks>
        /// Sets typo descender. See also
        /// <see cref="FontMetrics.SetTypoDescender(int)"/>.
        /// </remarks>
        /// <param name="descender">typo descender value in 1000-units</param>
        protected internal virtual void SetTypoDescender(int descender) {
            fontMetrics.SetTypoDescender(descender);
        }

        /// <summary>Sets the capital letters height.</summary>
        /// <remarks>
        /// Sets the capital letters height. See also
        /// <see cref="FontMetrics.SetCapHeight(int)"/>.
        /// </remarks>
        /// <param name="capHeight">cap height in 1000-units</param>
        protected internal virtual void SetCapHeight(int capHeight) {
            fontMetrics.SetCapHeight(capHeight);
        }

        /// <summary>Sets the x-height in source font units.</summary>
        /// <param name="xHeight">height of lowercase flat characters above the baseline</param>
        protected internal virtual void SetXHeight(int xHeight) {
            fontMetrics.SetXHeight(xHeight);
        }

        /// <summary>Sets the PostScript italic angle.</summary>
        /// <remarks>
        /// Sets the PostScript italic angle.
        /// <para />
        /// Italic angle in counterclockwise degrees from the vertical. Zero for upright text, negative for text that leans
        /// to the right (forward).
        /// </remarks>
        /// <param name="italicAngle">in counterclockwise degrees from the vertical</param>
        protected internal virtual void SetItalicAngle(int italicAngle) {
            fontMetrics.SetItalicAngle(italicAngle);
        }

        /// <summary>Sets the dominant vertical stem width.</summary>
        /// <param name="stemV">
        /// Type 1/CFF
        /// <c>StdVW</c>
        /// value in glyph units
        /// </param>
        protected internal virtual void SetStemV(int stemV) {
            fontMetrics.SetStemV(stemV);
        }

        /// <summary>Sets the dominant horizontal stem width.</summary>
        /// <param name="stemH">
        /// Type 1/CFF
        /// <c>StdHW</c>
        /// value in glyph units
        /// </param>
        protected internal virtual void SetStemH(int stemH) {
            fontMetrics.SetStemH(stemH);
        }

        /// <summary>Sets font weight.</summary>
        /// <param name="fontWeight">
        /// integer form 100 to 900. See
        /// <see cref="iText.IO.Font.Constants.FontWeights"/>.
        /// </param>
        protected internal virtual void SetFontWeight(int fontWeight) {
            fontNames.SetFontWeight(fontWeight);
        }

        /// <summary>Sets font width in css notation (font-stretch property)</summary>
        /// <param name="fontWidth">
        /// 
        /// <see cref="iText.IO.Font.Constants.FontStretches"/>.
        /// </param>
        protected internal virtual void SetFontStretch(String fontWidth) {
            fontNames.SetFontStretch(fontWidth);
        }

        /// <summary>Sets whether the font declares a fixed width.</summary>
        /// <param name="isFixedPitch">
        /// 
        /// <see langword="true"/>
        /// for a monospaced font
        /// </param>
        protected internal virtual void SetFixedPitch(bool isFixedPitch) {
            fontMetrics.SetIsFixedPitch(isFixedPitch);
        }

        protected internal virtual void SetBold(bool isBold) {
            if (isBold) {
                fontNames.SetMacStyle(fontNames.GetMacStyle() | FontMacStyleFlags.BOLD);
            }
            else {
                fontNames.SetMacStyle(fontNames.GetMacStyle() & (~FontMacStyleFlags.BOLD));
            }
        }

        /// <summary>Sets the normalized font bounding box from a four-element coordinate array.</summary>
        /// <param name="bbox">lower-left x/y and upper-right x/y coordinates</param>
        protected internal virtual void SetBbox(int[] bbox) {
            fontMetrics.SetBbox(bbox[0], bbox[1], bbox[2], bbox[3]);
        }

        /// <summary>Sets a preferred font family name.</summary>
        /// <param name="fontFamily">a preferred font family name.</param>
        protected internal virtual void SetFontFamily(String fontFamily) {
            fontNames.SetFamilyName(fontFamily);
        }

        /// <summary>Sets the PostScript name of the font.</summary>
        /// <remarks>
        /// Sets the PostScript name of the font.
        /// <para />
        /// If full name is null, it will be set as well.
        /// </remarks>
        /// <param name="fontName">the PostScript name of the font, shall not be null or empty.</param>
        protected internal virtual void SetFontName(String fontName) {
            fontNames.SetFontName(fontName);
            if (fontNames.GetFullName() == null) {
                fontNames.SetFullName(fontName);
            }
        }

        /// <summary>Ensures that an existing Unicode space glyph is also addressable through its font code.</summary>
        protected internal virtual void FixSpaceIssue() {
            Glyph space = unicodeToGlyph.Get(32);
            if (space != null) {
                codeToGlyph.Put(space.GetCode(), space);
            }
        }

        public override String ToString() {
            String name = GetFontNames().GetFontName();
            return name != null && name.Length > 0 ? name : base.ToString();
        }
    }
}
