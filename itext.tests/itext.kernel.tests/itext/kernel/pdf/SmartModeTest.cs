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
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SmartModeTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/SmartModeTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/SmartModeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeSameResourcesCopyingAndFlushing() {
            String outFile = destinationFolder + "smartModeSameResourcesCopyingAndFlushing.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSameResourcesCopyingAndFlushing.pdf";
            String[] srcFiles = new String[] { sourceFolder + "indirectResourcesStructure.pdf", sourceFolder + "indirectResourcesStructure2.pdf"
                 };
            PdfDocument outputDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile, new WriterProperties().UseSmartMode
                ()));
            foreach (String srcFile in srcFiles) {
                PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFile));
                sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), outputDoc);
                sourceDoc.Close();
                outputDoc.FlushCopiedObjects(sourceDoc);
            }
            outputDoc.Close();
            PdfDocument assertDoc = new PdfDocument(CompareTool.CreateOutputReader(outFile));
            PdfIndirectReference page1ResFontObj = assertDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page2ResFontObj = assertDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page3ResFontObj = assertDoc.GetPage(3).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            NUnit.Framework.Assert.IsTrue(page1ResFontObj.Equals(page2ResFontObj));
            NUnit.Framework.Assert.IsTrue(page1ResFontObj.Equals(page3ResFontObj));
            assertDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeSameResourcesCopyingModifyingAndFlushing() {
            String outFile = destinationFolder + "smartModeSameResourcesCopyingModifyingAndFlushing.pdf";
            String[] srcFiles = new String[] { sourceFolder + "indirectResourcesStructure.pdf", sourceFolder + "indirectResourcesStructure2.pdf"
                 };
            bool exceptionCaught = false;
            PdfDocument outputDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile, new WriterProperties().UseSmartMode
                ()));
            int lastPageNum = 1;
            PdfFont font = PdfFontFactory.CreateFont();
            foreach (String srcFile in srcFiles) {
                PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFile));
                sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), outputDoc);
                sourceDoc.Close();
                int i;
                for (i = lastPageNum; i <= outputDoc.GetNumberOfPages(); ++i) {
                    PdfCanvas canvas;
                    try {
                        canvas = new PdfCanvas(outputDoc.GetPage(i));
                    }
                    catch (NullReferenceException) {
                        // Smart mode makes it possible to share objects coming from different source documents.
                        // Flushing one object documents might make it impossible to modify further copied objects.
                        NUnit.Framework.Assert.AreEqual(2, i);
                        exceptionCaught = true;
                        break;
                    }
                    canvas.BeginText().MoveText(36, 36).SetFontAndSize(font, 12).ShowText("Page " + i).EndText();
                }
                lastPageNum = i;
                if (exceptionCaught) {
                    break;
                }
                outputDoc.FlushCopiedObjects(sourceDoc);
            }
            if (!exceptionCaught) {
                NUnit.Framework.Assert.Fail();
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeSameResourcesCopyingModifyingAndFlushing_ensureObjectFresh() {
            String outFile = destinationFolder + "smartModeSameResourcesCopyingModifyingAndFlushing_ensureObjectFresh.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSameResourcesCopyingModifyingAndFlushing_ensureObjectFresh.pdf";
            String[] srcFiles = new String[] { sourceFolder + "indirectResourcesStructure.pdf", sourceFolder + "indirectResourcesStructure2.pdf"
                 };
            PdfDocument outputDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile, new WriterProperties().UseSmartMode
                ()));
            int lastPageNum = 1;
            PdfFont font = PdfFontFactory.CreateFont();
            foreach (String srcFile in srcFiles) {
                PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFile));
                for (int i = 1; i <= sourceDoc.GetNumberOfPages(); ++i) {
                    PdfDictionary srcRes = sourceDoc.GetPage(i).GetPdfObject().GetAsDictionary(PdfName.Resources);
                    // Ensures that objects copied to the output document are fresh,
                    // i.e. are not reused from already copied objects cache.
                    bool ensureObjectIsFresh = true;
                    // it's crucial to copy first inner objects and then the container object!
                    foreach (PdfObject v in srcRes.Values()) {
                        if (v.GetIndirectReference() != null) {
                            // We are not interested in returned copied objects instances, they will be picked up by
                            // general copying mechanism from copied objects cache by default.
                            v.CopyTo(outputDoc, ensureObjectIsFresh);
                        }
                    }
                    if (srcRes.GetIndirectReference() != null) {
                        srcRes.CopyTo(outputDoc, ensureObjectIsFresh);
                    }
                }
                sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), outputDoc);
                sourceDoc.Close();
                int i_1;
                for (i_1 = lastPageNum; i_1 <= outputDoc.GetNumberOfPages(); ++i_1) {
                    PdfPage page = outputDoc.GetPage(i_1);
                    PdfCanvas canvas = new PdfCanvas(page);
                    canvas.BeginText().MoveText(36, 36).SetFontAndSize(font, 12).ShowText("Page " + i_1).EndText();
                }
                lastPageNum = i_1;
                outputDoc.FlushCopiedObjects(sourceDoc);
            }
            outputDoc.Close();
            PdfDocument assertDoc = new PdfDocument(CompareTool.CreateOutputReader(outFile));
            PdfIndirectReference page1ResFontObj = assertDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page2ResFontObj = assertDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page3ResFontObj = assertDoc.GetPage(3).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            NUnit.Framework.Assert.IsFalse(page1ResFontObj.Equals(page2ResFontObj));
            NUnit.Framework.Assert.IsFalse(page1ResFontObj.Equals(page3ResFontObj));
            NUnit.Framework.Assert.IsFalse(page2ResFontObj.Equals(page3ResFontObj));
            assertDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void PageCopyAsFormXObjectWithInheritedResourcesTest() {
            String cmpFile = sourceFolder + "cmp_pageCopyAsFormXObjectWithInheritedResourcesTest.pdf";
            String srcFile = sourceFolder + "pageCopyAsFormXObjectWithInheritedResourcesTest.pdf";
            String destFile = destinationFolder + "pageCopyAsFormXObjectWithInheritedResourcesTest.pdf";
            PdfDocument origPdf = new PdfDocument(new PdfReader(srcFile));
            PdfDocument copyPdfX = new PdfDocument(CompareTool.CreateTestPdfWriter(destFile).SetSmartMode(true));
            PdfDictionary pages = origPdf.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages);
            if (pages != null) {
                for (int i = 1; i < origPdf.GetNumberOfPages() + 1; i++) {
                    PdfPage origPage = origPdf.GetPage(i);
                    Rectangle ps = origPage.GetPageSize();
                    PdfPage page = copyPdfX.AddNewPage(new PageSize(ps));
                    PdfCanvas canvas = new PdfCanvas(page);
                    PdfFormXObject pageCopy = origPage.CopyAsFormXObject(copyPdfX);
                    canvas.AddXObjectAt(pageCopy, 0, 0);
                }
            }
            copyPdfX.Close();
            origPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeSameImageResourcesTest() {
            String srcFile = sourceFolder + "sameImageResources.pdf";
            String outFile = destinationFolder + "smartModeSameImageResources.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSameImageResources.pdf";
            using (PdfDocument newDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile).SetSmartMode(true))) {
                using (PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile))) {
                    srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), newDoc);
                }
                PdfIndirectReference page1ImgRes = newDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.XObject).GetAsStream(new PdfName("Im0")).GetIndirectReference();
                PdfIndirectReference page2ImgRes = newDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.XObject).GetAsStream(new PdfName("Im0")).GetIndirectReference();
                NUnit.Framework.Assert.AreEqual(page1ImgRes, page2ImgRes);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeSameColorSpaceResourcesTest() {
            String srcFile = sourceFolder + "colorSpaceResource.pdf";
            String outFile = destinationFolder + "smartModeSameColorSpaceResources.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSameColorSpaceResources.pdf";
            using (PdfDocument newDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile).SetSmartMode(true))) {
                for (int i = 0; i < 2; i++) {
                    using (PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile))) {
                        srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), newDoc);
                    }
                }
                PdfObject page1CsRes = newDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.XObject).Get(new PdfName("Im0")).GetIndirectReference();
                PdfObject page2CsRes = newDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.XObject).Get(new PdfName("Im0")).GetIndirectReference();
                // It's expected that indirect arrays are not processed by smart mode.
                // Smart mode only merges duplicate dictionaries and streams.
                NUnit.Framework.Assert.AreEqual(page1CsRes, page2CsRes);
                PdfIndirectReference page1CsStm = newDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.ColorSpace).GetAsArray(new PdfName("CS0")).GetAsStream(1).GetIndirectReference();
                PdfIndirectReference page2CsStm = newDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.ColorSpace).GetAsArray(new PdfName("CS0")).GetAsStream(1).GetIndirectReference();
                NUnit.Framework.Assert.AreEqual(page1CsStm, page2CsStm);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeSameExtGStateResourcesTest() {
            String srcFile = sourceFolder + "extGStateResource.pdf";
            String outFile = destinationFolder + "smartModeSameExtGStateResources.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSameExtGStateResources.pdf";
            using (PdfDocument newDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile).SetSmartMode(true))) {
                for (int i = 0; i < 2; i++) {
                    using (PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile))) {
                        srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), newDoc);
                    }
                }
                PdfIndirectReference page1GsRes = newDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.ExtGState).GetAsDictionary(new PdfName("Gs1")).GetIndirectReference();
                PdfIndirectReference page2GsRes = newDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                    (PdfName.ExtGState).GetAsDictionary(new PdfName("Gs1")).GetIndirectReference();
                NUnit.Framework.Assert.AreEqual(page1GsRes, page2GsRes);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeCopyingInTaggedPdfTest() {
            String srcFile = sourceFolder + "simpleTaggedDocument.pdf";
            String dstFile = destinationFolder + "smartModeCopyingInTaggedPdf.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeCopyingInTaggedPdf.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                pdfDest.SetTagged();
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                NUnit.Framework.Assert.IsNotNull(pdfDest.GetStructTreeRoot().GetKidsObject().GetAsDictionary(0).GetAsArray
                    (PdfName.K).GetAsDictionary(0).Get(PdfName.K));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dstFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeCopyingInPdfWithIdenticalPagesTaggedTest() {
            String srcFile = sourceFolder + "docWithAllPagesIdenticalTagged.pdf";
            String dstFile = destinationFolder + "smartModeCopyingInPdfWithIdenticalPagesTagged.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                pdfDest.SetTagged();
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                PdfIndirectReference expectedPageObj = pdfDest.GetPage(1).GetPdfObject().GetIndirectReference();
                PdfIndirectReference expectedContStm = pdfDest.GetPage(1).GetPdfObject().GetAsStream(PdfName.Contents).GetIndirectReference
                    ();
                for (int i = 2; i <= 10; i++) {
                    PdfIndirectReference pageObj = pdfDest.GetPage(i).GetPdfObject().GetIndirectReference();
                    NUnit.Framework.Assert.AreNotEqual(expectedPageObj, pageObj);
                    PdfIndirectReference pageContentStm = pdfDest.GetPage(i).GetPdfObject().GetAsStream(PdfName.Contents).GetIndirectReference
                        ();
                    NUnit.Framework.Assert.AreEqual(expectedContStm, pageContentStm);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeCopyingInPdfWithIdenticalPagesTest() {
            String srcFile = sourceFolder + "docWithAllPagesIdenticalNotTagged.pdf";
            String dstFile = destinationFolder + "smartModeCopyingInPdfWithIdenticalPagesNotTagged.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                PdfIndirectReference expectedPageObj = pdfDest.GetPage(1).GetPdfObject().GetIndirectReference();
                PdfIndirectReference expectedContStm = pdfDest.GetPage(1).GetPdfObject().GetAsStream(PdfName.Contents).GetIndirectReference
                    ();
                for (int i = 2; i <= 10; i++) {
                    PdfIndirectReference pageObj = pdfDest.GetPage(i).GetPdfObject().GetIndirectReference();
                    NUnit.Framework.Assert.AreNotEqual(expectedPageObj, pageObj);
                    PdfIndirectReference pageContentStm = pdfDest.GetPage(i).GetPdfObject().GetAsStream(PdfName.Contents).GetIndirectReference
                        ();
                    NUnit.Framework.Assert.AreEqual(expectedContStm, pageContentStm);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeCopyingInPdfSamePagesDifferentXObjectsTest() {
            String srcFile = sourceFolder + "identicalPagesDifferentXObjects.pdf";
            String dstFile = destinationFolder + "smartModeCopyingInPdfSamePagesDifferentXObjects.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                PdfIndirectReference expectedImgRes = pdfDest.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources)
                    .GetAsDictionary(PdfName.XObject).GetAsStream(new PdfName("Im1")).GetIndirectReference();
                for (int i = 2; i <= 99; i++) {
                    PdfIndirectReference pagesImgRes = pdfDest.GetPage(i).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                        (PdfName.XObject).GetAsStream(new PdfName("Im1")).GetIndirectReference();
                    NUnit.Framework.Assert.AreEqual(expectedImgRes, pagesImgRes);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmartCopyingOfArrayWithStringsTest() {
            String srcFile = sourceFolder + "keyValueStructure.pdf";
            String dstFile = destinationFolder + "smartCopyingOfArrayWithStrings.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                PdfIndirectReference key1Ref = pdfDest.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("Key1")).GetIndirectReference
                    ();
                PdfIndirectReference key2Ref = pdfDest.GetPage(2).GetPdfObject().GetAsDictionary(new PdfName("Key2")).GetIndirectReference
                    ();
                // Currently smart mode copying doesn't affect any other objects except streams and dictionaries
                NUnit.Framework.Assert.AreNotEqual(key1Ref, key2Ref);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmartCopyingOfNestedIndirectDictionariesTest() {
            String srcFile = sourceFolder + "nestedIndirectDictionaries.pdf";
            String dstFile = destinationFolder + "smartCopyingOfNestedIndirectDictionariesTest.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                PdfObject key1Page1Ref = pdfDest.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("Key1")).GetIndirectReference
                    ();
                PdfIndirectReference key1Page2Ref = pdfDest.GetPage(2).GetPdfObject().GetAsDictionary(new PdfName("Key1"))
                    .GetIndirectReference();
                PdfIndirectReference key2Page1Ref = pdfDest.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("Key1"))
                    .GetAsDictionary(new PdfName("Key2")).GetIndirectReference();
                PdfIndirectReference key2Page2Ref = pdfDest.GetPage(2).GetPdfObject().GetAsDictionary(new PdfName("Key1"))
                    .GetAsDictionary(new PdfName("Key2")).GetIndirectReference();
                PdfIndirectReference key3Page1Ref = pdfDest.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("Key1"))
                    .GetAsDictionary(new PdfName("Key2")).GetAsArray(new PdfName("Key3")).GetIndirectReference();
                PdfIndirectReference key3Page2Ref = pdfDest.GetPage(2).GetPdfObject().GetAsDictionary(new PdfName("Key1"))
                    .GetAsDictionary(new PdfName("Key2")).GetAsArray(new PdfName("Key3")).GetIndirectReference();
                // Currently smart mode copying doesn't affect any other objects except streams and dictionaries
                NUnit.Framework.Assert.AreEqual(key1Page1Ref, key1Page2Ref);
                NUnit.Framework.Assert.AreEqual(key2Page1Ref, key2Page2Ref);
                NUnit.Framework.Assert.AreEqual(key3Page1Ref, key3Page2Ref);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeSeparatedOutlinesCopyingTest() {
            String dstFile = destinationFolder + "smartModeSeparatedOutlinesCopying.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSeparatedOutlinesCopying.pdf";
            String[] srcFiles = new String[] { sourceFolder + "separatedOutlinesCopying.pdf", sourceFolder + "separatedOutlinesCopying.pdf"
                 };
            using (PdfDocument outputDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                outputDoc.InitializeOutlines();
                foreach (String srcFile in srcFiles) {
                    PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFile));
                    sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), outputDoc);
                    sourceDoc.Close();
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dstFile, cmpFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeCopyingInPdfWithLinksOnOnePageTest() {
            String srcFile = sourceFolder + "identical100PagesDiffObjectsLinksOnOnePage.pdf";
            String dstFile = destinationFolder + "smartModeCopyingInPdfWithLinksOnOnePage.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                PdfIndirectReference expectedImgRes = pdfDest.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources)
                    .GetAsDictionary(PdfName.XObject).GetAsStream(new PdfName("Im1")).GetIndirectReference();
                for (int i = 2; i <= 99; i++) {
                    PdfIndirectReference pagesImgRes = pdfDest.GetPage(i).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                        (PdfName.XObject).GetAsStream(new PdfName("Im1")).GetIndirectReference();
                    NUnit.Framework.Assert.AreEqual(expectedImgRes, pagesImgRes);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmartModeCopyingInPdfWithDiffImagesTest() {
            String srcFile = sourceFolder + "docWithDifferentImages.pdf";
            String dstFile = destinationFolder + "smartModeCopyingInPdfWithDiffImages.pdf";
            using (PdfDocument pdfDest = new PdfDocument(CompareTool.CreateTestPdfWriter(dstFile, new WriterProperties
                ().UseSmartMode()))) {
                using (PdfDocument pdfSrc = new PdfDocument(new PdfReader(srcFile))) {
                    pdfSrc.CopyPagesTo(1, pdfSrc.GetNumberOfPages(), pdfDest);
                }
                PdfIndirectReference expectedImgRes = pdfDest.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources)
                    .GetAsDictionary(PdfName.XObject).GetAsStream(new PdfName("Im1")).GetIndirectReference();
                for (int i = 2; i <= 99; i++) {
                    PdfIndirectReference pagesImgRes = pdfDest.GetPage(i).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                        (PdfName.XObject).GetAsStream(new PdfName("Im1")).GetIndirectReference();
                    NUnit.Framework.Assert.AreNotEqual(expectedImgRes, pagesImgRes);
                }
            }
        }
    }
}
