using System;
using System.Collections.Generic;
using iText.Barcodes.Qrcode;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    public class BarcodeQRCodeTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/barcodes/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/BarcodeQRCode/";

        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.PdfException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "barcodeQRCode01.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object>();
            hints[EncodeHintType.ERROR_CORRECTION] = ErrorCorrectionLevel.L;
            BarcodeQRCode barcode = new BarcodeQRCode("some specific text 239214 hello world");
            barcode.PlaceBarcode(canvas, Color.GRAY, 12);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.PdfException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcodeQRCode02.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object>();
            hints[EncodeHintType.CHARACTER_SET] = "UTF-8";
            BarcodeQRCode barcode1 = new BarcodeQRCode("дима", hints);
            barcode1.PlaceBarcode(canvas, Color.GRAY, 12);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }
    }
}
