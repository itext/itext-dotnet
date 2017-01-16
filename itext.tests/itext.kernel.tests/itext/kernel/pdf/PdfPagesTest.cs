using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfPagesTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfPagesTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfPagesTest/";

        internal static readonly PdfName PageNum = new PdfName("PageNum");

        internal static readonly PdfName PageNum5 = new PdfName("PageNum");

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void SimplePagesTest() {
            String filename = "simplePagesTest.pdf";
            int pageCount = 111;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(PageNum, new PdfNumber(i + 1));
                page.Flush();
            }
            pdfDoc.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount);
        }

        //    @Test
        //    public void simpleClonePagesTest() throws IOException {
        //        String filename = "simpleClonePagesTest.pdf";
        //        int pageCount = 111;
        //
        //        FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
        //        PdfWriter writer = new PdfWriter(fos);
        //        PdfDocument pdfDoc = new PdfDocument(writer);
        //
        //        for (int i = 0; i < pageCount; i++) {
        //            PdfPage page = pdfDoc.addNewPage();
        //            page.getPdfObject().put(PageNum, new PdfNumber(i + 1));
        //        }
        //        for (int i = 0; i < pageCount; i++) {
        //            PdfPage page = pdfDoc.addPage((PdfPage)pdfDoc.getPage(i + 1).clone());
        //            page.getPdfObject().put(PageNum, new PdfNumber(pageCount + i + 1));
        //            pdfDoc.getPage(i + 1).flush();
        //            page.flush();
        //        }
        //        pdfDoc.close();
        //        verifyPagesOrder(destinationFolder + filename, pageCount);
        //    }
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ReversePagesTest() {
            String filename = "reversePagesTest.pdf";
            int pageCount = 111;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = pageCount; i > 0; i--) {
                PdfPage page = new PdfPage(pdfDoc, pdfDoc.GetDefaultPageSize());
                pdfDoc.AddPage(1, page);
                page.GetPdfObject().Put(PageNum, new PdfNumber(i));
                page.Flush();
            }
            pdfDoc.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ReversePagesTest2() {
            String filename = "1000PagesDocument_reversed.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "1000PagesDocument.pdf"), new PdfWriter(
                destinationFolder + filename));
            for (int i = pdfDoc.GetNumberOfPages() - 1; i > 0; i--) {
                PdfPage page = pdfDoc.RemovePage(i);
                pdfDoc.AddPage(page);
            }
            pdfDoc.Close();
            new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff");
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void RandomObjectPagesTest() {
            String filename = "randomObjectPagesTest.pdf";
            int pageCount = 10000;
            int[] indexes = new int[pageCount];
            for (int i = 0; i < indexes.Length; i++) {
                indexes[i] = i + 1;
            }
            Random rnd = new Random();
            for (int i = indexes.Length - 1; i > 0; i--) {
                int index = rnd.Next(i + 1);
                int a = indexes[index];
                indexes[index] = indexes[i];
                indexes[i] = a;
            }
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfPage[] pages = new PdfPage[pageCount];
            for (int i = 0; i < indexes.Length; i++) {
                PdfPage page = document.AddNewPage();
                page.GetPdfObject().Put(PageNum, new PdfNumber(indexes[i]));
                //page.flush();
                pages[indexes[i] - 1] = page;
            }
            int xrefSize = document.GetXref().Size();
            PdfPage testPage = document.RemovePage(1000);
            NUnit.Framework.Assert.IsTrue(testPage.GetPdfObject().GetIndirectReference() == null);
            document.AddPage(1000, testPage);
            NUnit.Framework.Assert.IsTrue(testPage.GetPdfObject().GetIndirectReference().GetObjNumber() < xrefSize);
            for (int i = 0; i < pages.Length; i++) {
                NUnit.Framework.Assert.AreEqual(true, document.RemovePage(pages[i]), "Remove page");
                document.AddPage(i + 1, pages[i]);
            }
            document.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void RandomNumberPagesTest() {
            String filename = "randomNumberPagesTest.pdf";
            int pageCount = 3000;
            int[] indexes = new int[pageCount];
            for (int i = 0; i < indexes.Length; i++) {
                indexes[i] = i + 1;
            }
            Random rnd = new Random();
            for (int i = indexes.Length - 1; i > 0; i--) {
                int index = rnd.Next(i + 1);
                int a = indexes[index];
                indexes[index] = indexes[i];
                indexes[i] = a;
            }
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < indexes.Length; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(PageNum, new PdfNumber(indexes[i]));
            }
            for (int i = 1; i < pageCount; i++) {
                for (int j = i + 1; j <= pageCount; j++) {
                    int j_page = pdfDoc.GetPage(j).GetPdfObject().GetAsNumber(PageNum).IntValue();
                    int i_page = pdfDoc.GetPage(i).GetPdfObject().GetAsNumber(PageNum).IntValue();
                    if (j_page < i_page) {
                        PdfPage page = pdfDoc.RemovePage(j);
                        pdfDoc.AddPage(i + 1, page);
                        page = pdfDoc.RemovePage(i);
                        pdfDoc.AddPage(j, page);
                    }
                }
                NUnit.Framework.Assert.IsTrue(VerifyIntegrity(pdfDoc.GetCatalog().GetPageTree()) == -1);
            }
            pdfDoc.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED)]
        public virtual void InsertFlushedPageTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfPage page = pdfDoc.AddNewPage();
            bool error = false;
            try {
                page.Flush();
                pdfDoc.RemovePage(page);
                pdfDoc.AddPage(1, page);
                pdfDoc.Close();
            }
            catch (PdfException e) {
                if (PdfException.FlushedPageCannotBeAddedOrInserted.Equals(e.Message)) {
                    error = true;
                }
            }
            NUnit.Framework.Assert.IsTrue(error);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED)]
        public virtual void AddFlushedPageTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfPage page = pdfDoc.AddNewPage();
            bool error = false;
            try {
                page.Flush();
                pdfDoc.RemovePage(page);
                pdfDoc.AddPage(page);
                pdfDoc.Close();
            }
            catch (PdfException e) {
                if (PdfException.FlushedPageCannotBeAddedOrInserted.Equals(e.Message)) {
                    error = true;
                }
            }
            NUnit.Framework.Assert.IsTrue(error);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED, Count = 2)]
        public virtual void RemoveFlushedPage() {
            String filename = "removeFlushedPage.pdf";
            int pageCount = 10;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfPage removedPage = pdfDoc.AddNewPage();
            int removedPageObjectNumber = removedPage.GetPdfObject().GetIndirectReference().GetObjNumber();
            removedPage.Flush();
            pdfDoc.RemovePage(removedPage);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(PageNum, new PdfNumber(i + 1));
                page.Flush();
            }
            NUnit.Framework.Assert.AreEqual(true, pdfDoc.RemovePage(pdfDoc.GetPage(pageCount)), "Remove last page");
            NUnit.Framework.Assert.AreEqual(true, pdfDoc.GetXref().Get(removedPageObjectNumber).CheckState(PdfObject.FREE
                ), "Free reference");
            pdfDoc.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount - 1);
        }

        /// <exception cref="System.IO.IOException"/>
        internal virtual void VerifyPagesOrder(String filename, int numOfPages) {
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.IsNotNull(page);
                PdfNumber number = page.GetAsNumber(PageNum5);
                NUnit.Framework.Assert.AreEqual(i, number.IntValue(), "Page number");
            }
            NUnit.Framework.Assert.AreEqual(numOfPages, pdfDocument.GetNumberOfPages(), "Number of pages");
            pdfDocument.Close();
        }

        internal virtual int VerifyIntegrity(PdfPagesTree pagesTree) {
            IList<PdfPages> parents = pagesTree.GetParents();
            int from = 0;
            for (int i = 0; i < parents.Count; i++) {
                if (parents[i].GetFrom() != from) {
                    return i;
                }
                from = parents[i].GetFrom() + parents[i].GetCount();
            }
            return -1;
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestInheritedResources() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleInheritedResources.pdf"));
            PdfPage page = pdfDocument.GetPage(1);
            PdfDictionary dict = page.GetResources().GetResource(PdfName.ExtGState);
            NUnit.Framework.Assert.AreEqual(2, dict.Size());
            PdfExtGState gState = new PdfExtGState((PdfDictionary)dict.Get(new PdfName("Gs1")));
            NUnit.Framework.Assert.AreEqual(10, gState.GetLineWidth());
        }

        //    @Test(expected = PdfException.class)
        //    public void testCircularReferencesInResources() throws IOException {
        //        String inputFileName1 = sourceFolder + "circularReferencesInResources.pdf";
        //        PdfReader reader1 = new PdfReader(inputFileName1);
        //        PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
        //        PdfPage page = inputPdfDoc1.getPage(1);
        //        List<PdfFont> list = page.getResources().getFonts(true);
        //    }
        //
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestInheritedResourcesUpdate() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "simpleInheritedResources.pdf"), new PdfWriter
                (destinationFolder + "updateInheritedResources.pdf").SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                ));
            PdfName newGsName = pdfDoc.GetPage(1).GetResources().AddExtGState(new PdfExtGState().SetLineWidth(30));
            int gsCount = pdfDoc.GetPage(1).GetResources().GetResource(PdfName.ExtGState).Size();
            pdfDoc.Close();
            String compareResult = new CompareTool().CompareByContent(destinationFolder + "updateInheritedResources.pdf"
                , sourceFolder + "cmp_" + "updateInheritedResources.pdf", destinationFolder, "diff");
            NUnit.Framework.Assert.AreEqual(3, gsCount);
            NUnit.Framework.Assert.AreEqual("Gs3", newGsName.GetValue());
            NUnit.Framework.Assert.IsNull(compareResult);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void GetPageByDictionary() {
            String filename = sourceFolder + "1000PagesDocument.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfObject[] pageDictionaries = new PdfObject[] { pdfDoc.GetPdfObject(4), pdfDoc.GetPdfObject(255), pdfDoc.
                GetPdfObject(512), pdfDoc.GetPdfObject(1023), pdfDoc.GetPdfObject(2049), pdfDoc.GetPdfObject(3100) };
            foreach (PdfObject pageObject in pageDictionaries) {
                PdfDictionary pageDictionary = (PdfDictionary)pageObject;
                NUnit.Framework.Assert.AreEqual(PdfName.Page, pageDictionary.Get(PdfName.Type));
                PdfPage page = pdfDoc.GetPage(pageDictionary);
                NUnit.Framework.Assert.AreEqual(pageDictionary, page.GetPdfObject());
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void RemovePageWithFormFieldsTest() {
            String filename = sourceFolder + "docWithFields.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            pdfDoc.RemovePage(1);
            PdfArray fields = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).GetAsArray(PdfName.
                Fields);
            PdfDictionary field = (PdfDictionary)fields.Get(0);
            PdfDictionary kid = (PdfDictionary)field.GetAsArray(PdfName.Kids).Get(0);
            NUnit.Framework.Assert.AreEqual(6, kid.KeySet().Count);
            NUnit.Framework.Assert.AreEqual(3, fields.Size());
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void GetPageSizeWithInheritedMediaBox() {
            double eps = 0.0000001;
            String filename = sourceFolder + "inheritedMediaBox.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            NUnit.Framework.Assert.AreEqual(0, pdfDoc.GetPage(1).GetPageSize().GetLeft(), eps);
            NUnit.Framework.Assert.AreEqual(0, pdfDoc.GetPage(1).GetPageSize().GetBottom(), eps);
            NUnit.Framework.Assert.AreEqual(595, pdfDoc.GetPage(1).GetPageSize().GetRight(), eps);
            NUnit.Framework.Assert.AreEqual(842, pdfDoc.GetPage(1).GetPageSize().GetTop(), eps);
            pdfDoc.Close();
        }
    }
}
