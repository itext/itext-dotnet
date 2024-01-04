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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FloatTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FloatTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FloatTest/";

        private const String shortText = "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. ";

        private const String text = "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. "
             + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries. "
             + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. "
             + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign. "
             + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device. ";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FloatParagraphTest01() {
            String cmpFileName = sourceFolder + "cmp_floatParagraphTest01.pdf";
            String outFile = destinationFolder + "floatParagraphTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph();
            p.Add("paragraph1");
            p.SetWidth(70);
            p.SetHeight(100);
            p.SetBorder(new SolidBorder(1));
            p.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Paragraph p1 = new Paragraph();
            p1.Add("paragraph2");
            p1.SetWidth(70);
            p1.SetHeight(100);
            p1.SetBorder(new SolidBorder(1));
            p1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            doc.Add(p);
            doc.Add(p1);
            Paragraph p2 = new Paragraph();
            p2.Add("paragraph3");
            p2.SetBorder(new SolidBorder(1));
            doc.Add(p2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatParagraphTest02() {
            String cmpFileName = sourceFolder + "cmp_floatParagraphTest02.pdf";
            String outFile = destinationFolder + "floatParagraphTest02.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph();
            p.Add("paragraph1");
            p.SetWidth(70);
            p.SetHeight(100);
            p.SetBorder(new SolidBorder(1));
            Paragraph p1 = new Paragraph();
            p1.Add("paragraph2");
            p1.SetBorder(new SolidBorder(1));
            p.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            doc.Add(p);
            doc.Add(p1);
            Paragraph p2 = new Paragraph();
            p2.Add("paragraph3");
            p2.SetBorder(new SolidBorder(1));
            doc.Add(p2);
            doc.Add(p2);
            Paragraph p3 = new Paragraph("paragraph4aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                );
            p3.SetBorder(new SolidBorder(1));
            doc.Add(p3);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff02_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatDivTest01() {
            String cmpFileName = sourceFolder + "cmp_floatDivTest01.pdf";
            String outFile = destinationFolder + "floatDivTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetWidth(70);
            Paragraph p = new Paragraph();
            p.Add("div1");
            div.SetBorder(new SolidBorder(1));
            p.SetBorder(new SolidBorder(1));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.Add(p);
            doc.Add(div);
            doc.Add(new Paragraph("div2"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff03_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatDivTest02() {
            String cmpFileName = sourceFolder + "cmp_floatDivTest02.pdf";
            String outFile = destinationFolder + "floatDivTest02.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetMargin(0);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Paragraph p = new Paragraph();
            p.Add("More news");
            div.Add(p);
            doc.Add(div);
            div = new Div();
            div.SetMargin(0);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            p = new Paragraph();
            p.Add("Even more news");
            div.Add(p);
            doc.Add(div);
            Div coloredDiv = new Div();
            coloredDiv.SetMargin(0);
            coloredDiv.SetBackgroundColor(ColorConstants.RED);
            Paragraph p1 = new Paragraph();
            p1.Add("Some div");
            coloredDiv.Add(p1);
            doc.Add(coloredDiv);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff04_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatDivTest03() {
            String cmpFileName = sourceFolder + "cmp_floatDivTest03.pdf";
            String outFile = destinationFolder + "floatDivTest03.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetMargin(0);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.SetHeight(760);
            div.SetWidth(523);
            div.SetBorder(new SolidBorder(1));
            Paragraph p = new Paragraph();
            p.Add("More news");
            div.Add(p);
            doc.Add(div);
            div = new Div();
            div.SetMargin(0);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.SetBorder(new SolidBorder(1));
            p = new Paragraph();
            p.Add("Even more news");
            div.Add(p);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff05_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingImageInCell() {
            String cmpFileName = sourceFolder + "cmp_floatingImageInCell.pdf";
            String outFile = destinationFolder + "floatingImageInCell.pdf";
            String imageSrc = sourceFolder + "itis.jpg";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            iText.Layout.Element.Image img1 = new Image(ImageDataFactory.Create(imageSrc)).ScaleToFit(100, 100);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            img2.SetMarginRight(10);
            img2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 }));
            table.AddCell(new Cell().Add(img1));
            table.AddCell(new Cell().Add(img2).Add(new Paragraph("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.\n"
                 + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.\n"
                 + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.\n"
                 + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign.\n"
                 + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
                )));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff06_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingImageToNextPage() {
            String cmpFileName = sourceFolder + "cmp_floatingImageToNextPage.pdf";
            String outFile = destinationFolder + "floatingImageToNextPage.pdf";
            String imageSrc = sourceFolder + "itis.jpg";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            iText.Layout.Element.Image img1 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleAbsolute
                (100, 500);
            img1.SetMarginLeft(10);
            img1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            img2.SetMarginRight(10);
            img2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(img1);
            document.Add(new Paragraph(text));
            document.Add(new Paragraph(text));
            document.Add(img2);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff07_"));
        }

        [NUnit.Framework.Test]
        public virtual void InlineFloatingImageToNextPage() {
            String cmpFileName = sourceFolder + "cmp_inlineFloatingImageToNextPage.pdf";
            String outFile = destinationFolder + "inlineFloatingImageToNextPage.pdf";
            String imageSrc = sourceFolder + "itis.jpg";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)).SetTagged());
            iText.Layout.Element.Image img1 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleAbsolute
                (100, 500);
            img1.SetMarginLeft(10);
            img1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            img2.SetMarginRight(10);
            img2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(img1);
            document.Add(new Paragraph(text));
            document.Add(new Paragraph(text));
            Paragraph p = new Paragraph();
            p.Add(img2).Add(text);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff08_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingTwoImages() {
            String cmpFileName = sourceFolder + "cmp_floatingTwoImages.pdf";
            String outFile = destinationFolder + "floatingTwoImages.pdf";
            String imageSrc = sourceFolder + "itis.jpg";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            iText.Layout.Element.Image img1 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (400, 400);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (400, 400);
            img1.SetMarginRight(10);
            img1.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            img2.SetMarginRight(10);
            img2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(img1);
            document.Add(img2);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff09_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingTwoImagesLR() {
            String cmpFileName = sourceFolder + "cmp_floatingTwoImagesLR.pdf";
            String outFile = destinationFolder + "floatingTwoImagesLR.pdf";
            String imageSrc = sourceFolder + "itis.jpg";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            iText.Layout.Element.Image img1 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (350, 350);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (350, 350);
            img1.SetMarginLeft(10);
            img1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            img2.SetMarginRight(10);
            img2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(img1);
            document.Add(img2);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff10_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingImageInParagraph() {
            String cmpFileName = sourceFolder + "cmp_floatingImageInParagraph.pdf";
            String outFile = destinationFolder + "floatingImageInParagraph.pdf";
            String imageSrc = sourceFolder + "itis.jpg";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            // Image floats on the left inside the paragraph
            iText.Layout.Element.Image img1 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            img1.SetMarginRight(10);
            img1.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Paragraph p = new Paragraph();
            p.Add(img1).Add(text);
            document.Add(p);
            // Image floats on the right inside the paragraph
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            img2.SetMarginLeft(10);
            img2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            p = new Paragraph();
            p.Add(img2).Add(text);
            document.Add(p);
            // Paragraph containing image floats on the right inside the paragraph
            iText.Layout.Element.Image img3 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            img3.SetMarginLeft(10);
            p = new Paragraph();
            p.Add(img3);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(p);
            document.Add(new Paragraph(text));
            // Image floats on the left inside short paragraph
            iText.Layout.Element.Image img4 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            img4.SetMarginRight(10);
            img4.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            p = new Paragraph();
            p.Add(img4).Add("A little text.");
            document.Add(p);
            document.Add(new Paragraph(text));
            // Image floats on the left inside short paragraph
            iText.Layout.Element.Image img5 = new iText.Layout.Element.Image(ImageDataFactory.Create(imageSrc)).ScaleToFit
                (100, 100);
            img5.SetMarginRight(10);
            img5.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            p = new Paragraph();
            p.Add(img4).Add("A little text.");
            document.Add(p);
            p = new Paragraph(text);
            p.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff11_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnCanvas() {
            String cmpFileName = sourceFolder + "cmp_floatsOnCanvas.pdf";
            String outFile = destinationFolder + "floatsOnCanvas.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile)).SetTagged();
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas pdfCanvas = new PdfCanvas(page);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, page.GetPageSize().ApplyMargins(36, 36, 36
                , 36, false));
            canvas.EnableAutoTagging(page);
            Div div = new Div().SetBackgroundColor(ColorConstants.RED);
            Div fDiv = new Div().SetBackgroundColor(ColorConstants.BLUE).SetWidth(200).SetHeight(200);
            fDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Div fInnerDiv1 = new Div().SetWidth(50).SetHeight(50);
            fInnerDiv1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            fInnerDiv1.SetBackgroundColor(ColorConstants.YELLOW);
            Div fInnerDiv2 = new Div().SetWidth(50).SetHeight(50);
            fInnerDiv2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            fInnerDiv2.SetBackgroundColor(ColorConstants.CYAN);
            fDiv.Add(fInnerDiv1);
            fDiv.Add(fInnerDiv2);
            fDiv.Add(new Paragraph("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add"
                ));
            div.Add(fDiv).Add(new Paragraph("Hello"));
            canvas.Add(div);
            div = new Div().SetBackgroundColor(ColorConstants.GREEN);
            div.Add(new Paragraph("World"));
            canvas.Add(div);
            canvas.Add(div);
            canvas.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff12_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsFixedWidthTest01_floatRight() {
            String cmpFileName = sourceFolder + "cmp_floatsFixedWidthTest01_floatRight.pdf";
            String outFile = destinationFolder + "floatsFixedWidthTest01_floatRight.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div containerDiv = new Div().SetBorder(new SolidBorder(3)).SetPadding(10);
            Div parentFixedDiv = new Div().SetWidth(300).SetMarginLeft(150).SetBorder(new SolidBorder(ColorConstants.BLUE
                , 3));
            Div childFixedDiv = new Div().SetWidth(400).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
            childFixedDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            childFixedDiv.Add(new Paragraph("inside float; float right, width 400pt").SetMargin(0));
            parentFixedDiv.Add(new Paragraph("before float; width 300pt").SetMargin(0));
            parentFixedDiv.Add(childFixedDiv);
            parentFixedDiv.Add(new Paragraph("after float").SetMargin(0));
            containerDiv.Add(parentFixedDiv);
            Paragraph clearfix = new Paragraph("clearfix");
            clearfix.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            containerDiv.Add(clearfix);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_width01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsFixedWidth01_noFloat() {
            String cmpFileName = sourceFolder + "cmp_floatsFixedWidth01_noFloat.pdf";
            String outFile = destinationFolder + "floatsFixedWidth01_noFloat.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div containerDiv = new Div().SetBorder(new SolidBorder(3)).SetPadding(10);
            Div parentFixedDiv = new Div().SetWidth(300).SetMarginLeft(150).SetBorder(new SolidBorder(ColorConstants.BLUE
                , 3));
            Div childFixedDiv = new Div().SetWidth(400).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
            childFixedDiv.Add(new Paragraph("inside child; width 400pt").SetMargin(0));
            parentFixedDiv.Add(new Paragraph("before child; width 300pt").SetMargin(0));
            parentFixedDiv.Add(childFixedDiv);
            parentFixedDiv.Add(new Paragraph("after child").SetMargin(0));
            containerDiv.Add(parentFixedDiv);
            Paragraph clearfix = new Paragraph("clearfix");
            clearfix.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            containerDiv.Add(clearfix);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_width01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsFixedWidth01_floatLeft() {
            String cmpFileName = sourceFolder + "cmp_floatsFixedWidth01_floatLeft.pdf";
            String outFile = destinationFolder + "floatsFixedWidth01_floatLeft.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div containerDiv = new Div().SetBorder(new SolidBorder(3)).SetPadding(10);
            Div parentFixedDiv = new Div().SetWidth(300).SetMarginLeft(150).SetBorder(new SolidBorder(ColorConstants.BLUE
                , 3));
            Div childFixedDiv = new Div().SetWidth(400).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
            childFixedDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            childFixedDiv.Add(new Paragraph("inside float; float left; width 400pt").SetMargin(0));
            parentFixedDiv.Add(new Paragraph("before float; width 300pt").SetMargin(0));
            parentFixedDiv.Add(childFixedDiv);
            parentFixedDiv.Add(new Paragraph("after float").SetMargin(0));
            containerDiv.Add(parentFixedDiv);
            Paragraph clearfix = new Paragraph("clearfix");
            clearfix.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            containerDiv.Add(clearfix);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_width01_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 3)]
        public virtual void FloatFixedHeightContentNotFit() {
            String cmpFileName = sourceFolder + "cmp_floatFixedHeightContentNotFit.pdf";
            String outFile = destinationFolder + "floatFixedHeightContentNotFit.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)).SetTagged());
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div.")).Add(new Paragraph(text));
            div.SetHeight(200).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            document.Add(new Paragraph(text));
            Paragraph p = new Paragraph("Floating p.\n" + text).SetBorder(new SolidBorder(ColorConstants.RED, 2));
            p.SetHeight(200).SetWidth(100);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(p);
            document.Add(new Paragraph(text));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 0.3f, 0.7f })).SetBorder(new SolidBorder
                (ColorConstants.RED, 2));
            table.AddCell(new Paragraph("Floating table.")).AddCell(new Paragraph(text));
            table.SetHeight(200).SetWidth(300);
            table.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(table);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff13_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearanceFixedHeightPageSplitInRoot01() {
            String cmpFileName = sourceFolder + "cmp_clearanceFixedHeightPageSplitInRoot01.pdf";
            String outFile = destinationFolder + "clearanceFixedHeightPageSplitInRoot01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(200).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div.")).Add(new Paragraph(text));
            divClear.SetHeight(400);
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            document.Add(divClear);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff13_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatPartialInRoot01() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatPartialInRoot01.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatPartialInRoot01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)).SetTagged());
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared floating div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            divClear.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(divClear);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff14_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatPartialInRoot02() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatPartialInRoot02.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatPartialInRoot02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)).SetTagged());
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            document.Add(divClear);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff15_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatPartialInRoot03() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatPartialInRoot03.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatPartialInRoot03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)).SetTagged());
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared floating div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            divClear.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(divClear);
            Div div2 = new Div().SetBorder(new SolidBorder(ColorConstants.BLUE, 2));
            div2.Add(new Paragraph("Last float."));
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(div2);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff14_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatPartialInBlock01() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatPartialInBlock01.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatPartialInBlock01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            containerDiv.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared floating div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            divClear.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            // Float with clear shall be drawn under the previous float on second page.
            containerDiv.Add(divClear);
            // text shall start on the first page.
            containerDiv.Add(new Paragraph(text));
            document.Add(containerDiv);
            document.Add(new Paragraph(text));
            // TODO DEVSIX-1270: text around green cleared float is trying to wrap to the left of it (there are 2px of space)
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff23_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatPartialInBlock02() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatPartialInBlock02.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatPartialInBlock02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            containerDiv.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            containerDiv.Add(divClear);
            containerDiv.Add(new Paragraph(text));
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff24_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatPartialInBlock03() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatPartialInBlock03.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatPartialInBlock03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            containerDiv.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared floating div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            divClear.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            // Float with clear shall be drawn under the previous float on second page.
            containerDiv.Add(divClear);
            Div div2 = new Div().SetBorder(new SolidBorder(ColorConstants.BLUE, 2));
            div2.Add(new Paragraph("Last float."));
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            // This float top shall not appear higher than floats tops added before this one.
            containerDiv.Add(div2);
            // text shall start on the first page.
            containerDiv.Add(new Paragraph(text + text));
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff23_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ClearancePageSplitFloatNothingInRoot01() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatNothingInRoot01.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatNothingInRoot01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetHeight(400));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float at the end of the page, it doesn't fit and is to be forced placed.
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            // Adding cleared element which shall be after the previous float.
            document.Add(divClear);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff16_01_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatNothingInRoot02() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatNothingInRoot02.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatNothingInRoot02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetHeight(400).
                SetWidth(300));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float at the end of the page, it doesn't fit vertically.
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            // Adding cleared element which shall be after the previous float.
            document.Add(divClear);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff16_02_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatNothingInRoot03() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatNothingInRoot03.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatNothingInRoot03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetHeight(400).
                SetWidth(300));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float at the end of the page, it doesn't fit vertically.
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            divClear.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            // Adding cleared element which shall be after the previous float.
            document.Add(divClear);
            document.Add(new Paragraph(text + text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff16_02_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ClearancePageSplitFloatNothingInBlock01() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatNothingInBlock01.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatNothingInBlock01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetHeight(400));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float at the end of the page, it doesn't fit and is to be forced placed.
            containerDiv.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            // Adding cleared element which shall be after the previous float.
            containerDiv.Add(divClear);
            containerDiv.Add(new Paragraph(text));
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff25_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatNothingInBlock02() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatNothingInBlock02.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatNothingInBlock02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetHeight(400).
                SetWidth(300));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float at the end of the page, it doesn't fit vertically.
            containerDiv.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            // Adding cleared element which shall be after the previous float.
            containerDiv.Add(divClear);
            containerDiv.Add(new Paragraph(text));
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff25_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearancePageSplitFloatNothingInBlock03() {
            String cmpFileName = sourceFolder + "cmp_clearancePageSplitFloatNothingInBlock03.pdf";
            String outFile = destinationFolder + "clearancePageSplitFloatNothingInBlock03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetHeight(400).
                SetWidth(300));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float at the end of the page, it doesn't fit vertically.
            containerDiv.Add(div);
            Div divClear = new Div().SetBackgroundColor(ColorConstants.GREEN);
            divClear.Add(new Paragraph("Cleared div."));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            divClear.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            // Adding cleared element which shall be after the previous float.
            containerDiv.Add(divClear);
            containerDiv.Add(new Paragraph(text + text));
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff25_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearanceNoContentPageSplitFloatPartialInRoot01() {
            String cmpFileName = sourceFolder + "cmp_clearanceNoContentPageSplitFloatPartialInRoot01.pdf";
            String outFile = destinationFolder + "clearanceNoContentPageSplitFloatPartialInRoot01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float at the end of the page, it is split.
            document.Add(div);
            Div divClear = new Div();
            divClear.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            // Adding empty element with clearance - it shall be placed after the overflow part of the float.
            document.Add(divClear);
            document.Add(new Paragraph(text));
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff15_"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearanceNoContentPageSplitFloatPartialInBlock01() {
            String cmpFileName = sourceFolder + "cmp_clearanceNoContentPageSplitFloatPartialInBlock01.pdf";
            String outFile = destinationFolder + "clearanceNoContentPageSplitFloatPartialInBlock01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(400).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            containerDiv.Add(div);
            Div divClear = new Div();
            divClear.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            containerDiv.Add(divClear);
            containerDiv.Add(new Paragraph(text));
            containerDiv.Add(new Paragraph(text));
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff26_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void FloatsOnPageSplit01() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit01.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff17_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit02() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit02.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(200);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff18_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit03() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit03.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph(text).SetWidth(250));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400).SetWidth(250);
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff19_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsOnPageSplit04() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit04.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit04.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff20_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit05() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit05.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit05.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(280);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that doesn't fit on first page.
            document.Add(div);
            Div div2 = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div2.Add(new Paragraph(text)).SetWidth(300);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that shall be after the previous float.
            document.Add(div2);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff21_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit06_01() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit06_01.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit06_01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            // Setting fixed height for the div, that will be split between pages.
            div.SetHeight(600);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(280);
            img.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that will not fit on the first page.
            div.Add(img);
            div.Add(new Paragraph("some small text"));
            // div height shall be correct on the second page.
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff22_01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit06_02() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit06_02.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit06_02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            // Setting fixed height for the div, that will be split between pages.
            div.SetHeight(600);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(250);
            img.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that WILL fit on the first page.
            div.Add(img);
            div.Add(new Paragraph("some small text"));
            // div height shall be correct on the second page.
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff22_02"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsOnPageSplit06_03() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit06_03.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit06_03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            // Setting min height for the div, that will be split between pages.
            // Not setting max height, because float won't be forced placed because it doesn't fit in max height constraints.
            div.SetMinHeight(600);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400);
            img.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that will not fit on the first page and will have FORCED_PLACEMENT on the second.
            div.Add(img);
            div.Add(new Paragraph("some small text"));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff22_03"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit07() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit07.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit07.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(200);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that WILL fit on the first page.
            containerDiv.Add(div);
            // Adding img that shall be overflowed to the next page. containerDiv occupied area shall not have zero height on first page.
            containerDiv.Add(img);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff27_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit08_01() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit08_01.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit08_01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(310).SetWidth(310);
            // Adding image that will not fit on first page in floating div.
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            containerDiv.Add(div);
            // Adding normal image that will not fit on the first page.
            containerDiv.Add(img);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff28_01_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void FloatsOnPageSplit08_02() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit08_02.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit08_02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400);
            // Adding image that will not fit on first page in floating div.
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            containerDiv.Add(div);
            // Adding normal image that will not fit on the first page and requires forced placement.
            containerDiv.Add(img);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff28_02_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsOnPageSplit08_03() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit08_03.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit08_03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            containerDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(750).SetWidth(600);
            // Adding normal image that will not fit on the first page and requires forced placement.
            containerDiv.Add(img);
            // Adding more text that is naturally expected to be correctly shown.
            containerDiv.Add(new Paragraph(text));
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff28_03_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit09() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit09.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit09.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.Add(new Paragraph(text).SetWidth(250));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that will be split.
            containerDiv.Add(div);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400).SetWidth(250);
            // Adding image that will not fit on first page. containerDiv shall return PARTIAL status
            containerDiv.Add(img);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff29_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsOnPageSplit10() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit10.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit10.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            containerDiv.Add(div);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff30_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsOnPageSplit11() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit11.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit11.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div containerDiv = new Div();
            containerDiv.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that will not fit.
            containerDiv.Add(div);
            Div div2 = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div2.Add(new Paragraph(text)).SetWidth(300);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that shall be after the previous float. And shall overflow to the third page.
            containerDiv.Add(div2);
            document.Add(containerDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff31_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit12_01() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit12_01.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit12_01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400).SetWidth(100);
            img.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Div shall have height of 300pt.
            div.SetMinHeight(300).Add(img);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff32_01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit12_02() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit12_02.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit12_02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400).SetWidth(100);
            img.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Div shall have height of 500pt.
            div.SetHeight(500).Add(img);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff32_02_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit14() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit14.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit14.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            ImageData imgData = ImageDataFactory.Create(sourceFolder + "itis.jpg");
            iText.Layout.Element.Image img1 = new iText.Layout.Element.Image(imgData).SetHeight(200);
            div.Add(img1);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(imgData).SetHeight(200);
            img2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(img2);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff33_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit15() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit15.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit15.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)).SetTagged());
            Div mainDiv = new Div().SetBorder(new SolidBorder(ColorConstants.CYAN, 3));
            mainDiv.Add(new Paragraph(text + text));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(280);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that doesn't fit on first page.
            mainDiv.Add(div);
            Div div2 = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div2.Add(new Paragraph(text)).SetWidth(300);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that shall be after the previous float.
            mainDiv.Add(div2);
            document.Add(mainDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff34_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit16() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit16.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit16.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.CYAN, 3));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(280);
            div.Add(img);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that doesn't fit on first page.
            p.Add(div);
            Div div2 = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div2.Add(new Paragraph(text)).SetWidth(300);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            // Adding float that shall be after the previous float.
            p.Add(div2);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff34_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit17() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit17.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit17.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div div1 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.BLUE);
            Div div2 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.GREEN);
            Div div3 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.YELLOW);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div3.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div1);
            document.Add(div2);
            document.Add(div3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff35_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit18() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit18.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit18.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Div mainDiv = new Div();
            Div div1 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.BLUE);
            Div div2 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.GREEN);
            Div div3 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.YELLOW);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div3.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            mainDiv.Add(div1);
            mainDiv.Add(div2);
            mainDiv.Add(div3);
            document.Add(mainDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff36_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnPageSplit19() {
            String cmpFileName = sourceFolder + "cmp_floatsOnPageSplit19.pdf";
            String outFile = destinationFolder + "floatsOnPageSplit19.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph();
            Div div1 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.BLUE);
            Div div2 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.GREEN);
            Div div3 = new Div().SetWidth(100).SetHeight(500).SetBackgroundColor(ColorConstants.YELLOW);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div3.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            mainP.Add(div1);
            mainP.Add(div2);
            mainP.Add(div3);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff37_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsKeepTogetherOnPageSplit01() {
            String cmpFileName = sourceFolder + "cmp_floatsKeepTogetherOnPageSplit01.pdf";
            String outFile = destinationFolder + "floatsKeepTogetherOnPageSplit01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph floatP = new Paragraph(text + text).SetKeepTogether(true).SetWidth(300).SetBorder(new SolidBorder
                (ColorConstants.RED, 3));
            floatP.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(floatP);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff38_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsKeepTogetherOnPageSplit02() {
            String cmpFileName = sourceFolder + "cmp_floatsKeepTogetherOnPageSplit02.pdf";
            String outFile = destinationFolder + "floatsKeepTogetherOnPageSplit02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph("A bit of text."));
            Paragraph floatP = new Paragraph(text + text).SetKeepTogether(true).SetWidth(300).SetBorder(new SolidBorder
                (ColorConstants.RED, 3));
            floatP.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(floatP);
            for (int i = 0; i < 5; ++i) {
                document.Add(new Paragraph(text));
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff39_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void FloatsKeepTogetherOnPageSplit03() {
            String cmpFileName = sourceFolder + "cmp_floatsKeepTogetherOnPageSplit03.pdf";
            String outFile = destinationFolder + "floatsKeepTogetherOnPageSplit03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text));
            Div floatedKeptTogetherDiv = new Div().Add(new Paragraph(text + text)).SetBackgroundColor(ColorConstants.BLUE
                ).SetWidth(200).SetKeepTogether(true);
            floatedKeptTogetherDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(floatedKeptTogetherDiv);
            Div longKeptTogetherDiv = new Div().Add(new Paragraph(text + text + text + text + text + text)).SetKeepTogether
                (true);
            document.Add(longKeptTogetherDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff39_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit01() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit01.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph();
            Div div = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph(text));
            mainP.Add(div);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff40_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit02() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit02.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph();
            Div div0 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            Div div1 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            div0.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div0.Add(new Paragraph(text));
            mainP.Add(div0);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div1.Add(new Paragraph(text));
            mainP.Add(div1);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff41_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit03() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit03.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph();
            Div div0 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            Div div1 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            div0.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div0.Add(new Paragraph(text));
            mainP.Add(div0);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div1.Add(new Paragraph(text));
            mainP.Add(div1);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff42_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit04() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit04.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit04.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph();
            Div div0 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            Div div1 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            div0.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div0.Add(new Paragraph(text));
            mainP.Add(div0);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div1.Add(new Paragraph(text));
            mainP.Add(div1);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff43_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit05() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit05.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit05.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph();
            Div div0 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            div0.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div0.Add(new Paragraph(text));
            mainP.Add(div0);
            mainP.Add(text);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff44_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit06() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit06.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit06.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph();
            Div div0 = new Div().SetWidth(220).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            Div div1 = new Div().SetWidth(220).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            Div div2 = new Div().SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            div0.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div0.Add(new Paragraph(text));
            div1.Add(new Paragraph(text));
            div2.Add(new Paragraph(text));
            mainP.Add(div0);
            mainP.Add(div1);
            mainP.Add(new Text("Small text.").SetFontColor(ColorConstants.LIGHT_GRAY));
            mainP.Add(div2);
            mainP.Add(text);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff45_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit07() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit07.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit07.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph();
            Div div0 = new Div().SetWidth(200).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            Div div1 = new Div().SetWidth(200).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            Div div2 = new Div().SetWidth(70).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            div0.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div0.Add(new Paragraph(text));
            div1.Add(new Paragraph(text));
            div2.Add(new Paragraph(text));
            mainP.Add(div0);
            mainP.Add(div1);
            mainP.Add(new Text("Small text.").SetFontColor(ColorConstants.LIGHT_GRAY));
            mainP.Add(div2);
            mainP.Add(text);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff46_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsInParagraphPartialSplit08() {
            String cmpFileName = sourceFolder + "cmp_floatsInParagraphPartialSplit08.pdf";
            String outFile = destinationFolder + "floatsInParagraphPartialSplit08.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph();
            Div div0 = new Div().SetWidth(200).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            Div div1 = new Div().SetWidth(200).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            Div div2 = new Div().SetWidth(70).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            div0.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div0.Add(new Paragraph(text));
            div1.Add(new Paragraph(text));
            div2.Add(new Paragraph(text));
            mainP.Add(div0);
            mainP.Add(div1);
            mainP.Add(div2);
            mainP.Add(text);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff47_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingTextInParagraphPartialSplit01() {
            String cmpFileName = sourceFolder + "cmp_floatingTextInParagraphPartialSplit01.pdf";
            String outFile = destinationFolder + "floatingTextInParagraphPartialSplit01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph().SetBorder(new SolidBorder(ColorConstants.BLUE, 1.5f));
            Text floatText = new Text(text).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            floatText.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            mainP.Add(floatText);
            mainP.Add(text);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff51_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingTextInParagraphPartialSplit02() {
            String cmpFileName = sourceFolder + "cmp_floatingTextInParagraphPartialSplit02.pdf";
            String outFile = destinationFolder + "floatingTextInParagraphPartialSplit02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph().SetBorder(new SolidBorder(ColorConstants.BLUE, 1.5f));
            Div div1 = new Div().SetWidth(220).SetBorder(new SolidBorder(ColorConstants.DARK_GRAY, 2.8f)).SetBorderBottom
                (new SolidBorder(ColorConstants.DARK_GRAY, 1f)).SetFontColor(ColorConstants.DARK_GRAY);
            Div div2 = new Div().SetWidth(220).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 2.8f)).SetBorderBottom
                (new SolidBorder(ColorConstants.LIGHT_GRAY, 1f)).SetFontColor(ColorConstants.LIGHT_GRAY);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div1.Add(new Paragraph(text));
            div2.Add(new Paragraph(text));
            mainP.Add(div1);
            mainP.Add(div2);
            mainP.Add("Text. ");
            Text floatText = new Text(text).SetBorder(new SolidBorder(ColorConstants.RED, 3));
            floatText.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            mainP.Add(floatText);
            mainP.Add(text);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff52_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatingTextInParagraphPartialSplit03() {
            String cmpFileName = sourceFolder + "cmp_floatingTextInParagraphPartialSplit03.pdf";
            String outFile = destinationFolder + "floatingTextInParagraphPartialSplit03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph().SetBorder(new SolidBorder(ColorConstants.BLUE, 1.5f));
            Div div1 = new Div().SetWidth(190).SetBorder(new SolidBorder(ColorConstants.DARK_GRAY, 3)).SetFontColor(ColorConstants
                .DARK_GRAY);
            Div div2 = new Div().SetWidth(190).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 3)).SetFontColor(ColorConstants
                .LIGHT_GRAY);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div1.Add(new Paragraph(text));
            div2.Add(new Paragraph(text));
            mainP.Add(div1);
            mainP.Add(div2);
            Text floatText = new Text("A little bit of text.").SetBorder(new SolidBorder(ColorConstants.RED, 3));
            floatText.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            mainP.Add(floatText);
            mainP.Add(text);
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff53"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsFirstOnPageNotFit01() {
            String cmpFileName = sourceFolder + "cmp_floatsFirstOnPageNotFit01.pdf";
            String outFile = destinationFolder + "floatsFirstOnPageNotFit01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph mainP = new Paragraph();
            Div div = new Div().SetWidth(150).SetBorder(new SolidBorder(ColorConstants.BLUE, 3)).SetKeepTogether(true);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.Add(new Paragraph(text).SetFontColor(ColorConstants.LIGHT_GRAY));
            mainP.Add(div);
            mainP.Add(text);
            document.Add(mainP);
            document.Add(new Paragraph(text + text).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 3)).SetFontColor
                (ColorConstants.RED));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff48_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsFirstOnPageNotFit02() {
            String cmpFileName = sourceFolder + "cmp_floatsFirstOnPageNotFit02.pdf";
            String outFile = destinationFolder + "floatsFirstOnPageNotFit02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div mainDiv = new Div();
            Div div = new Div().SetWidth(150).SetBorder(new SolidBorder(ColorConstants.BLUE, 3)).SetKeepTogether(true);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.Add(new Paragraph(text).SetFontColor(ColorConstants.LIGHT_GRAY));
            mainDiv.Add(div);
            mainDiv.Add(new Paragraph(text).SetMargin(0));
            document.Add(mainDiv);
            document.Add(new Paragraph(text + text).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 3)).SetFontColor
                (ColorConstants.RED));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff49_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FloatsFirstOnPageNotFit03() {
            String cmpFileName = sourceFolder + "cmp_floatsFirstOnPageNotFit03.pdf";
            String outFile = destinationFolder + "floatsFirstOnPageNotFit03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetWidth(150).SetBorder(new SolidBorder(ColorConstants.BLUE, 3)).SetKeepTogether(true);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.Add(new Paragraph(text).SetFontColor(ColorConstants.LIGHT_GRAY));
            document.Add(div);
            document.Add(new Paragraph(text).SetMargin(0));
            document.Add(new Paragraph(text + text).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 3)).SetFontColor
                (ColorConstants.RED));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatPartialSplitBigGapAtPageEnd01() {
            String cmpFileName = sourceFolder + "cmp_floatPartialSplitBigGapAtPageEnd01.pdf";
            String outFile = destinationFolder + "floatPartialSplitBigGapAtPageEnd01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetWidth(350).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            // specifying fill available area option
            div.SetFillAvailableAreaOnSplit(true);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.Add(new Paragraph(text).SetFontColor(ColorConstants.LIGHT_GRAY));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetWidth(345).SetHeight
                (500));
            div.Add(new Paragraph(text).SetFontColor(ColorConstants.LIGHT_GRAY));
            document.Add(div);
            document.Add(new Paragraph(text + text + text).SetMargin(0));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff54_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatPartialSplitBigGapAtPageEnd02() {
            String cmpFileName = sourceFolder + "cmp_floatPartialSplitBigGapAtPageEnd02.pdf";
            String outFile = destinationFolder + "floatPartialSplitBigGapAtPageEnd02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetWidth(350).SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
            // specifying fill available area option
            div.SetFillAvailableAreaOnSplit(true);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.Add(new Paragraph(text).SetFontColor(ColorConstants.LIGHT_GRAY));
            div.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg")).SetWidth(345).SetHeight
                (500));
            div.Add(new Paragraph(text).SetFontColor(ColorConstants.LIGHT_GRAY));
            document.Add(div);
            document.Add(new Paragraph(text).SetMargin(0));
            Div wideFloatingDiv = new Div().Add(new Paragraph(text)).SetWidth(450).SetBorder(new SolidBorder(ColorConstants
                .RED, 3));
            wideFloatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            document.Add(wideFloatingDiv);
            document.Add(new Paragraph(text + text).SetMargin(0));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff55_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatInParagraphLastLineLeadingOverflow01() {
            String cmpFileName = sourceFolder + "cmp_floatInParagraphLastLineLeadingOverflow01.pdf";
            String outFile = destinationFolder + "floatInParagraphLastLineLeadingOverflow01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text).SetMargin(0).SetMultipliedLeading(1.3f));
            Paragraph p = new Paragraph().SetFontColor(ColorConstants.RED).SetFixedLeading(20f);
            p.Add("First line of red paragraph.\n");
            ImageData img = ImageDataFactory.Create(sourceFolder + "itis.jpg");
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(img);
            image.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            p.Add(image.SetHeight(730).SetWidth(300));
            p.Add(text);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff56_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatOverflowNothingInParagraph01() {
            String cmpFileName = sourceFolder + "cmp_floatOverflowNothingInParagraph01.pdf";
            String outFile = destinationFolder + "floatOverflowNothingInParagraph01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph p = new Paragraph().SetFontColor(ColorConstants.RED);
            ImageData img = ImageDataFactory.Create(sourceFolder + "itis.jpg");
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(img);
            image.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            p.Add(image.SetHeight(400).SetWidth(300));
            p.Add("Some text goes here. ");
            Div div1 = new Div().SetBorder(new SolidBorder(ColorConstants.BLUE, 3)).Add(new Paragraph("Floating div text"
                ));
            Div div2 = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 3)).Add(new Paragraph("Floating div text"
                ));
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            p.Add(div1);
            p.Add(div2);
            p.Add(text);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff57_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatOverflowNothingInParagraph02() {
            String cmpFileName = sourceFolder + "cmp_floatOverflowNothingInParagraph02.pdf";
            String outFile = destinationFolder + "floatOverflowNothingInParagraph02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph p = new Paragraph().SetFontColor(ColorConstants.RED);
            ImageData img = ImageDataFactory.Create(sourceFolder + "itis.jpg");
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(img);
            image.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            p.Add(image.SetHeight(400).SetWidth(300));
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff58_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatInlineBlockTest01() {
            String cmpFileName = sourceFolder + "cmp_floatInlineBlockTest01.pdf";
            String outFile = destinationFolder + "floatInlineBlockTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(1));
            p.Add("Float with large borders shall fit on first line with this text. ");
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 40));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph("Floating div."));
            p.Add(div);
            p.Add("Inline block with large borders floating. Inline block with large borders floating. " + "Inline block with large borders floating. Inline block with large borders floating."
                );
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff14_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsHeightFixedInBlock01() {
            String cmpFileName = sourceFolder + "cmp_floatsHeightFixedInBlock01.pdf";
            String outFile = destinationFolder + "floatsHeightFixedInBlock01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2)).SetHeight(100);
            Paragraph p = new Paragraph(text);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_height_01_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsHeightFixedInBlock02() {
            String cmpFileName = sourceFolder + "cmp_floatsHeightFixedInBlock02.pdf";
            String outFile = destinationFolder + "floatsHeightFixedInBlock02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text.JSubstring(0, text.Length / 2) + "."));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2)).SetHeight(200);
            Paragraph p = new Paragraph(text);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_height_02_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsHeightFixedInParagraph01() {
            String cmpFileName = sourceFolder + "cmp_floatsHeightFixedInParagraph01.pdf";
            String outFile = destinationFolder + "floatsHeightFixedInParagraph01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph parentParagraph = new Paragraph().SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetHeight
                (100);
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph(text));
            parentParagraph.Add(div);
            document.Add(parentParagraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_height_03_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsHeightFixedInParagraph02() {
            String cmpFileName = sourceFolder + "cmp_floatsHeightFixedInParagraph02.pdf";
            String outFile = destinationFolder + "floatsHeightFixedInParagraph02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text.JSubstring(0, text.Length / 2) + "."));
            Paragraph parentParagraph = new Paragraph().SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetHeight
                (200);
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph(text));
            parentParagraph.Add(div);
            document.Add(parentParagraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_height_04_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsMaxHeightFixedInBlock01() {
            String cmpFileName = sourceFolder + "cmp_floatsMaxHeightFixedInBlock01.pdf";
            String outFile = destinationFolder + "floatsMaxHeightFixedInBlock01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2)).SetMaxHeight(100);
            Paragraph p = new Paragraph(text);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_maxheight_01_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsMaxHeightFixedInBlock02() {
            String cmpFileName = sourceFolder + "cmp_floatsMaxHeightFixedInBlock02.pdf";
            String outFile = destinationFolder + "floatsMaxHeightFixedInBlock02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text.JSubstring(0, text.Length / 2) + "."));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2)).SetMaxHeight(200);
            Paragraph p = new Paragraph(text);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_maxheight_02_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsMaxHeightFixedInParagraph01() {
            String cmpFileName = sourceFolder + "cmp_floatsMaxHeightFixedInParagraph01.pdf";
            String outFile = destinationFolder + "floatsMaxHeightFixedInParagraph01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph parentParagraph = new Paragraph().SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetMaxHeight
                (100);
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph(text));
            parentParagraph.Add(div);
            document.Add(parentParagraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_maxheight_03_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsMaxHeightFixedInParagraph02() {
            String cmpFileName = sourceFolder + "cmp_floatsMaxHeightFixedInParagraph02.pdf";
            String outFile = destinationFolder + "floatsMaxHeightFixedInParagraph02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text.JSubstring(0, text.Length / 2) + "."));
            Paragraph parentParagraph = new Paragraph().SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetMaxHeight
                (200);
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph(text));
            parentParagraph.Add(div);
            document.Add(parentParagraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_maxheight_04_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsMinHeightFixedInBlock01() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightFixedInBlock01.pdf";
            String outFile = destinationFolder + "floatsMinHeightFixedInBlock01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2)).SetMinHeight(100);
            Paragraph p = new Paragraph(text);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheight_01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsMinHeightFixedInBlock02() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightFixedInBlock02.pdf";
            String outFile = destinationFolder + "floatsMinHeightFixedInBlock02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text.JSubstring(0, text.Length / 2) + "."));
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2)).SetMinHeight(200);
            Paragraph p = new Paragraph(text);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheight_02_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsMinHeightFixedInParagraph01() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightFixedInParagraph01.pdf";
            String outFile = destinationFolder + "floatsMinHeightFixedInParagraph01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph parentParagraph = new Paragraph().SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetMinHeight
                (100);
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph(text));
            parentParagraph.Add(div);
            document.Add(parentParagraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheight_03_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsMinHeightFixedInParagraph02() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightFixedInParagraph02.pdf";
            String outFile = destinationFolder + "floatsMinHeightFixedInParagraph02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text.JSubstring(0, text.Length / 2) + "."));
            Paragraph parentParagraph = new Paragraph().SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetMinHeight
                (200);
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(new Paragraph(text));
            parentParagraph.Add(div);
            document.Add(parentParagraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheight_04_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsMinHeightApplyingOnSplitTest01() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightApplyingOnSplitTest01.pdf";
            String outFile = destinationFolder + "floatsMinHeightApplyingOnSplitTest01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text));
            // Gray area in this test is expected to be not split, continuous and have height equal
            // exactly to mainDiv min height property value. Floating elements shall not affect
            // occupied area of parent and also there is no proper way to split it.
            Div mainDiv = new Div();
            mainDiv.SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetMinHeight(400);
            mainDiv.Add(new Paragraph(text));
            AddFloatingElements(mainDiv);
            document.Add(mainDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheightapplying_01_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsMinHeightApplyingOnSplitTest02() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightApplyingOnSplitTest02.pdf";
            String outFile = destinationFolder + "floatsMinHeightApplyingOnSplitTest02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text));
            // Gray area in this test is expected to be not split, continuous and have height equal
            // exactly to mainDiv min height property value. Floating elements shall not affect
            // occupied area of parent and also there is no proper way to split it.
            // Floats on the second page are expected to be clipped, due to max_height constraints.
            Div mainDiv = new Div();
            mainDiv.SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetMinHeight(400).SetMaxHeight(750);
            mainDiv.Add(new Paragraph(text));
            AddFloatingElements(mainDiv);
            document.Add(mainDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheightapplying_02_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void FloatsMinHeightApplyingOnSplitTest03() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightApplyingOnSplitTest03.pdf";
            String outFile = destinationFolder + "floatsMinHeightApplyingOnSplitTest03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text));
            // Gray area in this test is expected to be split, however also not to have a gap before page end.
            // Min height shall be resolved exactly the way as it would be resolved if no floats were there.
            // The place at which floats are clipped on the second page shall be the same as in previous two tests.
            Div mainDiv = new Div();
            mainDiv.SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetMinHeight(750).SetMaxHeight(750);
            mainDiv.Add(new Paragraph(text));
            AddFloatingElements(mainDiv);
            document.Add(mainDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheightapplying_03_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void FloatsMinHeightApplyingOnSplitTest04() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightApplyingOnSplitTest04.pdf";
            String outFile = destinationFolder + "floatsMinHeightApplyingOnSplitTest04.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div mainDiv = new Div();
            mainDiv.SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetMaxHeight(750);
            mainDiv.Add(new Paragraph(text));
            AddFloatingElements(mainDiv);
            // Both additions of mainDiv to document are the same except the second one also contains a bit of non-floating
            // content in it. Places at which floating elements are clipped due to max_height shall be the same in both cases.
            // The place at which they are clipped shall also be the same with tests floatsMinHeightApplyingOnSplitTest01-03.
            // first addition
            document.Add(new Paragraph(text));
            document.Add(mainDiv);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            // second addition
            document.Add(new Paragraph(text));
            int textLen = 100;
            mainDiv.Add(new Paragraph(text.Length > textLen ? text.JSubstring(0, textLen) : text));
            document.Add(mainDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheightapplying_04_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void FloatsMinHeightApplyingOnSplitTest05() {
            String cmpFileName = sourceFolder + "cmp_floatsMinHeightApplyingOnSplitTest05.pdf";
            String outFile = destinationFolder + "floatsMinHeightApplyingOnSplitTest05.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            // Since mainDiv is floating here, it encompasses all the floating children in it's occupied area.
            // In this case, behaviour is expected to be the same as with just normal content and min_height property:
            // height is not extended to all available height on first page in order not to "spend" height and ultimately
            // to have more space to show content constrained by max_height.
            Div mainDiv = new Div();
            mainDiv.SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetMinHeight(500).SetMaxHeight(750);
            mainDiv.Add(new Paragraph(text));
            mainDiv.SetWidth(UnitValue.CreatePercentValue(100)).SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            AddFloatingElements(mainDiv);
            // Places where floating elements are clipped shall be the same in both additions. However these places are
            // different from previous tests because floats are included in parent's occupied area here.
            // first addition
            document.Add(new Paragraph(text));
            document.Add(mainDiv);
            // TODO DEVSIX-1819: floats break area-break logic if min_height doesn't overflow to the next page on first addition: SMALL TICKET
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            // adding two page breaks two work around the issue
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            // second addition
            mainDiv.SetMinHeight(50);
            document.Add(new Paragraph(text));
            document.Add(mainDiv);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_minheightapplying_05_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsFixedMaxHeightAndOverflowHidden01() {
            String cmpFileName = sourceFolder + "cmp_floatsFixedMaxHeightAndOverflowHidden01.pdf";
            String outFile = destinationFolder + "floatsFixedMaxHeightAndOverflowHidden01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text + text.JSubstring(0, text.Length / 2) + "."));
            Paragraph parentParagraph = new Paragraph().SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetMaxHeight
                (200);
            Div div = new Div().SetBorder(new SolidBorder(ColorConstants.RED, 2));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph p = new Paragraph(text);
            div.Add(p);
            parentParagraph.Add(div);
            parentParagraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.HIDDEN);
            parentParagraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            div.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            div.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            p.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            document.Add(parentParagraph);
            document.Close();
            // TODO DEVSIX-1818: overflow value HIDDEN doesn't clip floats because they are drawn later in different part of content stream.
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_maxheighthidden_01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOverflowToNextLineAtPageEndInParagraph01() {
            String cmpFileName = sourceFolder + "cmp_floatsOverflowToNextLineAtPageEndInParagraph01.pdf";
            String outFile = destinationFolder + "floatsOverflowToNextLineAtPageEndInParagraph01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph().SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new SolidBorder(
                3)).SetMargin(0).SetFontSize(30).SetMinHeight(240);
            int textLen = 180;
            mainP.Add(text.Length > textLen ? text.JSubstring(0, textLen) : text);
            Div floatingDiv = new Div().SetBackgroundColor(ColorConstants.YELLOW);
            floatingDiv.Add(new Paragraph("Floating div contents.").SetMargin(0));
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            mainP.Add(floatingDiv);
            // On second page there shall be no parent paragraph artifacts: background and borders,
            // since min_height completely fits on first page.
            // Only floating element is overflown to the next page.
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_overflowNextLineAtPageEnd_01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOverflowToNextLineAtPageEndInParagraph02() {
            String cmpFileName = sourceFolder + "cmp_floatsOverflowToNextLineAtPageEndInParagraph02.pdf";
            String outFile = destinationFolder + "floatsOverflowToNextLineAtPageEndInParagraph02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph().SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new SolidBorder(
                3)).SetMargin(0);
            mainP.Add(text.JSubstring(0, (int)(text.Length * 0.8)));
            Text floatingText = new Text(text.JSubstring(0, text.Length / 3)).SetBorder(new SolidBorder(ColorConstants
                .CYAN, 3));
            floatingText.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            mainP.Add(floatingText);
            // Since it's floats-only split, min_height is expected to be applied fully on the first page (it fits there)
            // and also no parent artifacts (borders, background) shall be drawn on second page.
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_overflowNextLineAtPageEnd_02_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOverflowToNextLineAtPageEndInParagraph03() {
            String cmpFileName = sourceFolder + "cmp_floatsOverflowToNextLineAtPageEndInParagraph03.pdf";
            String outFile = destinationFolder + "floatsOverflowToNextLineAtPageEndInParagraph03.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text + text));
            Paragraph mainP = new Paragraph().SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new SolidBorder(
                3)).SetMargin(0).SetMinHeight(240);
            mainP.Add(text.JSubstring(0, (int)(text.Length * 0.6)));
            Text floatingText = new Text(text.JSubstring(0, text.Length / 8)).SetBorder(new SolidBorder(ColorConstants
                .CYAN, 3)).SetFontSize(40);
            floatingText.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            mainP.Add(floatingText);
            // Since it's floats-only split, min_height is expected to be applied fully on the first page (it fits there)
            // and also no parent artifacts (borders, background) shall be drawn on second page.
            document.Add(mainP);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_overflowNextLineAtPageEnd_03_"));
        }

        private void AddFloatingElements(Div mainDiv) {
            Div yellow = new Div().SetBackgroundColor(ColorConstants.YELLOW).SetHeight(150).SetWidth(UnitValue.CreatePercentValue
                (40)).SetMargin(5);
            Div green = new Div().SetBackgroundColor(ColorConstants.GREEN).SetHeight(150).SetWidth(UnitValue.CreatePercentValue
                (40)).SetMargin(5);
            Div blue = new Div().SetBackgroundColor(ColorConstants.BLUE).SetHeight(150).SetWidth(UnitValue.CreatePercentValue
                (90)).SetMargin(5);
            Div orange = new Div().SetBackgroundColor(ColorConstants.ORANGE).SetWidth(UnitValue.CreatePercentValue(40)
                ).SetMargin(5);
            Div cyan = new Div().SetBackgroundColor(ColorConstants.CYAN).SetWidth(UnitValue.CreatePercentValue(40)).SetMargin
                (5);
            yellow.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            green.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            blue.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            orange.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            cyan.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            blue.SetKeepTogether(true);
            orange.Add(new Paragraph(text + text));
            cyan.Add(new Paragraph(text + text));
            mainDiv.Add(yellow);
            mainDiv.Add(green);
            mainDiv.Add(blue);
            mainDiv.Add(orange);
            mainDiv.Add(cyan);
        }

        /// <summary>Suggested by Richard Cohn.</summary>
        [NUnit.Framework.Test]
        public virtual void FloatRootElementNotFitPage01() {
            String cmpFileName = sourceFolder + "cmp_floatRootElementNotFitPage01.pdf";
            String outFile = destinationFolder + "floatRootElementNotFitPage01.pdf";
            //Initialize PDF writer
            PdfWriter writer = new PdfWriter(outFile);
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(writer);
            pdf.SetDefaultPageSize(new PageSize(600, 350));
            pdf.SetTagged();
            // Initialize document
            Document document = new Document(pdf);
            // Document layout is correct if COLLAPSING_MARGINS is not true
            document.SetProperty(Property.COLLAPSING_MARGINS, true);
            document.Add(new Paragraph("Some text\nSome text\nSome text\nSome text\nSome text\nSome text"));
            byte[] data = new byte[1];
            ImageData raw = ImageDataFactory.Create(1, 1, 1, 8, data, null);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(raw).SetHeight(200);
            Div div = new Div();
            div.Add(image);
            Div captionDiv = new Div();
            captionDiv.Add(new Paragraph("Caption line 1\n").Add("line 2"));
            div.Add(captionDiv);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            //div.setKeepTogether(true);
            document.Add(div);
            document.Add(new Paragraph("After float"));
            document.Add(new List(ListNumberingType.DECIMAL).Add("Some text\nSome text\nSome text\nSome text").Add("Some text\nSome text\nSome text"
                ).Add("Some text\nSome text").Add("Some text\nSome text"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff15_"));
        }

        /// <summary>Suggested by Richard Cohn.</summary>
        [NUnit.Framework.Test]
        public virtual void FloatRootElementNotFitPage02() {
            String cmpFileName = sourceFolder + "cmp_floatRootElementNotFitPage02.pdf";
            String outFile = destinationFolder + "floatRootElementNotFitPage02.pdf";
            //Initialize PDF writer
            PdfWriter writer = new PdfWriter(outFile);
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(writer);
            pdf.SetDefaultPageSize(new PageSize(600, 350));
            pdf.SetTagged();
            // Initialize document
            Document document = new Document(pdf);
            // Document layout is correct if COLLAPSING_MARGINS is not true
            document.SetProperty(Property.COLLAPSING_MARGINS, true);
            document.Add(new Paragraph("Some text\nSome text\nSome text\nSome text\nSome text\nSome text\nSome text"));
            byte[] data = new byte[1];
            ImageData raw = ImageDataFactory.Create(1, 1, 1, 8, data, null);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(raw).SetHeight(200);
            Div div = new Div();
            div.Add(image);
            Div captionDiv = new Div();
            captionDiv.Add(new Paragraph("Caption line 1\n").Add("line 2"));
            div.Add(captionDiv);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.SetKeepTogether(true);
            //document.add(div);
            div = new Div();
            image = new iText.Layout.Element.Image(raw).SetHeight(200);
            div.Add(image);
            div.Add(captionDiv);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            div.SetKeepTogether(true);
            document.Add(div);
            document.Add(new Paragraph("After float").SetKeepWithNext(true));
            document.Add(new List(ListNumberingType.DECIMAL).Add("List text\nList text\nList text\nList text").Add("List text\nList text\nList text"
                ).Add("List text\nList text").Add("List text\nList text"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff16_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatOverflowAlongWithNewContent01() {
            String cmpFileName = sourceFolder + "cmp_floatOverflowAlongWithNewContent01.pdf";
            String outFile = destinationFolder + "floatOverflowAlongWithNewContent01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div divContainer = new Div();
            divContainer.SetMargin(20);
            divContainer.SetBorder(new SolidBorder(ColorConstants.BLACK, 10));
            divContainer.Add(new Paragraph(text + text));
            Paragraph pFloat = new Paragraph(text).SetFontColor(ColorConstants.RED).SetWidth(300).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            pFloat.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            divContainer.Add(pFloat);
            document.Add(divContainer);
            document.Add(new Paragraph(text + text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_overflowNewContent01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatOverflowAlongWithNewContent02() {
            String cmpFileName = sourceFolder + "cmp_floatOverflowAlongWithNewContent02.pdf";
            String outFile = destinationFolder + "floatOverflowAlongWithNewContent02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div divContainer = new Div();
            divContainer.SetMargin(20);
            divContainer.SetBorder(new SolidBorder(ColorConstants.BLACK, 10));
            divContainer.Add(new Paragraph(text + text));
            Paragraph pFloat = new Paragraph(text + text + text).SetFontColor(ColorConstants.RED).SetWidth(300).SetBackgroundColor
                (ColorConstants.LIGHT_GRAY);
            pFloat.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            divContainer.Add(pFloat);
            document.Add(divContainer);
            document.Add(new Paragraph(text + text + text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_overflowNewContent02_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void FloatTableTest01() {
            String cmpFileName = sourceFolder + "cmp_floatTableTest01.pdf";
            String outFile = destinationFolder + "floatTableTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetWidth(38);
            Div floatDiv = new Div();
            floatDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Table table = new Table(2);
            for (int i = 0; i < 26; i++) {
                table.AddCell(new Cell().Add(new Paragraph("abba a")));
                table.AddCell(new Cell().Add(new Paragraph("ab ab ab")));
            }
            floatDiv.Add(table);
            div.Add(floatDiv);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff03_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherEnoughSpaceOnNewPageWithFloatTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherEnoughSpaceOnNewPageWithFloatTest.pdf";
            String outFile = destinationFolder + "keepTogetherEnoughSpaceOnNewPageWithFloatTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            FillWithKeptTogetherElement(document, text, 2, false, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherNotEnoughSpaceOnNewPageWithFloatEnoughOnEmptyTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherNotEnoughSpaceOnNewPageWithFloatEnoughOnEmptyTest.pdf";
            String outFile = destinationFolder + "keepTogetherNotEnoughSpaceOnNewPageWithFloatEnoughOnEmptyTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            FillWithKeptTogetherElement(document, text, 3, false, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void KeepTogetherNotEnoughSpaceOnNewEmptyPageTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherNotEnoughSpaceOnNewEmptyPageTest.pdf";
            String outFile = destinationFolder + "keepTogetherNotEnoughSpaceOnNewEmptyPageTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            FillWithKeptTogetherElement(document, text, 4, false, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void KeepTogetherNotEnoughSpaceOnNewEmptyPageShortFloatTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherNotEnoughSpaceOnNewEmptyPageShortFloatTest.pdf";
            String outFile = destinationFolder + "keepTogetherNotEnoughSpaceOnNewEmptyPageShortFloatTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            FillWithKeptTogetherElement(document, "Some short text", 4, false, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void InnerKeepTogetherEnoughSpaceOnNewPageWithFloatTest() {
            String cmpFileName = sourceFolder + "cmp_innerKeepTogetherEnoughSpaceOnNewPageWithFloatTest.pdf";
            String outFile = destinationFolder + "innerKeepTogetherEnoughSpaceOnNewPageWithFloatTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            FillWithKeptTogetherElement(document, text, 2, true, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void InnerKeepTogetherNotEnoughSpaceOnNewPageWithFloatEnoughOnEmptyTest() {
            String cmpFileName = sourceFolder + "cmp_innerKeepTogetherNotEnoughSpaceOnNewPageWithFloatEnoughOnEmptyTest.pdf";
            String outFile = destinationFolder + "innerKeepTogetherNotEnoughSpaceOnNewPageWithFloatEnoughOnEmptyTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            FillWithKeptTogetherElement(document, text, 3, true, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void InnerKeepTogetherNotEnoughSpaceOnNewEmptyPageTest() {
            String cmpFileName = sourceFolder + "cmp_innerKeepTogetherNotEnoughSpaceOnNewEmptyPageTest.pdf";
            String outFile = destinationFolder + "innerKeepTogetherNotEnoughSpaceOnNewEmptyPageTest.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            FillWithKeptTogetherElement(document, text, 4, true, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff50_"));
        }

        [NUnit.Framework.Test]
        public virtual void IndentInParagraphAndFloatInInnerDivTest() {
            String outFile = destinationFolder + "indentInParagraphAndFloatInInnerDiv.pdf";
            String cmpFileName = sourceFolder + "cmp_indentInParagraphAndFloatInInnerDiv.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().Add(new Paragraph("Video provides a powerful way to help you prove" + " your point. When you click Online Video, you can"
                ));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.SetBackgroundColor(ColorConstants.YELLOW);
            Paragraph p = new Paragraph();
            p.SetFirstLineIndent(50);
            p.Add(div);
            p.Add(text);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void FloatAndIndentInFirstParagraphInDivTest() {
            String outFile = destinationFolder + "floatAndIndentInFirstParagraphInDiv.pdf";
            String cmpFileName = sourceFolder + "cmp_floatAndIndentInFirstParagraphInDiv.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph shortFloat = new Paragraph("Hello, iText! Hello, iText!").SetBackgroundColor(ColorConstants.CYAN
                );
            shortFloat.SetFirstLineIndent(50).SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph p = new Paragraph(text).SetBackgroundColor(ColorConstants.YELLOW);
            Div div = new Div();
            div.Add(shortFloat);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void ShortFloatRightAndIndentInSecondParagraphInDivTest() {
            String outFile = destinationFolder + "shortFloatRightAndIndentInSecondParagraphInDiv.pdf";
            String cmpFileName = sourceFolder + "cmp_shortFloatRightAndIndentInSecondParagraphInDiv.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph shortFloat = new Paragraph("Hello, iText! Hello, iText!").SetBackgroundColor(ColorConstants.CYAN
                );
            shortFloat.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph p = new Paragraph(text).SetFirstLineIndent(50).SetBackgroundColor(ColorConstants.YELLOW);
            Div div = new Div();
            div.Add(shortFloat);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void ShortFloatLeftAndIndentInSecondParagraphInDivTest() {
            String outFile = destinationFolder + "shortFloatLeftAndIndentInSecondParagraphInDiv.pdf";
            String cmpFileName = sourceFolder + "cmp_shortFloatLeftAndIndentInSecondParagraphInDiv.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph shortFloat = new Paragraph("Hello, iText! Hello, iText!").SetBackgroundColor(ColorConstants.CYAN
                );
            shortFloat.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Paragraph p = new Paragraph(text).SetFirstLineIndent(50).SetBackgroundColor(ColorConstants.YELLOW);
            Div div = new Div();
            div.Add(shortFloat);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void LongFloatAndIndentInSecondParagraphInDivTest() {
            String outFile = destinationFolder + "longFloatAndIndentInSecondParagraphInDiv.pdf";
            String cmpFileName = sourceFolder + "cmp_longFloatAndIndentInSecondParagraphInDiv.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph longFloat = new Paragraph(text).SetBackgroundColor(ColorConstants.CYAN);
            longFloat.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph p = new Paragraph(text).SetFirstLineIndent(50).SetBackgroundColor(ColorConstants.YELLOW);
            Div div = new Div();
            div.Add(longFloat);
            div.Add(p);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void IndentInParentParagraphShortFirstFloatTest() {
            String outFile = destinationFolder + "indentInParentParagraphShortFirstFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_indentInParentParagraphShortFirstFloat.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph parent = new Paragraph(text).SetFirstLineIndent(50);
            Paragraph shortFloat = new Paragraph(shortText).SetBackgroundColor(ColorConstants.CYAN);
            shortFloat.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parent.Add(shortFloat);
            Paragraph p = new Paragraph(text).SetBackgroundColor(ColorConstants.YELLOW);
            parent.Add(p);
            document.Add(parent);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void IndentInParentParagraphLongFirstFloatTest() {
            String outFile = destinationFolder + "indentInParentParagraphLongFirstFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_indentInParentParagraphLongFirstFloat.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Paragraph parent = new Paragraph(text).SetFirstLineIndent(50);
            Paragraph longFloat = new Paragraph(text).SetBackgroundColor(ColorConstants.CYAN);
            longFloat.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parent.Add(longFloat);
            Paragraph p = new Paragraph(text).SetBackgroundColor(ColorConstants.YELLOW);
            parent.Add(p);
            document.Add(parent);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
        }

        private static void FillWithKeptTogetherElement(Document doc, String floatText, int textTimes, bool isInner
            , bool floatAsFirst) {
            Div floatedDiv = new Div().SetWidth(150).SetBorder(new SolidBorder(ColorConstants.BLUE, 3)).SetKeepTogether
                (true);
            floatedDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            floatedDiv.Add(new Paragraph(floatText).SetFontColor(ColorConstants.LIGHT_GRAY));
            Paragraph keptTogetherParagraph = new Paragraph().SetKeepTogether(true);
            for (int i = 0; i < textTimes; i++) {
                keptTogetherParagraph.Add(text);
            }
            if (isInner) {
                Div container = new Div();
                container.Add(floatedDiv);
                if (!floatAsFirst) {
                    container.Add(new Paragraph("Hello"));
                    container.Add(new Paragraph("Hello"));
                    container.Add(new Paragraph("Hello"));
                    container.Add(new Paragraph("Hello"));
                }
                container.Add(keptTogetherParagraph);
                doc.Add(container);
            }
            else {
                doc.Add(floatedDiv);
                if (!floatAsFirst) {
                    doc.Add(new Paragraph("Hello"));
                    doc.Add(new Paragraph("Hello"));
                    doc.Add(new Paragraph("Hello"));
                    doc.Add(new Paragraph("Hello"));
                }
                doc.Add(keptTogetherParagraph);
            }
        }
    }
}
