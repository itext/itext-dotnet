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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfFontUnitTest : ExtendedITextTest {
        public const int FONT_METRICS_DESCENT = -40;

        public const int FONT_METRICS_ASCENT = 700;

        public const int FONT_SIZE = 50;

        public class TestFont : PdfFont {
            public const int SIMPLE_GLYPH = 97;

            public const int SIMPLE_GLYPH_WITHOUT_BBOX = 98;

            public const int SIMPLE_GLYPH_WITH_POSITIVE_DESCENT = 99;

            public const int COMPLEX_GLYPH = 119070;

            public const int ZERO_CODE_GLYPH = 0;

            // these are two parts of G-clef glyph
            public static readonly char[] COMPLEX_GLYPH_AS_CHARS = new char[] { '\ud834', '\udd1e' };

            public const int SIMPLE_GLYPH_WIDTH = 100;

            public const int COMPLEX_GLYPH_WIDTH = 200;

            public TestFont()
                : base() {
            }

            public TestFont(PdfDictionary dictionary)
                : base(dictionary) {
            }

            public virtual void SetFontProgram(FontProgram fontProgram) {
                this.fontProgram = fontProgram;
            }

            public override Glyph GetGlyph(int unicode) {
                if (unicode == SIMPLE_GLYPH) {
                    return new Glyph(1, SIMPLE_GLYPH_WIDTH, SIMPLE_GLYPH, new int[] { 10, -20, 200, 600 });
                }
                else {
                    if (unicode == SIMPLE_GLYPH_WITHOUT_BBOX) {
                        return new Glyph(2, SIMPLE_GLYPH_WIDTH, SIMPLE_GLYPH);
                    }
                    else {
                        if (unicode == SIMPLE_GLYPH_WITH_POSITIVE_DESCENT) {
                            return new Glyph(3, SIMPLE_GLYPH_WIDTH, SIMPLE_GLYPH, new int[] { 10, 10, 200, 600 });
                        }
                        else {
                            if (unicode == COMPLEX_GLYPH) {
                                return new Glyph(4, COMPLEX_GLYPH_WIDTH, COMPLEX_GLYPH, new int[] { 20, -100, 400, 800 });
                            }
                            else {
                                if (unicode == ZERO_CODE_GLYPH) {
                                    return new Glyph(0, 0, 0);
                                }
                            }
                        }
                    }
                }
                return null;
            }

            public override GlyphLine CreateGlyphLine(String content) {
                return null;
            }

            public override int AppendGlyphs(String text, int from, int to, IList<Glyph> glyphs) {
                return 0;
            }

            public override int AppendAnyGlyph(String text, int from, IList<Glyph> glyphs) {
                return 0;
            }

            public override byte[] ConvertToBytes(String text) {
                return new byte[0];
            }

            public override byte[] ConvertToBytes(GlyphLine glyphLine) {
                return new byte[0];
            }

            public override String Decode(PdfString content) {
                return null;
            }

            public override GlyphLine DecodeIntoGlyphLine(PdfString content) {
                return null;
            }

            public override float GetContentWidth(PdfString content) {
                return 0;
            }

            public override byte[] ConvertToBytes(Glyph glyph) {
                return new byte[0];
            }

            public override void WriteText(GlyphLine text, int from, int to, PdfOutputStream stream) {
            }

            public override void WriteText(String text, PdfOutputStream stream) {
            }

            protected internal override PdfDictionary GetFontDescriptor(String fontName) {
                return null;
            }
        }

        public class TestFontProgram : FontProgram {
            public override int GetPdfFontFlags() {
                return 0;
            }

            public override int GetKerning(Glyph first, Glyph second) {
                return 0;
            }

            public override FontMetrics GetFontMetrics() {
                return new PdfFontUnitTest.TestFontMetrics();
            }

            public override bool IsFontSpecific() {
                return true;
            }
        }

        public class TestFontMetrics : FontMetrics {
            public TestFontMetrics() {
                SetTypoDescender(FONT_METRICS_DESCENT);
                SetTypoAscender(FONT_METRICS_ASCENT);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithoutParamsTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.AreEqual(PdfName.Font, font.GetPdfObject().Get(PdfName.Type));
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithDictionaryTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.A, PdfName.B);
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont(dictionary);
            NUnit.Framework.Assert.AreEqual(PdfName.Font, font.GetPdfObject().Get(PdfName.Type));
            NUnit.Framework.Assert.AreEqual(PdfName.B, font.GetPdfObject().Get(PdfName.A));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsGlyphTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.IsTrue(font.ContainsGlyph(PdfFontUnitTest.TestFont.SIMPLE_GLYPH));
            NUnit.Framework.Assert.IsFalse(font.ContainsGlyph(111));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroGlyphIsAllowedOnlyIfFontIsSymbolicTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.IsFalse(font.ContainsGlyph(PdfFontUnitTest.TestFont.ZERO_CODE_GLYPH));
            font.SetFontProgram(new PdfFontUnitTest.TestFontProgram());
            NUnit.Framework.Assert.IsTrue(font.ContainsGlyph(PdfFontUnitTest.TestFont.ZERO_CODE_GLYPH));
        }

        [NUnit.Framework.Test]
        public virtual void GetWidthUnicodeTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.AreEqual(PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WIDTH, font.GetWidth(PdfFontUnitTest.TestFont
                .SIMPLE_GLYPH));
            NUnit.Framework.Assert.AreEqual(0, font.GetWidth(111));
        }

        [NUnit.Framework.Test]
        public virtual void GetWidthFontSizeTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            double expectedValue = PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WIDTH * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION;
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetWidth(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE
                ), 0.1);
            NUnit.Framework.Assert.AreEqual(0, font.GetWidth(111));
        }

        [NUnit.Framework.Test]
        public virtual void GetWidthOfStringTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] text = GetSentence(3);
            String textAsString = new String(text);
            NUnit.Framework.Assert.AreEqual(3 * PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WIDTH, font.GetWidth(textAsString
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetWidthOfSurrogatePairTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] text = new char[] { PdfFontUnitTest.TestFont.COMPLEX_GLYPH_AS_CHARS[0], PdfFontUnitTest.TestFont.COMPLEX_GLYPH_AS_CHARS
                [1], (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH };
            String textAsString = new String(text);
            NUnit.Framework.Assert.AreEqual(PdfFontUnitTest.TestFont.COMPLEX_GLYPH_WIDTH + PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WIDTH
                , font.GetWidth(textAsString));
        }

        [NUnit.Framework.Test]
        public virtual void GetWidthOfUnknownGlyphsTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] text = new char[] { (char)111, (char)222, (char)333 };
            String textAsString = new String(text);
            NUnit.Framework.Assert.AreEqual(0, font.GetWidth(textAsString));
        }

        [NUnit.Framework.Test]
        public virtual void GetWidthOfStringWithFontSizeTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] text = GetSentence(3);
            String textAsString = new String(text);
            double expectedValue = 3 * PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WIDTH * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION;
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetWidth(textAsString, FONT_SIZE), 0.1);
        }

        [NUnit.Framework.Test]
        public virtual void GetDescentOfGlyphTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            int expectedDescent = font.GetGlyph(PdfFontUnitTest.TestFont.SIMPLE_GLYPH).GetBbox()[1];
            int expectedValue = (int)(expectedDescent * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetDescent(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DescentCannotBePositiveTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.AreEqual(0, font.GetDescent(PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WITH_POSITIVE_DESCENT
                , 50));
        }

        [NUnit.Framework.Test]
        public virtual void GetDescentOfUnknownGlyphTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.AreEqual(0, font.GetDescent(111, 50));
        }

        [NUnit.Framework.Test]
        public virtual void GetDescentOfGlyphWithoutBBoxTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            font.SetFontProgram(new PdfFontUnitTest.TestFontProgram());
            int expectedValue = (int)(FONT_METRICS_DESCENT * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetDescent(PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WITHOUT_BBOX
                , FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void GetDescentOfTextTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] text = new char[] { (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH, PdfFontUnitTest.TestFont.COMPLEX_GLYPH_AS_CHARS
                [0], PdfFontUnitTest.TestFont.COMPLEX_GLYPH_AS_CHARS[1] };
            String textAsString = new String(text);
            int expectedMinDescent = Math.Min(font.GetGlyph(PdfFontUnitTest.TestFont.SIMPLE_GLYPH).GetBbox()[1], font.
                GetGlyph(PdfFontUnitTest.TestFont.COMPLEX_GLYPH).GetBbox()[1]);
            int expectedValue = (int)(expectedMinDescent * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetDescent(textAsString, FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void GetDescentOfTextWithGlyphWithoutBBoxTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            font.SetFontProgram(new PdfFontUnitTest.TestFontProgram());
            char[] text = new char[] { (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH, (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WITHOUT_BBOX
                 };
            String textAsString = new String(text);
            int expectedMinDescent = Math.Min(font.GetGlyph(PdfFontUnitTest.TestFont.SIMPLE_GLYPH).GetBbox()[1], FONT_METRICS_DESCENT
                );
            int expectedValue = (int)(expectedMinDescent * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetDescent(textAsString, FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void GetAscentOfGlyphTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            int expectedAscent = font.GetGlyph(PdfFontUnitTest.TestFont.SIMPLE_GLYPH).GetBbox()[3];
            int expectedValue = (int)(expectedAscent * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetAscent(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetAscentOfGlyphWithoutBBoxTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            font.SetFontProgram(new PdfFontUnitTest.TestFontProgram());
            int expectedValue = (int)(FONT_METRICS_ASCENT * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetAscent(PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WITHOUT_BBOX
                , FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void GetAscentOfTextTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] text = new char[] { (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH, PdfFontUnitTest.TestFont.COMPLEX_GLYPH_AS_CHARS
                [0], PdfFontUnitTest.TestFont.COMPLEX_GLYPH_AS_CHARS[1] };
            String textAsString = new String(text);
            int expectedMaxAscent = Math.Max(font.GetGlyph(PdfFontUnitTest.TestFont.SIMPLE_GLYPH).GetBbox()[3], font.GetGlyph
                (PdfFontUnitTest.TestFont.COMPLEX_GLYPH).GetBbox()[3]);
            int expectedValue = (int)(expectedMaxAscent * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetAscent(textAsString, FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void GetAscentOfTextWithGlyphWithoutBBoxTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            font.SetFontProgram(new PdfFontUnitTest.TestFontProgram());
            char[] text = new char[] { (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH, (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH_WITHOUT_BBOX
                 };
            String textAsString = new String(text);
            int expectedMaxAscent = Math.Max(font.GetGlyph(PdfFontUnitTest.TestFont.SIMPLE_GLYPH).GetBbox()[3], FONT_METRICS_ASCENT
                );
            int expectedValue = (int)(expectedMaxAscent * FONT_SIZE / (double)FontProgram.UNITS_NORMALIZATION);
            NUnit.Framework.Assert.AreEqual(expectedValue, font.GetAscent(textAsString, FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void IsEmbeddedTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
            font.embedded = true;
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void IsSubsetTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.IsTrue(font.IsSubset());
            font.SetSubset(false);
            NUnit.Framework.Assert.IsFalse(font.IsSubset());
        }

        [NUnit.Framework.Test]
        public virtual void AddSubsetRangeTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            font.SetSubset(false);
            int[] range1 = new int[] { 1, 2 };
            int[] range2 = new int[] { 10, 20 };
            font.AddSubsetRange(range1);
            font.AddSubsetRange(range2);
            NUnit.Framework.Assert.IsTrue(font.IsSubset());
            NUnit.Framework.Assert.AreEqual(2, font.subsetRanges.Count);
            NUnit.Framework.Assert.AreEqual(range1, font.subsetRanges[0]);
            NUnit.Framework.Assert.AreEqual(range2, font.subsetRanges[1]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitSentenceFitMaxWidthTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] words = GetSentence(3, 3);
            String wordsAsString = new String(words);
            double width = 6 * font.GetWidth(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE);
            IList<String> result = font.SplitString(wordsAsString, FONT_SIZE, (float)width + 0.01f);
            NUnit.Framework.Assert.AreEqual(1, result.Count);
            NUnit.Framework.Assert.AreEqual(wordsAsString, result[0]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitSentenceWordFitMaxWidthTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] words = GetSentence(3, 4, 2);
            String wordsAsString = new String(words);
            double width = 4 * font.GetWidth(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE);
            IList<String> result = font.SplitString(wordsAsString, FONT_SIZE, (float)width + 0.01f);
            NUnit.Framework.Assert.AreEqual(3, result.Count);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(3)), result[0]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(4)), result[1]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(2)), result[2]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitSentenceWordDoesNotFitMaxWidthCase_PartIsCombinedWithTheFollowingWordTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] words = GetSentence(3, 4, 2);
            String wordsAsString = new String(words);
            double width = 3 * font.GetWidth(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE);
            IList<String> result = font.SplitString(wordsAsString, FONT_SIZE, (float)width + 0.01f);
            NUnit.Framework.Assert.AreEqual(3, result.Count);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(3)), result[0]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(3)), result[1]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(1, 2)), result[2]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitSentenceWordDoesNotFitMaxWidthCase_PartIsOnTheSeparateLineTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] words = GetSentence(2, 4, 3);
            String wordsAsString = new String(words);
            double width = 3 * font.GetWidth(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE);
            IList<String> result = font.SplitString(wordsAsString, FONT_SIZE, (float)width + 0.01f);
            NUnit.Framework.Assert.AreEqual(4, result.Count);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(2)), result[0]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(3)), result[1]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(1)), result[2]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(3)), result[3]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitSentenceSymbolDoesNotFitLineTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] words = GetSentence(3);
            String wordsAsString = new String(words);
            double width = font.GetWidth(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE) / 2.0;
            IList<String> result = font.SplitString(wordsAsString, FONT_SIZE, (float)width + 0.01f);
            NUnit.Framework.Assert.AreEqual(4, result.Count);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(1)), result[0]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(1)), result[1]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(1)), result[2]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(0)), result[3]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitSentenceWithLineBreakTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            char[] words = new char[] { (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH, '\n', (char)PdfFontUnitTest.TestFont
                .SIMPLE_GLYPH };
            String wordsAsString = new String(words);
            double width = 10 * font.GetWidth(PdfFontUnitTest.TestFont.SIMPLE_GLYPH, FONT_SIZE);
            IList<String> result = font.SplitString(wordsAsString, FONT_SIZE, (float)width + 0.01f);
            NUnit.Framework.Assert.AreEqual(2, result.Count);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(1)), result[0]);
            NUnit.Framework.Assert.AreEqual(new String(GetSentence(1)), result[1]);
        }

        [NUnit.Framework.Test]
        public virtual void IsBuiltWithTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.IsFalse(font.IsBuiltWith("Any String Here", "Any Encoding"));
        }

        [NUnit.Framework.Test]
        public virtual void IsWrappedObjectMustBeIndirectTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.IsTrue(font.IsWrappedObjectMustBeIndirect());
        }

        [NUnit.Framework.Test]
        public virtual void UpdateEmbeddedSubsetPrefixTest() {
            String fontName = "FontTest";
            String embeddedSubsetFontName = PdfFontUnitTest.TestFont.UpdateSubsetPrefix(fontName, true, true);
            String onlySubsetFontName = PdfFontUnitTest.TestFont.UpdateSubsetPrefix(fontName, true, false);
            String onlyEmbeddedFontName = PdfFontUnitTest.TestFont.UpdateSubsetPrefix(fontName, false, true);
            String justFontName = PdfFontUnitTest.TestFont.UpdateSubsetPrefix(fontName, false, false);
            NUnit.Framework.Assert.AreEqual(fontName, onlySubsetFontName);
            NUnit.Framework.Assert.AreEqual(fontName, onlyEmbeddedFontName);
            NUnit.Framework.Assert.AreEqual(fontName, justFontName);
            Regex prefixPattern = iText.Commons.Utils.StringUtil.RegexCompile("^[A-Z]{6}\\+FontTest$");
            NUnit.Framework.Assert.IsTrue(iText.Commons.Utils.Matcher.Match(prefixPattern, embeddedSubsetFontName).Matches
                ());
        }

        [NUnit.Framework.Test]
        public virtual void GetEmptyPdfStreamTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => font.GetPdfFontStream(null, null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FONT_EMBEDDING_ISSUE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfStreamTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            byte[] data = new byte[10];
            for (int i = 0; i < 10; i++) {
                data[i] = (byte)i;
            }
            int[] fontStreamLength = new int[] { 10, 20, 30 };
            PdfStream stream = font.GetPdfFontStream(data, fontStreamLength);
            NUnit.Framework.Assert.AreEqual(data, stream.GetBytes());
            NUnit.Framework.Assert.AreEqual(10, stream.GetAsNumber(new PdfName("Length1")).IntValue());
            NUnit.Framework.Assert.AreEqual(20, stream.GetAsNumber(new PdfName("Length2")).IntValue());
            NUnit.Framework.Assert.AreEqual(30, stream.GetAsNumber(new PdfName("Length3")).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void GetFontProgramTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            PdfFontUnitTest.TestFontProgram program = new PdfFontUnitTest.TestFontProgram();
            NUnit.Framework.Assert.IsNull(font.GetFontProgram());
            font.SetFontProgram(program);
            NUnit.Framework.Assert.AreEqual(program, font.GetFontProgram());
        }

        [NUnit.Framework.Test]
        public virtual void ToStringTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            NUnit.Framework.Assert.AreEqual("PdfFont{fontProgram=" + font.fontProgram + "}", font.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void MakeObjectIndirectWhileFontIsIndirectTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                // to avoid an exception
                document.AddNewPage();
                PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
                font.GetPdfObject().MakeIndirect(document);
                PdfDictionary dictionary = new PdfDictionary();
                NUnit.Framework.Assert.IsTrue(font.MakeObjectIndirect(dictionary));
                NUnit.Framework.Assert.IsNotNull(dictionary.GetIndirectReference());
                NUnit.Framework.Assert.AreEqual(document, dictionary.GetIndirectReference().GetDocument());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MakeObjectIndirectWhileFontIsDirectTest() {
            PdfFontUnitTest.TestFont font = new PdfFontUnitTest.TestFont();
            PdfDictionary dictionary = new PdfDictionary();
            NUnit.Framework.Assert.IsFalse(font.MakeObjectIndirect(dictionary));
            NUnit.Framework.Assert.IsNull(dictionary.GetIndirectReference());
        }

        private char[] GetSentence(params int[] lengthsOfWords) {
            int length = 0;
            foreach (int lengthOfWord in lengthsOfWords) {
                length += lengthOfWord;
            }
            int numberOfSpaces = lengthsOfWords.Length - 1;
            length += numberOfSpaces;
            char[] sentence = new char[length];
            int index = 0;
            foreach (int lengthOfWord in lengthsOfWords) {
                for (int i = 0; i < lengthOfWord; i++) {
                    sentence[index] = (char)PdfFontUnitTest.TestFont.SIMPLE_GLYPH;
                    index++;
                }
                if (index < length) {
                    sentence[index] = ' ';
                    index++;
                }
            }
            return sentence;
        }

        [NUnit.Framework.Test]
        public virtual void CannotGetFontStreamForNullBytesTest() {
            PdfFont pdfFont = PdfFontFactory.CreateFont();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfFont.GetPdfFontStream(null
                , new int[] { 1 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FONT_EMBEDDING_ISSUE, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotGetFontStreamForNullLengthsTest() {
            PdfFont pdfFont = PdfFontFactory.CreateFont();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfFont.GetPdfFontStream(new 
                byte[] { 1 }, null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FONT_EMBEDDING_ISSUE, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotGetFontStreamForNullBytesAndLengthsTest() {
            PdfFont pdfFont = PdfFontFactory.CreateFont();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfFont.GetPdfFontStream(null
                , null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FONT_EMBEDDING_ISSUE, exception.Message);
        }
    }
}
