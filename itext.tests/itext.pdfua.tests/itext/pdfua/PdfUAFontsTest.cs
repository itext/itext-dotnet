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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAFontsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAFontsTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/";

        private static readonly String FONT = FONTS_FOLDER + "FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TryToUseType0Cid0FontTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont("KozMinPro-Regular", "UniJIS-UCS2-H", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            );
            framework.AssertBothFail("tryToUseType0Cid0FontTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .FONT_SHOULD_BE_EMBEDDED, "KozMinPro-Regular"), false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Type0Cid2FontTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("type0Cid2FontTest");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TrueTypeFontTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("trueTypeFontTest");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TrueTypeFontGlyphNotPresentTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, "# simple 32 0020 00C5 1987", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 36).ShowText("world").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("trueTypeFontGlyphNotPresentTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE, "w"), false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TrueTypeFontWithDifferencesTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, "# simple 32 0077 006f 0072 006c 0064", PdfFontFactory.EmbeddingStrategy
                        .PREFER_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 36).ShowText("world").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("trueTypeFontWithDifferencesTest", PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TryToUseStandardFontsTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(StandardFonts.COURIER, "", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Helloworld");
                document.Add(paragraph);
            }
            );
            framework.AssertBothFail("tryToUseStandardFontsTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .FONT_SHOULD_BE_EMBEDDED, "Courier"), false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Type1EmbeddedFontTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(FONTS_FOLDER + "cmr10.afm", FONTS_FOLDER
                         + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Helloworld");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("type1EmbeddedFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void NonSymbolicTtfWithChangedCmapTest() {
            // TODO DEVSIX-9076 NPE when cmap of True Type Font doesn't contain Microsoft Unicode or Macintosh Roman encodings
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => PdfFontFactory.CreateFont(FONTS_FOLDER 
                + "FreeSans_changed_cmap.ttf", PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED)
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSymbolicTtfWithValidEncodingTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("nonSymbolicTtfWithValidEncodingTest");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSymbolicTtfWithIncompatibleEncodingTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            framework.AssertBothFail("nonSymbolicTtfWithIncompatibleEncoding", PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SymbolicTtfTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONTS_FOLDER + "iTextSymbolicFont.ttf", PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("symbolicTtf");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SymbolicTtfWithEncodingTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONTS_FOLDER + "iTextSymbolicFont.ttf", PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                font.GetPdfObject().Put(PdfName.Encoding, PdfName.MacRomanEncoding);
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            // VeraPDF is valid since iText fixes symbolic flag to non-symbolic on closing.
            framework.AssertITextFailVeraPdfValid("symbolicTtfWithEncoding", PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_NOT_CONTAIN_ENCODING
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SymbolicTtfWithInvalidCmapTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    TrueTypeFont fontProgram = new PdfUAFontsTest.CustomSymbolicTrueTypeFont(FONT);
                    font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            // VeraPDF is valid since iText fixes symbolic flag to non-symbolic on closing.
            if (PdfConformance.PDF_UA_1.Equals(conformance)) {
                framework.AssertITextFailVeraPdfValid("symbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_CONTAIN_EXACTLY_ONE_OR_AT_LEAST_MICROSOFT_SYMBOL_CMAP
                    );
            }
            else {
                framework.AssertITextFailVeraPdfValid("symbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_SYMBOL_CMAP
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSymbolicTtfWithInvalidCmapTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    TrueTypeFont fontProgram = new PdfUAFontsTest.CustomNonSymbolicTrueTypeFont(FONT);
                    font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            // VeraPDF is valid since the file itself is valid, but itext code is modified for testing.
            if (PdfConformance.PDF_UA_1.Equals(conformance)) {
                framework.AssertITextFailVeraPdfValid("nonSymbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.
                    NON_SYMBOLIC_TTF_SHALL_CONTAIN_NON_SYMBOLIC_CMAP);
            }
            else {
                framework.AssertITextFailVeraPdfValid("nonSymbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.
                    NON_SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_UNI_CMAP);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfWithChangedCmapTest() {
            // TODO DEVSIX-9076 NPE when cmap of True Type Font doesn't contain Microsoft Unicode or Macintosh Roman encodings
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => PdfFontFactory.CreateFont(FONTS_FOLDER 
                + "iTextSymbolicFontChangedCmap.ttf", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED));
        }

        [NUnit.Framework.Test]
        public virtual void NotdefGlyphTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false, PdfConformance
                .PDF_UA_2);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoNaskhArabic-Regular.ttf");
                }
                catch (System.IO.IOException) {
                }
                // ignore
                GlyphLine glyphLine = new GlyphLine();
                FontProgram fontProgram = font.GetFontProgram();
                // zero glyph in the font is .notdef glyph without Unicode
                glyphLine.Add(fontProgram.GetGlyphByCode(0));
                glyphLine.SetEnd(glyphLine.Size());
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 10).ShowText(glyphLine).EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("notdefGlyph", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "�"));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroUnicodeGlyphTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false, PdfConformance
                .PDF_UA_2);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoNaskhArabic-Regular.ttf");
                }
                catch (System.IO.IOException) {
                }
                // ignore
                GlyphLine glyphLine = new GlyphLine();
                FontProgram fontProgram = font.GetFontProgram();
                // 1 index glyph in the font is .null glyph with Unicode U+0000
                glyphLine.Add(fontProgram.GetGlyphByCode(1));
                glyphLine.SetEnd(glyphLine.Size());
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 10).ShowText(glyphLine).EndText().RestoreState().CloseTag();
            }
            );
            // TODO DEVSIX-10160 missing check on iText side for ToUnicode mapping to 0, fffe and feff
            framework.AssertVeraPdfFailITextValid("zeroUnicodeGlyph");
        }

        [NUnit.Framework.Test]
        public virtual void GlyphsWithoutUnicodeTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false, PdfConformance
                .PDF_UA_2);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoNaskhArabic-Regular.ttf");
                }
                catch (System.IO.IOException) {
                }
                // ignore
                GlyphLine glyphLine = new GlyphLine();
                FontProgram fontProgram = font.GetFontProgram();
                // 0 index glyph is .notdef without Unicode
                // 1 index glyph is .null with Unicode U+0000
                for (int i = 2; i < fontProgram.CountOfGlyphs(); i++) {
                    glyphLine.Add(fontProgram.GetGlyphByCode(i));
                }
                glyphLine.SetEnd(glyphLine.Size());
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 10).ShowText(glyphLine).EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("glyphsWithoutUnicode", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "�"));
        }

        [NUnit.Framework.Test]
        public virtual void FontWithReplacementCharTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false, PdfConformance
                .PDF_UA_2);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoSans-Regular.ttf");
                }
                catch (System.IO.IOException) {
                }
                // ignore
                GlyphLine glyphLine = new GlyphLine();
                FontProgram fontProgram = font.GetFontProgram();
                // font contain replacement char U+FFFD
                for (int i = 0; i < fontProgram.CountOfGlyphs(); i++) {
                    glyphLine.Add(fontProgram.GetGlyphByCode(i));
                }
                glyphLine.SetEnd(glyphLine.Size());
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 10).ShowText(glyphLine).EndText().RestoreState().CloseTag();
            }
            );
            // TODO DEVSIX-10160 missing check on iText side for ToUnicode mapping to 0, fffe and feff
            // TODO DEVSIX-10160 glyphs without Unicode mapped to Replacement Char which exist in the font, it's why iText doesn't fail
            framework.AssertVeraPdfFailITextValid("fontWithReplacementChar");
        }

        [NUnit.Framework.Test]
        public virtual void NotdefGlyphType3FontTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false, PdfConformance
                .PDF_UA_2);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfType3Font font = PdfFontFactory.CreateType3Font(pdfDoc, "itextFont", "itextFont", false);
                Type3Glyph a = font.AddGlyph('A', 600, 0, 0, 600, 700);
                a.SetLineWidth(100);
                a.MoveTo(5, 5);
                a.LineTo(300, 695);
                a.LineTo(595, 5);
                a.ClosePathFillStroke();
                // Need to populate CharProcs, because it's done only on font flushing,
                // but iText check that field before document closing
                PdfDictionary charProcs = new PdfDictionary();
                charProcs.Put(new PdfName("A"), a.GetContentStream());
                font.GetPdfObject().Put(PdfName.CharProcs, charProcs);
                Document doc = new Document(pdfDoc);
                doc.SetFont(font);
                // In simple fonts (which is Type3) we just ignore not defined glyphs, see PdfSimpleFont.createGlyphLine
                Paragraph p = new Paragraph("AB");
                doc.Add(p);
            }
            );
            framework.AssertBothValid("notdefGlyphType3Font");
        }

        private class CustomSymbolicTrueTypeFont : TrueTypeFont {
            public CustomSymbolicTrueTypeFont(String path)
                : base(path) {
            }

            public override int GetPdfFontFlags() {
                return 4;
            }

            public override bool IsCmapPresent(int platformID, int encodingID) {
                if (platformID == 1) {
                    return false;
                }
                return base.IsCmapPresent(platformID, encodingID);
            }
        }

        private class CustomNonSymbolicTrueTypeFont : TrueTypeFont {
            public CustomNonSymbolicTrueTypeFont(String path)
                : base(path) {
            }

            public override int GetPdfFontFlags() {
                return 32;
            }

            public override bool IsCmapPresent(int platformID, int encodingID) {
                if (platformID == 1 || encodingID == 1) {
                    return false;
                }
                return base.IsCmapPresent(platformID, encodingID);
            }

            public override int GetNumberOfCmaps() {
                return 0;
            }
        }
    }
}
