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
