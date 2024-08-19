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
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FloatExampleTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FloatExampleTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FloatExampleTest/";

        private static readonly Color imageBorderColor = ColorConstants.LIGHT_GRAY;

        private const float BORDER_MARGIN = 5f;

        private const float IMAGE_BORDER_WIDTH = 15f;

        private const float DIV_BORDER_WIDTH = 1f;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FloatMaxWidthTest01() {
            /* This test illustrate behaviour of images with different width and mas_width properties, that have
            there is text paragraph below image,
            shown examples, wrapped and not wrapped in divs
            Divs have property CLEAR = BOTH, and different values of FLOAT
            */
            String cmpFileName = sourceFolder + "cmp_floatMaxWidthTest01.pdf";
            String outFile = destinationFolder + "floatMaxWidthTest01.pdf";
            // defined range is 0..3
            int firstImage = 0;
            int lastImage = 1;
            //Initialize PDF writer
            PdfWriter writer = new PdfWriter(outFile);
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(writer);
            // Initialize document
            Document document = new Document(pdf);
            pdf.SetTagged();
            // divWidthProperty, divWidth are n/a when not wrapping image in a div
            document.Add(new Paragraph("IMAGE IS NOT WRAPPED IN A DIV.\n"));
            document.Add(new Paragraph("Actual width of image -- no explicit width, no max.\n"));
            AddContent(document, false, 0, null, 0, null, ClearPropertyValue.BOTH, firstImage, lastImage);
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Width < actual width.\n"));
            AddContent(document, false, Property.WIDTH, new UnitValue(UnitValue.PERCENT, 30f), 0, null, ClearPropertyValue
                .BOTH, firstImage, lastImage);
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Max width < actual width.\n"));
            AddContent(document, false, Property.MAX_WIDTH, new UnitValue(UnitValue.PERCENT, 30f), 0, null, ClearPropertyValue
                .BOTH, firstImage, lastImage);
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Max width > actual width.\n"));
            AddContent(document, false, Property.MAX_WIDTH, new UnitValue(UnitValue.PERCENT, 60f), 0, null, ClearPropertyValue
                .BOTH, firstImage, lastImage);
            document.Add(new AreaBreak());
            // Image wrapped in div
            document.Add(new Paragraph("IMAGE IS WRAPPED IN A DIV.\n"));
            // Width of Paragraph inside Div if width of parent
            document.Add(new Paragraph("No explicit width or max: Non-floating text width is parent width.\n"));
            AddContent(document, true, 0, null, 0, null, ClearPropertyValue.BOTH, firstImage, lastImage);
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Bug: Non-floating text width is parent width (limited by max).\n"));
            // 100% would require forced placement, since border box has width and is not included in 100% width
            AddContent(document, true, Property.MAX_WIDTH, new UnitValue(UnitValue.PERCENT, 80f), Property.WIDTH, new 
                UnitValue(UnitValue.PERCENT, 30f), ClearPropertyValue.BOTH, firstImage, lastImage);
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Max width < actual width.\n"));
            // 100% would require forced placement, since border box has width and is not included in 100% width
            AddContent(document, true, Property.MAX_WIDTH, new UnitValue(UnitValue.PERCENT, 80f), Property.MAX_WIDTH, 
                new UnitValue(UnitValue.PERCENT, 30f), ClearPropertyValue.BOTH, firstImage, lastImage);
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Bug: Non-floating text width is parent width (limited by max).\nMax width > actual width.\n"
                ));
            // 100% would require forced placement, since border box has width and is not included in 100% width
            AddContent(document, true, Property.MAX_WIDTH, new UnitValue(UnitValue.PERCENT, 80f), Property.MAX_WIDTH, 
                new UnitValue(UnitValue.PERCENT, 60f), ClearPropertyValue.BOTH, firstImage, lastImage);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatMaxWidthTest02() {
            /* This test illustrate behaviour of images, wrapped in Div containers, that have paragraph below image
            Divs have property CLEAR = BOTH, and different values of FLOAT
            Text in divs has HorizontalAlignment.CENTER
            */
            String cmpFileName = sourceFolder + "cmp_floatMaxWidthTest02.pdf";
            String outFile = destinationFolder + "floatMaxWidthTest02.pdf";
            // defined range is 0..3
            int firstImage = 0;
            int lastImage = 2;
            //Initialize PDF writer
            PdfWriter writer = new PdfWriter(outFile);
            //Initialize PDF document with non-default size
            PdfDocument pdf = new PdfDocument(writer);
            pdf.SetDefaultPageSize(new PageSize(new Rectangle(537, 800)));
            // Initialize document
            Document document = new Document(pdf);
            pdf.SetTagged();
            document.Add(new Paragraph("IMAGE IS WRAPPED IN A DIV.\n"));
            document.Add(new Paragraph("No explicit width or max: Non-floating text width is parent width.\n"));
            AddContent(document, true, 0, null, 0, null, ClearPropertyValue.BOTH, firstImage, lastImage);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff01_"));
        }

        private void AddContent(Document document, bool wrapImages, int imageWidthProperty, UnitValue imageWidth, 
            int divWidthProperty, UnitValue divWidth, ClearPropertyValue? clearValue, int firstImage, int lastImage
            ) {
            FloatExampleTest.ImageProperties[] images = new FloatExampleTest.ImageProperties[4];
            images[0] = new FloatExampleTest.ImageProperties(this, FloatPropertyValue.NONE, clearValue, HorizontalAlignment
                .CENTER);
            images[1] = new FloatExampleTest.ImageProperties(this, FloatPropertyValue.RIGHT, clearValue, HorizontalAlignment
                .CENTER);
            images[2] = new FloatExampleTest.ImageProperties(this, FloatPropertyValue.LEFT, clearValue, HorizontalAlignment
                .CENTER);
            images[3] = new FloatExampleTest.ImageProperties(this, FloatPropertyValue.NONE, clearValue, HorizontalAlignment
                .CENTER);
            Paragraph paragraph = new Paragraph().Add("Four images followed by two paragraphs.\n");
            if (wrapImages) {
                String s = "Each image is wrapped in a div.\n";
                s += "All divs specify CLEAR = " + clearValue;
                if (divWidthProperty > 0) {
                    s += ", " + ((divWidthProperty == Property.WIDTH) ? "WIDTH" : "MAX_WIDTH") + "= " + divWidth;
                }
                if (imageWidthProperty > 0) {
                    s += ".\nAll images specify " + ((imageWidthProperty == Property.WIDTH) ? "WIDTH" : "MAX_WIDTH") + " = " +
                         imageWidth;
                }
                paragraph.Add(s + ".\n");
            }
            else {
                String s = "All images specify CLEAR = " + clearValue;
                if (imageWidthProperty > 0) {
                    s += ", " + ((imageWidthProperty == Property.WIDTH) ? "WIDTH" : "MAX_WIDTH") + "= " + imageWidth;
                }
                paragraph.Add(s + ".\n");
            }
            for (int i = firstImage; i <= lastImage; i++) {
                paragraph.Add((wrapImages ? "Div" : "Image") + " " + (i) + ": " + images[i] + "\n");
            }
            document.Add(paragraph);
            for (int i = firstImage; i <= lastImage; i++) {
                int pictNumber = i + 1;
                iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(sourceFolder + pictNumber + ".png")).
                    SetBorder(new SolidBorder(imageBorderColor, IMAGE_BORDER_WIDTH)).SetHorizontalAlignment(images[i].horizontalAlignment
                    );
                if (wrapImages) {
                    Div div = new Div().SetBorder(new SolidBorder(DIV_BORDER_WIDTH)).SetMargins(BORDER_MARGIN, 0, BORDER_MARGIN
                        , BORDER_MARGIN);
                    div.SetHorizontalAlignment(images[i].horizontalAlignment);
                    div.SetProperty(Property.CLEAR, images[i].clearPropertyValue);
                    div.SetProperty(Property.FLOAT, images[i].floatPropertyValue);
                    if (divWidthProperty > 0) {
                        div.SetProperty(divWidthProperty, divWidth);
                    }
                    if (imageWidthProperty > 0) {
                        image.SetProperty(imageWidthProperty, imageWidth);
                    }
                    div.Add(image);
                    div.Add(new Paragraph("Figure for Div" + i + ": This is longer text that wraps This is longer text that wraps"
                        ).SetTextAlignment(TextAlignment.CENTER)).SimulateBold();
                    document.Add(div);
                }
                else {
                    image.SetMargins(BORDER_MARGIN, 0, BORDER_MARGIN, BORDER_MARGIN);
                    image.SetProperty(Property.CLEAR, images[i].clearPropertyValue);
                    image.SetProperty(Property.FLOAT, images[i].floatPropertyValue);
                    if (imageWidthProperty > 0) {
                        image.SetProperty(imageWidthProperty, imageWidth);
                    }
                    document.Add(image);
                }
            }
            document.Add(new Paragraph("The following outline is provided as an over-view of and topical guide to Zambia:"
                ));
            document.Add(new Paragraph("Zambia – landlocked sovereign country located in Southern Africa.[1] Zambia has been inhabited for thousands of years by hunter-gatherers and migrating tribes. After sporadic visits by European explorers starting in the 18th century, Zambia was gradually claimed and occupied by the British as protectorate of Northern Rhodesia towards the end of the nineteenth century. On 24 October 1964, the protectorate gained independence with the new name of Zambia, derived from the Zam-bezi river which flows through the country. After independence the country moved towards a system of one party rule with Kenneth Kaunda as president. Kaunda dominated Zambian politics until multiparty elections were held in 1991."
                ));
        }

        private class ImageProperties {
//\cond DO_NOT_DOCUMENT
            internal FloatPropertyValue? floatPropertyValue;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal ClearPropertyValue? clearPropertyValue;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal HorizontalAlignment? horizontalAlignment;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal ImageProperties(FloatExampleTest _enclosing, FloatPropertyValue? floatPropertyValue, ClearPropertyValue?
                 clearPropertyValue, HorizontalAlignment? horizontalAlignment) {
                this._enclosing = _enclosing;
                this.floatPropertyValue = floatPropertyValue;
                this.clearPropertyValue = clearPropertyValue;
                this.horizontalAlignment = horizontalAlignment;
            }
//\endcond

            public override String ToString() {
                return "float=" + this.floatPropertyValue + ", clear=" + this.clearPropertyValue + ", horiz_align=" + this
                    .horizontalAlignment;
            }

            private readonly FloatExampleTest _enclosing;
        }
    }
}
