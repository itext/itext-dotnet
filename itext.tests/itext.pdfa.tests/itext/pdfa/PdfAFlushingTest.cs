using System;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    public class PdfAFlushingTest : ITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAFlushingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FlushingTest01() {
            String outPdf = destinationFolder + "pdfA1b_flushingTest01.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA1b_flushingTest01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            imageXObject.MakeIndirect(doc);
            canvas.AddXObject(imageXObject, new Rectangle(30, 300, 300, 300));
            imageXObject.Flush();
            if (imageXObject.IsFlushed()) {
                NUnit.Framework.Assert.Fail("Flushing of unchecked objects shall be forbidden.");
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FlushingTest02() {
            String outPdf = destinationFolder + "pdfA2b_flushingTest02.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA2b_flushingTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            imageXObject.MakeIndirect(doc);
            canvas.AddXObject(imageXObject, new Rectangle(30, 300, 300, 300));
            PdfPage lastPage = doc.GetLastPage();
            lastPage.Flush();
            if (lastPage.IsFlushed()) {
                NUnit.Framework.Assert.Fail("Flushing of unchecked objects shall be forbidden.");
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FlushingTest03() {
            String outPdf = destinationFolder + "pdfA3b_flushingTest03.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA3b_flushingTest03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            canvas.AddXObject(imageXObject, new Rectangle(30, 300, 300, 300));
            PdfPage lastPage = doc.GetLastPage();
            lastPage.Flush(true);
            if (!imageXObject.IsFlushed()) {
                NUnit.Framework.Assert.Fail("When flushing the page along with it's resources, page check should be performed also page and all resources should be flushed."
                    );
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AddUnusedStreamObjectsTest() {
            String outPdf = destinationFolder + "pdfA1b_docWithUnusedObjects_3.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA1b_docWithUnusedObjects_3.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = ((PdfArray)new PdfArray().MakeIndirect(pdfDocument));
            unusedArray.Add(new PdfNumber(42));
            PdfStream stream = new PdfStream(new byte[] { 1, 2, 34, 45 }, 0);
            unusedArray.Add(stream);
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            ((PdfDictionary)unusedDictionary.MakeIndirect(pdfDocument)).Flush();
            unusedDictionary.Flush();
            pdfDocument.Close();
            PdfReader testerReader = new PdfReader(outPdf);
            PdfDocument testerDocument = new PdfDocument(testerReader);
            NUnit.Framework.Assert.AreEqual(testerDocument.ListIndirectReferences().Count, 11);
            testerDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CompareResult(String outFile, String cmpFile) {
            String differences = new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_");
            if (differences != null) {
                NUnit.Framework.Assert.Fail(differences);
            }
        }
    }
}
