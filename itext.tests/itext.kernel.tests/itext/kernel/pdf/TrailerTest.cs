using System;
using System.IO;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf {
    /// <author>Michael Demey</author>
    public class TrailerTest : ExtendedITextTest {
        private ProductInfo productInfo;

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/TrailerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void BeforeTest() {
            this.productInfo = new ProductInfo("pdfProduct", 1, 0, 0, true);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TrailerFingerprintTest() {
            FileStream fos = new FileStream(destinationFolder + "output.pdf", FileMode.Create);
            PdfDocument pdf = new PdfDocument(new PdfWriter(fos));
            pdf.RegisterProduct(this.productInfo);
            PdfPage page = pdf.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 12f).ShowText("Hello World").EndText();
            pdf.Close();
            NUnit.Framework.Assert.IsTrue(DoesTrailerContainFingerprint(new FileInfo(destinationFolder + "output.pdf")
                , productInfo.ToString()));
        }

        /// <exception cref="System.IO.IOException"/>
        private bool DoesTrailerContainFingerprint(FileInfo file, String fingerPrint) {
            FileStream raf = FileUtil.GetRandomAccessFile(file);
            // put the pointer at the end of the file
            raf.Seek(raf.Length);
            // look for startxref
            String startxref = "startxref";
            String templine = "";
            while (!templine.Contains(startxref)) {
                templine = (char)raf.ReadByte() + templine;
                raf.Seek(raf.Position - 2);
            }
            // look for fingerprint
            char read = ' ';
            templine = "";
            while (read != '%') {
                read = (char)raf.ReadByte();
                templine = read + templine;
                raf.Seek(raf.Position - 2);
            }
            bool output = templine.Contains(fingerPrint);
            raf.Dispose();
            return output;
        }
    }
}
