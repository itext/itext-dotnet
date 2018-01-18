/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-944")]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
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

        /// <exception cref="System.IO.FileNotFoundException"/>
        private Document CreateDocument() {
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            return new Document(pdfDocument);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CloseDocumentAndCompareOutputs(Document document) {
            document.Close();
            String compareResult = new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder, "diff"
                );
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
