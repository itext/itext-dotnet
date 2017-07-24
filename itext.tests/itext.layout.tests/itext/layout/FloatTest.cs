/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class FloatTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FloatTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FloatTest/";

        private const String text = "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. "
             + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries. "
             + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. "
             + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign. "
             + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device. ";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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
            coloredDiv.SetBackgroundColor(Color.RED);
            Paragraph p1 = new Paragraph();
            p1.Add("Some div");
            coloredDiv.Add(p1);
            doc.Add(coloredDiv);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff04_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("block level floating elements page-overflow and splitting not supported yet")]
        public virtual void FloatDivTest03() {
            //
            // TODO probably we shouldn't review forced placement applying on floated elements
            // May be check if there are any floated elements already on page
            //
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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
            table.AddCell(new Cell().Add(img2).Add("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.\n"
                 + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.\n"
                 + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.\n"
                 + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign.\n"
                 + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
                ));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff06_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("block level floating elements page-overflow and splitting not supported yet")]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void InlineFloatingImageToNextPage() {
            String cmpFileName = sourceFolder + "cmp_inlineFloatingImageToNextPage.pdf";
            String outFile = destinationFolder + "inlineFloatingImageToNextPage.pdf";
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
            Paragraph p = new Paragraph();
            p.Add(img2).Add(text);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff08_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FloatsOnCanvas() {
            String cmpFileName = sourceFolder + "cmp_floatsOnCanvas.pdf";
            String outFile = destinationFolder + "floatsOnCanvas.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas pdfCanvas = new PdfCanvas(page);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, pdfDoc, page.GetPageSize().ApplyMargins<Rectangle
                >(36, 36, 36, 36, false));
            Div div = new Div().SetBackgroundColor(Color.RED);
            Div fDiv = new Div().SetBackgroundColor(Color.BLUE).SetWidth(200).SetHeight(200);
            fDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            Div fInnerDiv1 = new Div().SetWidth(50).SetHeight(50);
            fInnerDiv1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            fInnerDiv1.SetBackgroundColor(Color.YELLOW);
            Div fInnerDiv2 = new Div().SetWidth(50).SetHeight(50);
            fInnerDiv2.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            fInnerDiv2.SetBackgroundColor(Color.CYAN);
            fDiv.Add(fInnerDiv1);
            fDiv.Add(fInnerDiv2);
            fDiv.Add(new Paragraph("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add"
                ));
            div.Add(fDiv).Add(new Paragraph("Hello"));
            canvas.Add(div);
            div = new Div().SetBackgroundColor(Color.GREEN);
            div.Add(new Paragraph("World"));
            canvas.Add(div);
            canvas.Add(div);
            canvas.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff12_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CLIP_ELEMENT, Count = 3)]
        public virtual void FloatFixedHeightContentNotFit() {
            String cmpFileName = sourceFolder + "cmp_floatFixedHeightContentNotFit.pdf";
            String outFile = destinationFolder + "floatFixedHeightContentNotFit.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            Div div = new Div().SetBorder(new SolidBorder(Color.RED, 2));
            div.Add(new Paragraph("Floating div.")).Add(new Paragraph(text));
            div.SetHeight(200).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            document.Add(new Paragraph(text));
            Paragraph p = new Paragraph("Floating p.\n" + text).SetBorder(new SolidBorder(Color.RED, 2));
            p.SetHeight(200).SetWidth(100);
            p.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(p);
            document.Add(new Paragraph(text));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 0.3f, 0.7f })).SetBorder(new SolidBorder
                (Color.RED, 2));
            table.AddCell(new Paragraph("Floating table.")).AddCell(new Paragraph(text));
            table.SetHeight(200).SetWidth(300);
            table.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(table);
            document.Add(new Paragraph(text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff13_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ClearanceApplyingPageSplit() {
            String cmpFileName = sourceFolder + "cmp_clearanceApplyingPageSplit.pdf";
            String outFile = destinationFolder + "clearanceApplyingPageSplit.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph(text));
            Div div = new Div().SetBorder(new SolidBorder(Color.RED, 2));
            div.Add(new Paragraph("Floating div."));
            div.SetHeight(200).SetWidth(100);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            document.Add(div);
            Div divClear = new Div().SetBackgroundColor(Color.GREEN);
            divClear.Add(new Paragraph("Cleared div.")).Add(new Paragraph(text));
            divClear.SetHeight(400);
            divClear.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            document.Add(divClear);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff13_"));
        }
    }
}
