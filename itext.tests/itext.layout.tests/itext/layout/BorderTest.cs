/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
    public class BorderTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BorderTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/BorderTest/";

        public const String cmpPrefix = "cmp_";

        internal String fileName;

        internal String outFileName;

        internal String cmpFileName;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBordersTest() {
            fileName = "simpleBordersTest.pdf";
            Document doc = CreateDocument();
            List list = new List();
            ListItem solidBorderItem = new ListItem("solid");
            solidBorderItem.SetBorder(new SolidBorder(ColorConstants.RED, 6)).SetMarginBottom(5);
            solidBorderItem.SetBorderTop(new SolidBorder(ColorConstants.BLUE, 10));
            list.Add(solidBorderItem);
            ListItem doubleBorderItem = new ListItem("double");
            doubleBorderItem.SetBorder(new DoubleBorder(ColorConstants.RED, 10)).SetMarginBottom(5);
            doubleBorderItem.SetBorderRight(new DoubleBorder(ColorConstants.BLUE, 6));
            list.Add(doubleBorderItem);
            ListItem dashedBorderItem = new ListItem("dashed");
            dashedBorderItem.SetBorder(new DashedBorder(ColorConstants.GRAY, 2)).SetMarginBottom(5);
            dashedBorderItem.SetBorderBottom(new DashedBorder(ColorConstants.BLACK, 4));
            list.Add(dashedBorderItem);
            ListItem dottedBorderItem = new ListItem("dotted");
            dottedBorderItem.SetBorder(new DottedBorder(ColorConstants.BLACK, 3)).SetMarginBottom(5);
            dottedBorderItem.SetBorderLeft(new DottedBorder(ColorConstants.GRAY, 6));
            list.Add(dottedBorderItem);
            ListItem roundDotsBorderItem = new ListItem("round dots");
            roundDotsBorderItem.SetBorder(new RoundDotsBorder(ColorConstants.LIGHT_GRAY, 3)).SetMarginBottom(5);
            roundDotsBorderItem.SetBorderLeft(new RoundDotsBorder(ColorConstants.BLUE, 5));
            list.Add(roundDotsBorderItem);
            doc.Add(list);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void DrawBordersByRectangleTest() {
            String outPdf = destinationFolder + "drawBordersByRectangle.pdf";
            String cmpPdf = sourceFolder + "cmp_drawBordersByRectangle.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf))) {
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                new SolidBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(50, 700, 100, 100));
                new DashedBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(200, 700, 100, 100));
                new DottedBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(350, 700, 100, 100));
                new DoubleBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(50, 550, 100, 100));
                new GrooveBorder(new DeviceRgb(0, 255, 0), 5).Draw(canvas, new Rectangle(200, 550, 100, 100));
                new InsetBorder(new DeviceRgb(0, 255, 0), 5).Draw(canvas, new Rectangle(350, 550, 100, 100));
                new OutsetBorder(new DeviceRgb(0, 255, 0), 5).Draw(canvas, new Rectangle(50, 400, 100, 100));
                new RidgeBorder(new DeviceRgb(0, 255, 0), 5).Draw(canvas, new Rectangle(200, 400, 100, 100));
                new RoundDotsBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(350, 400, 100, 100));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void Borders3DTest() {
            fileName = "borders3DTest.pdf";
            Document doc = CreateDocument();
            List list = new List();
            ListItem grooveBorderItem = new ListItem("groove");
            grooveBorderItem.SetBorder(new GrooveBorder(2)).SetMarginBottom(5).SetWidth(100);
            list.Add(grooveBorderItem);
            ListItem ridgeBorderItem = new ListItem("ridge");
            ridgeBorderItem.SetBorder(new RidgeBorder(2)).SetMarginBottom(5).SetWidth(100);
            list.Add(ridgeBorderItem);
            ListItem insetBorderItem = new ListItem("inset");
            insetBorderItem.SetBorder(new InsetBorder(1)).SetMarginBottom(5).SetWidth(100);
            list.Add(insetBorderItem);
            ListItem outsetBorderItem = new ListItem("outset");
            outsetBorderItem.SetBorder(new OutsetBorder(1)).SetMarginBottom(5).SetWidth(100);
            list.Add(outsetBorderItem);
            doc.Add(list);
            Paragraph emptyParagraph = new Paragraph("\n");
            doc.Add(emptyParagraph);
            DeviceRgb blueRgb = new DeviceRgb(0, 0, 200);
            DeviceRgb greenRgb = new DeviceRgb(0, 255, 0);
            DeviceCmyk magentaCmyk = new DeviceCmyk(0, 100, 0, 0);
            DeviceCmyk yellowCmyk = new DeviceCmyk(0, 0, 100, 0);
            list = new List();
            grooveBorderItem = new ListItem("groove");
            grooveBorderItem.SetBorder(new GrooveBorder(blueRgb, 2)).SetMarginBottom(5).SetWidth(100);
            list.Add(grooveBorderItem);
            ridgeBorderItem = new ListItem("ridge");
            ridgeBorderItem.SetBorder(new RidgeBorder(greenRgb, 2)).SetMarginBottom(5).SetWidth(100);
            list.Add(ridgeBorderItem);
            insetBorderItem = new ListItem("inset");
            insetBorderItem.SetBorder(new InsetBorder(magentaCmyk, 1)).SetMarginBottom(5).SetWidth(100);
            list.Add(insetBorderItem);
            outsetBorderItem = new ListItem("outset");
            outsetBorderItem.SetBorder(new OutsetBorder(yellowCmyk, 1)).SetMarginBottom(5).SetWidth(100);
            list.Add(outsetBorderItem);
            doc.Add(list);
            emptyParagraph = new Paragraph("\n");
            doc.Add(emptyParagraph);
            list = new List();
            grooveBorderItem = new ListItem("groove");
            grooveBorderItem.SetBorder(new GrooveBorder(yellowCmyk, 8)).SetMarginBottom(5);
            list.Add(grooveBorderItem);
            ridgeBorderItem = new ListItem("ridge");
            ridgeBorderItem.SetBorder(new RidgeBorder(magentaCmyk, 8)).SetMarginBottom(5);
            list.Add(ridgeBorderItem);
            insetBorderItem = new ListItem("inset");
            insetBorderItem.SetBorder(new InsetBorder(greenRgb, 8)).SetMarginBottom(5);
            list.Add(insetBorderItem);
            outsetBorderItem = new ListItem("outset");
            outsetBorderItem.SetBorder(new OutsetBorder(blueRgb, 8)).SetMarginBottom(5);
            list.Add(outsetBorderItem);
            doc.Add(list);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BorderSidesTest() {
            fileName = "borderSidesTest.pdf";
            Document doc = CreateDocument();
            String text = "<p class=\"none\"  >No border.</p>\n" + "<p class=\"dotted\">A dotted border.</p>\n" + "<p class=\"dashed\">A dashed border.</p>\n"
                 + "<p class=\"solid\" >A solid border.</p>\n" + "<p class=\"double\">A double border.</p>\n" + "<p class=\"groove\">A groove border.</p>\n"
                 + "<p class=\"ridge\" >A ridge border.</p>\n" + "<p class=\"inset\" >An inset border.</p>\n" + "<p class=\"outset\">An outset border.</p>\n"
                 + "<p class=\"hidden\">A hidden border.</p>";
            Paragraph p = new Paragraph(text);
            p.SetBorderTop(new SolidBorder(DeviceCmyk.MAGENTA, 4));
            p.SetBorderRight(new DoubleBorder(ColorConstants.RED, 6));
            p.SetBorderBottom(new RoundDotsBorder(DeviceCmyk.CYAN, 2));
            p.SetBorderLeft(new DashedBorder(DeviceGray.BLACK, 3));
            doc.Add(p);
            doc.Add(new Paragraph(text).SetBorderTop(new SolidBorder(DeviceCmyk.MAGENTA, 8)));
            doc.Add(new Paragraph(text).SetBorderRight(new DoubleBorder(ColorConstants.RED, 4)));
            doc.Add(new Paragraph(text).SetBorderBottom(new RoundDotsBorder(DeviceCmyk.CYAN, 3)));
            doc.Add(new Paragraph(text).SetBorderLeft(new DashedBorder(DeviceGray.BLACK, 5)));
            doc.Add(new Paragraph(text).SetBorder(new DottedBorder(DeviceGray.BLACK, 1)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BorderBoxTest() {
            fileName = "borderBoxTest.pdf";
            Document doc = CreateDocument();
            String textBefore = "At the mid-oceanic ridges, two tectonic plates diverge from one another as new oceanic crust is formed by the cooling and "
                 + "solidifying of hot molten rock. Because the crust is very thin at these ridges due to the pull of the tectonic plates, the release of "
                 + "pressure leads to adiabatic expansion and the partial melting of the mantle, causing volcanism and creating new oceanic crust. Most divergent "
                 + "plate boundaries are at the bottom of the oceans; therefore, most volcanic activity is submarine, forming new seafloor. Black smokers (also "
                 + "known as deep sea vents) are an example of this kind of volcanic activity. Where the mid-oceanic ridge is above sea-level, volcanic islands are "
                 + "formed, for example, Iceland.";
            String text = "Earth's volcanoes occur because its crust is broken into 17 major, rigid tectonic plates that float on a hotter,"
                 + " softer layer in its mantle. Therefore, on Earth, volcanoes are generally found where tectonic plates are diverging or converging. "
                 + "For example, a mid-oceanic ridge, such as the Mid-Atlantic Ridge, has volcanoes caused by divergent tectonic plates pulling apart;"
                 + " the Pacific Ring of Fire has volcanoes caused by convergent tectonic plates coming together. Volcanoes can also form where there is "
                 + "stretching and thinning of the crust's interior plates, e.g., in the East African Rift and the Wells Gray-Clearwater volcanic field and "
                 + "Rio Grande Rift in North America. This type of volcanism falls under the umbrella of \"plate hypothesis\" volcanism. Volcanism away "
                 + "from plate boundaries has also been explained as mantle plumes. These so-called \"hotspots\", for example Hawaii, are postulated to arise "
                 + "from upwelling diapirs with magma from the core-mantle boundary, 3,000 km deep in the Earth. Volcanoes are usually not created where two "
                 + "tectonic plates slide past one another.";
            String textAfter = "Subduction zones are places where two plates, usually an oceanic plate and a continental plate, collide. In this case, the oceanic "
                 + "plate subducts, or submerges under the continental plate forming a deep ocean trench just offshore. In a process called flux melting, water released"
                 + " from the subducting plate lowers the melting temperature of the overlying mantle wedge, creating magma. This magma tends to be very viscous due to "
                 + "its high silica content, so often does not reach the surface and cools at depth. When it does reach the surface, a volcano is formed. Typical examples"
                 + " of this kind of volcano are Mount Etna and the volcanoes in the Pacific Ring of Fire.";
            doc.Add(new Paragraph(textBefore).SetMargins(25, 60, 70, 80));
            Paragraph p = new Paragraph(text).SetBackgroundColor(ColorConstants.GRAY);
            p.SetMargins(25, 60, 70, 80);
            p.SetBorderLeft(new DoubleBorder(ColorConstants.RED, 25));
            p.SetBorder(new DoubleBorder(ColorConstants.BLACK, 6));
            doc.Add(p);
            doc.Add(new Paragraph(textAfter).SetBorder(new DottedBorder(ColorConstants.BLACK, 3)).SetBorderRight(new DottedBorder
                (ColorConstants.BLACK, 12)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BorderOutlineTest() {
            fileName = "borderOutlineTest.pdf";
            Document doc = CreateDocument();
            String textBefore = "At the mid-oceanic ridges, two tectonic plates diverge from one another as new oceanic crust is formed by the cooling and "
                 + "solidifying of hot molten rock. Because the crust is very thin at these ridges due to the pull of the tectonic plates, the release of "
                 + "pressure leads to adiabatic expansion and the partial melting of the mantle, causing volcanism and creating new oceanic crust. Most divergent "
                 + "plate boundaries are at the bottom of the oceans; therefore, most volcanic activity is submarine, forming new seafloor. Black smokers (also "
                 + "known as deep sea vents) are an example of this kind of volcanic activity. Where the mid-oceanic ridge is above sea-level, volcanic islands are "
                 + "formed, for example, Iceland.";
            String text = "Earth's volcanoes occur because its crust is broken into 17 major, rigid tectonic plates that float on a hotter,"
                 + " softer layer in its mantle. Therefore, on Earth, volcanoes are generally found where tectonic plates are diverging or converging. "
                 + "For example, a mid-oceanic ridge, such as the Mid-Atlantic Ridge, has volcanoes caused by divergent tectonic plates pulling apart;"
                 + " the Pacific Ring of Fire has volcanoes caused by convergent tectonic plates coming together. Volcanoes can also form where there is "
                 + "stretching and thinning of the crust's interior plates, e.g., in the East African Rift and the Wells Gray-Clearwater volcanic field and "
                 + "Rio Grande Rift in North America. This type of volcanism falls under the umbrella of \"plate hypothesis\" volcanism. Volcanism away "
                 + "from plate boundaries has also been explained as mantle plumes. These so-called \"hotspots\", for example Hawaii, are postulated to arise "
                 + "from upwelling diapirs with magma from the core-mantle boundary, 3,000 km deep in the Earth. Volcanoes are usually not created where two "
                 + "tectonic plates slide past one another.";
            doc.Add(new Paragraph(textBefore).SetMargins(25, 60, 70, 80));
            Paragraph p = new Paragraph(text).SetBackgroundColor(ColorConstants.GRAY);
            p.SetMargins(25, 60, 70, 80);
            p.SetProperty(Property.OUTLINE, new DoubleBorder(ColorConstants.RED, 25));
            doc.Add(p);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void RotatedBordersTest() {
            fileName = "rotatedBordersTest.pdf";
            Document doc = CreateDocument();
            doc.SetMargins(0, 0, 0, 0);
            Paragraph p = new Paragraph("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n"
                );
            p.SetBorder(new SolidBorder(50));
            p.SetRotationAngle(Math.PI / 6);
            doc.Add(p);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            img.SetBorder(new SolidBorder(50));
            img.SetRotationAngle(Math.PI / 6);
            doc.Add(img);
            doc.Close();
            CloseDocumentAndCompareOutputs(doc);
        }

        private Document CreateDocument() {
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            return new Document(pdfDocument);
        }

        private void CloseDocumentAndCompareOutputs(Document document) {
            document.Close();
            String compareResult = new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder, "diff"
                );
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        private class TestBorder : DashedBorder {
            public TestBorder(float width)
                : base(width) {
            }

            public virtual float PublicGetDotsGap(double distance, float initialGap) {
                return GetDotsGap(distance, initialGap);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetDotsGapTest() {
            float expected = 0.2f;
            double distance = 0.2;
            float initialGap = 0.2f;
            BorderTest.TestBorder border = new BorderTest.TestBorder(1f);
            float actual = border.PublicGetDotsGap(distance, initialGap);
            NUnit.Framework.Assert.AreEqual(expected, actual, 0.0001f);
        }
    }
}
