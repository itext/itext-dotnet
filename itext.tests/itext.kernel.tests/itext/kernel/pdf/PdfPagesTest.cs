/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
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

        [NUnit.Framework.Test]
        public virtual void ReversePagesTest2() {
            String filename = "1000PagesDocument_reversed.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "1000PagesDocument.pdf"), new PdfWriter(
                destinationFolder + filename));
            int n = pdfDoc.GetNumberOfPages();
            for (int i = n - 1; i > 0; --i) {
                pdfDoc.MovePage(i, n + 1);
            }
            pdfDoc.Close();
            new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff");
        }

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
            int testPageXref = document.GetPage(1000).GetPdfObject().GetIndirectReference().GetObjNumber();
            document.MovePage(1000, 1000);
            NUnit.Framework.Assert.AreEqual(testPageXref, document.GetPage(1000).GetPdfObject().GetIndirectReference()
                .GetObjNumber());
            for (int i = 0; i < pages.Length; i++) {
                NUnit.Framework.Assert.IsTrue(document.MovePage(pages[i], i + 1), "Move page");
            }
            document.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount);
        }

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
                        pdfDoc.MovePage(i, j);
                        pdfDoc.MovePage(j, i);
                    }
                }
                NUnit.Framework.Assert.IsTrue(VerifyIntegrity(pdfDoc.GetCatalog().GetPageTree()) == -1);
            }
            pdfDoc.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount);
        }

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
            NUnit.Framework.Assert.IsTrue(pdfDoc.RemovePage(pdfDoc.GetPage(pageCount)), "Remove last page");
            NUnit.Framework.Assert.IsFalse(pdfDoc.GetXref().Get(removedPageObjectNumber).CheckState(PdfObject.FREE), "Free reference"
                );
            pdfDoc.Close();
            VerifyPagesOrder(destinationFolder + filename, pageCount - 1);
        }

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

        [NUnit.Framework.Test]
        public virtual void TestInheritedResources() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleInheritedResources.pdf"));
            PdfPage page = pdfDocument.GetPage(1);
            PdfDictionary dict = page.GetResources().GetResource(PdfName.ExtGState);
            NUnit.Framework.Assert.AreEqual(2, dict.Size());
            PdfExtGState gState = new PdfExtGState((PdfDictionary)dict.Get(new PdfName("Gs1")));
            NUnit.Framework.Assert.AreEqual(10, gState.GetLineWidth());
        }

        [NUnit.Framework.Test]
        public virtual void ReadFormXObjectsWithCircularReferencesInResources() {
            // given input file contains circular reference in resources of form xobjects
            // (form xobjects are nested inside each other)
            String input = sourceFolder + "circularReferencesInResources.pdf";
            PdfReader reader1 = new PdfReader(input);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfPage page = inputPdfDoc1.GetPage(1);
            PdfResources resources = page.GetResources();
            IList<PdfFormXObject> formXObjects = new List<PdfFormXObject>();
            // We just try to work with resources in arbitrary way and make sure that circular reference
            // doesn't block it. However it is expected that PdfResources doesn't try to "look in deep"
            // and recursively resolves resources, so this test should never meet any issues.
            foreach (PdfName xObjName in resources.GetResourceNames(PdfName.XObject)) {
                PdfFormXObject form = resources.GetForm(xObjName);
                if (form != null) {
                    formXObjects.Add(form);
                }
            }
            // ensure resources XObject entry is read correctly
            NUnit.Framework.Assert.AreEqual(2, formXObjects.Count);
        }

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

        [NUnit.Framework.Test]
        public virtual void ReorderInheritedResourcesTest() {
            //TODO: DEVSIX-1643 Inherited resources aren't copied on page reordering
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "inheritedFontResources.pdf"), new PdfWriter
                (destinationFolder + "reorderInheritedFontResources.pdf"));
            pdfDoc.MovePage(1, pdfDoc.GetNumberOfPages() + 1);
            pdfDoc.RemovePage(1);
            pdfDoc.Close();
            String compareResult = new CompareTool().CompareByContent(destinationFolder + "reorderInheritedFontResources.pdf"
                , sourceFolder + "cmp_reorderInheritedFontResources.pdf", destinationFolder, "diff_reorderInheritedFontResources_"
                );
            NUnit.Framework.Assert.IsNull(compareResult);
        }

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

        [NUnit.Framework.Test]
        public virtual void PageThumbnailTest() {
            String filename = "pageThumbnail.pdf";
            String imageSrc = "icon.jpg";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename).SetCompressionLevel(CompressionConstants
                .NO_COMPRESSION));
            PdfPage page = pdfDoc.AddNewPage().SetThumbnailImage(new PdfImageXObject(ImageDataFactory.Create(sourceFolder
                 + imageSrc)));
            new PdfCanvas(page).SetFillColor(ColorConstants.RED).Rectangle(100, 100, 400, 400).Fill();
            pdfDoc.Close();
            new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff");
        }

        [NUnit.Framework.Test]
        public virtual void RotationPagesRotationTest() {
            String filename = "singlePageDocumentWithRotation.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + filename));
            PdfPage page = pdfDoc.GetPage(1);
            NUnit.Framework.Assert.AreEqual(90, page.GetRotation(), "Inherited value is invalid");
        }

        [NUnit.Framework.Test]
        public virtual void PageTreeCleanupParentRefTest() {
            String src = sourceFolder + "CatalogWithPageAndPagesEntries.pdf";
            String dest = destinationFolder + "CatalogWithPageAndPagesEntries_opened.pdf";
            PdfReader reader = new PdfReader(src);
            PdfWriter writer = new PdfWriter(dest);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsTrue(TestPageTreeParentsValid(src) && TestPageTreeParentsValid(dest));
        }

        [NUnit.Framework.Test]
        public virtual void PdfNumberInPageContentArrayTest() {
            String src = sourceFolder + "pdfNumberInPageContentArray.pdf";
            String dest = destinationFolder + "pdfNumberInPageContentArray_saved.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            pdfDoc.Close();
            // test is mainly to ensure document is successfully opened-and-closed without exceptions
            pdfDoc = new PdfDocument(new PdfReader(dest));
            PdfObject pageDictWithInvalidContents = pdfDoc.GetPdfObject(10);
            PdfArray invalidContentsArray = ((PdfDictionary)pageDictWithInvalidContents).GetAsArray(PdfName.Contents);
            NUnit.Framework.Assert.AreEqual(5, invalidContentsArray.Size());
            NUnit.Framework.Assert.IsFalse(invalidContentsArray.Get(0).IsStream());
            NUnit.Framework.Assert.IsFalse(invalidContentsArray.Get(1).IsStream());
            NUnit.Framework.Assert.IsFalse(invalidContentsArray.Get(2).IsStream());
            NUnit.Framework.Assert.IsFalse(invalidContentsArray.Get(3).IsStream());
            NUnit.Framework.Assert.IsTrue(invalidContentsArray.Get(4).IsStream());
        }

        private bool TestPageTreeParentsValid(String src) {
            bool valid = true;
            PdfReader reader = new PdfReader(src);
            PdfDocument pdfDocument = new PdfDocument(reader);
            PdfDictionary page_root = pdfDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages);
            for (int x = 1; x < pdfDocument.GetNumberOfPdfObjects(); x++) {
                PdfObject obj = pdfDocument.GetPdfObject(x);
                if (obj != null && obj.IsDictionary() && ((PdfDictionary)obj).GetAsName(PdfName.Type) != null && ((PdfDictionary
                    )obj).GetAsName(PdfName.Type).Equals(PdfName.Pages)) {
                    if (obj != page_root) {
                        PdfDictionary parent = ((PdfDictionary)obj).GetAsDictionary(PdfName.Parent);
                        if (parent == null) {
                            System.Console.Out.WriteLine(obj);
                            valid = false;
                        }
                    }
                }
            }
            return valid;
        }

        [NUnit.Framework.Test]
        public virtual void TestExcessiveXrefEntriesForCopyXObject() {
            PdfDocument inputPdf = new PdfDocument(new PdfReader(sourceFolder + "input500.pdf"));
            PdfDocument outputPdf = new PdfDocument(new PdfWriter(destinationFolder + "output500.pdf"));
            float scaleX = 595f / 612f;
            float scaleY = 842f / 792f;
            for (int i = 1; i <= inputPdf.GetNumberOfPages(); ++i) {
                PdfPage sourcePage = inputPdf.GetPage(i);
                PdfFormXObject pageCopy = sourcePage.CopyAsFormXObject(outputPdf);
                PdfPage page = outputPdf.AddNewPage(PageSize.A4);
                PdfCanvas outputCanvas = new PdfCanvas(page);
                outputCanvas.AddXObject(pageCopy, scaleX, 0, 0, scaleY, 0, 0);
                page.Flush();
            }
            outputPdf.Close();
            inputPdf.Close();
            NUnit.Framework.Assert.IsNotNull(outputPdf.GetXref());
            NUnit.Framework.Assert.AreEqual(500, outputPdf.GetXref().Size() - inputPdf.GetXref().Size());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.WRONG_MEDIABOX_SIZE_TOO_MANY_ARGUMENTS, Count = 1)]
        public virtual void PageGetMediaBoxTooManyArgumentsTest() {
            PdfReader reader = new PdfReader(sourceFolder + "helloWorldMediaboxTooManyArguments.pdf");
            Rectangle expected = new Rectangle(0, 0, 375, 300);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfPage pageOne = pdfDoc.GetPage(1);
            Rectangle actual = pageOne.GetPageSize();
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void PageGetMediaBoxNotEnoughArgumentsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader reader = new PdfReader(sourceFolder + "helloWorldMediaboxNotEnoughArguments.pdf");
                PdfDocument pdfDoc = new PdfDocument(reader);
                PdfPage pageOne = pdfDoc.GetPage(1);
                Rectangle actual = pageOne.GetPageSize();
                NUnit.Framework.Assert.Fail("Exception was not thrown");
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(MessageFormatUtil.Format(PdfException.WRONGMEDIABOXSIZETOOFEWARGUMENTS, 3)))
;
        }

        [NUnit.Framework.Test]
        public virtual void InsertIntermediateParentTest() {
            String filename = "insertIntermediateParentTest.pdf";
            PdfReader reader = new PdfReader(sourceFolder + filename);
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            PdfPage page = pdfDoc.GetFirstPage();
            PdfPages pdfPages = new PdfPages(page.parentPages.GetFrom(), pdfDoc, page.parentPages);
            page.parentPages.GetKids().Set(0, pdfPages.GetPdfObject());
            page.parentPages.DecrementCount();
            pdfPages.AddPage(page.GetPdfObject());
            pdfDoc.Close();
            NUnit.Framework.Assert.IsTrue(page.GetPdfObject().IsModified());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPagesAreNotReadOnOpenTest() {
            String srcFile = sourceFolder + "taggedOnePage.pdf";
            PdfPagesTest.CustomPdfReader reader = new PdfPagesTest.CustomPdfReader(this, srcFile);
            PdfDocument document = new PdfDocument(reader);
            document.Close();
            NUnit.Framework.Assert.IsFalse(reader.pagesAreRead);
        }

        [NUnit.Framework.Test]
        public virtual void ReadPagesInBlocksTest() {
            String srcFile = sourceFolder + "docWithBalancedPageTree.pdf";
            int maxAmountOfPagesReadAtATime = 0;
            PdfPagesTest.CustomPdfReader reader = new PdfPagesTest.CustomPdfReader(this, srcFile);
            PdfDocument document = new PdfDocument(reader);
            for (int page = 1; page <= document.GetNumberOfPages(); page++) {
                document.GetPage(page);
                if (reader.numOfPagesRead > maxAmountOfPagesReadAtATime) {
                    maxAmountOfPagesReadAtATime = reader.numOfPagesRead;
                }
                reader.numOfPagesRead = 0;
            }
            NUnit.Framework.Assert.AreEqual(111, document.GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(10, maxAmountOfPagesReadAtATime);
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ReadSinglePageTest() {
            String srcFile = sourceFolder + "allPagesAreLeaves.pdf";
            PdfPagesTest.CustomPdfReader reader = new PdfPagesTest.CustomPdfReader(this, srcFile);
            reader.SetMemorySavingMode(true);
            PdfDocument document = new PdfDocument(reader);
            int amountOfPages = document.GetNumberOfPages();
            PdfPages pdfPages = document.catalog.GetPageTree().GetRoot();
            PdfArray pageIndRefArray = ((PdfDictionary)pdfPages.GetPdfObject()).GetAsArray(PdfName.Kids);
            document.GetPage(amountOfPages);
            NUnit.Framework.Assert.AreEqual(1, GetAmountOfReadPages(pageIndRefArray));
            document.GetPage(amountOfPages / 2);
            NUnit.Framework.Assert.AreEqual(2, GetAmountOfReadPages(pageIndRefArray));
            document.GetPage(1);
            NUnit.Framework.Assert.AreEqual(3, GetAmountOfReadPages(pageIndRefArray));
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ImplicitPagesTreeRebuildingTest() {
            String inFileName = sourceFolder + "implicitPagesTreeRebuilding.pdf";
            String outFileName = destinationFolder + "implicitPagesTreeRebuilding.pdf";
            String cmpFileName = sourceFolder + "cmp_implicitPagesTreeRebuilding.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName), new PdfWriter(outFileName));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE)]
        public virtual void BrokenPageTreeWithExcessiveLastPageTest() {
            String inFileName = sourceFolder + "brokenPageTreeNullLast.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(4);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE)]
        public virtual void BrokenPageTreeWithExcessiveMiddlePageTest() {
            String inFileName = sourceFolder + "brokenPageTreeNullMiddle.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(3);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE, Count = 7)]
        public virtual void BrokenPageTreeWithExcessiveMultipleNegativePagesTest() {
            String inFileName = sourceFolder + "brokenPageTreeNullMultipleSequence.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(2, 3, 4, 6, 7, 8, 9);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE, Count = 2)]
        public virtual void BrokenPageTreeWithExcessiveRangeNegativePagesTest() {
            String inFileName = sourceFolder + "brokenPageTreeNullRangeNegative.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(2, 4);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        private static void FindAndAssertNullPages(PdfDocument pdfDocument, ICollection<int> nullPages) {
            foreach (int? e in nullPages) {
                NUnit.Framework.Assert.IsNull(pdfDocument.GetPage((int)e));
            }
        }

        private int GetAmountOfReadPages(PdfArray pageIndRefArray) {
            int amountOfLoadedPages = 0;
            for (int i = 0; i < pageIndRefArray.Size(); i++) {
                if (((PdfIndirectReference)pageIndRefArray.Get(i, false)).refersTo != null) {
                    amountOfLoadedPages++;
                }
            }
            return amountOfLoadedPages;
        }

        private class CustomPdfReader : PdfReader {
            public bool pagesAreRead = false;

            public int numOfPagesRead = 0;

            public CustomPdfReader(PdfPagesTest _enclosing, String filename)
                : base(filename) {
                this._enclosing = _enclosing;
            }

            protected internal override PdfObject ReadObject(PdfIndirectReference reference) {
                PdfObject toReturn = base.ReadObject(reference);
                if (toReturn is PdfDictionary && PdfName.Page.Equals(((PdfDictionary)toReturn).Get(PdfName.Type))) {
                    this.numOfPagesRead++;
                    this.pagesAreRead = true;
                }
                return toReturn;
            }

            private readonly PdfPagesTest _enclosing;
        }
    }
}
