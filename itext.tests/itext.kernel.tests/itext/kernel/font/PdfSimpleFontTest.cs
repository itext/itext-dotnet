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
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfSimpleFontTest : ExtendedITextTest {
        private const byte T_CODE = 116;

        private const byte E_CODE = 101;

        private const byte E_CUSTOM_CODE = 103;

        private const byte OPEN_BRACKET_CODE = 40;

        private const byte CLOSE_BRACKET_CODE = 41;

        private static Glyph E_GLYPH_FONT_SPECIFIC;

        private static Glyph T_GLYPH_FONT_SPECIFIC;

        private static Glyph E_GLYPH_CUSTOM_MAPPED;

        [NUnit.Framework.OneTimeSetUp]
        public static void Init() {
            T_GLYPH_FONT_SPECIFIC = new Glyph(T_CODE, 278, 116, new int[] { 14, -7, 257, 669 });
            T_GLYPH_FONT_SPECIFIC.SetChars(new char[] { 't' });
            E_GLYPH_FONT_SPECIFIC = new Glyph(E_CODE, 556, 101, new int[] { 40, -15, 516, 538 });
            E_GLYPH_FONT_SPECIFIC.SetChars(new char[] { 'e' });
            E_GLYPH_CUSTOM_MAPPED = new Glyph(E_CUSTOM_CODE, 44, 103);
            E_GLYPH_CUSTOM_MAPPED.SetChars(new char[] { 'e' });
        }

        [NUnit.Framework.Test]
        public virtual void CreateGlyphLineWithSpecificEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateFontSpecificEncoding
                ());
            GlyphLine glyphLine = fontToTest.CreateGlyphLine("te");
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            glyphs.Add(E_GLYPH_FONT_SPECIFIC);
            GlyphLine expected = new GlyphLine(glyphs, 0, 2);
            NUnit.Framework.Assert.AreEqual(expected, glyphLine);
        }

        [NUnit.Framework.Test]
        public virtual void CreateGlyphLineWithEmptyEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateEmptyFontEncoding
                ());
            GlyphLine glyphLine = fontToTest.CreateGlyphLine("te");
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(E_GLYPH_CUSTOM_MAPPED);
            GlyphLine expected = new GlyphLine(glyphs, 0, 1);
            NUnit.Framework.Assert.AreEqual(expected, glyphLine);
        }

        [NUnit.Framework.Test]
        public virtual void AppendGlyphsWithSpecificEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateFontSpecificEncoding
                ());
            IList<Glyph> toAppend = new List<Glyph>();
            int processed = fontToTest.AppendGlyphs("te", 0, 1, toAppend);
            NUnit.Framework.Assert.AreEqual(2, processed);
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            glyphs.Add(E_GLYPH_FONT_SPECIFIC);
            NUnit.Framework.Assert.AreEqual(glyphs, toAppend);
        }

        [NUnit.Framework.Test]
        public virtual void AppendGlyphsWithEmptyEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateEmptyFontEncoding
                ());
            IList<Glyph> toAppend = new List<Glyph>();
            int processed = fontToTest.AppendGlyphs("e ete", 0, 4, toAppend);
            NUnit.Framework.Assert.AreEqual(3, processed);
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(E_GLYPH_CUSTOM_MAPPED);
            glyphs.Add(E_GLYPH_CUSTOM_MAPPED);
            NUnit.Framework.Assert.AreEqual(glyphs, toAppend);
        }

        [NUnit.Framework.Test]
        public virtual void AppendAnyGlyphWithSpecificEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateFontSpecificEncoding
                ());
            IList<Glyph> toAppend = new List<Glyph>();
            int processed = fontToTest.AppendAnyGlyph("te", 0, toAppend);
            NUnit.Framework.Assert.AreEqual(1, processed);
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            NUnit.Framework.Assert.AreEqual(glyphs, toAppend);
        }

        [NUnit.Framework.Test]
        public virtual void AppendAnyGlyphWithEmptyEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateEmptyFontEncoding
                ());
            IList<Glyph> toAppend = new List<Glyph>();
            int processed = fontToTest.AppendAnyGlyph("e ete", 0, toAppend);
            NUnit.Framework.Assert.AreEqual(1, processed);
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(E_GLYPH_CUSTOM_MAPPED);
            NUnit.Framework.Assert.AreEqual(glyphs, toAppend);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertGlyphLineToBytesWithSpecificEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateFontSpecificEncoding
                ());
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            glyphs.Add(E_GLYPH_FONT_SPECIFIC);
            GlyphLine glyphLine = new GlyphLine(glyphs, 0, 2);
            byte[] bytes = fontToTest.ConvertToBytes(glyphLine);
            NUnit.Framework.Assert.AreEqual(new byte[] { T_CODE, E_CODE }, bytes);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertGlyphLineToBytesWithEmptyEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateEmptyFontEncoding
                ());
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            glyphs.Add(E_GLYPH_FONT_SPECIFIC);
            GlyphLine glyphLine = new GlyphLine(glyphs, 0, 2);
            byte[] bytes = fontToTest.ConvertToBytes(glyphLine);
            NUnit.Framework.Assert.AreEqual(new byte[0], bytes);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToBytesWithNullEntry() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateEmptyFontEncoding
                ());
            byte[] bytes = fontToTest.ConvertToBytes((GlyphLine)null);
            NUnit.Framework.Assert.AreEqual(new byte[0], bytes);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertGlyphToBytesWithSpecificEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateFontSpecificEncoding
                ());
            byte[] bytes = fontToTest.ConvertToBytes(E_GLYPH_FONT_SPECIFIC);
            NUnit.Framework.Assert.AreEqual(new byte[] { E_CODE }, bytes);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertGlyphToBytesWithCustomEncodingTest() {
            FontEncoding emptyFontEncoding = FontEncoding.CreateEmptyFontEncoding();
            emptyFontEncoding.AddSymbol(E_CUSTOM_CODE, E_CODE);
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(emptyFontEncoding);
            byte[] bytes = fontToTest.ConvertToBytes(E_GLYPH_FONT_SPECIFIC);
            NUnit.Framework.Assert.AreEqual(new byte[] { E_CUSTOM_CODE }, bytes);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertGlyphToBytesWithEmptyEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateEmptyFontEncoding
                ());
            byte[] bytes = fontToTest.ConvertToBytes(E_GLYPH_FONT_SPECIFIC);
            NUnit.Framework.Assert.AreEqual(new byte[0], bytes);
        }

        [NUnit.Framework.Test]
        public virtual void WriteTextGlyphLineWithSpecificEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateFontSpecificEncoding
                ());
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            glyphs.Add(E_GLYPH_FONT_SPECIFIC);
            GlyphLine glyphLine = new GlyphLine(glyphs, 0, 2);
            MemoryStream bos = new MemoryStream();
            using (PdfOutputStream pos = new PdfOutputStream(bos)) {
                fontToTest.WriteText(glyphLine, 0, 1, pos);
            }
            NUnit.Framework.Assert.AreEqual(new byte[] { OPEN_BRACKET_CODE, T_CODE, E_CODE, CLOSE_BRACKET_CODE }, bos.
                ToArray());
        }

        [NUnit.Framework.Test]
        public virtual void WriteTextGlyphLineWithCustomEncodingTest() {
            FontEncoding fontEncoding = FontEncoding.CreateEmptyFontEncoding();
            fontEncoding.AddSymbol(E_CUSTOM_CODE, E_CODE);
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(fontEncoding);
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(E_GLYPH_FONT_SPECIFIC);
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            GlyphLine glyphLine = new GlyphLine(glyphs, 0, 2);
            MemoryStream bos = new MemoryStream();
            using (PdfOutputStream pos = new PdfOutputStream(bos)) {
                fontToTest.WriteText(glyphLine, 0, 1, pos);
            }
            NUnit.Framework.Assert.AreEqual(new byte[] { OPEN_BRACKET_CODE, E_CUSTOM_CODE, CLOSE_BRACKET_CODE }, bos.ToArray
                ());
        }

        [NUnit.Framework.Test]
        public virtual void WriteTextGlyphLineWithEmptyEncodingTest() {
            PdfSimpleFont<FontProgram> fontToTest = new PdfSimpleFontTest.TestSimpleFont(FontEncoding.CreateEmptyFontEncoding
                ());
            IList<Glyph> glyphs = new List<Glyph>();
            glyphs.Add(E_GLYPH_FONT_SPECIFIC);
            glyphs.Add(T_GLYPH_FONT_SPECIFIC);
            GlyphLine glyphLine = new GlyphLine(glyphs, 0, 2);
            MemoryStream bos = new MemoryStream();
            using (PdfOutputStream pos = new PdfOutputStream(bos)) {
                fontToTest.WriteText(glyphLine, 0, 1, pos);
            }
            NUnit.Framework.Assert.AreEqual(new byte[] { OPEN_BRACKET_CODE, CLOSE_BRACKET_CODE }, bos.ToArray());
        }

        private class TestSimpleFont : PdfSimpleFont<FontProgram> {
            public TestSimpleFont(FontEncoding fontEncoding) {
                this.fontEncoding = fontEncoding;
                SetFontProgram(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
            }

            public override Glyph GetGlyph(int unicode) {
                if (unicode == E_CODE) {
                    return E_GLYPH_CUSTOM_MAPPED;
                }
                return null;
            }

            protected internal override void AddFontStream(PdfDictionary fontDescriptor) {
            }
        }
    }
}
