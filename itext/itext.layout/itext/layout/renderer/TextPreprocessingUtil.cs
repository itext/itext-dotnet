/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Font;

namespace iText.Layout.Renderer {
    public sealed class TextPreprocessingUtil {
        private TextPreprocessingUtil() {
        }

        /// <summary>Replaces special whitespace glyphs to new whitespace '\u0020' glyph that has custom width.</summary>
        /// <remarks>
        /// Replaces special whitespace glyphs to new whitespace '\u0020' glyph that has custom width.
        /// Special whitespace glyphs are symbols such as '\u2002', '\u2003', '\u2009' and '\t'.
        /// </remarks>
        /// <param name="line">the string for preprocessing</param>
        /// <param name="font">the font that will be used when displaying the string</param>
        /// <returns>old line with new special whitespace glyphs</returns>
        public static GlyphLine ReplaceSpecialWhitespaceGlyphs(GlyphLine line, PdfFont font) {
            if (null != line) {
                bool isMonospaceFont = font.GetFontProgram().GetFontMetrics().IsFixedPitch();
                Glyph space = font.GetGlyph('\u0020');
                int spaceWidth = space.GetWidth();
                int lineSize = line.Size();
                for (int i = 0; i < lineSize; i++) {
                    Glyph glyph = line.Get(i);
                    int? xAdvance = CalculateXAdvancement(spaceWidth, isMonospaceFont, glyph);
                    bool isSpecialWhitespaceGlyph = xAdvance != null;
                    if (isSpecialWhitespaceGlyph) {
                        Glyph newGlyph = new Glyph(space, glyph.GetUnicode());
                        System.Diagnostics.Debug.Assert(xAdvance <= short.MaxValue && xAdvance >= short.MinValue);
                        newGlyph.SetXAdvance((short)(int)xAdvance);
                        line.Set(i, newGlyph);
                    }
                }
            }
            return line;
        }

        internal const int NON_MONO_SPACE_ENSP_WIDTH = 500;

        internal const int NON_MONO_SPACE_THINSP_WIDTH = 200;

        internal const int AMOUNT_OF_SPACE_IN_TAB = 3;

        private static int? CalculateXAdvancement(int spaceWidth, bool isMonospaceFont, Glyph glyph) {
            int? xAdvance = null;
            if (glyph.GetCode() <= 0) {
                switch (glyph.GetUnicode()) {
                    // ensp
                    case '\u2002': {
                        xAdvance = isMonospaceFont ? 0 : (NON_MONO_SPACE_ENSP_WIDTH - spaceWidth);
                        break;
                    }

                    // emsp
                    case '\u2003': {
                        xAdvance = isMonospaceFont ? 0 : (FontProgram.UNITS_NORMALIZATION - spaceWidth);
                        break;
                    }

                    // thinsp
                    case '\u2009': {
                        xAdvance = isMonospaceFont ? 0 : (NON_MONO_SPACE_THINSP_WIDTH - spaceWidth);
                        break;
                    }

                    case '\t': {
                        xAdvance = AMOUNT_OF_SPACE_IN_TAB * spaceWidth;
                        break;
                    }

                    default: {
                        return xAdvance;
                    }
                }
            }
            return xAdvance;
        }
    }
}
