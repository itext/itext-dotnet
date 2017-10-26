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
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class ImageTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ImageTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ImageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageTest09() {
            String outFileName = destinationFolder + "imageTest09.pdf";
            String cmpFileName = sourceFolder + "cmp_imageTest09.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc, new PageSize(500, 300));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetWidthPercent(100);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 50)]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Ignore("DEVSIX-1658")]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <summary>Image can be reused in layout, so flushing it on the very first draw is a bad thing.</summary>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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
            Table table = new Table(8);
            table.SetWidthPercent(100);
            for (int k = 0; k < rowCount; k++) {
                for (int j = 0; j < 7; j++) {
                    table.AddCell("Hello");
                }
                Cell c = new Cell().Add(img.SetWidthPercent(50));
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
        /// <see cref="System.ArgumentNullException"/>
        /// that was caused by getting
        /// a value from flushed image.
        /// </remarks>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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
            Table table = new Table(4);
            table.SetWidthPercent(100);
            for (int k = 0; k < 5; k++) {
                table.AddCell("Hello World from iText7");
                List list = new List().SetListSymbol("-> ");
                list.Add("list item").Add("list item").Add("list item").Add("list item").Add("list item");
                Cell cell = new Cell().Add(list);
                table.AddCell(cell);
                Cell c = new Cell().Add(img);
                table.AddCell(c);
                Table innerTable = new Table(3);
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1022")]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void ImageInTableTest01() {
            String outFileName = destinationFolder + "imageInTableTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageInTableTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            Table table = new Table(1).SetWidth(UnitValue.CreatePercentValue(100)).SetFixedLayout();
            table.SetMaxHeight(300);
            table.SetBorder(new SolidBorder(ColorConstants.BLUE, 10));
            Cell c = new Cell().Add(img.SetHeight(500));
            table.AddCell(c);
            document.Add(table);
            document.Add(new Table(1).AddCell("Is my occupied area right?"));
            document.Add(new AreaBreak());
            table.SetMinHeight(150);
            document.Add(table);
            document.Add(new Table(1).AddCell("Is my occupied area right?"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void ImageInTableTest02() {
            String outFileName = destinationFolder + "imageInTableTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_imageInTableTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            Table table = new Table(1).SetWidth(UnitValue.CreatePercentValue(100)).SetFixedLayout();
            table.SetMaxHeight(300);
            table.SetBorder(new SolidBorder(ColorConstants.BLUE, 10));
            Cell c = new Cell().Add(img.SetHeight(500));
            table.AddCell("First cell");
            table.AddCell(c);
            document.Add(table);
            document.Add(new Table(1).AddCell("Is my occupied area right?"));
            document.Add(new AreaBreak());
            table.SetMinHeight(150);
            document.Add(table);
            document.Add(new Table(1).AddCell("Is my occupied area right?"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1045")]
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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
    }
}
