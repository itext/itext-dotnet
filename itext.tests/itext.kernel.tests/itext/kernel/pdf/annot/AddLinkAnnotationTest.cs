/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AddLinkAnnotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/annot/AddLinkAnnotationTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/kernel/pdf/annot/AddLinkAnnotationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddLinkAnnotation01() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "linkAnnotation01.pdf"
                ));
            document.SetTagged();
            PdfPage page1 = document.AddNewPage();
            PdfPage page2 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Page 1");
            canvas.MoveText(0, -30);
            canvas.ShowText("Link to page 2. Click here!");
            canvas.EndText();
            canvas.Release();
            page1.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 260, 25)).SetDestination(PdfExplicitDestination
                .CreateFit(page2)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1.Flush();
            canvas = new PdfCanvas(page2);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Page 2");
            canvas.EndText();
            canvas.Release();
            page2.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotation01.pdf"
                , sourceFolder + "cmp_linkAnnotation01.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddLinkAnnotation02() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "linkAnnotation02.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Click here to go to itextpdf site.");
            canvas.EndText();
            canvas.Release();
            page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetDestination(PdfExplicitDestination
                .CreateFit(page)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })).SetColor(new PdfArray(new float[] { 
                1, 0, 0 })));
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotation02.pdf"
                , sourceFolder + "cmp_linkAnnotation02.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationReferenceTest() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "linkAnnotationReference.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            document.SetTagged();
            document.GetTagStructureContext().GetAutoTaggingPointer().AddTag("P");
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Click here to go to itextpdf site.");
            canvas.EndText();
            canvas.Release();
            page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetDestination(PdfExplicitDestination
                .CreateFit(page)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })).SetColor(new PdfArray(new float[] { 
                1, 0, 0 })));
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotationReference.pdf"
                , sourceFolder + "cmp_linkAnnotationReference.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationReference2Test() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "linkAnnotationReference2.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            document.SetTagged();
            document.GetTagStructureContext().GetAutoTaggingPointer().AddTag("P").SetNamespaceForNewTags(PdfNamespace.
                GetDefault(document)).AddTag("Reference");
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Click here to go to itextpdf site.");
            canvas.EndText();
            canvas.Release();
            page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction.CreateGoTo(
                PdfExplicitDestination.CreateFit(page))).SetBorder(new PdfArray(new float[] { 0, 0, 1 })).SetColor(new 
                PdfArray(new float[] { 1, 0, 0 })));
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotationReference2.pdf"
                , sourceFolder + "cmp_linkAnnotationReference2.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SeveralLinkAnnotationsTest() {
            using (PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "severalLinkAnnotations.pdf"
                ))) {
                document.SetTagged();
                document.GetTagStructureContext().GetAutoTaggingPointer();
                PdfPage page = document.AddNewPage();
                page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction.CreateGoTo(
                    PdfExplicitDestination.CreateFit(page))));
                page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction.CreateGoToR
                    (new PdfStringFS("Some fake destination"), new PdfExplicitDestination(new PdfArray(new PdfNumber(2))))
                    ));
                PdfDictionary destinationDictionary = new PdfDictionary();
                destinationDictionary.Put(PdfName.D, PdfExplicitDestination.CreateFit(page).GetPdfObject());
                PdfDictionary destinationDictionary2 = new PdfDictionary();
                destinationDictionary2.Put(PdfName.SD, PdfExplicitDestination.CreateFit(page).GetPdfObject());
                PdfDictionary dests = new PdfDictionary();
                dests.Put(new PdfName("destination_name"), destinationDictionary);
                dests.Put(new PdfName("destination_name_2"), destinationDictionary2);
                document.GetCatalog().Put(PdfName.Dests, dests);
                page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetDestination(new PdfNamedDestination
                    ("destination_name")));
                document.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("destination_name2", new PdfExplicitRemoteGoToDestination
                    (new PdfArray(new PdfNumber(1))).GetPdfObject());
                page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetDestination(new PdfStringDestination
                    ("destination_name2")));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "severalLinkAnnotations.pdf"
                , sourceFolder + "cmp_severalLinkAnnotations.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationWithDictionaryStringDestinationTest() {
            using (PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "linkAnnotationWithDictionaryStringDestination.pdf"
                ))) {
                document.SetTagged();
                document.GetTagStructureContext().GetAutoTaggingPointer();
                PdfPage page = document.AddNewPage();
                PdfDictionary destinationDictionary = new PdfDictionary();
                destinationDictionary.Put(PdfName.D, PdfExplicitDestination.CreateFit(page).GetPdfObject());
                document.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("destination_name", destinationDictionary);
                page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetDestination(new PdfStringDestination
                    ("destination_name")));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotationWithDictionaryStringDestination.pdf"
                , sourceFolder + "cmp_linkAnnotationWithDictionaryStringDestination.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationWithCyclicReferencesTest() {
            using (PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "linkAnnotationWithCyclicReferences.pdf"
                ))) {
                document.SetTagged();
                document.GetTagStructureContext().GetAutoTaggingPointer();
                PdfPage page = document.AddNewPage();
                PdfDictionary dests = new PdfDictionary();
                dests.Put(new PdfName("destination_name"), new PdfName("destination_name"));
                document.GetCatalog().Put(PdfName.Dests, dests);
                page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction.CreateGoTo(
                    new PdfNamedDestination("destination_name"))));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotationWithCyclicReferences.pdf"
                , sourceFolder + "cmp_linkAnnotationWithCyclicReferences.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddAndGetLinkAnnotations() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "linkAnnotation03.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Click here to go to itextpdf site.");
            canvas.MoveText(0, -50);
            canvas.ShowText("Click here to go to itextpdf blog.");
            canvas.MoveText(0, -50);
            canvas.ShowText("Click here to go to itextpdf FAQ.");
            canvas.EndText();
            canvas.Release();
            int[] borders = new int[] { 0, 0, 1 };
            page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction.CreateURI("http://itextpdf.com"
                )).SetBorder(new PdfArray(borders)).SetColor(new PdfArray(new float[] { 1, 0, 0 })));
            page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 540, 300, 25)).SetAction(PdfAction.CreateURI("http://itextpdf.com/node"
                )).SetBorder(new PdfArray(borders)).SetColor(new PdfArray(new float[] { 0, 1, 0 })));
            page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 490, 300, 25)).SetAction(PdfAction.CreateURI("http://itextpdf.com/salesfaq"
                )).SetBorder(new PdfArray(borders)).SetColor(new PdfArray(new float[] { 0, 0, 1 })));
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotation03.pdf"
                , sourceFolder + "cmp_linkAnnotation03.pdf", destinationFolder, "diff_"));
            document = new PdfDocument(CompareTool.CreateOutputReader(destinationFolder + "linkAnnotation03.pdf"));
            page = document.GetPage(1);
            NUnit.Framework.Assert.AreEqual(3, page.GetAnnotsSize());
            IList<PdfAnnotation> annotations = page.GetAnnotations();
            NUnit.Framework.Assert.AreEqual(3, annotations.Count);
            PdfLinkAnnotation link = (PdfLinkAnnotation)annotations[0];
            NUnit.Framework.Assert.AreEqual(page, link.GetPage());
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DESTINATION_NOT_PERMITTED_WHEN_ACTION_IS_SET)]
        public virtual void LinkAnnotationActionDestinationTest() {
            String fileName = "linkAnnotationActionDestinationTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + fileName));
            PdfArray array = new PdfArray();
            array.Add(pdfDocument.AddNewPage().GetPdfObject());
            array.Add(PdfName.XYZ);
            array.Add(new PdfNumber(36));
            array.Add(new PdfNumber(100));
            array.Add(new PdfNumber(1));
            PdfDestination dest = PdfDestination.MakeDestination(array);
            PdfLinkAnnotation link = new PdfLinkAnnotation(new Rectangle(0, 0, 0, 0));
            link.SetAction(PdfAction.CreateGoTo("abc"));
            link.SetDestination(dest);
            pdfDocument.GetPage(1).AddAnnotation(link);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveLinkAnnotationTaggedAsLinkTest() {
            String input = sourceFolder + "taggedLinkAnnotationAsLink.pdf";
            String output = destinationFolder + "removeLinkAnnotationTaggedAsLinkTest.pdf";
            String cmp = sourceFolder + "cmp_" + "removeLinkAnnotationTaggedAsLinkTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output))
                ) {
                PdfPage page = pdfDoc.GetPage(1);
                page.RemoveAnnotation(page.GetAnnotations()[0]);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveLinkAnnotationTaggedAsAnnotTest() {
            String input = sourceFolder + "taggedLinkAnnotationAsAnnot.pdf";
            String output = destinationFolder + "removeLinkAnnotationTaggedAsAnnotTest.pdf";
            String cmp = sourceFolder + "cmp_" + "removeLinkAnnotationTaggedAsAnnotTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output))
                ) {
                PdfPage page = pdfDoc.GetPage(1);
                page.RemoveAnnotation(page.GetAnnotations()[0]);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveLinkAnnotationTagWithContentTest() {
            String input = sourceFolder + "taggedLinkAnnotationTagWithContent.pdf";
            String output = destinationFolder + "removeLinkAnnotationTagWithContentTest.pdf";
            String cmp = sourceFolder + "cmp_" + "removeLinkAnnotationTagWithContentTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output))
                ) {
                PdfPage page = pdfDoc.GetPage(1);
                page.RemoveAnnotation(page.GetAnnotations()[0]);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveLinkAnnotationWithNoTagTest() {
            String input = sourceFolder + "taggedInvalidNoLinkAnnotationTag.pdf";
            String output = destinationFolder + "removeLinkAnnotationWithNoTagTest.pdf";
            String cmp = sourceFolder + "cmp_" + "removeLinkAnnotationWithNoTagTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), CompareTool.CreateTestPdfWriter(output))
                ) {
                PdfPage page = pdfDoc.GetPage(1);
                page.RemoveAnnotation(page.GetAnnotations()[0]);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void AddLinkAnnotInTagged13PdfTest() {
            String outPdf = destinationFolder + "addLinkAnnotInTagged13PdfTest.pdf";
            String cmpPdf = sourceFolder + "cmp_addLinkAnnotInTagged13PdfTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf, new WriterProperties()
                .SetPdfVersion(PdfVersion.PDF_1_3)))) {
                pdfDoc.SetTagged();
                PdfPage page = pdfDoc.AddNewPage();
                PdfLinkAnnotation annot = (PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(100, 600, 50, 40)).SetAction
                    (PdfAction.CreateURI("http://itextpdf.com")).SetBorder(new PdfArray(new float[] { 0, 0, 1 })).SetColor
                    (new PdfArray(new float[] { 1, 0, 0 }));
                page.AddAnnotation(annot);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }
    }
}
