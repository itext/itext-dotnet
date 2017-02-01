using System;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    public class Barcode39Test : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/Barcode39/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.PdfException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "barcode39_01.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Barcode1D barcode = new Barcode39(document);
            barcode.SetCode("9781935182610");
            barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
            barcode.PlaceBarcode(canvas, Color.BLACK, Color.BLACK);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.PdfException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcode39_02.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "DocumentWithTrueTypeFont1.pdf"), new 
                PdfWriter(destinationFolder + filename));
            PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
            Barcode1D barcode = new Barcode39(document);
            barcode.SetCode("9781935182610");
            barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
            barcode.PlaceBarcode(canvas, Color.BLACK, Color.BLACK);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode03Test() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Barcode39 barcode = new Barcode39(document);
            try {
                Barcode39.GetBarsCode39("9781935*182610");
                NUnit.Framework.Assert.Fail("IllegalArgumentException expected");
            }
            catch (ArgumentException) {
            }
        }
    }
}
