using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AddLinkAnnotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/annot/AddLinkAnnotationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/annot/AddLinkAnnotationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddLinkAnnotation01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "linkAnnotation01.pdf"));
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
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "linkAnnotation02.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Click here to go to itextpdf site.");
            canvas.EndText();
            canvas.Release();
            page.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction.CreateURI("http://itextpdf.com"
                )).SetBorder(new PdfArray(new float[] { 0, 0, 1 })).SetColor(new PdfArray(new float[] { 1, 0, 0 })));
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotation02.pdf"
                , sourceFolder + "cmp_linkAnnotation02.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddAndGetLinkAnnotations() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "linkAnnotation03.pdf"));
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
            document = new PdfDocument(new PdfReader(destinationFolder + "linkAnnotation03.pdf"));
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
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(destinationFolder + fileName, FileMode.Create
                )));
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
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), new PdfWriter(output))) {
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
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), new PdfWriter(output))) {
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
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), new PdfWriter(output))) {
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
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(input), new PdfWriter(output))) {
                PdfPage page = pdfDoc.GetPage(1);
                page.RemoveAnnotation(page.GetAnnotations()[0]);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder));
        }
    }
}
