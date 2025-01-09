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
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA3EmbeddedFilesCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA3EmbeddedFilesCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA3EmbeddedFilesCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest01() {
            String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest01.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformance.PDF_A_3B, outputIntent);
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
            pdfDocument.AddAssociatedFile("foo file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, txt.ToArray(), "foo file"
                , "foo.xml", PdfName.ApplicationXml, null, PdfName.Source));
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest02() {
            String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest02.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformance.PDF_A_3B, outputIntent);
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
            pdfDocument.AddAssociatedFile("foo file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, txt.ToArray(), "foo file"
                , "foo.xml", null, PdfName.Unspecified));
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest03() {
            String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest03.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformance.PDF_A_3B, outputIntent);
            PdfPage page = pdfDocument.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText("Hello World!").EndText
                ().RestoreState();
            byte[] somePdf = new byte[25];
            pdfDocument.AddAssociatedFile("some pdf file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, somePdf, "some pdf file"
                , "foo.pdf", PdfName.ApplicationPdf, null, PdfName.Data));
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest04() {
            String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest04.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest04.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformance.PDF_A_3B, outputIntent);
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
            pdfDocument.AddAssociatedFile("foo file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, txt.ToArray(), "foo file"
                , "foo.xml", PdfName.Unspecified));
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
