using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Test;

namespace iText.Layout.Renderer {
    public class TextPreprocessingUtilTest : ExtendedITextTest {
        private static PdfFont pdfFont;

        [NUnit.Framework.OneTimeSetUp]
        public static void InitializeFont() {
            pdfFont = PdfFontFactory.CreateFont();
        }

        [NUnit.Framework.Test]
        public virtual void EnSpaceTest() {
            SpecialWhitespaceGlyphTest('\u2002');
        }

        [NUnit.Framework.Test]
        public virtual void EmSpaceTest() {
            SpecialWhitespaceGlyphTest('\u2003');
        }

        [NUnit.Framework.Test]
        public virtual void ThinSpaceTest() {
            SpecialWhitespaceGlyphTest('\u2009');
        }

        [NUnit.Framework.Test]
        public virtual void HorizontalTabulationTest() {
            SpecialWhitespaceGlyphTest('\t');
        }

        [NUnit.Framework.Test]
        public virtual void RegularSymbolTest() {
            GlyphLine glyphLine = new GlyphLine();
            Glyph regularGlyph = pdfFont.GetGlyph('a');
            glyphLine.Add(0, regularGlyph);
            TextPreprocessingUtil.ReplaceSpecialWhitespaceGlyphs(glyphLine, pdfFont);
            Glyph glyph = glyphLine.Get(0);
            NUnit.Framework.Assert.AreEqual(regularGlyph, glyph);
        }

        private void SpecialWhitespaceGlyphTest(int unicode) {
            GlyphLine glyphLine = new GlyphLine();
            // Create a new glyph, because it is a special glyph, and it is not contained in the regular font
            glyphLine.Add(0, new Glyph(0, unicode));
            TextPreprocessingUtil.ReplaceSpecialWhitespaceGlyphs(glyphLine, pdfFont);
            Glyph glyph = glyphLine.Get(0);
            Glyph space = pdfFont.GetGlyph('\u0020');
            NUnit.Framework.Assert.IsTrue(space.GetCode() == glyph.GetCode() && space.GetWidth() == glyph.GetWidth());
        }
    }
}
