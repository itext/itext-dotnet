using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Barcodes {
    public class BarcodeUnitTest : ExtendedITextTest {
        private const double EPS = 0.0001;

        [NUnit.Framework.Test]
        public virtual void BarcodeMSIGetBarcodeSizeWithChecksumTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            PdfDocument document = new PdfDocument(writer);
            document.AddNewPage();
            Barcode1D barcode = new BarcodeMSI(document);
            document.Close();
            barcode.SetCode("123456789");
            barcode.SetGenerateChecksum(true);
            Rectangle barcodeSize = barcode.GetBarcodeSize();
            NUnit.Framework.Assert.AreEqual(33.656, barcodeSize.GetHeight(), EPS);
            NUnit.Framework.Assert.AreEqual(101.6, barcodeSize.GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeMSIGetBarcodeSizeWithoutChecksumTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            PdfDocument document = new PdfDocument(writer);
            document.AddNewPage();
            Barcode1D barcode = new BarcodeMSI(document);
            document.Close();
            barcode.SetCode("123456789");
            barcode.SetGenerateChecksum(false);
            Rectangle barcodeSize = barcode.GetBarcodeSize();
            NUnit.Framework.Assert.AreEqual(33.656, barcodeSize.GetHeight(), EPS);
            NUnit.Framework.Assert.AreEqual(92.0, barcodeSize.GetWidth(), EPS);
        }
    }
}
