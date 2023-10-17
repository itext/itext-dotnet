/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfPagesTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfPagesTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfPagesTest/";

        private static readonly PdfName PageNum = new PdfName("PageNum");

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void HugeNumberOfPagesWithOnePageTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hugeNumberOfPagesWithOnePage.pdf"), new 
                PdfWriter(new MemoryStream()));
            PdfPage page = new PdfPage(pdfDoc, pdfDoc.GetDefaultPageSize());
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.AddPage(1, page));
        }

        [NUnit.Framework.Test]
        public virtual void CountDontCorrespondToRealTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "countDontCorrespondToReal.pdf"), new PdfWriter
                (new MemoryStream()));
            PdfPage page = new PdfPage(pdfDoc, pdfDoc.GetDefaultPageSize());
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.AddPage(1, page));
            // we don't expect that Count will be different from real number of pages
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => pdfDoc.Close());
        }

        [NUnit.Framework.Test]
        public virtual void SimplePagesTest() {
            String filename = "simplePagesTest.pdf";
            int pageCount = 111;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(PageNum, new PdfNumber(i + 1));
                page.Flush();
            }
            pdfDoc.Close();
            VerifyPagesOrder(DESTINATION_FOLDER + filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void ReversePagesTest() {
            String filename = "reversePagesTest.pdf";
            int pageCount = 111;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            for (int i = pageCount; i > 0; i--) {
                PdfPage page = new PdfPage(pdfDoc, pdfDoc.GetDefaultPageSize());
                pdfDoc.AddPage(1, page);
                page.GetPdfObject().Put(PageNum, new PdfNumber(i));
                page.Flush();
            }
            pdfDoc.Close();
            VerifyPagesOrder(DESTINATION_FOLDER + filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void ReversePagesTest2() {
            String filename = "1000PagesDocument_reversed.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "1000PagesDocument.pdf"), new PdfWriter
                (DESTINATION_FOLDER + filename));
            int n = pdfDoc.GetNumberOfPages();
            for (int i = n - 1; i > 0; --i) {
                pdfDoc.MovePage(i, n + 1);
            }
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff"));
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
            PdfDocument document = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
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
            VerifyPagesOrder(DESTINATION_FOLDER + filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void RandomNumberPagesTest() {
            String filename = "randomNumberPagesTest.pdf";
            int pageCount = 1000;
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
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
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
            VerifyPagesOrder(DESTINATION_FOLDER + filename, pageCount);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED)]
        public virtual void InsertFlushedPageTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.RemovePage(page);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.AddPage(1, page));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FLUSHED_PAGE_CANNOT_BE_ADDED_OR_INSERTED, e
                .Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED)]
        public virtual void AddFlushedPageTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.RemovePage(page);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.AddPage(page));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FLUSHED_PAGE_CANNOT_BE_ADDED_OR_INSERTED, e
                .Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED, Count = 2)]
        public virtual void RemoveFlushedPage() {
            String filename = "removeFlushedPage.pdf";
            int pageCount = 10;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
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
            VerifyPagesOrder(DESTINATION_FOLDER + filename, pageCount - 1);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFlushedPageFromTaggedDocument() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.SetTagged();
                pdfDocument.AddNewPage();
                pdfDocument.GetPage(1).Flush();
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.RemovePage(1));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FLUSHED_PAGE_CANNOT_BE_REMOVED, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemoveFlushedPageFromDocumentWithAcroForm() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.GetCatalog().Put(PdfName.AcroForm, new PdfDictionary());
                pdfDocument.AddNewPage();
                pdfDocument.GetPage(1).Flush();
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.RemovePage(1));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.FLUSHED_PAGE_CANNOT_BE_REMOVED, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestInheritedResources() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "simpleInheritedResources.pdf"));
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
            String input = SOURCE_FOLDER + "circularReferencesInResources.pdf";
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "simpleInheritedResources.pdf"), new PdfWriter
                (DESTINATION_FOLDER + "updateInheritedResources.pdf").SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                ));
            PdfName newGsName = pdfDoc.GetPage(1).GetResources().AddExtGState(new PdfExtGState().SetLineWidth(30));
            int gsCount = pdfDoc.GetPage(1).GetResources().GetResource(PdfName.ExtGState).Size();
            pdfDoc.Close();
            String compareResult = new CompareTool().CompareByContent(DESTINATION_FOLDER + "updateInheritedResources.pdf"
                , SOURCE_FOLDER + "cmp_" + "updateInheritedResources.pdf", DESTINATION_FOLDER, "diff");
            NUnit.Framework.Assert.AreEqual(3, gsCount);
            NUnit.Framework.Assert.AreEqual("Gs3", newGsName.GetValue());
            NUnit.Framework.Assert.IsNull(compareResult);
        }

        [NUnit.Framework.Test]
        public virtual void ReorderInheritedResourcesTest() {
            //TODO: DEVSIX-1643 Inherited resources aren't copied on page reordering
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "inheritedFontResources.pdf"), new PdfWriter
                (DESTINATION_FOLDER + "reorderInheritedFontResources.pdf"));
            pdfDoc.MovePage(1, pdfDoc.GetNumberOfPages() + 1);
            pdfDoc.RemovePage(1);
            pdfDoc.Close();
            String compareResult = new CompareTool().CompareByContent(DESTINATION_FOLDER + "reorderInheritedFontResources.pdf"
                , SOURCE_FOLDER + "cmp_reorderInheritedFontResources.pdf", DESTINATION_FOLDER, "diff_reorderInheritedFontResources_"
                );
            NUnit.Framework.Assert.IsNull(compareResult);
        }

        [NUnit.Framework.Test]
        public virtual void GetPageByDictionary() {
            String filename = SOURCE_FOLDER + "1000PagesDocument.pdf";
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
            String testName = "docWithFieldsRemovePage.pdf";
            String outPdf = DESTINATION_FOLDER + testName;
            String sourceFile = SOURCE_FOLDER + "docWithFields.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFile), new PdfWriter(outPdf))) {
                pdfDoc.RemovePage(1);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_" + testName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void GetPageSizeWithInheritedMediaBox() {
            double eps = 0.0000001;
            String filename = SOURCE_FOLDER + "inheritedMediaBox.pdf";
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
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename).SetCompressionLevel(CompressionConstants
                .NO_COMPRESSION));
            PdfPage page = pdfDoc.AddNewPage().SetThumbnailImage(new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER
                 + imageSrc)));
            new PdfCanvas(page).SetFillColor(ColorConstants.RED).Rectangle(100, 100, 400, 400).Fill();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RotationPagesRotationTest() {
            String filename = "singlePageDocumentWithRotation.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + filename));
            PdfPage page = pdfDoc.GetPage(1);
            NUnit.Framework.Assert.AreEqual(90, page.GetRotation(), "Inherited value is invalid");
        }

        [NUnit.Framework.Test]
        public virtual void PageTreeCleanupParentRefTest() {
            String src = SOURCE_FOLDER + "CatalogWithPageAndPagesEntries.pdf";
            String dest = DESTINATION_FOLDER + "CatalogWithPageAndPagesEntries_opened.pdf";
            PdfReader reader = new PdfReader(src);
            PdfWriter writer = new PdfWriter(dest);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsTrue(TestPageTreeParentsValid(src) && TestPageTreeParentsValid(dest));
        }

        [NUnit.Framework.Test]
        public virtual void PdfNumberInPageContentArrayTest() {
            String src = SOURCE_FOLDER + "pdfNumberInPageContentArray.pdf";
            String dest = DESTINATION_FOLDER + "pdfNumberInPageContentArray_saved.pdf";
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
            PdfDocument inputPdf = new PdfDocument(new PdfReader(SOURCE_FOLDER + "input500.pdf"));
            PdfDocument outputPdf = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + "output500.pdf"));
            float scaleX = 595f / 612f;
            float scaleY = 842f / 792f;
            for (int i = 1; i <= inputPdf.GetNumberOfPages(); ++i) {
                PdfPage sourcePage = inputPdf.GetPage(i);
                PdfFormXObject pageCopy = sourcePage.CopyAsFormXObject(outputPdf);
                PdfPage page = outputPdf.AddNewPage(PageSize.A4);
                PdfCanvas outputCanvas = new PdfCanvas(page);
                outputCanvas.AddXObjectWithTransformationMatrix(pageCopy, scaleX, 0, 0, scaleY, 0, 0);
                page.Flush();
            }
            outputPdf.Close();
            inputPdf.Close();
            NUnit.Framework.Assert.IsNotNull(outputPdf.GetXref());
            NUnit.Framework.Assert.AreEqual(500, outputPdf.GetXref().Size() - inputPdf.GetXref().Size());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.WRONG_MEDIABOX_SIZE_TOO_MANY_ARGUMENTS, Count = 1)]
        public virtual void PageGetMediaBoxTooManyArgumentsTest() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "helloWorldMediaboxTooManyArguments.pdf");
            Rectangle expected = new Rectangle(0, 0, 375, 300);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfPage pageOne = pdfDoc.GetPage(1);
            Rectangle actual = pageOne.GetPageSize();
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void CloseDocumentWithRecursivePagesNodeReferencesThrowsExTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "recursivePagesNodeReference.pdf")) {
                using (PdfWriter writer = new PdfWriter(new MemoryStream())) {
                    PdfDocument pdfDocument = new PdfDocument(reader, writer);
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.Close());
                    NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE
                        , 2), e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPageWithRecursivePagesNodeReferenceInAppendModeThrowExTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "recursivePagesNodeReference.pdf")) {
                using (PdfWriter writer = new PdfWriter(new MemoryStream())) {
                    using (PdfDocument pdfDocument = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode())
                        ) {
                        NUnit.Framework.Assert.AreEqual(2, pdfDocument.GetNumberOfPages());
                        NUnit.Framework.Assert.IsNotNull(pdfDocument.GetPage(1));
                        Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.GetPage(2));
                        NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE
                            , 2), e.Message);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CloseDocumentWithRecursivePagesNodeInAppendModeDoesNotThrowsTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "recursivePagesNodeReference.pdf")) {
                using (PdfWriter writer = new PdfWriter(new MemoryStream())) {
                    using (PdfDocument pdfDocument = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode())
                        ) {
                        NUnit.Framework.Assert.DoesNotThrow(() => pdfDocument.Close());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PageGetMediaBoxNotEnoughArgumentsTest() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "helloWorldMediaboxNotEnoughArguments.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfPage pageOne = pdfDoc.GetPage(1);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pageOne.GetPageSize());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.WRONG_MEDIA_BOX_SIZE_TOO_FEW_ARGUMENTS
                , 3), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InsertIntermediateParentTest() {
            String filename = "insertIntermediateParentTest.pdf";
            PdfReader reader = new PdfReader(SOURCE_FOLDER + filename);
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
            String srcFile = SOURCE_FOLDER + "taggedOnePage.pdf";
            PdfPagesTest.CustomPdfReader reader = new PdfPagesTest.CustomPdfReader(this, srcFile);
            PdfDocument document = new PdfDocument(reader);
            document.Close();
            NUnit.Framework.Assert.IsFalse(reader.pagesAreRead);
        }

        [NUnit.Framework.Test]
        public virtual void CopyAnnotationWithoutSubtypeTest() {
            using (MemoryStream baos = CreateSourceDocumentWithEmptyAnnotation(new MemoryStream())) {
                using (PdfDocument documentToMerge = new PdfDocument(new PdfReader(new RandomAccessSourceFactory().CreateSource
                    (baos.ToArray()), new ReaderProperties()))) {
                    using (MemoryStream resultantBaos = new MemoryStream()) {
                        using (PdfDocument resultantDocument = new PdfDocument(new PdfWriter(resultantBaos))) {
                            // We do expect that the following line will not throw any NPE
                            PdfPage copiedPage = documentToMerge.GetPage(1).CopyTo(resultantDocument);
                            NUnit.Framework.Assert.AreEqual(1, copiedPage.GetAnnotations().Count);
                            NUnit.Framework.Assert.IsNull(copiedPage.GetAnnotations()[0].GetSubtype());
                            resultantDocument.AddPage(copiedPage);
                        }
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadPagesInBlocksTest() {
            String srcFile = SOURCE_FOLDER + "docWithBalancedPageTree.pdf";
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
            String srcFile = SOURCE_FOLDER + "allPagesAreLeaves.pdf";
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
            String inFileName = SOURCE_FOLDER + "implicitPagesTreeRebuilding.pdf";
            String outFileName = DESTINATION_FOLDER + "implicitPagesTreeRebuilding.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_implicitPagesTreeRebuilding.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName), new PdfWriter(outFileName));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE)]
        public virtual void BrokenPageTreeWithExcessiveLastPageTest() {
            String inFileName = SOURCE_FOLDER + "brokenPageTreeNullLast.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(4);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE)]
        public virtual void BrokenPageTreeWithExcessiveMiddlePageTest() {
            String inFileName = SOURCE_FOLDER + "brokenPageTreeNullMiddle.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(3);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE, Count = 7)]
        public virtual void BrokenPageTreeWithExcessiveMultipleNegativePagesTest() {
            String inFileName = SOURCE_FOLDER + "brokenPageTreeNullMultipleSequence.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(2, 3, 4, 6, 7, 8, 9);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE, Count = 2)]
        public virtual void BrokenPageTreeWithExcessiveRangeNegativePagesTest() {
            String inFileName = SOURCE_FOLDER + "brokenPageTreeNullRangeNegative.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            IList<int> pages = JavaUtil.ArraysAsList(2, 4);
            ICollection<int> nullPages = new HashSet<int>(pages);
            FindAndAssertNullPages(pdfDocument, nullPages);
        }

        [NUnit.Framework.Test]
        public virtual void TestPageTreeGenerationWhenFirstPdfPagesHasOnePageOnly() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            int totalPageCount = PdfPagesTree.DEFAULT_LEAF_SIZE + 4;
            for (int i = 0; i < totalPageCount; i++) {
                pdfDocument.AddNewPage();
            }
            NUnit.Framework.Assert.AreEqual(2, pdfDocument.GetCatalog().GetPageTree().GetParents().Count);
            NUnit.Framework.Assert.AreEqual(PdfPagesTree.DEFAULT_LEAF_SIZE, pdfDocument.GetCatalog().GetPageTree().GetParents
                ()[0].GetCount());
            // Leave only one page in the first pages tree
            for (int i = PdfPagesTree.DEFAULT_LEAF_SIZE - 1; i >= 1; i--) {
                pdfDocument.RemovePage(i);
            }
            NUnit.Framework.Assert.AreEqual(2, pdfDocument.GetCatalog().GetPageTree().GetParents().Count);
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetCatalog().GetPageTree().GetParents()[0].GetCount());
            // TODO DEVSIX-5575 remove expected exception and add proper assertions
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => pdfDocument.Close());
        }

        private static void FindAndAssertNullPages(PdfDocument pdfDocument, ICollection<int> nullPages) {
            foreach (int? nullPage in nullPages) {
                int pageNum = (int)nullPage;
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.GetPage(pageNum
                    ));
                NUnit.Framework.Assert.AreEqual(exception.Message, MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant
                    .PAGE_TREE_IS_BROKEN_FAILED_TO_RETRIEVE_PAGE, pageNum));
            }
        }

        private static int GetAmountOfReadPages(PdfArray pageIndRefArray) {
            int amountOfLoadedPages = 0;
            for (int i = 0; i < pageIndRefArray.Size(); i++) {
                if (((PdfIndirectReference)pageIndRefArray.Get(i, false)).refersTo != null) {
                    amountOfLoadedPages++;
                }
            }
            return amountOfLoadedPages;
        }

        private static void VerifyPagesOrder(String filename, int numOfPages) {
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.IsNotNull(page);
                PdfNumber number = page.GetAsNumber(PageNum);
                NUnit.Framework.Assert.AreEqual(i, number.IntValue(), "Page number");
            }
            NUnit.Framework.Assert.AreEqual(numOfPages, pdfDocument.GetNumberOfPages(), "Number of pages");
            pdfDocument.Close();
        }

        private static int VerifyIntegrity(PdfPagesTree pagesTree) {
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

        private static MemoryStream CreateSourceDocumentWithEmptyAnnotation(MemoryStream baos) {
            using (PdfDocument sourceDocument = new PdfDocument(new PdfWriter(baos))) {
                PdfPage page = sourceDocument.AddNewPage();
                PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(new PdfDictionary());
                page.AddAnnotation(annotation);
                return baos;
            }
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
