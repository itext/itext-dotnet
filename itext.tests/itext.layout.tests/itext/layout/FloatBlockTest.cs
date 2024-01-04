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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FloatBlockTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FloatBlockTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FloatBlockTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FloatImageInDivClearNoneTest() {
            String dest = destinationFolder + "floatImageInDivClearNone.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));
            Document document = new Document(pdf);
            pdf.SetTagged();
            AddFloatingImagesInDivs(document, new UnitValue(UnitValue.POINT, 200f), ClearPropertyValue.NONE);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, sourceFolder + "cmp_floatImageInDivClearNone.pdf"
                , destinationFolder));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(dest, sourceFolder + "cmp_floatImageInDivClearNone.pdf"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FloatImageInDivClearBothTest() {
            String dest = destinationFolder + "floatImageInDivClearBoth.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest));
            Document document = new Document(pdf);
            pdf.SetTagged();
            AddFloatingImagesInDivs(document, new UnitValue(UnitValue.POINT, 200f), ClearPropertyValue.BOTH);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, sourceFolder + "cmp_floatImageInDivClearBoth.pdf"
                , destinationFolder));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(dest, sourceFolder + "cmp_floatImageInDivClearBoth.pdf"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FloatImageDifferentSizeInDivTest() {
            String cmpFileName = sourceFolder + "cmp_floatImageDifferentSizeInDiv.pdf";
            String outFile = destinationFolder + "floatImageInDiv.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            pdfDoc.SetTagged();
            UnitValue width = new UnitValue(UnitValue.PERCENT, 33f);
            iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(sourceFolder + "5.png"));
            image.SetBorder(new SolidBorder(1f)).SetWidth(width).SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Div div = new Div();
            div.SetBorder(new DashedBorder(ColorConstants.LIGHT_GRAY, 1));
            div.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div.Add(image);
            doc.Add(div);
            iText.Layout.Element.Image image1 = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + 
                "4.png"));
            image1.SetBorder(new SolidBorder(1f)).SetWidth(width).SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT
                );
            Div div1 = new Div();
            div1.SetBorder(new DashedBorder(ColorConstants.LIGHT_GRAY, 1));
            div1.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            div1.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            div1.Add(image1);
            doc.Add(div1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(outFile, cmpFileName));
        }

        private static void AddFloatingImagesInDivs(Document document, UnitValue width, ClearPropertyValue? clearValue
            ) {
            IList<FloatBlockTest.ImageProperties> imagePropertiesList = new List<FloatBlockTest.ImageProperties>();
            imagePropertiesList.Add(new FloatBlockTest.ImageProperties(FloatPropertyValue.LEFT, clearValue, null, width
                ));
            imagePropertiesList.Add(new FloatBlockTest.ImageProperties(FloatPropertyValue.RIGHT, clearValue, null, width
                ));
            imagePropertiesList.Add(new FloatBlockTest.ImageProperties(FloatPropertyValue.NONE, clearValue, HorizontalAlignment
                .CENTER, width));
            imagePropertiesList.Add(new FloatBlockTest.ImageProperties(FloatPropertyValue.LEFT, clearValue, null, width
                ));
            document.Add(new Paragraph("Four images followed by two paragraphs. All images are wrapped in Divs.\n" + "All images specify WIDTH = "
                 + width));
            for (int i = 0; i < imagePropertiesList.Count; i++) {
                document.Add(new Paragraph("Image " + (i + 1) + ": " + imagePropertiesList[i]));
            }
            for (int i = 0; i < imagePropertiesList.Count; i++) {
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(MessageFormatUtil
                    .Format(sourceFolder + "{0}.png", i + 1)));
                image.SetBorder(new SolidBorder(1f));
                image.SetWidth(width);
                Div div = new Div();
                div.SetProperty(Property.CLEAR, clearValue);
                div.SetProperty(Property.FLOAT, imagePropertiesList[i].floatPropertyValue);
                div.Add(image);
                document.Add(div);
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

        private class ImageProperties {
            internal FloatPropertyValue? floatPropertyValue;

            internal ClearPropertyValue? clearPropertyValue;

            internal HorizontalAlignment? horizontalAlignment;

            internal UnitValue width;

            public ImageProperties(FloatPropertyValue? floatPropertyValue, ClearPropertyValue? clearPropertyValue, HorizontalAlignment?
                 horizontalAlignment, UnitValue width) {
                this.floatPropertyValue = floatPropertyValue;
                this.clearPropertyValue = clearPropertyValue;
                this.horizontalAlignment = horizontalAlignment;
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
                return MessageFormatUtil.Format("float={0} clear={1} horiz_align={2}", floatPropertyValue, clearPropertyValue
                    , hAlignString);
            }
        }
    }
}
