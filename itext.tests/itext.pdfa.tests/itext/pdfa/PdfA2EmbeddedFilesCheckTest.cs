/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA2EmbeddedFilesCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA2EmbeddedFilesCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA2EmbeddedFilesCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfAConformanceLogMessageConstant.EMBEDDED_FILE_SHALL_BE_COMPLIANT_WITH_SPEC, Count = 1)]
        public virtual void FileSpecNonConformingTest01() {
            // According to spec, only pdfa-1 or pdfa-2 compliant pdf document are allowed to be added to the
            // conforming pdfa-2 document. We only check they mime type, to define embedded file type, but we don't check
            // the bytes of the file. That's why this test creates invalid pdfa document.
            String outPdf = destinationFolder + "pdfA2b_fileSpecNonConformingTest01.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_fileSpecNonConformingTest01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfPage page = pdfDocument.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText("Hello World!").EndText
                ().RestoreState();
            byte[] somePdf = new byte[25];
            pdfDocument.AddAssociatedFile("some pdf file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, somePdf, "some pdf file"
                , "foo.pdf", PdfName.ApplicationPdf, null, new PdfName("Data")));
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfAConformanceLogMessageConstant.EMBEDDED_FILE_SHALL_BE_COMPLIANT_WITH_SPEC, Count = 1)]
        public virtual void FileSpecCheckTest02() {
            String outPdf = destinationFolder + "pdfA2b_fileSpecCheckTest02.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_fileSpecCheckTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfPage page = pdfDocument.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText("Hello World!").EndText
                ().RestoreState();
            FileStream fis = new FileStream(sourceFolder + "pdfs/pdfa.pdf", FileMode.Open, FileAccess.Read);
            MemoryStream os = new MemoryStream();
            byte[] buffer = new byte[1024];
            int length;
            while ((length = fis.JRead(buffer, 0, buffer.Length)) > 0) {
                os.Write(buffer, 0, length);
            }
            pdfDocument.AddFileAttachment("some pdf file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, os.ToArray(
                ), "some pdf file", "foo.pdf", PdfName.ApplicationPdf, null, null));
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfAConformanceLogMessageConstant.EMBEDDED_FILE_SHALL_BE_COMPLIANT_WITH_SPEC, Count = 1)]
        public virtual void FileSpecCheckTest03() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfPage page = pdfDocument.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText("Hello World!").EndText
                ().RestoreState();
            MemoryStream txt = new MemoryStream();
            FormattingStreamWriter @out = new FormattingStreamWriter(txt);
            @out.Write("<foo><foo2>Hello world</foo2></foo>");
            @out.Dispose();
            pdfDocument.AddFileAttachment("foo file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, txt.ToArray(), "foo file"
                , "foo.xml", PdfName.ApplicationXml, null, PdfName.Source));
            pdfDocument.Close();
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
