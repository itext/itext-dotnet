using System;
using System.IO;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("ignore")]
        public virtual void FileSpecCheckTest01() {
            // According to spec, only pdfa-1 or pdfa-2 compliant pdf document are allowed to be added to the
            // conforming pdfa-2 document. We only check they mime type, to define embedded file type, but we don't check
            // the bytes of the file. That's why this test creates invalid pdfa document.
            String outPdf = destinationFolder + "pdfA2b_fileSpecCheckTest01.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_fileSpecCheckTest01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfPage page = pdfDocument.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText("Hello World!").EndText
                ().RestoreState();
            byte[] somePdf = new byte[25];
            pdfDocument.AddFileAttachment("some pdf file", somePdf, "foo.pdf", PdfName.ApplicationPdf, null, new PdfName
                ("Data"));
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest02() {
            String outPdf = destinationFolder + "pdfA2b_fileSpecCheckTest02.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_fileSpecCheckTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfPage page = pdfDocument.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText("Hello World!").EndText
                ().RestoreState();
            FileStream fis = new FileStream(sourceFolder + "pdfa.pdf", FileMode.Open, FileAccess.Read);
            MemoryStream os = new MemoryStream();
            byte[] buffer = new byte[1024];
            int length;
            while ((length = fis.JRead(buffer, 0, buffer.Length)) > 0) {
                os.Write(buffer, 0, length);
            }
            pdfDocument.AddFileAttachment("some pdf file", os.ToArray(), "foo.pdf", PdfName.ApplicationPdf, null, null
                );
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void FileSpecCheckTest03() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new MemoryStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                    , @is);
                PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
                PdfPage page = pdfDocument.AddNewPage();
                PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText("Hello World!").EndText
                    ().RestoreState();
                MemoryStream txt = new MemoryStream();
                StreamWriter @out = new StreamWriter(txt);
                @out.Write("<foo><foo2>Hello world</foo2></foo>");
                @out.Close();
                pdfDocument.AddFileAttachment("foo file", txt.ToArray(), "foo.xml", PdfName.ApplicationXml, null, PdfName.
                    Source);
                pdfDocument.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.EmbeddedFileShallBeOfPdfMimeType));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
