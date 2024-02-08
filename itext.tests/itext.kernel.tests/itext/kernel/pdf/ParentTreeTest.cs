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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ParentTreeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/ParentTreeTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/ParentTreeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void Test01() {
            String outFile = destinationFolder + "parentTreeTest01.pdf";
            String cmpFile = sourceFolder + "cmp_parentTreeTest01.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage firstPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(firstPage);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, firstPage));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(firstPage, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(firstPage, span1))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfPage secondPage = document.AddNewPage();
            document.Close();
            NUnit.Framework.Assert.IsTrue(CheckParentTree(outFile, cmpFile));
        }

        [NUnit.Framework.Test]
        public virtual void StampingFormXObjectInnerContentTaggedTest() {
            String pdf = sourceFolder + "alreadyTaggedFormXObjectInnerContent.pdf";
            String outPdf = destinationFolder + "stampingFormXObjectInnerContentTaggedTest.pdf";
            String cmpPdf = sourceFolder + "cmp_stampingFormXObjectInnerContentTaggedTest.pdf";
            PdfDocument taggedPdf = new PdfDocument(new PdfReader(pdf), CompareTool.CreateTestPdfWriter(outPdf));
            taggedPdf.SetTagged();
            taggedPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SeveralXObjectsOnOnePageTest() {
            String pdf = sourceFolder + "severalXObjectsOnOnePageTest.pdf";
            String outPdf = destinationFolder + "severalXObjectsOnOnePageTest.pdf";
            String cmpPdf = sourceFolder + "cmp_severalXObjectsOnOnePageTest.pdf";
            PdfDocument taggedPdf = new PdfDocument(new PdfReader(pdf), CompareTool.CreateTestPdfWriter(outPdf));
            taggedPdf.SetTagged();
            taggedPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void EarlyFlushXObjectTaggedTest() {
            String pdf = sourceFolder + "earlyFlushXObjectTaggedTest.pdf";
            String outPdf = destinationFolder + "earlyFlushXObjectTaggedTest.pdf";
            String cmpPdf = sourceFolder + "cmp_earlyFlushXObjectTaggedTest.pdf";
            PdfDocument taggedPdf = new PdfDocument(new PdfReader(pdf), CompareTool.CreateTestPdfWriter(outPdf));
            PdfDictionary resource = taggedPdf.GetFirstPage().GetResources().GetResource(PdfName.XObject);
            resource.Get(new PdfName("Fm1")).Flush();
            taggedPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void IdenticalMcidIdInOneStreamTest() {
            String pdf = sourceFolder + "identicalMcidIdInOneStreamTest.pdf";
            String outPdf = destinationFolder + "identicalMcidIdInOneStreamTest.pdf";
            String cmpPdf = sourceFolder + "cmp_identicalMcidIdInOneStreamTest.pdf";
            PdfDocument taggedPdf = new PdfDocument(new PdfReader(pdf), CompareTool.CreateTestPdfWriter(outPdf));
            taggedPdf.SetTagged();
            taggedPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopyPageWithFormXObjectTaggedTest() {
            String cmpPdf = sourceFolder + "cmp_copyPageWithFormXobjectTaggedTest.pdf";
            String outDoc = destinationFolder + "copyPageWithFormXobjectTaggedTest.pdf";
            PdfDocument srcPdf = new PdfDocument(new PdfReader(sourceFolder + "copyFromFile.pdf"));
            PdfDocument outPdf = new PdfDocument(new PdfReader(sourceFolder + "copyToFile.pdf"), CompareTool.CreateTestPdfWriter
                (outDoc));
            outPdf.SetTagged();
            srcPdf.CopyPagesTo(1, 1, outPdf);
            srcPdf.Close();
            outPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outDoc, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void RemovePageWithFormXObjectTaggedTest() {
            String cmpPdf = sourceFolder + "cmp_removePageWithFormXobjectTaggedTest.pdf";
            String outDoc = destinationFolder + "removePageWithFormXobjectTaggedTest.pdf";
            PdfDocument outPdf = new PdfDocument(new PdfReader(sourceFolder + "forRemovePage.pdf"), CompareTool.CreateTestPdfWriter
                (outDoc));
            outPdf.SetTagged();
            outPdf.RemovePage(1);
            outPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outDoc, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            String outFile = destinationFolder + "parentTreeTest02.pdf";
            String cmpFile = sourceFolder + "cmp_parentTreeTest02.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage firstPage = document.AddNewPage();
            PdfPage secondPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(secondPage);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, secondPage));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(secondPage, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(secondPage, span1))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsTrue(CheckParentTree(outFile, cmpFile));
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            String outFile = destinationFolder + "parentTreeTest03.pdf";
            String cmpFile = sourceFolder + "cmp_parentTreeTest03.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage firstPage = document.AddNewPage();
            for (int i = 0; i < 51; i++) {
                PdfPage anotherPage = document.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(anotherPage);
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
                canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
                PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
                PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, anotherPage));
                canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(anotherPage, span1))));
                canvas.ShowText("Hello ");
                canvas.CloseTag();
                canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(anotherPage, span1))));
                canvas.ShowText("World");
                canvas.CloseTag();
                canvas.EndText();
                canvas.Release();
            }
            document.Close();
            NUnit.Framework.Assert.IsTrue(CheckParentTree(outFile, cmpFile));
        }

        [NUnit.Framework.Test]
        public virtual void Test04() {
            String outFile = destinationFolder + "parentTreeTest04.pdf";
            String cmpFile = sourceFolder + "cmp_parentTreeTest04.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            for (int i = 0; i < 51; i++) {
                PdfPage anotherPage = document.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(anotherPage);
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
                canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
                PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
                PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, anotherPage));
                canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(anotherPage, span1))));
                canvas.ShowText("Hello ");
                canvas.CloseTag();
                canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(anotherPage, span1))));
                canvas.ShowText("World");
                canvas.CloseTag();
                canvas.EndText();
                canvas.Release();
            }
            document.Close();
            NUnit.Framework.Assert.IsTrue(CheckParentTree(outFile, cmpFile));
        }

        [NUnit.Framework.Test]
        public virtual void Test05() {
            String outFile = destinationFolder + "parentTreeTest05.pdf";
            String cmpFile = sourceFolder + "cmp_parentTreeTest05.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page1));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page1, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(page1, span1))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfPage page2 = document.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page2));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page2, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page2));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page2, span2))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsTrue(CheckParentTree(outFile, cmpFile));
        }

        [NUnit.Framework.Test]
        public virtual void Test06() {
            String outFile = destinationFolder + "parentTreeTest06.pdf";
            String cmpFile = sourceFolder + "cmp_parentTreeTest06.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage firstPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(firstPage);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, firstPage));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(firstPage, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(firstPage, span1))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfPage secondPage = document.AddNewPage();
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            secondPage.AddAnnotation(linkExplicitDest);
            document.Close();
            NUnit.Framework.Assert.IsTrue(CheckParentTree(outFile, cmpFile));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.STRUCT_PARENT_INDEX_MISSED_AND_RECREATED, Count = 4)]
        public virtual void AllObjRefDontHaveStructParentTest() {
            String pdf = sourceFolder + "allObjRefDontHaveStructParent.pdf";
            String outPdf = destinationFolder + "allObjRefDontHaveStructParent.pdf";
            String cmpPdf = sourceFolder + "cmp_allObjRefDontHaveStructParent.pdf";
            PdfDocument taggedPdf = new PdfDocument(new PdfReader(pdf), new PdfWriter(outPdf));
            taggedPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.XOBJECT_STRUCT_PARENT_INDEX_MISSED_AND_RECREATED)]
        public virtual void XObjDoesntHaveStructParentTest() {
            String pdf = sourceFolder + "xObjDoesntHaveStructParentTest.pdf";
            String outPdf = destinationFolder + "xObjDoesntHaveStructParentTest.pdf";
            String cmpPdf = sourceFolder + "cmp_xObjDoesntHaveStructParentTest.pdf";
            PdfDocument taggedPdf = new PdfDocument(new PdfReader(pdf), new PdfWriter(outPdf));
            taggedPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
        }

        private bool CheckParentTree(String outFileName, String cmpFileName) {
            PdfReader outReader = CompareTool.CreateOutputReader(outFileName);
            PdfDocument outDocument = new PdfDocument(outReader);
            PdfReader cmpReader = CompareTool.CreateOutputReader(cmpFileName);
            PdfDocument cmpDocument = new PdfDocument(cmpReader);
            CompareTool.CompareResult result = new CompareTool().CompareByCatalog(outDocument, cmpDocument);
            if (!result.IsOk()) {
                System.Console.Out.WriteLine(result.GetReport());
            }
            return result.IsOk();
        }
    }
}
