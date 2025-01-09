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
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
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
            NUnit.Framework.Assert.AreEqual(space.GetCode(), glyph.GetCode());
            NUnit.Framework.Assert.AreEqual(space.GetWidth(), glyph.GetWidth());
            NUnit.Framework.Assert.AreEqual(space.GetUnicode(), glyph.GetUnicode());
            NUnit.Framework.Assert.AreEqual(iText.IO.Util.TextUtil.ConvertFromUtf32(unicode), glyph.GetChars());
        }
    }
}
