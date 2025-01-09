/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Font;
using iText.IO.Font.Otf;

namespace iText.Kernel.Font {
    /// <summary>FontProgram class for Type 3 font.</summary>
    /// <remarks>
    /// FontProgram class for Type 3 font. Contains map of
    /// <see cref="Type3Glyph"/>.
    /// Type3Glyphs belong to a particular pdf document.
    /// Note, an instance of Type3Font can not be reused for multiple pdf documents.
    /// </remarks>
    public class Type3Font : FontProgram {
        private readonly IDictionary<int, Type3Glyph> type3Glyphs = new Dictionary<int, Type3Glyph>();

        /// <summary>Stores glyphs without associated unicode.</summary>
        private readonly IDictionary<int, Type3Glyph> type3GlyphsWithoutUnicode = new Dictionary<int, Type3Glyph>(
            );

        private bool colorized = false;

        private int flags = 0;

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a Type 3 font program.</summary>
        /// <param name="colorized">defines whether the glyph color is specified in the glyph descriptions in the font.
        ///     </param>
        internal Type3Font(bool colorized) {
            this.colorized = colorized;
            this.fontNames = new FontNames();
            GetFontMetrics().SetBbox(0, 0, 0, 0);
        }
//\endcond

        /// <summary>Returns a glyph by unicode.</summary>
        /// <param name="unicode">glyph unicode</param>
        /// <returns>
        /// 
        /// <see cref="Type3Glyph"/>
        /// glyph, or
        /// <see langword="null"/>
        /// if this font does not contain glyph for the unicode
        /// </returns>
        public virtual Type3Glyph GetType3Glyph(int unicode) {
            return type3Glyphs.Get(unicode);
        }

        /// <summary>Returns a glyph by its code.</summary>
        /// <remarks>Returns a glyph by its code. These glyphs may not have unicode.</remarks>
        /// <param name="code">glyph code</param>
        /// <returns>
        /// 
        /// <see cref="Type3Glyph"/>
        /// glyph, or
        /// <see langword="null"/>
        /// if this font does not contain glyph for the code
        /// </returns>
        public virtual Type3Glyph GetType3GlyphByCode(int code) {
            Type3Glyph glyph = type3GlyphsWithoutUnicode.Get(code);
            if (glyph == null && codeToGlyph.Get(code) != null) {
                glyph = type3Glyphs.Get(codeToGlyph.Get(code).GetUnicode());
            }
            return glyph;
        }

        public override int GetPdfFontFlags() {
            return flags;
        }

        public override bool IsFontSpecific() {
            return false;
        }

        public virtual bool IsColorized() {
            return colorized;
        }

        public override int GetKerning(Glyph glyph1, Glyph glyph2) {
            return 0;
        }

        /// <summary>Returns number of glyphs for this font.</summary>
        /// <remarks>
        /// Returns number of glyphs for this font.
        /// Its also count glyphs without unicode.
        /// See
        /// <see cref="type3GlyphsWithoutUnicode"/>.
        /// </remarks>
        /// <returns>
        /// 
        /// <c>int</c>
        /// number off all glyphs
        /// </returns>
        public virtual int GetNumberOfGlyphs() {
            return type3Glyphs.Count + type3GlyphsWithoutUnicode.Count;
        }

        /// <summary>Sets the PostScript name of the font.</summary>
        /// <remarks>
        /// Sets the PostScript name of the font.
        /// <para />
        /// If full name is null, it will be set as well.
        /// </remarks>
        /// <param name="fontName">the PostScript name of the font, shall not be null or empty.</param>
        protected internal override void SetFontName(String fontName) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetFontName(fontName);
        }

        /// <summary>Sets a preferred font family name.</summary>
        /// <param name="fontFamily">a preferred font family name.</param>
        protected internal override void SetFontFamily(String fontFamily) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetFontFamily(fontFamily);
        }

        /// <summary>Sets font weight.</summary>
        /// <param name="fontWeight">
        /// integer form 100 to 900. See
        /// <see cref="iText.IO.Font.Constants.FontWeights"/>.
        /// </param>
        protected internal override void SetFontWeight(int fontWeight) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetFontWeight(fontWeight);
        }

        /// <summary>Sets font width in css notation (font-stretch property)</summary>
        /// <param name="fontWidth">
        /// 
        /// <see cref="iText.IO.Font.Constants.FontStretches"/>.
        /// </param>
        protected internal override void SetFontStretch(String fontWidth) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetFontStretch(fontWidth);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void SetCapHeight(int capHeight) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetCapHeight(capHeight);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void SetItalicAngle(int italicAngle) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetItalicAngle(italicAngle);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void SetTypoAscender(int ascender) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetTypoAscender(ascender);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void SetTypoDescender(int descender) {
            // This dummy override allows PdfType3Font to use setter from different module.
            base.SetTypoDescender(descender);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets Font descriptor flags.</summary>
        /// <seealso cref="iText.IO.Font.Constants.FontDescriptorFlags"/>
        /// <param name="flags">
        /// 
        /// <see cref="iText.IO.Font.Constants.FontDescriptorFlags"/>.
        /// </param>
        internal virtual void SetPdfFontFlags(int flags) {
            this.flags = flags;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddGlyph(int code, int unicode, int width, int[] bbox, Type3Glyph type3Glyph) {
            if (codeToGlyph.ContainsKey(code)) {
                RemoveGlyphFromMappings(code);
            }
            Glyph glyph = new Glyph(code, width, unicode, bbox);
            codeToGlyph.Put(code, glyph);
            if (unicode < 0) {
                type3GlyphsWithoutUnicode.Put(code, type3Glyph);
            }
            else {
                unicodeToGlyph.Put(unicode, glyph);
                type3Glyphs.Put(unicode, type3Glyph);
            }
            RecalculateAverageWidth();
        }
//\endcond

        private void RemoveGlyphFromMappings(int glyphCode) {
            Glyph removed = codeToGlyph.JRemove(glyphCode);
            if (removed == null) {
                return;
            }
            int unicode = removed.GetUnicode();
            if (unicode < 0) {
                type3GlyphsWithoutUnicode.JRemove(glyphCode);
            }
            else {
                unicodeToGlyph.JRemove(unicode);
                type3Glyphs.JRemove(unicode);
            }
        }

        private void RecalculateAverageWidth() {
            int widthSum = 0;
            int glyphsNumber = codeToGlyph.Count;
            foreach (Glyph glyph in codeToGlyph.Values) {
                if (glyph.GetWidth() == 0) {
                    glyphsNumber--;
                    continue;
                }
                widthSum += glyph.GetWidth();
            }
            avgWidth = glyphsNumber == 0 ? 0 : widthSum / glyphsNumber;
        }
    }
}
