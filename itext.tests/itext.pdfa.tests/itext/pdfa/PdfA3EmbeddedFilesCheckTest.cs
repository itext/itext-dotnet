/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, outputIntent);
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
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, outputIntent);
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
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, outputIntent);
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
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, outputIntent);
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
