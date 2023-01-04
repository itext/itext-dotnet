/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Annot.DA;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAnnotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfAnnotationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfAnnotationTest/";

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
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

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

        [NUnit.Framework.Test]
        public virtual void CaretTest() {
            String filename = destinationFolder + "caretAnnotation.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("This is a text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(236, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
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

        [NUnit.Framework.Test]
        public virtual void AddFreeTextAnnotation01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "freeTextAnnotation01.pdf"));
            PdfPage page = document.AddNewPage();
            new PdfCanvas(page).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24).MoveText
                (100, 600).ShowText("Annotated text").EndText().Release();
            PdfFreeTextAnnotation textannot = new PdfFreeTextAnnotation(new Rectangle(300, 700, 150, 20), new PdfString
                ("FreeText annotation"));
            textannot.SetDefaultAppearance(new AnnotationDefaultAppearance().SetFont(StandardAnnotationFont.TimesRoman
                ));
            textannot.SetColor(new float[] { 1, 0, 0 });
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

        [NUnit.Framework.Test]
        public virtual void FileAttachmentTest() {
            String filename = destinationFolder + "fileAttachmentAnnotation.pdf";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, sourceFolder + "sample.wav", null, "sample.wav"
                , null, null);
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

        [NUnit.Framework.Test]
        public virtual void FileAttachmentTargetTest() {
            String filename = destinationFolder + "fileAttachmentTargetTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, sourceFolder + "sample.pdf", null, "embedded_doc.pdf"
                , null, null);
            PdfFileAttachmentAnnotation fileAttachmentAnnotation = new PdfFileAttachmentAnnotation(new Rectangle(300, 
                500, 50, 50), spec);
            fileAttachmentAnnotation.SetName(new PdfString("FileAttachmentAnnotation1"));
            pdfDoc.AddNewPage();
            pdfDoc.AddNewPage().AddAnnotation(fileAttachmentAnnotation);
            PdfArray array = new PdfArray();
            array.Add(pdfDoc.GetPage(2).GetPdfObject());
            array.Add(PdfName.XYZ);
            array.Add(new PdfNumber(pdfDoc.GetPage(2).GetPageSize().GetLeft()));
            array.Add(new PdfNumber(pdfDoc.GetPage(2).GetPageSize().GetTop()));
            array.Add(new PdfNumber(1));
            pdfDoc.AddNamedDestination("FileAttachmentDestination1", array);
            PdfTarget target = PdfTarget.CreateChildTarget();
            target.GetPdfObject().Put(PdfName.P, new PdfString("FileAttachmentDestination1"));
            target.GetPdfObject().Put(PdfName.A, fileAttachmentAnnotation.GetName());
            // just test functionality to get annotation /* DEVSIX-1503 */
            target.GetAnnotation(pdfDoc);
            PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(400, 500, 50, 50));
            linkAnnotation.SetColor(ColorConstants.RED);
            linkAnnotation.SetAction(PdfAction.CreateGoToE(new PdfStringFS("Some fake destination"), new PdfNamedDestination
                ("prime"), true, target));
            pdfDoc.GetFirstPage().AddAnnotation(linkAnnotation);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_fileAttachmentTargetTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EMBEDDED_GO_TO_DESTINATION_NOT_SPECIFIED)]
        public virtual void NoFileAttachmentTargetTest() {
            String fileName = "noFileAttachmentTargetTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + fileName));
            pdfDoc.AddNewPage();
            PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(400, 500, 50, 50));
            linkAnnotation.SetAction(PdfAction.CreateGoToE(null, true, null));
            pdfDoc.GetFirstPage().AddAnnotation(linkAnnotation);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + fileName, sourceFolder + "cmp_" + fileName
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <summary>see DEVSIX-1539</summary>
        [NUnit.Framework.Test]
        public virtual void FileAttachmentAppendModeTest() {
            String fileName = destinationFolder + "fileAttachmentAppendModeTest.pdf";
            MemoryStream baos = new MemoryStream();
            PdfDocument inputDoc = new PdfDocument(new PdfWriter(baos));
            PdfPage page1 = inputDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("This is a text").EndText().RestoreState();
            inputDoc.Close();
            PdfDocument finalDoc = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), new PdfWriter(fileName
                ), new StampingProperties().UseAppendMode());
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(finalDoc, "Some test".GetBytes(), null, "test.txt", 
                null);
            finalDoc.AddFileAttachment("some_test", spec);
            finalDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, sourceFolder + "cmp_fileAttachmentAppendModeTest.pdf"
                , destinationFolder, "diff_"));
        }

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

        [NUnit.Framework.Test]
        public virtual void TextMarkupTest01() {
            String filename = destinationFolder + "textMarkupAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
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

        [NUnit.Framework.Test]
        public virtual void TextMarkupTest02() {
            String filename = destinationFolder + "textMarkupAnnotation02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
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

        [NUnit.Framework.Test]
        public virtual void TextMarkupTest03() {
            String filename = destinationFolder + "textMarkupAnnotation03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
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

        [NUnit.Framework.Test]
        public virtual void TextMarkupTest04() {
            String filename = destinationFolder + "textMarkupAnnotation04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
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

        [NUnit.Framework.Test]
        public virtual void PrinterMarkText() {
            String filename = destinationFolder + "printerMarkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvasText = new PdfCanvas(page1);
            canvasText.SaveState().BeginText().MoveText(36, 790).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts
                .HELVETICA), 16).ShowText("This is Printer Mark annotation:").EndText().RestoreState();
            PdfFormXObject form = new PdfFormXObject(PageSize.A4);
            PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
            canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
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

        [NUnit.Framework.Test]
        public virtual void TrapNetworkText() {
            String filename = destinationFolder + "trapNetworkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvasText = new PdfCanvas(page);
            canvasText.SaveState().BeginText().MoveText(36, 790).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts
                .HELVETICA), 16).ShowText("This is Trap Network annotation:").EndText().RestoreState();
            PdfFormXObject form = new PdfFormXObject(PageSize.A4);
            PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
            canvas.SaveState().Circle(272, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
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

        [NUnit.Framework.Test]
        public virtual void ScreenTestExternalWavFile() {
            String filename = destinationFolder + "screenAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            FileStream fos = new FileStream(destinationFolder + "sample.wav", FileMode.Create);
            FileStream fis = new FileStream(sourceFolder + "sample.wav", FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[1024];
            int length;
            while ((length = fis.Read(buffer)) > 0) {
                fos.Write(buffer, 0, length);
            }
            fos.Dispose();
            fis.Dispose();
            PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
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

        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile01() {
            String filename = destinationFolder + "screenAnnotation02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, sourceFolder + "sample.wav", null, "sample.wav"
                , null, null);
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
        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile02() {
            String filename = destinationFolder + "screenAnnotation03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, new FileStream(sourceFolder + "sample.wav", 
                FileMode.Open, FileAccess.Read), null, "sample.wav", null, null);
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
        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile03() {
            String filename = destinationFolder + "screenAnnotation04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
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
                , null);
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
        [NUnit.Framework.Test]
        public virtual void WaterMarkTest() {
            String filename = destinationFolder + "watermarkAnnotation01.pdf";
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
            canvas.SaveState().Circle(100, 100, 50).SetColor(ColorConstants.BLACK, true).Fill().RestoreState();
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
            canvasN.SetColor(ColorConstants.RED, true).SetLineWidth(1.5f).SetLineCapStyle(PdfCanvasConstants.LineCapStyle
                .PROJECTING_SQUARE).Rectangle(180, 531, 120, 48).Stroke().Rectangle(181, 532, 118, 47).ClosePath();
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

        [NUnit.Framework.Test]
        public virtual void DefaultAppearanceTest() {
            String name = "defaultAppearance";
            String inPath = sourceFolder + "in_" + name + ".pdf";
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = sourceFolder + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPath), new PdfWriter(outPath));
            PdfPage page = pdfDoc.GetPage(1);
            Rectangle rect = new Rectangle(20, 700, 250, 50);
            page.AddAnnotation(new PdfRedactAnnotation(rect).SetDefaultAppearance(new AnnotationDefaultAppearance().SetColor
                (new DeviceRgb(1.0f, 0, 0)).SetFont(StandardAnnotationFont.TimesBold).SetFontSize(20)).SetOverlayText(
                new PdfString("Redact RGB times-bold")));
            rect.MoveDown(80);
            page.AddAnnotation(new PdfRedactAnnotation(rect).SetDefaultAppearance(new AnnotationDefaultAppearance().SetColor
                (DeviceCmyk.MAGENTA).SetFont(StandardAnnotationFont.CourierOblique).SetFontSize(20)).SetOverlayText(new 
                PdfString("Redact CMYK courier-oblique")));
            rect.MoveDown(80);
            page.AddAnnotation(new PdfRedactAnnotation(rect).SetDefaultAppearance(new AnnotationDefaultAppearance().SetColor
                (DeviceGray.GRAY).SetFont(ExtendedAnnotationFont.HeiseiMinW3).SetFontSize(20)).SetOverlayText(new PdfString
                ("Redact Gray HeiseiMinW3")));
            rect.MoveUp(160).MoveRight(260);
            page.AddAnnotation(new PdfFreeTextAnnotation(rect, new PdfString("FreeText RGB times-bold")).SetDefaultAppearance
                (new AnnotationDefaultAppearance().SetColor(new DeviceRgb(1.0f, 0, 0)).SetFont(StandardAnnotationFont.
                TimesBold).SetFontSize(20)).SetColor(ColorConstants.WHITE));
            rect.MoveDown(80);
            page.AddAnnotation(new PdfFreeTextAnnotation(rect, new PdfString("FreeText CMYK courier-oblique")).SetDefaultAppearance
                (new AnnotationDefaultAppearance().SetColor(DeviceCmyk.MAGENTA).SetFont(StandardAnnotationFont.CourierOblique
                ).SetFontSize(20)).SetColor(ColorConstants.WHITE));
            rect.MoveDown(80);
            page.AddAnnotation(new PdfFreeTextAnnotation(rect, new PdfString("FreeText Gray HeiseiMinW3")).SetDefaultAppearance
                (new AnnotationDefaultAppearance().SetColor(DeviceGray.GRAY).SetFont(ExtendedAnnotationFont.HeiseiMinW3
                ).SetFontSize(20)).SetColor(ColorConstants.WHITE));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void Make3dAnnotationTest() {
            String filename = sourceFolder + "3d_annotation.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfPage page1 = pdfDoc.GetPage(1);
            IList<PdfAnnotation> annots = page1.GetAnnotations();
            NUnit.Framework.Assert.IsTrue(annots[0] is Pdf3DAnnotation);
        }

        [NUnit.Framework.Test]
        public virtual void Add3dAnnotationTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "add3DAnnotation01.pdf"));
            Rectangle rect = new Rectangle(100, 400, 400, 400);
            PdfStream stream3D = new PdfStream(pdfDoc, new FileStream(sourceFolder + "teapot.u3d", FileMode.Open, FileAccess.Read
                ));
            stream3D.Put(PdfName.Type, new PdfName("3D"));
            stream3D.Put(PdfName.Subtype, new PdfName("U3D"));
            stream3D.SetCompressionLevel(CompressionConstants.UNDEFINED_COMPRESSION);
            stream3D.Flush();
            PdfDictionary dict3D = new PdfDictionary();
            dict3D.Put(PdfName.Type, new PdfName("3DView"));
            dict3D.Put(new PdfName("XN"), new PdfString("Default"));
            dict3D.Put(new PdfName("IN"), new PdfString("Unnamed"));
            dict3D.Put(new PdfName("MS"), PdfName.M);
            dict3D.Put(new PdfName("C2W"), new PdfArray(new float[] { 1, 0, 0, 0, 0, -1, 0, 1, 0, 3, -235, 28 }));
            dict3D.Put(PdfName.CO, new PdfNumber(235));
            Pdf3DAnnotation annot = new Pdf3DAnnotation(rect, stream3D);
            annot.SetContents(new PdfString("3D Model"));
            annot.SetDefaultInitialView(dict3D);
            pdfDoc.AddNewPage().AddAnnotation(annot);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "add3DAnnotation01.pdf"
                , sourceFolder + "cmp_add3DAnnotation01.pdf", destinationFolder, "diff_"));
        }
    }
}
