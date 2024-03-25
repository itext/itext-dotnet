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
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot.DA;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AddMiscTypesAnnotationsTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/annot/AddMiscTypesAnnotationsTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/annot/AddMiscTypesAnnotationsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AddTextAnnotation01() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "textAnnotation01.pdf"
                ));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "textAnnotation01.pdf"
                , SOURCE_FOLDER + "cmp_textAnnotation01.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddTextAnnotInTagged14PdfTest() {
            String outPdf = DESTINATION_FOLDER + "addTextAnnotInTagged14PdfTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_addTextAnnotInTagged14PdfTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf, new WriterProperties()
                .SetPdfVersion(PdfVersion.PDF_1_4)))) {
                pdfDoc.SetTagged();
                PdfPage page = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = new PdfTextAnnotation(new Rectangle(100, 600, 50, 40));
                annot.SetText(new PdfString("Text Annotation 01")).SetContents(new PdfString("Some contents..."));
                page.AddAnnotation(annot);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CaretTest() {
            String filename = DESTINATION_FOLDER + "caretAnnotation.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_CaretAnnotation.pdf", DESTINATION_FOLDER
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddFreeTextAnnotation01() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "freeTextAnnotation01.pdf"
                ));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "freeTextAnnotation01.pdf"
                , SOURCE_FOLDER + "cmp_freeTextAnnotation01.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddSquareAndCircleAnnotations01() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "squareAndCircleAnnotations01.pdf"
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "squareAndCircleAnnotations01.pdf"
                , SOURCE_FOLDER + "cmp_squareAndCircleAnnotations01.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FileAttachmentTest() {
            String filename = DESTINATION_FOLDER + "fileAttachmentAnnotation.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, SOURCE_FOLDER + "sample.wav", null, "sample.wav"
                , null, null);
            PdfFileAttachmentAnnotation fileAttach = new PdfFileAttachmentAnnotation(new Rectangle(100, 100), spec);
            fileAttach.SetIconName(PdfName.Paperclip);
            page1.AddAnnotation(fileAttach);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_fileAttachmentAnnotation.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FileAttachmentTargetTest() {
            String filename = DESTINATION_FOLDER + "fileAttachmentTargetTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, SOURCE_FOLDER + "sample.pdf", null, "embedded_doc.pdf"
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_fileAttachmentTargetTest.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EMBEDDED_GO_TO_DESTINATION_NOT_SPECIFIED)]
        public virtual void NoFileAttachmentTargetTest() {
            String fileName = "noFileAttachmentTargetTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + fileName));
            pdfDoc.AddNewPage();
            PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(400, 500, 50, 50));
            linkAnnotation.SetAction(PdfAction.CreateGoToE(null, true, null));
            pdfDoc.GetFirstPage().AddAnnotation(linkAnnotation);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(DESTINATION_FOLDER + fileName, SOURCE_FOLDER + "cmp_" +
                 fileName, DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <summary>see DEVSIX-1539</summary>
        [NUnit.Framework.Test]
        public virtual void FileAttachmentAppendModeTest() {
            String fileName = DESTINATION_FOLDER + "fileAttachmentAppendModeTest.pdf";
            MemoryStream baos = new MemoryStream();
            PdfDocument inputDoc = new PdfDocument(new PdfWriter(baos));
            PdfPage page1 = inputDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("This is a text").EndText().RestoreState();
            inputDoc.Close();
            PdfDocument finalDoc = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), CompareTool.CreateTestPdfWriter
                (fileName), new StampingProperties().UseAppendMode());
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(finalDoc, "Some test".GetBytes(), null, "test.txt", 
                null);
            finalDoc.AddFileAttachment("some_test", spec);
            finalDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, SOURCE_FOLDER + "cmp_fileAttachmentAppendModeTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void RubberStampTest() {
            String filename = DESTINATION_FOLDER + "rubberStampAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_rubberStampAnnotation01.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RubberStampWrongStampTest() {
            String filename = DESTINATION_FOLDER + "rubberStampAnnotation02.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
            stamp.SetStampName(PdfName.StrikeOut);
            page1.AddAnnotation(stamp);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_rubberStampAnnotation02.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.IsNull(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void InkTest() {
            String filename = DESTINATION_FOLDER + "inkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_inkAnnotation01.pdf", DESTINATION_FOLDER
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.IsNull(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PrinterMarkText() {
            String filename = DESTINATION_FOLDER + "printerMarkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_printerMarkAnnotation01.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TrapNetworkText() {
            String filename = DESTINATION_FOLDER + "trapNetworkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_trapNetworkAnnotation01.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WaterMarkTest() {
            String filename = DESTINATION_FOLDER + "watermarkAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_watermarkAnnotation01.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RedactionTest() {
            String filename = DESTINATION_FOLDER + "redactionAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
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
            String errorMessage = compareTool.CompareByContent(filename, SOURCE_FOLDER + "cmp_redactionAnnotation01.pdf"
                , DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAppearanceTest() {
            String name = "defaultAppearance";
            String inPath = SOURCE_FOLDER + "in_" + name + ".pdf";
            String outPath = DESTINATION_FOLDER + name + ".pdf";
            String cmpPath = SOURCE_FOLDER + "cmp_" + name + ".pdf";
            String diff = "diff_" + name + "_";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPath), CompareTool.CreateTestPdfWriter(outPath));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, DESTINATION_FOLDER, diff
                ));
        }

        [NUnit.Framework.Test]
        public virtual void Make3dAnnotationTest() {
            String filename = SOURCE_FOLDER + "3d_annotation.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfPage page1 = pdfDoc.GetPage(1);
            IList<PdfAnnotation> annots = page1.GetAnnotations();
            NUnit.Framework.Assert.IsTrue(annots[0] is Pdf3DAnnotation);
        }

        [NUnit.Framework.Test]
        public virtual void Add3dAnnotationTest() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "add3DAnnotation01.pdf"
                ));
            Rectangle rect = new Rectangle(100, 400, 400, 400);
            PdfStream stream3D = new PdfStream(pdfDoc, new FileStream(SOURCE_FOLDER + "teapot.u3d", FileMode.Open, FileAccess.Read
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "add3DAnnotation01.pdf"
                , SOURCE_FOLDER + "cmp_add3DAnnotation01.pdf", DESTINATION_FOLDER, "diff_"));
        }
    }
}
