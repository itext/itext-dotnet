/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FloatImageTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FloatImageTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FloatImageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FloatAllClearNoneImageTest() {
            String dest = destinationFolder + "floatAllClearNoneImage.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));
            Document document = new Document(pdf);
            pdf.SetTagged();
            IList<FloatImageTest.ImagesContainer> images = new List<FloatImageTest.ImagesContainer>();
            images.Add(new FloatImageTest.ImagesContainer("1", new Image(ImageDataFactory.Create(sourceFolder + "1.png"
                )), FloatPropertyValue.LEFT, null, ClearPropertyValue.NONE, new UnitValue(UnitValue.POINT, 200f)));
            images.Add(new FloatImageTest.ImagesContainer("2", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "2.png")), FloatPropertyValue.RIGHT, null, ClearPropertyValue.NONE, new UnitValue(UnitValue
                .POINT, 200f)));
            images.Add(new FloatImageTest.ImagesContainer("3", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "3.png")), FloatPropertyValue.NONE, HorizontalAlignment.CENTER, ClearPropertyValue.NONE
                , new UnitValue(UnitValue.POINT, 200f)));
            images.Add(new FloatImageTest.ImagesContainer("4", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "4.png")), FloatPropertyValue.LEFT, null, ClearPropertyValue.NONE, new UnitValue(UnitValue
                .POINT, 200f)));
            AddFloatingImagesAndText(document, images);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, sourceFolder + "cmp_floatAllClearNoneImage.pdf"
                , destinationFolder));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(dest, sourceFolder + "cmp_floatAllClearNoneImage.pdf"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FloatAllClearBothImageTest() {
            String dest = destinationFolder + "floatAllClearBothImage.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));
            Document document = new Document(pdf);
            pdf.SetTagged();
            IList<FloatImageTest.ImagesContainer> images = new List<FloatImageTest.ImagesContainer>();
            images.Add(new FloatImageTest.ImagesContainer("1", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "1.png")), FloatPropertyValue.LEFT, null, ClearPropertyValue.BOTH, new UnitValue(UnitValue
                .POINT, 200f)));
            images.Add(new FloatImageTest.ImagesContainer("2", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "2.png")), FloatPropertyValue.RIGHT, null, ClearPropertyValue.BOTH, new UnitValue(UnitValue
                .POINT, 200f)));
            images.Add(new FloatImageTest.ImagesContainer("3", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "3.png")), FloatPropertyValue.NONE, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH
                , new UnitValue(UnitValue.POINT, 200f)));
            images.Add(new FloatImageTest.ImagesContainer("4", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "4.png")), FloatPropertyValue.LEFT, null, ClearPropertyValue.BOTH, new UnitValue(UnitValue
                .POINT, 200f)));
            AddFloatingImagesAndText(document, images);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, sourceFolder + "cmp_floatAllClearBothImage.pdf"
                , destinationFolder));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(dest, sourceFolder + "cmp_floatAllClearBothImage.pdf"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FloatNoneRightClearBothImageTest() {
            String dest = destinationFolder + "floatNoneRightClearBothImage.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));
            Document document = new Document(pdf);
            pdf.SetTagged();
            IList<FloatImageTest.ImagesContainer> images = new List<FloatImageTest.ImagesContainer>();
            images.Add(new FloatImageTest.ImagesContainer("5", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "5.png")), FloatPropertyValue.NONE, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH
                , new UnitValue(UnitValue.PERCENT, 33f)));
            images.Add(new FloatImageTest.ImagesContainer("6", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "6.png")), FloatPropertyValue.RIGHT, null, ClearPropertyValue.BOTH, new UnitValue(UnitValue
                .PERCENT, 33f)));
            images.Add(new FloatImageTest.ImagesContainer("7", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "7.png")), FloatPropertyValue.RIGHT, null, ClearPropertyValue.BOTH, new UnitValue(UnitValue
                .PERCENT, 33f)));
            AddFloatingImagesAndText(document, images);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, sourceFolder + "cmp_floatNoneRightClearBothImage.pdf"
                , destinationFolder));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(dest, sourceFolder + "cmp_floatNoneRightClearBothImage.pdf"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FloatNoneRightClearNoneImageTest() {
            String dest = destinationFolder + "floatNoneRightClearNoneImage.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));
            Document document = new Document(pdf);
            pdf.SetTagged();
            IList<FloatImageTest.ImagesContainer> images = new List<FloatImageTest.ImagesContainer>();
            images.Add(new FloatImageTest.ImagesContainer("5", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "5.png")), FloatPropertyValue.NONE, HorizontalAlignment.CENTER, ClearPropertyValue.NONE
                , new UnitValue(UnitValue.PERCENT, 33f)));
            images.Add(new FloatImageTest.ImagesContainer("6", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "6.png")), FloatPropertyValue.RIGHT, null, ClearPropertyValue.NONE, new UnitValue(UnitValue
                .PERCENT, 33f)));
            images.Add(new FloatImageTest.ImagesContainer("7", new iText.Layout.Element.Image(ImageDataFactory.Create(
                sourceFolder + "7.png")), FloatPropertyValue.RIGHT, null, ClearPropertyValue.NONE, new UnitValue(UnitValue
                .PERCENT, 33f)));
            AddFloatingImagesAndText(document, images);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, sourceFolder + "cmp_floatNoneRightClearNoneImage.pdf"
                , destinationFolder));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(dest, sourceFolder + "cmp_floatNoneRightClearNoneImage.pdf"
                ));
        }

        private static void AddFloatingImagesAndText(Document document, IList<FloatImageTest.ImagesContainer> images
            ) {
            document.Add(new Paragraph("Images followed by two paragraphs.\n" + "Image properties: "));
            for (int i = 0; i < images.Count; i++) {
                document.Add(new Paragraph(images[i].ToString()));
            }
            for (int i = 0; i < images.Count; i++) {
                iText.Layout.Element.Image image = images[i].img;
                image.SetBorder(new SolidBorder(1f));
                image.SetWidth(images[i].width);
                image.SetProperty(Property.CLEAR, images[i].clearPropertyValue);
                image.SetHorizontalAlignment(images[i].horizontalAlignment);
                image.SetProperty(Property.FLOAT, images[i].floatPropertyValue);
                document.Add(image);
            }
            document.Add(new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor..."
                ));
            document.Add(new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod " + "tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim "
                 + "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex " + "ea commodo consequat. Duis aute irure dolor in reprehenderit in "
                 + "voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur" + " sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt"
                 + " mollit anim id est laborum.\n" + "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod"
                 + " tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim " + "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea "
                 + "commodo consequat. Duis aute irure dolor in ......"));
        }

        private class ImagesContainer {
            internal String imageName;

            internal iText.Layout.Element.Image img;

            internal FloatPropertyValue? floatPropertyValue;

            internal HorizontalAlignment? horizontalAlignment;

            internal ClearPropertyValue? clearPropertyValue;

            internal UnitValue width;

            public ImagesContainer(String imageName, iText.Layout.Element.Image img, FloatPropertyValue? floatPropertyValue
                , HorizontalAlignment? horizontalAlignment, ClearPropertyValue? clearPropertyValue, UnitValue width) {
                this.imageName = imageName;
                this.img = img;
                this.floatPropertyValue = floatPropertyValue;
                this.horizontalAlignment = horizontalAlignment;
                this.clearPropertyValue = clearPropertyValue;
                this.width = width;
            }

            public override String ToString() {
                String hAlignString;
                if (horizontalAlignment == null) {
                    hAlignString = "null";
                }
                else {
                    hAlignString = horizontalAlignment.ToString();
                }
                return MessageFormatUtil.Format("Image={0}, float={1}, horiz_align={2}, clear={3}, width={4}", imageName, 
                    floatPropertyValue, hAlignString, clearPropertyValue, width);
            }
        }
    }
}
