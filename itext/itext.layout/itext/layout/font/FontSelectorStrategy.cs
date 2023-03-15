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
using System.Collections.Generic;
using iText.IO.Font.Otf;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    /// <summary>
    /// <see cref="FontSelectorStrategy"/>
    /// is responsible for splitting text into sub texts with one particular font.
    /// </summary>
    /// <remarks>
    /// <see cref="FontSelectorStrategy"/>
    /// is responsible for splitting text into sub texts with one particular font.
    /// <see cref="NextGlyphs()"/>
    /// will create next sub text and set current font.
    /// </remarks>
    public abstract class FontSelectorStrategy {
        protected internal String text;

        protected internal int index;

        protected internal readonly FontProvider provider;

        protected internal readonly FontSet additionalFonts;

        protected internal FontSelectorStrategy(String text, FontProvider provider, FontSet additionalFonts) {
            this.text = text;
            this.index = 0;
            this.provider = provider;
            this.additionalFonts = additionalFonts;
        }

        public virtual bool EndOfText() {
            return text == null || index >= text.Length;
        }

        public abstract PdfFont GetCurrentFont();

        public abstract IList<Glyph> NextGlyphs();

        /// <summary>Utility method to create PdfFont.</summary>
        /// <param name="fontInfo">instance of FontInfo.</param>
        /// <returns>cached or just created PdfFont on success, otherwise null.</returns>
        /// <seealso cref="FontProvider.GetPdfFont(FontInfo, FontSet)"/>
        protected internal virtual PdfFont GetPdfFont(FontInfo fontInfo) {
            return provider.GetPdfFont(fontInfo, additionalFonts);
        }
    }
}
