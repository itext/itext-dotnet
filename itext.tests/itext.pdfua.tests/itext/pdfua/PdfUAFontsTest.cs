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
using iText.Test.Pdfa;

namespace iText.Pdfua {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAFontsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAFontsTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String FONT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TryToUseType0Cid0FontTest() {
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
                .FONT_SHOULD_BE_EMBEDDED, "KozMinPro-Regular"), false);
        }

        [NUnit.Framework.Test]
        public virtual void Type0Cid2FontTest() {
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
            framework.AssertBothValid("type0Cid2FontTest");
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontTest() {
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
            framework.AssertBothValid("trueTypeFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontGlyphNotPresentTest() {
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
                .GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE, "w"), false);
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontWithDifferencesTest() {
            String outPdf = DESTINATION_FOLDER + "trueTypeFontWithDifferencesTest.pdf";
            using (PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf))) {
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
                    .H);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).BeginText().MoveText(36, 786).SetFontAndSize(font
                    , 36).ShowText("world").EndText().RestoreState().CloseTag();
            }
            new VeraPdfValidator().ValidateFailure(outPdf);
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void TryToUseStandardFontsTest() {
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
                document.Close();
            }
            );
            framework.AssertBothFail("tryToUseStandardFontsTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .FONT_SHOULD_BE_EMBEDDED, "Courier"), false);
        }

        [NUnit.Framework.Test]
        public virtual void Type1EmbeddedFontTest() {
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
            framework.AssertBothValid("type1EmbeddedFontTest");
        }
    }
}
