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
                Glyph space = font.GetGlyph('\u0020');
                Glyph glyph;
                for (int i = 0; i < line.Size(); i++) {
                    glyph = line.Get(i);
                    int? xAdvance = GetSpecialWhitespaceXAdvance(glyph, space, font.GetFontProgram().GetFontMetrics().IsFixedPitch
                        ());
                    if (xAdvance != null) {
                        Glyph newGlyph = new Glyph(space, glyph.GetUnicode());
                        System.Diagnostics.Debug.Assert(xAdvance <= short.MaxValue && xAdvance >= short.MinValue);
                        newGlyph.SetXAdvance((short)(int)xAdvance);
                        line.Set(i, newGlyph);
                    }
                }
            }
            return line;
        }

        private static int? GetSpecialWhitespaceXAdvance(Glyph glyph, Glyph spaceGlyph, bool isMonospaceFont) {
            if (glyph.GetCode() > 0) {
                return null;
            }
            switch (glyph.GetUnicode()) {
                // ensp
                case '\u2002': {
                    return isMonospaceFont ? 0 : 500 - spaceGlyph.GetWidth();
                }

                // emsp
                case '\u2003': {
                    return isMonospaceFont ? 0 : 1000 - spaceGlyph.GetWidth();
                }

                // thinsp
                case '\u2009': {
                    return isMonospaceFont ? 0 : 200 - spaceGlyph.GetWidth();
                }

                case '\t': {
                    return 3 * spaceGlyph.GetWidth();
                }
            }
            return null;
        }
    }
}
