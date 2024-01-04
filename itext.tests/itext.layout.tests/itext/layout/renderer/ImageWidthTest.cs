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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImageWidthTest : ExtendedITextTest {
        private const double EPSILON = 0.01;

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ImageWidthTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ImageWidthTest/";

        public static readonly String imageFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ImageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWidthTest01() {
            String outFileName = destinationFolder + "imageWidthTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageWidthTest01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph(new Text("First Line")));
            Paragraph p = new Paragraph();
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(imageFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject);
            image.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePercentValue(100));
            p.Add(image);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Second Line")));
            p = new Paragraph();
            xObject = new PdfImageXObject(ImageDataFactory.Create(imageFolder + "itis.jpg"));
            image = new iText.Layout.Element.Image(xObject);
            image.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePercentValue(100));
            p.Add(image);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Third Line")));
            p = new Paragraph();
            xObject = new PdfImageXObject(ImageDataFactory.Create(imageFolder + "Desert.jpg"));
            image = new iText.Layout.Element.Image(xObject);
            image.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePercentValue(100));
            image.SetProperty(Property.MAX_HEIGHT, UnitValue.CreatePointValue(200f));
            p.Add(image);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Fourth Line")));
            p = new Paragraph();
            xObject = new PdfImageXObject(ImageDataFactory.Create(imageFolder + "itis.jpg"));
            image = new iText.Layout.Element.Image(xObject);
            image.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(100));
            p.Add(image);
            doc.Add(p);
            doc.Add(new Paragraph(new Text("Fifth Line")));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWidthTest02() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(imageFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject);
            ImageRenderer renderer = new ImageRenderer(image);
            image.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(50));
            MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(50.0, minMaxWidth.GetMaxWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0.0, minMaxWidth.GetMaxWidth() - minMaxWidth.GetMinWidth(), EPSILON);
            image.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePercentValue(50));
            minMaxWidth = renderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(1024.0, minMaxWidth.GetMaxWidth(), EPSILON);
            image.SetProperty(Property.MAX_HEIGHT, UnitValue.CreatePointValue(100f));
            minMaxWidth = renderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(100.0 * 1024.0 / 768.0, minMaxWidth.GetMaxWidth(), EPSILON);
            image = new iText.Layout.Element.Image(xObject);
            renderer = new ImageRenderer(image);
            image.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(2000));
            image.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(3000));
            minMaxWidth = renderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(2000.0, minMaxWidth.GetMaxWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0.0, minMaxWidth.GetMaxWidth() - minMaxWidth.GetMinWidth(), EPSILON);
            image.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(100f));
            image.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100f));
            minMaxWidth = renderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(100.0 * 1024.0 / 768.0, minMaxWidth.GetMaxWidth(), EPSILON);
        }
    }
}
