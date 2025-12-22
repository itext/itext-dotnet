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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Constants;
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

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String FONT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TryToUseType0Cid0FontTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont("KozMinPro-Regular", "UniJIS-UCS2-H", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            );
            framework.AssertBothFail("tryToUseType0Cid0FontTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .FONT_SHOULD_BE_EMBEDDED, "KozMinPro-Regular"), false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Type0Cid2FontTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("type0Cid2FontTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TrueTypeFontTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("trueTypeFontTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TrueTypeFontGlyphNotPresentTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, "# simple 32 0020 00C5 1987", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 36).ShowText("world").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("trueTypeFontGlyphNotPresentTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE, "w"), false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TrueTypeFontWithDifferencesTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, "# simple 32 0077 006f 0072 006c 0064", PdfFontFactory.EmbeddingStrategy
                        .PREFER_EMBEDDED);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 36).ShowText("world").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("trueTypeFontWithDifferencesTest", PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TryToUseStandardFontsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(StandardFonts.COURIER, "", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Helloworld");
                document.Add(paragraph);
            }
            );
            framework.AssertBothFail("tryToUseStandardFontsTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .FONT_SHOULD_BE_EMBEDDED, "Courier"), false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Type1EmbeddedFontTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(FONT_FOLDER + "cmr10.afm", FONT_FOLDER
                         + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Helloworld");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("type1EmbeddedFontTest", pdfUAConformance);
        }

        [NUnit.Framework.Test]
        public virtual void NonSymbolicTtfWithChangedCmapTest() {
            // TODO DEVSIX-9076 NPE when cmap of True Type Font doesn't contain Microsoft Unicode or Macintosh Roman encodings
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => PdfFontFactory.CreateFont(FONT_FOLDER +
                 "FreeSans_changed_cmap.ttf", PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED));
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSymbolicTtfWithValidEncodingTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("nonSymbolicTtfWithValidEncodingTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSymbolicTtfWithIncompatibleEncodingTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            framework.AssertBothFail("nonSymbolicTtfWithIncompatibleEncoding", PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SymbolicTtfTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    // TODO DEVSIX-9589 Create symbol font with cmap 3,0 for testing
                    font = PdfFontFactory.CreateFont(FONT_FOLDER + "Symbols1.ttf", PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            framework.AssertBothValid("symbolicTtf", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SymbolicTtfWithEncodingTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    // TODO DEVSIX-9589 Create symbol font with cmap 3,0 for testing
                    font = PdfFontFactory.CreateFont(FONT_FOLDER + "Symbols1.ttf", PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                font.GetPdfObject().Put(PdfName.Encoding, PdfName.MacRomanEncoding);
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            // VeraPDF is valid since iText fixes symbolic flag to non-symbolic on closing.
            framework.AssertOnlyITextFail("symbolicTtfWithEncoding", PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_NOT_CONTAIN_ENCODING
                , pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SymbolicTtfWithInvalidCmapTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    TrueTypeFont fontProgram = new PdfUAFontsTest.CustomSymbolicTrueTypeFont(FONT);
                    font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            // VeraPDF is valid since iText fixes symbolic flag to non-symbolic on closing.
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertOnlyITextFail("symbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_CONTAIN_EXACTLY_ONE_OR_AT_LEAST_MICROSOFT_SYMBOL_CMAP
                    , pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertOnlyITextFail("symbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_SYMBOL_CMAP
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSymbolicTtfWithInvalidCmapTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font;
                try {
                    TrueTypeFont fontProgram = new PdfUAFontsTest.CustomNonSymbolicTrueTypeFont(FONT);
                    font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("ABC");
                document.Add(paragraph);
            }
            );
            // VeraPDF is valid since the file itself is valid, but itext code is modified for testing.
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertOnlyITextFail("nonSymbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_CONTAIN_NON_SYMBOLIC_CMAP
                    , pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertOnlyITextFail("nonSymbolicTtfWithInvalidCmapTest", PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_UNI_CMAP
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfWithChangedCmapTest() {
            // TODO DEVSIX-9076 NPE when cmap of True Type Font doesn't contain Microsoft Unicode or Macintosh Roman encodings
            // TODO DEVSIX-9589 Create symbol font with cmap 3,0 for testing
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => PdfFontFactory.CreateFont(FONT_FOLDER +
                 "Symbols1_changed_cmap.ttf", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED));
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
