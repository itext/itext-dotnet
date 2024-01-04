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
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImageTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ImageTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ImageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest01() {
            String outFileName = destinationFolder + "imageTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            doc.Add(new Paragraph(new Text("First Line")));
            Paragraph p = new Paragraph();
            p.Add(image);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Second Line")));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest02() {
            String outFileName = destinationFolder + "imageTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest02.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(sourceFolder + "Desert.jpg"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            Paragraph p = new Paragraph();
            p.Add(new Text("before image"));
            p.Add(image);
            p.Add(new Text("after image"));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest03() {
            String outFileName = destinationFolder + "imageTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest03.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            doc.Add(new Paragraph(new Text("First Line")));
            Paragraph p = new Paragraph();
            p.Add(image);
            image.SetRotationAngle(Math.PI / 6);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Second Line")));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest04() {
            String outFileName = destinationFolder + "imageTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest04.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            Paragraph p = new Paragraph();
            p.Add(new Text("before image"));
            p.Add(image);
            image.SetRotationAngle(Math.PI / 6);
            p.Add(new Text("after image"));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest05() {
            String outFileName = destinationFolder + "imageTest05.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest05.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            doc.Add(new Paragraph(new Text("First Line")));
            Paragraph p = new Paragraph();
            p.Add(image);
            image.Scale(1, 0.5f);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Second Line")));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest06() {
            String outFileName = destinationFolder + "imageTest06.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest06.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            doc.Add(new Paragraph(new Text("First Line")));
            Paragraph p = new Paragraph();
            p.Add(image);
            image.SetMarginLeft(100).SetMarginTop(100);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Second Line")));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageTest07() {
            String outFileName = destinationFolder + "imageTest07.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest07.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            Div div = new Div();
            div.Add(image);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void ImageTest08() {
            String outFileName = destinationFolder + "imageTest08.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest08.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            Div div = new Div();
            div.Add(image);
            div.Add(image);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageTest09() {
            String outFileName = destinationFolder + "imageTest09.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest09.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc, new PageSize(500, 300));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetWidth(UnitValue.CreatePercentValue(100));
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest10() {
            String outFileName = destinationFolder + "imageTest10.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest10.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc, new PageSize(500, 300));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetAutoScale(true);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest11() {
            String outFileName = destinationFolder + "imageTest11.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest11.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetAutoScale(true);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest12_HorizontalAlignment_CENTER() {
            String outFileName = destinationFolder + "imageTest12.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest12.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                ));
            image.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest13_HorizontalAlignment_RIGHT() {
            String outFileName = destinationFolder + "imageTest13.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest13.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                ));
            image.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest14_HorizontalAlignment_LEFT() {
            String outFileName = destinationFolder + "imageTest14.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest14.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                ));
            image.SetHorizontalAlignment(HorizontalAlignment.LEFT);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageTest15() {
            String outFileName = destinationFolder + "imageTest15.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest15.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetBorder(new SolidBorder(ColorConstants.BLUE, 5));
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest16() {
            String outFileName = destinationFolder + "imageTest16.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest16.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetBorder(new SolidBorder(ColorConstants.BLUE, 5));
            image.SetAutoScale(true);
            image.SetRotationAngle(Math.PI / 2);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 50)]
        public virtual void ImageTest17() {
            String outFileName = destinationFolder + "imageTest17.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest17.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image1 = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + 
                "Desert.jpg"));
            image1.SetBorder(new SolidBorder(ColorConstants.BLUE, 5));
            iText.Layout.Element.Image image2 = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + 
                "scarf.jpg"));
            image2.SetBorder(new SolidBorder(ColorConstants.BLUE, 5));
            for (int i = 0; i <= 24; i++) {
                image1.SetRotationAngle(i * Math.PI / 12);
                image2.SetRotationAngle(i * Math.PI / 12);
                doc.Add(image1);
                doc.Add(image2);
            }
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest18() {
            String outFileName = destinationFolder + "imageTest18.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest18.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetAutoScale(true);
            Div container = new Div();
            container.SetBorder(new SolidBorder(1f));
            container.SetWidth(UnitValue.CreatePercentValue(50f));
            container.SetHeight(UnitValue.CreatePointValue(300f));
            container.Add(image);
            doc.Add(container);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        //TODO(DEVSIX-1659)
        [NUnit.Framework.Test]
        public virtual void ImageTest19() {
            String outFileName = destinationFolder + "imageTest19.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest19.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetAutoScaleHeight(true);
            Div container = new Div();
            container.SetBorder(new SolidBorder(1f));
            container.SetWidth(UnitValue.CreatePercentValue(50f));
            container.SetHeight(UnitValue.CreatePointValue(300f));
            container.Add(image);
            doc.Add(container);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest20() {
            String outFileName = destinationFolder + "imageTest20.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest20.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetAutoScaleWidth(true);
            Div container = new Div();
            container.SetBorder(new SolidBorder(1f));
            container.SetWidth(UnitValue.CreatePercentValue(60f));
            container.SetHeight(UnitValue.CreatePointValue(300f));
            container.Add(image);
            doc.Add(container);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        //TODO(DEVSIX-1659)
        [NUnit.Framework.Test]
        public virtual void ImageTest21() {
            String outFileName = destinationFolder + "imageTest21.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest21.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetAutoScaleHeight(true);
            float[] colWidths = new float[] { 1f, 1f };
            Table container = new Table(UnitValue.CreatePercentArray(colWidths));
            container.AddCell("Text");
            container.AddCell("autoscaling image, height only");
            int textIterations = 50;
            Paragraph p = new Paragraph();
            for (int i = 0; i < textIterations; i++) {
                p.Add("Text will wrap");
            }
            container.AddCell(p);
            container.AddCell(image);
            doc.Add(container);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void ImageTest22() {
            String cmpFileName = sourceFolder + "cmp_imageTest22.pdf";
            String outFile = destinationFolder + "imageTest22.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFile)));
            document.Add(new Paragraph("Very small paragraph with text."));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(400);
            // Image doesn't fit horizontally, so it's force placed.
            // However even though based on code, image should also be autoscaled to fit the available area,
            // current forced placement autoscaling implementation results in ignoring fixed dimensions in this case.
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest23() {
            String outFileName = destinationFolder + "imageTest23.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest23.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "encoded_tiff.tiff"
                ));
            image.ScaleToFit(500, 500);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <summary>Image can be reused in layout, so flushing it on the very first draw is a bad thing.</summary>
        [NUnit.Framework.Test]
        public virtual void FlushOnDrawTest() {
            String outFileName = destinationFolder + "flushOnDrawTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flushOnDrawTest.pdf";
            int rowCount = 60;
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            Table table = new Table(UnitValue.CreatePercentArray(8)).UseAllAvailableWidth();
            for (int k = 0; k < rowCount; k++) {
                for (int j = 0; j < 7; j++) {
                    table.AddCell("Hello");
                }
                Cell c = new Cell().Add(img.SetWidth(UnitValue.CreatePercentValue(50)));
                table.AddCell(c);
            }
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <summary>
        /// If an image is flushed automatically on draw, we will later check it for circular references
        /// as it is an XObject.
        /// </summary>
        /// <remarks>
        /// If an image is flushed automatically on draw, we will later check it for circular references
        /// as it is an XObject. This is a test for
        /// <see cref="System.NullReferenceException"/>
        /// that was caused by getting
        /// a value from flushed image.
        /// </remarks>
        [NUnit.Framework.Test]
        public virtual void FlushOnDrawCheckCircularReferencesTest() {
            String outFileName = destinationFolder + "flushOnDrawCheckCircularReferencesTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flushOnDrawCheckCircularReferencesTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            //Initialize document
            Document document = new Document(pdf);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                ));
            img.SetAutoScale(true);
            Table table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();
            for (int k = 0; k < 5; k++) {
                table.AddCell("Hello World from iText");
                List list = new List().SetListSymbol("-> ");
                list.Add("list item").Add("list item").Add("list item").Add("list item").Add("list item");
                Cell cell = new Cell().Add(list);
                table.AddCell(cell);
                Cell c = new Cell().Add(img);
                table.AddCell(c);
                Table innerTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
                int j = 0;
                while (j < 9) {
                    innerTable.AddCell("Hi");
                    j++;
                }
                table.AddCell(innerTable);
            }
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithBordersSurroundedByTextTest() {
            String outFileName = destinationFolder + "imageBordersTextTest.pdf";
            String cmpFileName = sourceFolder + "cmp_imageBordersTextTest.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            Paragraph p = new Paragraph();
            p.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            p.Add(new Text("before image"));
            p.Add(image);
            image.SetBorder(new SolidBorder(ColorConstants.BLUE, 5));
            p.Add(new Text("after image"));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageInParagraphBorderTest() {
            String outFileName = destinationFolder + "imageParagraphBorderTest.pdf";
            String cmpFileName = sourceFolder + "cmp_imageParagraphBorderTest.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            Paragraph p = new Paragraph();
            p.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            p.Add(image);
            image.SetBorder(new SolidBorder(ColorConstants.BLUE, 5));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        //TODO(DEVSIX-1022)
        [NUnit.Framework.Test]
        public virtual void ImageRelativePositionTest() {
            String outFileName = destinationFolder + "imageRelativePositionTest.pdf";
            String cmpFileName = sourceFolder + "cmp_imageRelativePositionTest.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100).SetRelativePosition(30, 30
                , 0, 0);
            Paragraph p = new Paragraph();
            p.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            p.Add(image);
            image.SetBorder(new SolidBorder(ColorConstants.BLUE, 5));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void ImageInTableTest01() {
            String outFileName = destinationFolder + "imageInTableTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageInTableTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.SetMaxHeight(300);
            table.SetBorder(new SolidBorder(ColorConstants.BLUE, 10));
            Cell c = new Cell().Add(img.SetHeight(500));
            table.AddCell(c);
            document.Add(table);
            document.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Is my occupied area right?"
                ));
            document.Add(new AreaBreak());
            table.SetMinHeight(150);
            document.Add(table);
            document.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Is my occupied area right?"
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void ImageInTableTest02() {
            String outFileName = destinationFolder + "imageInTableTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_imageInTableTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.SetMaxHeight(300);
            table.SetBorder(new SolidBorder(ColorConstants.BLUE, 10));
            Cell c = new Cell().Add(img.SetHeight(500));
            table.AddCell("First cell");
            table.AddCell(c);
            document.Add(table);
            document.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Is my occupied area right?"
                ));
            document.Add(new AreaBreak());
            table.SetMinHeight(150);
            document.Add(table);
            document.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Is my occupied area right?"
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        //TODO(DEVSIX-1045)
        [NUnit.Framework.Test]
        public virtual void FixedPositionImageTest01() {
            String outFileName = destinationFolder + "fixedPositionImageTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_fixedPositionImageTest01.pdf";
            String imgPath = sourceFolder + "Desert.jpg";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            document.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(imgPath), 12, pdfDoc.GetDefaultPageSize
                ().GetHeight() - 36, 24).SetBorder(new SolidBorder(ColorConstants.RED, 5)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithMinMaxHeightTest01() {
            String outFileName = destinationFolder + "imageWithMinMaxHeightTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageWithMinMaxHeightTest01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            doc.Add(new Paragraph(new Text("Default height")));
            doc.Add(image);
            doc.Add(new Paragraph(new Text("Min height bigger than default")));
            doc.Add(image.SetMinHeight(200));
            doc.Add(new Paragraph(new Text("Min height smaller than default")));
            image.DeleteOwnProperty(Property.MIN_HEIGHT);
            doc.Add(image.SetMinHeight(10));
            doc.Add(new Paragraph(new Text("Max height bigger than default")));
            image.DeleteOwnProperty(Property.MIN_HEIGHT);
            doc.Add(image.SetMaxHeight(250));
            doc.Add(new Paragraph(new Text("Max height smaller than default")));
            image.DeleteOwnProperty(Property.MAX_HEIGHT);
            doc.Add(image.SetMaxHeight(30));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void PrecisionTest01() {
            String outFileName = destinationFolder + "precisionTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_precisionTest01.pdf";
            String imageFileName = sourceFolder + "LOGO_PDF_77.jpg";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas currentPdfCanvas = new PdfCanvas(page);
            Rectangle rc = new Rectangle(56.6929131f, 649.13385f, 481.889771f, 136.062988f);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(currentPdfCanvas, rc);
            Table table = new Table(UnitValue.CreatePointArray(new float[] { 158f }));
            table.SetTextAlignment(TextAlignment.LEFT);
            iText.Layout.Element.Image logoImage = new iText.Layout.Element.Image(ImageDataFactory.Create(imageFileName
                ));
            Paragraph p = new Paragraph().Add(logoImage.SetAutoScale(true));
            Cell cell = new Cell();
            cell.SetKeepTogether(true);
            cell.Add(p);
            table.AddCell(cell.SetHeight(85.03937f).SetVerticalAlignment(VerticalAlignment.TOP).SetPadding(0));
            canvas.Add(table);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageBorderRadiusTest01() {
            String outFileName = destinationFolder + "imageBorderRadiusTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageBorderRadiusTest01.pdf";
            String imageFileName = sourceFolder + "itis.jpg";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(imageFileName));
            image.SetBorderRadius(new BorderRadius(20));
            image.SetBorderBottomLeftRadius(new BorderRadius(35));
            image.SetBorder(new SolidBorder(ColorConstants.ORANGE, 5));
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 3)]
        public virtual void CreateTiffImageTest() {
            String outFileName = destinationFolder + "createTiffImageTest.pdf";
            String cmpFileName = sourceFolder + "cmp_createTiffImageTest.pdf";
            String imgPath = sourceFolder + "group4Compression.tif";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            ImageData id = ImageDataFactory.Create(imgPath);
            ImageData idAsTiff = ImageDataFactory.CreateTiff(UrlUtil.ToURL(imgPath), true, 1, true);
            ImageData idAsTiffFalse = ImageDataFactory.CreateTiff(UrlUtil.ToURL(imgPath), false, 1, false);
            document.Add(new iText.Layout.Element.Image(id));
            document.Add(new iText.Layout.Element.Image(idAsTiff));
            document.Add(new iText.Layout.Element.Image(idAsTiffFalse));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TiffImageWithoutCompressionTest() {
            String outFileName = destinationFolder + "tiffImageWithoutCompression.pdf";
            String cmpFileName = sourceFolder + "cmp_tiffImageWithoutCompression.pdf";
            String imgPath = sourceFolder + "no-compression-tag.tiff";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            ImageData id = ImageDataFactory.Create(imgPath);
            document.Add(new iText.Layout.Element.Image(id));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff02_"));
        }
    }
}
