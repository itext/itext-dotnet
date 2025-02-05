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
using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Layout {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2FontTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PdfUA2FontTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/PdfUA2FontTest/";

        public static readonly String FONT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPuritan2WithUTF8Test() {
            String outFile = DESTINATION_FOLDER + "puritan2WithUTF8Test.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_puritan2WithUTF8Test.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "Puritan2.otf", PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFreeSansWithMacromanTest() {
            String outFile = DESTINATION_FOLDER + "freeSansWithMacromanTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_freeSansWithMacromanTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", PdfEncodings.MACROMAN, PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckNotoSansRegularTest() {
            String outFile = DESTINATION_FOLDER + "notoSansRegularTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_notoSansRegularTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "NotoSans-Regular.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckOpenSansRegularTest() {
            String outFile = DESTINATION_FOLDER + "openSansRegularTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_openSansRegularTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "Open_Sans/OpenSans-Regular.ttf", PdfEncodings.WINANSI
                    , PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckType0FontTest() {
            String outFile = DESTINATION_FOLDER + "type0FontTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_type0FontTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph paragraph = new Paragraph("Simple paragraph");
                document.Add(paragraph);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        private void CreateSimplePdfUA2Document(PdfDocument pdfDocument) {
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simplePdfUA2.xmp"));
            XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
            pdfDocument.SetXmpMetadata(xmpMeta);
            pdfDocument.SetTagged();
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle("PdfUA2 Title");
        }

        private void CompareAndValidate(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
