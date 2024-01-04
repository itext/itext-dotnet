/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageFlushingTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PageFlushingTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PageFlushingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BaseWriting01() {
            // not all objects are made indirect before closing
            int total = 414;
            int flushedExpected = 0;
            int notReadExpected = 0;
            Test("baseWriting01.pdf", PageFlushingTest.DocMode.WRITING, PageFlushingTest.FlushMode.NONE, PageFlushingTest.PagesOp
                .MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void PageFlushWriting01() {
            int total = 715;
            int flushedExpected = 400;
            int notReadExpected = 0;
            Test("pageFlushWriting01.pdf", PageFlushingTest.DocMode.WRITING, PageFlushingTest.FlushMode.PAGE_FLUSH, PageFlushingTest.PagesOp
                .MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void UnsafeDeepFlushWriting01() {
            int total = 816;
            // 100 still hanging: new font dictionaries on every page shall not be flushed before closing
            int flushedExpected = 702;
            int notReadExpected = 0;
            Test("unsafeDeepFlushWriting01.pdf", PageFlushingTest.DocMode.WRITING, PageFlushingTest.FlushMode.UNSAFE_DEEP
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeFlushWriting01() {
            int total = 715;
            int flushedExpected = 400;
            int notReadExpected = 0;
            Test("appendModeFlushWriting01.pdf", PageFlushingTest.DocMode.WRITING, PageFlushingTest.FlushMode.APPEND_MODE
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void BaseReading01() {
            int total = 817;
            int flushedExpected = 0;
            // link annots, line annots, actions and images: one hundred of each
            int notReadExpected = 402;
            Test("baseReading01.pdf", PageFlushingTest.DocMode.READING, PageFlushingTest.FlushMode.NONE, PageFlushingTest.PagesOp
                .READ, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void ReleaseDeepReading01() {
            int total = 817;
            int flushedExpected = 0;
            int notReadExpected = 804;
            Test("releaseDeepReading01.pdf", PageFlushingTest.DocMode.READING, PageFlushingTest.FlushMode.RELEASE_DEEP
                , PageFlushingTest.PagesOp.READ, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void BaseStamping01() {
            // not all objects are made indirect before closing
            int total = 1618;
            int flushedExpected = 0;
            int notReadExpected = 603;
            Test("baseStamping01.pdf", PageFlushingTest.DocMode.STAMPING, PageFlushingTest.FlushMode.NONE, PageFlushingTest.PagesOp
                .MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void PageFlushStamping01() {
            int total = 2219;
            int flushedExpected = 1200;
            int notReadExpected = 403;
            Test("pageFlushStamping01.pdf", PageFlushingTest.DocMode.STAMPING, PageFlushingTest.FlushMode.PAGE_FLUSH, 
                PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void UnsafeDeepFlushStamping01() {
            int total = 2420;
            // 200 still hanging: new font dictionaries on every page shall not be flushed before closing
            int flushedExpected = 1602;
            int notReadExpected = 603;
            Test("unsafeDeepFlushStamping01.pdf", PageFlushingTest.DocMode.STAMPING, PageFlushingTest.FlushMode.UNSAFE_DEEP
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeFlushStamping01() {
            int total = 2219;
            // 300 less than with page#flush, because of not modified released objects
            int flushedExpected = 900;
            int notReadExpected = 703;
            Test("appendModeFlushStamping01.pdf", PageFlushingTest.DocMode.STAMPING, PageFlushingTest.FlushMode.APPEND_MODE
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void ReleaseDeepStamping01() {
            int total = 1618;
            int flushedExpected = 0;
            // new objects cannot be released
            int notReadExpected = 703;
            Test("releaseDeepStamping01.pdf", PageFlushingTest.DocMode.STAMPING, PageFlushingTest.FlushMode.RELEASE_DEEP
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void BaseAppendMode01() {
            int total = 1618;
            int flushedExpected = 0;
            int notReadExpected = 603;
            Test("baseAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.NONE, PageFlushingTest.PagesOp
                .MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void PageFlushAppendMode01() {
            int total = 2219;
            int flushedExpected = 900;
            int notReadExpected = 403;
            Test("pageFlushAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.PAGE_FLUSH, 
                PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void UnsafeDeepFlushAppendMode01() {
            int total = 2420;
            // 200 still hanging: new font dictionaries on every page shall not be flushed before closing
            int flushedExpected = 1502;
            int notReadExpected = 703;
            Test("unsafeDeepFlushAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.UNSAFE_DEEP
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeFlushAppendMode01() {
            int total = 2219;
            // 600 still hanging: every new page contains image, font and action
            int flushedExpected = 900;
            int notReadExpected = 703;
            Test("appendModeFlushAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.APPEND_MODE
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void ReleaseDeepAppendMode01() {
            int total = 1618;
            int flushedExpected = 0;
            // new objects cannot be released
            int notReadExpected = 703;
            Test("releaseDeepAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.RELEASE_DEEP
                , PageFlushingTest.PagesOp.MODIFY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void BaseLightAppendMode01() {
            int total = 1018;
            int flushedExpected = 0;
            int notReadExpected = 603;
            Test("baseLightAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.NONE, PageFlushingTest.PagesOp
                .MODIFY_LIGHTLY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void PageFlushLightAppendMode01() {
            int total = 1318;
            int flushedExpected = 500;
            // in default PdfPage#flush annotations are always read and attempted to be flushed.
            int notReadExpected = 403;
            Test("pageFlushLightAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.PAGE_FLUSH
                , PageFlushingTest.PagesOp.MODIFY_LIGHTLY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void UnsafeDeepFlushLightAppendMode01() {
            int total = 1318;
            int flushedExpected = 600;
            int notReadExpected = 703;
            Test("unsafeDeepFlushLightAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.UNSAFE_DEEP
                , PageFlushingTest.PagesOp.MODIFY_LIGHTLY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeFlushLightAppendMode01() {
            int total = 1318;
            // resources are not flushed, here it's font dictionaries for every page which in any case shall not be flushed before closing.
            int flushedExpected = 500;
            int notReadExpected = 703;
            Test("appendModeFlushLightAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.APPEND_MODE
                , PageFlushingTest.PagesOp.MODIFY_LIGHTLY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void ReleaseDeepLightAppendMode01() {
            int total = 1018;
            int flushedExpected = 0;
            int notReadExpected = 703;
            Test("releaseDeepLightAppendMode01.pdf", PageFlushingTest.DocMode.APPEND, PageFlushingTest.FlushMode.RELEASE_DEEP
                , PageFlushingTest.PagesOp.MODIFY_LIGHTLY, total, flushedExpected, notReadExpected);
        }

        [NUnit.Framework.Test]
        public virtual void ModifyAnnotationOnlyAppendMode() {
            String input = sourceFolder + "100pages.pdf";
            String output = destinationFolder + "modifyAnnotOnly.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output), new StampingProperties
                ().UseAppendMode());
            PdfPage page = pdfDoc.GetPage(1);
            PdfIndirectReference pageIndRef = page.GetPdfObject().GetIndirectReference();
            PdfDictionary annotObj = page.GetAnnotations()[0].SetRectangle(new PdfArray(new Rectangle(0, 0, 300, 300))
                ).SetPage(page).GetPdfObject();
            PageFlushingHelper flushingHelper = new PageFlushingHelper(pdfDoc);
            flushingHelper.AppendModeFlush(1);
            // annotation is flushed
            NUnit.Framework.Assert.IsTrue(annotObj.IsFlushed());
            // page is not flushed
            NUnit.Framework.Assert.IsFalse(pageIndRef.CheckState(PdfObject.FLUSHED));
            // page is released
            NUnit.Framework.Assert.IsNull(pageIndRef.refersTo);
            // exception is not thrown
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SetLinkDestinationToPageAppendMode() {
            String input = sourceFolder + "100pages.pdf";
            String output = destinationFolder + "setLinkDestinationToPageAppendMode.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output), new StampingProperties
                ().UseAppendMode());
            PdfPage page1 = pdfDoc.GetPage(1);
            PdfPage page2 = pdfDoc.GetPage(2);
            PdfIndirectReference page1IndRef = page1.GetPdfObject().GetIndirectReference();
            PdfIndirectReference page2IndRef = page2.GetPdfObject().GetIndirectReference();
            PdfDictionary aDict = ((PdfLinkAnnotation)page1.GetAnnotations()[0]).GetAction();
            new PdfAction(aDict).Put(PdfName.D, PdfExplicitDestination.CreateXYZ(page2, 300, 400, 1).GetPdfObject());
            PageFlushingHelper flushingHelper = new PageFlushingHelper(pdfDoc);
            flushingHelper.AppendModeFlush(2);
            flushingHelper.UnsafeFlushDeep(1);
            // annotation is flushed
            NUnit.Framework.Assert.IsTrue(aDict.IsFlushed());
            // page is not flushed
            NUnit.Framework.Assert.IsFalse(page1IndRef.CheckState(PdfObject.FLUSHED));
            // page is released
            NUnit.Framework.Assert.IsNull(page1IndRef.refersTo);
            // page is not flushed
            NUnit.Framework.Assert.IsFalse(page2IndRef.CheckState(PdfObject.FLUSHED));
            // page is released
            NUnit.Framework.Assert.IsNull(page2IndRef.refersTo);
            // exception is not thrown
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FlushSelfContainingObjectsWritingMode() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfDictionary pageDict = pdfDoc.AddNewPage().GetPdfObject();
            PdfDictionary dict1 = new PdfDictionary();
            pageDict.Put(new PdfName("dict1"), dict1);
            PdfArray arr1 = new PdfArray();
            pageDict.Put(new PdfName("arr1"), arr1);
            dict1.Put(new PdfName("dict1"), dict1);
            dict1.Put(new PdfName("arr1"), arr1);
            arr1.Add(arr1);
            arr1.Add(dict1);
            arr1.MakeIndirect(pdfDoc);
            dict1.MakeIndirect(pdfDoc);
            PageFlushingHelper flushingHelper = new PageFlushingHelper(pdfDoc);
            flushingHelper.UnsafeFlushDeep(1);
            NUnit.Framework.Assert.IsTrue(dict1.IsFlushed());
            NUnit.Framework.Assert.IsTrue(arr1.IsFlushed());
            pdfDoc.Close();
        }

        // exception is not thrown
        [NUnit.Framework.Test]
        public virtual void FlushingPageResourcesMadeIndependent() {
            String inputFile = sourceFolder + "100pagesSharedResDict.pdf";
            String outputFile = destinationFolder + "flushingPageResourcesMadeIndependent.pdf";
            PdfDocument pdf = new PdfDocument(new PdfReader(inputFile), CompareTool.CreateTestPdfWriter(outputFile));
            int numOfAddedXObjectsPerPage = 10;
            for (int i = 1; i <= pdf.GetNumberOfPages(); ++i) {
                PdfPage sourcePage = pdf.GetPage(i);
                PdfDictionary res = sourcePage.GetPdfObject().GetAsDictionary(PdfName.Resources);
                PdfDictionary resClone = new PdfDictionary();
                // clone dictionary manually to ensure this object is direct and is flushed together with the page
                foreach (KeyValuePair<PdfName, PdfObject> e in res.EntrySet()) {
                    resClone.Put(e.Key, e.Value.Clone());
                }
                sourcePage.GetPdfObject().Put(PdfName.Resources, resClone);
                PdfCanvas pdfCanvas = new PdfCanvas(sourcePage);
                pdfCanvas.SaveState();
                for (int j = 0; j < numOfAddedXObjectsPerPage; ++j) {
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "simple.jpg"));
                    pdfCanvas.AddXObjectFittedIntoRectangle(xObject, new Rectangle(36, 720 - j * 150, 20, 20));
                    xObject.MakeIndirect(pdf).Flush();
                }
                pdfCanvas.RestoreState();
                pdfCanvas.Release();
                sourcePage.Flush();
            }
            VerifyFlushedObjectsNum(pdf, 1416, 1400, 0);
            pdf.Close();
            PrintOutputPdfNameAndDir(outputFile);
            PdfDocument result = new PdfDocument(CompareTool.CreateOutputReader(outputFile));
            PdfObject page15Res = result.GetPage(15).GetPdfObject().Get(PdfName.Resources, false);
            PdfObject page34Res = result.GetPage(34).GetPdfObject().Get(PdfName.Resources, false);
            NUnit.Framework.Assert.IsTrue(page15Res.IsDictionary());
            NUnit.Framework.Assert.AreEqual(numOfAddedXObjectsPerPage, ((PdfDictionary)page15Res).GetAsDictionary(PdfName
                .XObject).Size());
            NUnit.Framework.Assert.IsTrue(page34Res.IsDictionary());
            NUnit.Framework.Assert.AreNotEqual(page15Res, page34Res);
            result.Close();
        }

        private static void Test(String filename, PageFlushingTest.DocMode docMode, PageFlushingTest.FlushMode flushMode
            , PageFlushingTest.PagesOp pagesOp, int total, int flushedExpected, int notReadExpected) {
            String input = sourceFolder + "100pages.pdf";
            String output = destinationFolder + filename;
            PdfDocument pdfDoc;
            switch (docMode) {
                case PageFlushingTest.DocMode.WRITING: {
                    pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(output));
                    break;
                }

                case PageFlushingTest.DocMode.READING: {
                    pdfDoc = new PdfDocument(new PdfReader(input));
                    break;
                }

                case PageFlushingTest.DocMode.STAMPING: {
                    pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output));
                    break;
                }

                case PageFlushingTest.DocMode.APPEND: {
                    pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output), new StampingProperties
                        ().UseAppendMode());
                    break;
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
            PageFlushingHelper flushingHelper = new PageFlushingHelper(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itext.png"));
            if (docMode != PageFlushingTest.DocMode.WRITING) {
                for (int i = 0; i < 100; ++i) {
                    PdfPage page = pdfDoc.GetPage(i + 1);
                    switch (pagesOp) {
                        case PageFlushingTest.PagesOp.READ: {
                            PdfTextExtractor.GetTextFromPage(page);
                            break;
                        }

                        case PageFlushingTest.PagesOp.MODIFY: {
                            AddContentToPage(page, font, xObject);
                            break;
                        }

                        case PageFlushingTest.PagesOp.MODIFY_LIGHTLY: {
                            AddBasicContent(page, font);
                            break;
                        }
                    }
                    switch (flushMode) {
                        case PageFlushingTest.FlushMode.UNSAFE_DEEP: {
                            flushingHelper.UnsafeFlushDeep(i + 1);
                            break;
                        }

                        case PageFlushingTest.FlushMode.RELEASE_DEEP: {
                            flushingHelper.ReleaseDeep(i + 1);
                            break;
                        }

                        case PageFlushingTest.FlushMode.APPEND_MODE: {
                            flushingHelper.AppendModeFlush(i + 1);
                            break;
                        }

                        case PageFlushingTest.FlushMode.PAGE_FLUSH: {
                            page.Flush();
                            break;
                        }
                    }
                }
            }
            if (docMode != PageFlushingTest.DocMode.READING && pagesOp == PageFlushingTest.PagesOp.MODIFY) {
                for (int i = 0; i < 100; ++i) {
                    PdfPage page = pdfDoc.AddNewPage();
                    AddContentToPage(page, font, xObject);
                    switch (flushMode) {
                        case PageFlushingTest.FlushMode.UNSAFE_DEEP: {
                            flushingHelper.UnsafeFlushDeep(pdfDoc.GetNumberOfPages());
                            break;
                        }

                        case PageFlushingTest.FlushMode.RELEASE_DEEP: {
                            flushingHelper.ReleaseDeep(pdfDoc.GetNumberOfPages());
                            break;
                        }

                        case PageFlushingTest.FlushMode.APPEND_MODE: {
                            flushingHelper.AppendModeFlush(pdfDoc.GetNumberOfPages());
                            break;
                        }

                        case PageFlushingTest.FlushMode.PAGE_FLUSH: {
                            page.Flush();
                            break;
                        }
                    }
                }
            }
            VerifyFlushedObjectsNum(pdfDoc, total, flushedExpected, notReadExpected);
            pdfDoc.Close();
        }

        private static void VerifyFlushedObjectsNum(PdfDocument pdfDoc, int total, int flushedExpected, int notReadExpected
            ) {
            int flushedActual = 0;
            int notReadActual = 0;
            for (int i = 0; i < pdfDoc.GetXref().Size(); ++i) {
                PdfIndirectReference indRef = pdfDoc.GetXref().Get(i);
                if (indRef.CheckState(PdfObject.FLUSHED)) {
                    ++flushedActual;
                }
                else {
                    if (!indRef.IsFree() && indRef.refersTo == null) {
                        ++notReadActual;
                    }
                }
            }
            if (pdfDoc.GetXref().Size() != total || flushedActual != flushedExpected || notReadActual != notReadExpected
                ) {
                NUnit.Framework.Assert.Fail(MessageFormatUtil.Format("\nExpected total: {0}, flushed: {1}, not read: {2};"
                     + "\nbut actual was: {3}, flushed: {4}, not read: {5}.", total, flushedExpected, notReadExpected, pdfDoc
                    .GetXref().Size(), flushedActual, notReadActual));
            }
            NUnit.Framework.Assert.AreEqual(total, pdfDoc.GetXref().Size(), "wrong num of total objects");
            NUnit.Framework.Assert.AreEqual(flushedExpected, flushedActual, "wrong num of flushed objects");
            NUnit.Framework.Assert.AreEqual(notReadExpected, notReadActual, "wrong num of not read objects");
        }

        private static void AddContentToPage(PdfPage pdfPage, PdfFont font, PdfImageXObject xObject) {
            PdfCanvas canvas = AddBasicContent(pdfPage, font);
            canvas.SaveState().Rectangle(250, 500, 100, 100).Fill().RestoreState();
            PdfFont courier = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            courier.MakeIndirect(pdfPage.GetDocument());
            canvas.SaveState().BeginText().MoveText(36, 650).SetFontAndSize(courier, 16).ShowText("Hello Courier!").EndText
                ().RestoreState();
            canvas.SaveState().Circle(100, 400, 25).Fill().RestoreState();
            canvas.SaveState().RoundRectangle(100, 650, 100, 100, 10).Fill().RestoreState();
            canvas.SaveState().SetLineWidth(10).RoundRectangle(250, 650, 100, 100, 10).Stroke().RestoreState();
            canvas.SaveState().SetLineWidth(5).Arc(400, 650, 550, 750, 0, 180).Stroke().RestoreState();
            canvas.SaveState().SetLineWidth(5).MoveTo(400, 550).CurveTo(500, 570, 450, 450, 550, 550).Stroke().RestoreState
                ();
            canvas.AddXObjectFittedIntoRectangle(xObject, new Rectangle(100, 500, 400, xObject.GetHeight()));
            PdfImageXObject xObject2 = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itext.png"));
            xObject2.MakeIndirect(pdfPage.GetDocument());
            canvas.AddXObjectFittedIntoRectangle(xObject2, new Rectangle(100, 500, 400, xObject2.GetHeight()));
        }

        private static PdfCanvas AddBasicContent(PdfPage pdfPage, PdfFont font) {
            Rectangle lineAnnotRect = new Rectangle(0, 0, PageSize.A4.GetRight(), PageSize.A4.GetTop());
            pdfPage.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 600, 100, 20)).SetAction(PdfAction.CreateURI
                ("http://itextpdf.com"))).AddAnnotation(new PdfLineAnnotation(lineAnnotRect, new float[] { lineAnnotRect
                .GetX(), lineAnnotRect.GetY(), lineAnnotRect.GetRight(), lineAnnotRect.GetTop() }).SetColor(ColorConstants
                .BLACK));
            PdfCanvas canvas = new PdfCanvas(pdfPage);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.SaveState().BeginText().SetTextMatrix(AffineTransform.GetRotateInstance(Math.PI / 4, 36, 350)).SetFontAndSize
                (font, 72).ShowText("Hello Helvetica!").EndText().RestoreState();
            return canvas;
        }

        private enum DocMode {
            WRITING,
            READING,
            STAMPING,
            APPEND
        }

        private enum FlushMode {
            NONE,
            PAGE_FLUSH,
            UNSAFE_DEEP,
            RELEASE_DEEP,
            APPEND_MODE
        }

        private enum PagesOp {
            NONE,
            READ,
            MODIFY,
            MODIFY_LIGHTLY
        }
    }
}
