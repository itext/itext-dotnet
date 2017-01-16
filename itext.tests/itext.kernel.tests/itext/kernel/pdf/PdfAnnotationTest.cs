using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfAnnotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfAnnotationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfAnnotationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddLinkAnnotation01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "linkAnnotation01.pdf"));
            PdfPage page1 = document.AddNewPage();
            PdfPage page2 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER_BOLD), 14);
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
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Page 2");
            canvas.EndText();
            canvas.Release();
            page2.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotation01.pdf"
                , sourceFolder + "cmp_linkAnnotation01.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddLinkAnnotation02() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "linkAnnotation02.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Click here to go to itextpdf site.");
            canvas.EndText();
            canvas.Release();
            page.AddAnnotation(((PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction
                .CreateURI("http://itextpdf.com"))).SetBorder(new PdfArray(new float[] { 0, 0, 1 })).SetColor(new PdfArray
                (new float[] { 1, 0, 0 })));
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "linkAnnotation02.pdf"
                , sourceFolder + "cmp_linkAnnotation02.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddAndGetLinkAnnotations() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "linkAnnotation03.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Click here to go to itextpdf site.");
            canvas.MoveText(0, -50);
            canvas.ShowText("Click here to go to itextpdf blog.");
            canvas.MoveText(0, -50);
            canvas.ShowText("Click here to go to itextpdf FAQ.");
            canvas.EndText();
            canvas.Release();
            int[] borders = new int[] { 0, 0, 1 };
            page.AddAnnotation(((PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(100, 590, 300, 25)).SetAction(PdfAction
                .CreateURI("http://itextpdf.com"))).SetBorder(new PdfArray(borders)).SetColor(new PdfArray(new float[]
                 { 1, 0, 0 })));
            page.AddAnnotation(((PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(100, 540, 300, 25)).SetAction(PdfAction
                .CreateURI("http://itextpdf.com/node"))).SetBorder(new PdfArray(borders)).SetColor(new PdfArray(new float
                [] { 0, 1, 0 })));
            page.AddAnnotation(((PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(100, 490, 300, 25)).SetAction(PdfAction
                .CreateURI("http://itextpdf.com/salesfaq"))).SetBorder(new PdfArray(borders)).SetColor(new PdfArray(new 
                float[] { 0, 0, 1 })));
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.DESTINATION_NOT_PERMITTED_WHEN_ACTION_IS_SET)]
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
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddTextAnnotation01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "textAnnotation01.pdf"));
            PdfPage page = document.AddNewPage();
            PdfTextAnnotation textannot = new PdfTextAnnotation(new Rectangle(100, 600, 50, 40));
            textannot.SetText(new PdfString("Text Annotation 01")).SetContents(new PdfString("Some contents..."));
            PdfPopupAnnotation popupAnnot = new PdfPopupAnnotation(new Rectangle(150, 640, 200, 100));
            popupAnnot.SetOpen(true);
            textannot.SetPopup(popupAnnot);
            popupAnnot.SetParent(textannot);
            page.AddAnnotation(textannot);
            page.AddAnnotation(popupAnnot);
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "textAnnotation01.pdf"
                , sourceFolder + "cmp_textAnnotation01.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CaretTest() {
            String filename = destinationFolder + "caretAnnotation.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("This is a text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(236, 750).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("This is an edited text").EndText().RestoreState();
            PdfCaretAnnotation caret = new PdfCaretAnnotation(new Rectangle(36, 745, 350, 20));
            caret.SetSymbol(new PdfString("P"));
            PdfPopupAnnotation popup = new PdfPopupAnnotation(new Rectangle(36, 445, 100, 100));
            popup.SetContents(new PdfString("Popup"));
            popup.SetOpen(true);
            caret.SetPopup(popup);
            page1.AddAnnotation(caret);
            page1.AddAnnotation(popup);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_CaretAnnotation.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddFreeTextAnnotation01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "freeTextAnnotation01.pdf"));
            PdfPage page = document.AddNewPage();
            new PdfCanvas(page).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 24).MoveText
                (100, 600).ShowText("Annotated text").EndText().Release();
            PdfFreeTextAnnotation textannot = new PdfFreeTextAnnotation(new Rectangle(300, 700, 150, 20), "");
            textannot.SetContents(new PdfString("FreeText annotation")).SetColor(new float[] { 1, 0, 0 });
            textannot.SetIntent(PdfName.FreeTextCallout);
            textannot.SetCalloutLine(new float[] { 120, 616, 180, 680, 300, 710 }).SetLineEndingStyle(PdfName.OpenArrow
                );
            page.AddAnnotation(textannot);
            textannot.Flush();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "freeTextAnnotation01.pdf"
                , sourceFolder + "cmp_freeTextAnnotation01.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddSquareAndCircleAnnotations01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "squareAndCircleAnnotations01.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfSquareAnnotation square = new PdfSquareAnnotation(new Rectangle(100, 700, 100, 100));
            square.SetInteriorColor(new float[] { 1, 0, 0 }).SetColor(new float[] { 0, 1, 0 }).SetContents("RED Square"
                );
            page.AddAnnotation(square);
            PdfCircleAnnotation circle = new PdfCircleAnnotation(new Rectangle(300, 700, 100, 100));
            circle.SetInteriorColor(new float[] { 0, 1, 0 }).SetColor(new float[] { 0, 0, 1 }).SetContents(new PdfString
                ("GREEN Circle"));
            page.AddAnnotation(circle);
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "squareAndCircleAnnotations01.pdf"
                , sourceFolder + "cmp_squareAndCircleAnnotations01.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FileAttachmentTest() {
            String filename = destinationFolder + "fileAttachmentAnnotation.pdf";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, sourceFolder + "sample.wav", null, "sample.wav"
                , null, null, true);
            PdfFileAttachmentAnnotation fileAttach = new PdfFileAttachmentAnnotation(new Rectangle(100, 100), spec);
            fileAttach.SetIconName(PdfName.Paperclip);
            page1.AddAnnotation(fileAttach);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_fileAttachmentAnnotation.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RubberStampTest() {
            String filename = destinationFolder + "rubberStampAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
            stamp.SetStampName(PdfName.Approved);
            PdfStampAnnotation stamp1 = new PdfStampAnnotation(new Rectangle(0, 50, 100, 50));
            stamp1.SetStampName(PdfName.AsIs);
            PdfStampAnnotation stamp2 = new PdfStampAnnotation(new Rectangle(0, 100, 100, 50));
            stamp2.SetStampName(PdfName.Confidential);
            PdfStampAnnotation stamp3 = new PdfStampAnnotation(new Rectangle(0, 150, 100, 50));
            stamp3.SetStampName(PdfName.Departmental);
            PdfStampAnnotation stamp4 = new PdfStampAnnotation(new Rectangle(0, 200, 100, 50));
            stamp4.SetStampName(PdfName.Draft);
            PdfStampAnnotation stamp5 = new PdfStampAnnotation(new Rectangle(0, 250, 100, 50));
            stamp5.SetStampName(PdfName.Experimental);
            PdfStampAnnotation stamp6 = new PdfStampAnnotation(new Rectangle(0, 300, 100, 50));
            stamp6.SetStampName(PdfName.Expired);
            PdfStampAnnotation stamp7 = new PdfStampAnnotation(new Rectangle(0, 350, 100, 50));
            stamp7.SetStampName(PdfName.Final);
            PdfStampAnnotation stamp8 = new PdfStampAnnotation(new Rectangle(0, 400, 100, 50));
            stamp8.SetStampName(PdfName.ForComment);
            PdfStampAnnotation stamp9 = new PdfStampAnnotation(new Rectangle(0, 450, 100, 50));
            stamp9.SetStampName(PdfName.ForPublicRelease);
            PdfStampAnnotation stamp10 = new PdfStampAnnotation(new Rectangle(0, 500, 100, 50));
            stamp10.SetStampName(PdfName.NotApproved);
            PdfStampAnnotation stamp11 = new PdfStampAnnotation(new Rectangle(0, 550, 100, 50));
            stamp11.SetStampName(PdfName.NotForPublicRelease);
            PdfStampAnnotation stamp12 = new PdfStampAnnotation(new Rectangle(0, 600, 100, 50));
            stamp12.SetStampName(PdfName.Sold);
            PdfStampAnnotation stamp13 = new PdfStampAnnotation(new Rectangle(0, 650, 100, 50));
            stamp13.SetStampName(PdfName.TopSecret);
            page1.AddAnnotation(stamp);
            page1.AddAnnotation(stamp1);
            page1.AddAnnotation(stamp2);
            page1.AddAnnotation(stamp3);
            page1.AddAnnotation(stamp4);
            page1.AddAnnotation(stamp5);
            page1.AddAnnotation(stamp6);
            page1.AddAnnotation(stamp7);
            page1.AddAnnotation(stamp8);
            page1.AddAnnotation(stamp9);
            page1.AddAnnotation(stamp10);
            page1.AddAnnotation(stamp11);
            page1.AddAnnotation(stamp12);
            page1.AddAnnotation(stamp13);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_rubberStampAnnotation01.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RubberStampWrongStampTest() {
            String filename = destinationFolder + "rubberStampAnnotation02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
            stamp.SetStampName(PdfName.StrikeOut);
            page1.AddAnnotation(stamp);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_rubberStampAnnotation02.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.IsNull(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void InkTest() {
            String filename = destinationFolder + "inkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            float[] array1 = new float[] { 100, 100, 100, 200, 200, 200, 300, 300 };
            PdfArray firstPoint = new PdfArray(array1);
            PdfArray resultArray = new PdfArray();
            resultArray.Add(firstPoint);
            PdfDictionary borderStyle = new PdfDictionary();
            borderStyle.Put(PdfName.Type, PdfName.Border);
            borderStyle.Put(PdfName.W, new PdfNumber(3));
            PdfInkAnnotation ink = new PdfInkAnnotation(new Rectangle(0, 0, 575, 842), resultArray);
            ink.SetBorderStyle(borderStyle);
            float[] rgb = new float[] { 1, 0, 0 };
            PdfArray colors = new PdfArray(rgb);
            ink.SetColor(colors);
            page1.AddAnnotation(ink);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_inkAnnotation01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.IsNull(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextMarkupTest01() {
            String filename = destinationFolder + "textMarkupAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Underline!").EndText().RestoreState();
            float[] points = new float[] { 36, 765, 109, 765, 36, 746, 109, 746 };
            PdfTextMarkupAnnotation markup = PdfTextMarkupAnnotation.CreateUnderline(PageSize.A4, points);
            markup.SetContents(new PdfString("TextMarkup"));
            float[] rgb = new float[] { 1, 0, 0 };
            PdfArray colors = new PdfArray(rgb);
            markup.SetColor(colors);
            page1.AddAnnotation(markup);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_textMarkupAnnotation01.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.IsNull(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextMarkupTest02() {
            String filename = destinationFolder + "textMarkupAnnotation02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Highlight!").EndText().RestoreState();
            float[] points = new float[] { 36, 765, 109, 765, 36, 746, 109, 746 };
            PdfTextMarkupAnnotation markup = PdfTextMarkupAnnotation.CreateHighLight(PageSize.A4, points);
            markup.SetContents(new PdfString("TextMarkup"));
            float[] rgb = new float[] { 1, 0, 0 };
            PdfArray colors = new PdfArray(rgb);
            markup.SetColor(colors);
            page1.AddAnnotation(markup);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_textMarkupAnnotation02.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.IsNull(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextMarkupTest03() {
            String filename = destinationFolder + "textMarkupAnnotation03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Squiggly!").EndText().RestoreState();
            float[] points = new float[] { 36, 765, 109, 765, 36, 746, 109, 746 };
            PdfTextMarkupAnnotation markup = PdfTextMarkupAnnotation.CreateSquiggly(PageSize.A4, points);
            markup.SetContents(new PdfString("TextMarkup"));
            float[] rgb = new float[] { 1, 0, 0 };
            PdfArray colors = new PdfArray(rgb);
            markup.SetColor(colors);
            page1.AddAnnotation(markup);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_textMarkupAnnotation03.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.IsNull(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextMarkupTest04() {
            String filename = destinationFolder + "textMarkupAnnotation04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Strikeout!").EndText().RestoreState();
            float[] points = new float[] { 36, 765, 109, 765, 36, 746, 109, 746 };
            PdfTextMarkupAnnotation markup = PdfTextMarkupAnnotation.CreateStrikeout(PageSize.A4, points);
            markup.SetContents(new PdfString("TextMarkup"));
            float[] rgb = new float[] { 1, 0, 0 };
            PdfArray colors = new PdfArray(rgb);
            markup.SetColor(colors);
            page1.AddAnnotation(markup);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_textMarkupAnnotation04.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PrinterMarkText() {
            String filename = destinationFolder + "printerMarkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvasText = new PdfCanvas(page1);
            canvasText.SaveState().BeginText().MoveText(36, 790).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants
                .HELVETICA), 16).ShowText("This is Printer Mark annotation:").EndText().RestoreState();
            PdfFormXObject form = new PdfFormXObject(PageSize.A4);
            PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
            canvas.SaveState().Circle(265, 795, 5).SetColor(Color.GREEN, true).Fill().RestoreState();
            canvas.Release();
            PdfPrinterMarkAnnotation printer = new PdfPrinterMarkAnnotation(PageSize.A4, form);
            page1.AddAnnotation(printer);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_printerMarkAnnotation01.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TrapNetworkText() {
            String filename = destinationFolder + "trapNetworkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvasText = new PdfCanvas(page);
            canvasText.SaveState().BeginText().MoveText(36, 790).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants
                .HELVETICA), 16).ShowText("This is Trap Network annotation:").EndText().RestoreState();
            PdfFormXObject form = new PdfFormXObject(PageSize.A4);
            PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
            canvas.SaveState().Circle(272, 795, 5).SetColor(Color.GREEN, true).Fill().RestoreState();
            canvas.Release();
            form.SetProcessColorModel(PdfName.DeviceN);
            PdfTrapNetworkAnnotation trap = new PdfTrapNetworkAnnotation(PageSize.A4, form);
            page.AddAnnotation(trap);
            page.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_trapNetworkAnnotation01.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Sound.Sampled.UnsupportedAudioFileException"/>
        [NUnit.Framework.Test]
        public virtual void SoundTestAif() {
            String filename = destinationFolder + "soundAnnotation02.pdf";
            String audioFile = sourceFolder + "sample.aif";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            String @string = "";
            for (int i = 0; i < 4; i++) {
                @string = @string + (char)@is.Read();
            }
            if (@string.Equals("RIFF")) {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
                @is.Read();
            }
            else {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            }
            PdfStream sound1 = new PdfStream(pdfDoc, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            PdfSoundAnnotation sound = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), sound1);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation02.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SoundTestAiff() {
            String filename = destinationFolder + "soundAnnotation03.pdf";
            String audioFile = sourceFolder + "sample.aiff";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            String @string = "";
            for (int i = 0; i < 4; i++) {
                @string = @string + (char)@is.Read();
            }
            if (@string.Equals("RIFF")) {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
                @is.Read();
            }
            else {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            }
            PdfStream sound1 = new PdfStream(pdfDoc, @is);
            sound1.Put(PdfName.R, new PdfNumber(44100));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            PdfSoundAnnotation sound = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), sound1);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation03.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Sound.Sampled.UnsupportedAudioFileException"/>
        [NUnit.Framework.Test]
        public virtual void SoundTestSnd() {
            String filename = destinationFolder + "soundAnnotation04.pdf";
            String audioFile = sourceFolder + "sample.snd";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            PdfSoundAnnotation sound = new PdfSoundAnnotation(pdfDoc, new Rectangle(100, 100, 100, 100), @is, 44100, PdfName
                .Signed, 2, 16);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation04.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Sound.Sampled.UnsupportedAudioFileException"/>
        [NUnit.Framework.Test]
        public virtual void SoundTestWav() {
            String filename = destinationFolder + "soundAnnotation01.pdf";
            String audioFile = sourceFolder + "sample.wav";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            PdfSoundAnnotation sound = new PdfSoundAnnotation(pdfDoc, new Rectangle(100, 100, 100, 100), @is, 48000, PdfName
                .Signed, 2, 16);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Sound.Sampled.UnsupportedAudioFileException"/>
        [NUnit.Framework.Test]
        public virtual void SoundTestWav01() {
            String filename = destinationFolder + "soundAnnotation05.pdf";
            String audioFile = sourceFolder + "sample.wav";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            String header = "";
            for (int i = 0; i < 4; i++) {
                header = header + (char)@is.Read();
            }
            if (header.Equals("RIFF")) {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
                @is.Read();
            }
            else {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            }
            PdfStream soundStream = new PdfStream(pdfDoc, @is);
            soundStream.Put(PdfName.R, new PdfNumber(48000));
            soundStream.Put(PdfName.E, PdfName.Signed);
            soundStream.Put(PdfName.B, new PdfNumber(16));
            soundStream.Put(PdfName.C, new PdfNumber(2));
            PdfSoundAnnotation sound = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), soundStream);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation05.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ScreenTestExternalWavFile() {
            String filename = destinationFolder + "screenAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            FileStream fos = new FileStream(destinationFolder + "sample.wav", FileMode.Create);
            FileStream fis = new FileStream(sourceFolder + "sample.wav", FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[1024];
            int length;
            while ((length = fis.Read(buffer)) > 0) {
                fos.Write(buffer, 0, length);
            }
            fos.Close();
            fis.Close();
            PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav", true);
            PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_screenAnnotation01.pdf", 
                destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile01() {
            String filename = destinationFolder + "screenAnnotation02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, sourceFolder + "sample.wav", null, "sample.wav"
                , null, null, true);
            PdfAction action = PdfAction.CreateRendition(sourceFolder + "sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
        }

        //        CompareTool compareTool = new CompareTool();
        //        String errorMessage = compareTool.compareByContent(filename, sourceFolder + "cmp_screenAnnotation02.pdf", destinationFolder, "diff_");
        //        if (errorMessage != null) {
        //            Assert.fail(errorMessage);
        //        }
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile02() {
            String filename = destinationFolder + "screenAnnotation03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, new FileStream(sourceFolder + "sample.wav", 
                FileMode.Open, FileAccess.Read), null, "sample.wav", null, null, true);
            PdfAction action = PdfAction.CreateRendition(sourceFolder + "sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
        }

        //        CompareTool compareTool = new CompareTool();
        //        String errorMessage = compareTool.compareByContent(filename, sourceFolder + "cmp_screenAnnotation03.pdf", destinationFolder, "diff_");
        //        if (errorMessage != null) {
        //            Assert.fail(errorMessage);
        //        }
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile03() {
            String filename = destinationFolder + "screenAnnotation04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            Stream @is = new FileStream(sourceFolder + "sample.wav", FileMode.Open, FileAccess.Read);
            MemoryStream baos = new MemoryStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, baos.ToArray(), null, "sample.wav", null, null
                , null, true);
            PdfAction action = PdfAction.CreateRendition(sourceFolder + "sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
        }

        //        CompareTool compareTool = new CompareTool();
        //        String errorMessage = compareTool.compareByContent(filename, sourceFolder + "cmp_screenAnnotation04.pdf", destinationFolder, "diff_");
        //        if (errorMessage != null) {
        //            Assert.fail(errorMessage);
        //        }
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WaterMarkTest() {
            String filename = destinationFolder + "waterMarkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfWatermarkAnnotation watermark = new PdfWatermarkAnnotation(new Rectangle(400, 400, 200, 200));
            float[] arr = new float[] { 1, 0, 0, 1, 0, 0 };
            PdfFixedPrint fixedPrint = new PdfFixedPrint();
            fixedPrint.SetMatrix(arr);
            fixedPrint.SetHorizontalTranslation(0.5f);
            fixedPrint.SetVerticalTranslation(0);
            watermark.SetFixedPrint(fixedPrint);
            PdfFormXObject form = new PdfFormXObject(new Rectangle(200, 200));
            PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
            canvas.SaveState().Circle(100, 100, 50).SetColor(Color.BLACK, true).Fill().RestoreState();
            canvas.Release();
            watermark.SetNormalAppearance(form.GetPdfObject());
            watermark.SetFlags(PdfAnnotation.PRINT);
            page1.AddAnnotation(watermark);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_watermarkAnnotation01.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RedactionTest() {
            String filename = destinationFolder + "redactionAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            float[] rgb = new float[] { 0, 0, 0 };
            float[] rgb1 = new float[] { 1, 0, 0 };
            PdfRedactAnnotation redact = new PdfRedactAnnotation(new Rectangle(180, 531, 120, 49));
            PdfFormXObject formD = new PdfFormXObject(new Rectangle(180, 531, 120, 49));
            PdfCanvas canvasD = new PdfCanvas(formD, pdfDoc);
            canvasD.SetFillColorGray(0).Rectangle(180, 531, 120, 48).Fill();
            redact.SetDownAppearance(formD.GetPdfObject());
            PdfFormXObject formN = new PdfFormXObject(new Rectangle(179, 530, 122, 51));
            PdfCanvas canvasN = new PdfCanvas(formN, pdfDoc);
            canvasN.SetColor(Color.RED, true).SetLineWidth(1.5f).SetLineCapStyle(PdfCanvasConstants.LineCapStyle.PROJECTING_SQUARE
                ).Rectangle(180, 531, 120, 48).Stroke().Rectangle(181, 532, 118, 47).ClosePath();
            redact.SetNormalAppearance(formN.GetPdfObject());
            PdfFormXObject formR = new PdfFormXObject(new Rectangle(180, 531, 120, 49));
            PdfCanvas canvasR = new PdfCanvas(formR, pdfDoc);
            canvasR.SaveState().Rectangle(180, 531, 120, 48).Fill().RestoreState().Release();
            redact.SetRolloverAppearance(formR.GetPdfObject());
            PdfFormXObject formRO = new PdfFormXObject(new Rectangle(180, 531, 120, 49));
            PdfCanvas canvasRO = new PdfCanvas(formRO, pdfDoc);
            canvasRO.SaveState().Rectangle(180, 531, 120, 48).Fill().RestoreState().Release();
            redact.SetRedactRolloverAppearance(formRO.GetPdfObject());
            redact.Put(PdfName.OC, new PdfArray(rgb1));
            redact.SetColor(rgb1);
            redact.SetInteriorColor(rgb);
            page1.AddAnnotation(redact);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_redactionAnnotation01.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
