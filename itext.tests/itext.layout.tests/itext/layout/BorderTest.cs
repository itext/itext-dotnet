using System;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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
            solidBorderItem.SetBorder(new SolidBorder(Color.RED, 6)).SetMarginBottom(5);
            solidBorderItem.SetBorderTop(new SolidBorder(Color.BLUE, 10));
            list.Add(solidBorderItem);
            ListItem doubleBorderItem = new ListItem("double");
            doubleBorderItem.SetBorder(new DoubleBorder(Color.RED, 10)).SetMarginBottom(5);
            doubleBorderItem.SetBorderRight(new DoubleBorder(Color.BLUE, 6));
            list.Add(doubleBorderItem);
            ListItem dashedBorderItem = new ListItem("dashed");
            dashedBorderItem.SetBorder(new DashedBorder(Color.GRAY, 2)).SetMarginBottom(5);
            dashedBorderItem.SetBorderBottom(new DashedBorder(Color.BLACK, 4));
            list.Add(dashedBorderItem);
            ListItem dottedBorderItem = new ListItem("dotted");
            dottedBorderItem.SetBorder(new DottedBorder(Color.BLACK, 3)).SetMarginBottom(5);
            dottedBorderItem.SetBorderLeft(new DottedBorder(Color.GRAY, 6));
            list.Add(dottedBorderItem);
            ListItem roundDotsBorderItem = new ListItem("round dots");
            roundDotsBorderItem.SetBorder(new RoundDotsBorder(Color.LIGHT_GRAY, 3)).SetMarginBottom(5);
            roundDotsBorderItem.SetBorderLeft(new RoundDotsBorder(Color.BLUE, 5));
            list.Add(roundDotsBorderItem);
            doc.Add(list);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        [NUnit.Framework.Test]
        public virtual void IncompleteTableTest01() {
            fileName = "incompleteTableTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(Color.GREEN, 5));
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add("One");
            table.AddCell(cell);
            // row 1 and 2, cell 2
            cell = new Cell(2, 1).Add("Two");
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add("Three");
            table.AddCell(cell);
            // row 3, cell 1
            cell = new Cell().Add("Four");
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest02() {
            fileName = "simpleBorderTest02.pdf";
            Document doc = CreateDocument();
            Table table = new Table(1);
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add("One");
            cell.SetBorderTop(new SolidBorder(20));
            cell.SetBorderBottom(new SolidBorder(20));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add("Two");
            cell.SetBorderTop(new SolidBorder(30));
            cell.SetBorderBottom(new SolidBorder(40));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest03() {
            fileName = "simpleBorderTest03.pdf";
            Document doc = CreateDocument();
            Table table = new Table(2);
            table.AddCell(new Cell().Add("1"));
            table.AddCell(new Cell(2, 1).Add("2"));
            table.AddCell(new Cell().Add("3"));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest04() {
            fileName = "simpleBorderTest04.pdf";
            Document doc = CreateDocument();
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            String textHelloWorld = "Hello World\n" + "Hello World\n" + "Hello World\n" + "Hello World\n" + "Hello World\n";
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(Color.RED, 2f));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(textHelloWorld)));
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(new Paragraph(textByron)));
            }
            table.AddCell(new Cell(1, 2).Add(textByron));
            doc.Add(table);
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
            p.SetBorderRight(new DoubleBorder(DeviceRgb.RED, 6));
            p.SetBorderBottom(new RoundDotsBorder(DeviceCmyk.CYAN, 2));
            p.SetBorderLeft(new DashedBorder(DeviceGray.BLACK, 3));
            doc.Add(p);
            doc.Add(new Paragraph(text).SetBorderTop(new SolidBorder(DeviceCmyk.MAGENTA, 8)));
            doc.Add(new Paragraph(text).SetBorderRight(new DoubleBorder(DeviceRgb.RED, 4)));
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
            Paragraph p = new Paragraph(text).SetBackgroundColor(Color.GRAY);
            p.SetMargins(25, 60, 70, 80);
            p.SetBorderLeft(new DoubleBorder(DeviceRgb.RED, 25));
            p.SetBorder(new DoubleBorder(DeviceRgb.BLACK, 6));
            doc.Add(p);
            doc.Add(new Paragraph(textAfter).SetBorder(new DottedBorder(Color.BLACK, 3)).SetBorderRight(new DottedBorder
                (Color.BLACK, 12)));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NoVerticalBorderTest() {
            fileName = "noVerticalBorderTest.pdf";
            Document doc = CreateDocument();
            Table mainTable = new Table(1);
            Cell cell = new Cell().SetBorder(Border.NO_BORDER).SetBorderTop(new SolidBorder(Color.BLACK, 0.5f));
            cell.Add("TESCHTINK");
            mainTable.AddCell(cell);
            doc.Add(mainTable);
            doc.Close();
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WideBorderTest01() {
            fileName = "wideBorderTest01.pdf";
            Document doc = CreateDocument();
            doc.Add(new Paragraph("ROWS SHOULD BE THE SAME"));
            Table table = new Table(new float[] { 1, 3 });
            table.SetWidthPercent(50);
            Cell cell;
            // row 21, cell 1
            cell = new Cell().Add("BORDERS");
            table.AddCell(cell);
            // row 1, cell 2
            cell = new Cell().Add("ONE");
            cell.SetBorderLeft(new SolidBorder(Color.RED, 16f));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add("BORDERS");
            table.AddCell(cell);
            // row 2, cell 2
            cell = new Cell().Add("TWO");
            cell.SetBorderLeft(new SolidBorder(Color.RED, 16f));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WideBorderTest02() {
            fileName = "wideBorderTest02.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, new PageSize(842, 842));
            Table table = new Table(3);
            table.SetBorder(new SolidBorder(Color.GREEN, 91f));
            Cell cell;
            cell = new Cell(1, 2).Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 70f));
            table.AddCell(cell);
            cell = new Cell(2, 1).Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 70f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 70f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.BLUE, 20f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell(1, 2).Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell(2, 1).Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell(1, 2).Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 45f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 40f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 35f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.BLUE, 5f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 45f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 64f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 102f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 11f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 12f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 44f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 27f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 16f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 59));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 20f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 20f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 20f));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WideBorderTest04() {
            fileName = "wideBorderTest04.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, new PageSize(200, 150));
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(Color.RED, 5));
            for (int i = 0; i < 5; i++) {
                table.AddCell(new Cell().Add("Cell " + i));
            }
            table.AddCell(new Cell().Add("Cell 5").SetBorderTop(new SolidBorder(Color.GREEN, 20)));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BorderCollapseTest01() {
            fileName = "borderCollapseTest01.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(Color.RED, 5));
            Cell cell;
            table.AddCell(new Cell(1, 2).Add("first").SetBorder(Border.NO_BORDER));
            cell = new Cell(1, 2).Add("second");
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BorderCollapseTest02() {
            fileName = "borderCollapseTest02.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Cell cell;
            Table table = new Table(2);
            // first row
            // column 1
            cell = new Cell().Add("1");
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add("2");
            table.AddCell(cell);
            // second row
            // column 1
            cell = new Cell().Add("3");
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add("4");
            table.AddCell(cell);
            cell = new Cell(1, 2).Add("5");
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BorderCollapseTest03() {
            fileName = "borderCollapseTest03.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Cell cell;
            Table table = new Table(2);
            // first row
            // column 1
            cell = new Cell().Add("1");
            cell.SetBorderBottom(new SolidBorder(Color.RED, 4));
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add("2");
            cell.SetBorderBottom(new SolidBorder(Color.YELLOW, 5));
            table.AddCell(cell);
            // second row
            // column 1
            cell = new Cell().Add("3");
            cell.SetBorder(new SolidBorder(Color.GREEN, 3));
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add("4");
            cell.SetBorderBottom(new SolidBorder(Color.MAGENTA, 2));
            table.AddCell(cell);
            cell = new Cell(1, 2).Add("5");
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WideBorderTest03() {
            fileName = "wideBorderTest03.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, new PageSize(842, 400));
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(Color.GREEN, 90f));
            Cell cell;
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.BLUE, 20f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 120f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add("Borders shouldn't be layouted outside the layout area.");
            cell.SetBorder(new SolidBorder(Color.RED, 50f));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void InfiniteLoopTest01() {
            fileName = "infiniteLoopTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(new float[] { 1, 3 });
            table.SetWidthPercent(50);
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add("1ORD");
            cell.SetBorderLeft(new SolidBorder(Color.BLUE, 5));
            table.AddCell(cell);
            // row 1, cell 2
            cell = new Cell().Add("ONE");
            cell.SetBorderLeft(new SolidBorder(Color.RED, 100f));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add("2ORD");
            cell.SetBorderTop(new SolidBorder(Color.YELLOW, 100f));
            table.AddCell(cell);
            // row 2, cell 2
            cell = new Cell().Add("TWO");
            cell.SetBorderLeft(new SolidBorder(Color.RED, 0.5f));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest01() {
            fileName = "splitCellsTest01.pdf";
            Document doc = CreateDocument();
            String longText = "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text."
                 + "Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.Very very very very very very very very very very very very very very very very very very long text.";
            Table table = new Table(2);
            table.SetBorderTop(new DottedBorder(Color.MAGENTA, 3f));
            table.SetBorderRight(new DottedBorder(Color.RED, 3f));
            table.SetBorderBottom(new DottedBorder(Color.BLUE, 3f));
            table.SetBorderLeft(new DottedBorder(Color.GRAY, 3f));
            Cell cell;
            cell = new Cell().Add("Some text");
            cell.SetBorderRight(new SolidBorder(Color.RED, 2f));
            table.AddCell(cell);
            cell = new Cell().Add("Some text");
            cell.SetBorderLeft(new SolidBorder(Color.GREEN, 4f));
            table.AddCell(cell);
            cell = new Cell().Add(longText);
            cell.SetBorderBottom(new SolidBorder(Color.RED, 5f));
            table.AddCell(cell);
            cell = new Cell().Add("Hello");
            cell.SetBorderBottom(new SolidBorder(Color.BLUE, 5f));
            table.AddCell(cell);
            cell = new Cell().Add("Some text.");
            cell.SetBorderTop(new SolidBorder(Color.GREEN, 6f));
            table.AddCell(cell);
            cell = new Cell().Add("World");
            cell.SetBorderTop(new SolidBorder(Color.YELLOW, 6f));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest02() {
            fileName = "splitCellsTest02.pdf";
            Document doc = CreateDocument();
            String text = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n";
            Table table = new Table(2);
            Cell cell;
            for (int i = 0; i < 38; i++) {
                cell = new Cell().Add(text);
                cell.SetBorder(new SolidBorder(Color.RED, 2f));
                table.AddCell(cell);
            }
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorder(new SolidBorder(Color.YELLOW, 3));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest03() {
            fileName = "splitCellsTest03.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(100, 160));
            String textAlphabet = "ABCDEFGHIJKLMNOPQRSTUWXYZ";
            Table table = new Table(1);
            table.AddCell(new Cell().Add(textAlphabet).SetBorder(new SolidBorder(4)));
            table.AddFooterCell(new Cell().Add("Footer"));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest04() {
            fileName = "splitCellsTest04.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 100 + 72));
            String text = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "A\n" + "B\n" + "C\n" + "D";
            Table table = new Table(1);
            Cell cell;
            cell = new Cell().Add(text);
            cell.SetBorderBottom(new SolidBorder(Color.RED, 20));
            cell.SetBorderTop(new SolidBorder(Color.GREEN, 20));
            table.AddCell(cell);
            table.AddFooterCell(new Cell().Add("Footer").SetBorderTop(new SolidBorder(Color.YELLOW, 20)));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest05() {
            fileName = "splitCellsTest05.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(130, 150));
            String textAlphabet = "Cell";
            Table table = new Table(3);
            table.AddCell(new Cell().Add(textAlphabet));
            table.AddCell(new Cell(2, 1).Add(textAlphabet));
            table.AddCell(new Cell().Add(textAlphabet));
            table.AddCell(new Cell().Add(textAlphabet));
            table.AddCell(new Cell().Add(textAlphabet));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest06() {
            fileName = "splitCellsTest06.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(300, 150));
            doc.Add(new Paragraph("No more"));
            doc.Add(new Paragraph("place"));
            doc.Add(new Paragraph("here"));
            Table table = new Table(3);
            Cell cell = new Cell(3, 1);
            cell.Add("G");
            cell.Add("R");
            cell.Add("P");
            table.AddCell(cell);
            table.AddCell("middle row 1");
            cell = new Cell(3, 1);
            cell.Add("A");
            cell.Add("B");
            cell.Add("C");
            table.AddCell(cell);
            table.AddCell("middle row 2");
            table.AddCell("middle row 3");
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Ignore("DEVSIX-931")]
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest07() {
            fileName = "splitCellsTest07.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(130, 150));
            String textAlphabet = "Cell";
            Table table = new Table(3);
            table.AddCell(new Cell().Add(textAlphabet + "1"));
            table.AddCell(new Cell(2, 1).Add(textAlphabet + "2"));
            table.AddCell(new Cell().Add(textAlphabet + "3"));
            table.AddCell(new Cell().Add(textAlphabet + "4"));
            table.AddCell(new Cell().Add(textAlphabet + "5"));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest08() {
            fileName = "splitCellsTest08.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(130, 160));
            String textAlphabet = "Cell";
            Table table = new Table(3);
            table.AddCell(new Cell().Add(textAlphabet + "1"));
            table.AddCell(new Cell(2, 1).Add(textAlphabet + "2").SetBorder(new SolidBorder(Color.GREEN, 4)));
            table.AddCell(new Cell().Add(textAlphabet + "3"));
            table.AddCell(new Cell().Add(textAlphabet + "4"));
            table.AddCell(new Cell().Add(textAlphabet + "5"));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitCellsTest09() {
            fileName = "splitCellsTest09.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 160));
            String textAlphabet = "Cell";
            Table table = new Table(3);
            table.AddCell(new Cell().Add("Make Gretzky great again! Make Gretzky great again! Make Gretzky great again! Make Gretzky great again! Make Gretzky great again! Make Gretzky great again!"
                ));
            table.AddCell(new Cell(2, 1).Add(textAlphabet + "3"));
            table.AddCell(new Cell().Add(textAlphabet + "4").SetBorder(new SolidBorder(Color.GREEN, 2)));
            table.AddCell(new Cell().Add(textAlphabet + "5"));
            table.AddCell(new Cell().Add(textAlphabet + "5"));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest01() {
            fileName = "tableWithHeaderFooterTest01.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(1000, 1000));
            String text = "Cell";
            Table table = new Table(3);
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(text + "1").SetHeight(40).SetBorderBottom(new SolidBorder(Color.MAGENTA, 100)
                    ));
                table.AddCell(new Cell().Add(text + "4").SetHeight(40).SetBorderBottom(new SolidBorder(Color.MAGENTA, 100)
                    ));
                table.AddCell(new Cell().Add(text + "5").SetHeight(40).SetBorderBottom(new SolidBorder(Color.MAGENTA, 100)
                    ));
            }
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add("Header").SetHeight(40));
                table.AddFooterCell(new Cell().Add("Header").SetHeight(40));
            }
            table.SetBorder(new SolidBorder(Color.GREEN, 100));
            doc.Add(table);
            doc.Add(new Table(1).AddCell(new Cell().Add("Hello")).SetBorder(new SolidBorder(Color.BLACK, 10)));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest02() {
            fileName = "tableWithHeaderFooterTest02.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 1500));
            Table table = new Table(2);
            table.AddHeaderCell(new Cell().SetHeight(30).Add("Header1").SetBorderTop(new SolidBorder(Color.RED, 100)));
            table.AddHeaderCell(new Cell().SetHeight(30).Add("Header2").SetBorderTop(new SolidBorder(Color.RED, 200)));
            table.AddFooterCell(new Cell().SetHeight(30).Add("Footer1").SetBorderTop(new SolidBorder(Color.RED, 100)));
            table.AddFooterCell(new Cell().SetHeight(30).Add("Footer2").SetBorderTop(new SolidBorder(Color.RED, 200)));
            table.AddFooterCell(new Cell().SetHeight(30).Add("Footer3"));
            table.AddFooterCell(new Cell().SetHeight(30).Add("Footer4"));
            for (int i = 1; i < 43; i += 2) {
                table.AddCell(new Cell().SetHeight(30).Add("Cell" + i).SetBorderBottom(new SolidBorder(Color.BLUE, 400)).SetBorderRight
                    (new SolidBorder(20)));
                table.AddCell(new Cell().SetHeight(30).Add("Cell" + (i + 1)).SetBorderBottom(new SolidBorder(Color.BLUE, 100
                    )).SetBorderLeft(new SolidBorder(20)));
            }
            table.SetSkipLastFooter(true);
            table.SetSkipFirstHeader(true);
            doc.Add(table);
            doc.Add(new Table(1).AddCell("Hello").SetBorder(new SolidBorder(Color.ORANGE, 2)));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 2)]
        public virtual void TableWithHeaderFooter03() {
            fileName = "tableWithHeaderFooterTest03.pdf";
            Document doc = CreateDocument();
            Table table = new Table(1);
            table.AddHeaderCell(new Cell().Add("Header").SetHeight(400).SetBorder(new SolidBorder(Color.BLUE, 40)));
            table.SetBorder(new SolidBorder(Color.GREEN, 100));
            doc.Add(table);
            doc.Add(new Table(1).AddCell("Hello").SetBorder(new SolidBorder(Color.MAGENTA, 5)));
            doc.Add(new AreaBreak());
            table = new Table(1);
            table.AddFooterCell(new Cell().Add("Footer").SetHeight(400).SetBorder(new SolidBorder(Color.BLUE, 40)));
            table.SetBorder(new SolidBorder(Color.GREEN, 100));
            doc.Add(table);
            doc.Add(new Table(1).AddCell("Hello").SetBorder(new SolidBorder(Color.MAGENTA, 5)));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooter04() {
            fileName = "tableWithHeaderFooterTest04.pdf";
            Document doc = CreateDocument();
            Table table = new Table(1);
            table.AddHeaderCell(new Cell().Add("Header").SetBorder(new SolidBorder(Color.BLUE, 40)));
            table.AddCell(new Cell().Add("Cell").SetBorder(new SolidBorder(Color.MAGENTA, 30)));
            table.AddFooterCell(new Cell().Add("Footer").SetBorder(new SolidBorder(Color.BLUE, 20)));
            doc.Add(table);
            doc.Add(new Table(1).AddCell("Hello").SetBorder(new SolidBorder(Color.MAGENTA, 5)));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooter05() {
            fileName = "tableWithHeaderFooterTest05.pdf";
            Document doc = CreateDocument();
            Table table = new Table(1);
            table.AddCell(new Cell().Add("Cell").SetBorder(new SolidBorder(Color.MAGENTA, 30)).SetHeight(30));
            table.AddFooterCell(new Cell().Add("Footer").SetBorder(new SolidBorder(Color.BLUE, 50)).SetHeight(30));
            table.SetBorder(new SolidBorder(100));
            table.SetSkipLastFooter(true);
            doc.Add(table);
            doc.Add(new Table(1).AddCell("Hello").SetBorder(new SolidBorder(Color.ORANGE, 5)));
            table.DeleteOwnProperty(Property.BORDER);
            doc.Add(table);
            doc.Add(new Table(1).AddCell("Hello").SetBorder(new SolidBorder(Color.ORANGE, 5)));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest06() {
            fileName = "tableWithHeaderFooterTest06.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(PageSize.A6.Rotate());
            Table table = new Table(5);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (Color.RED, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (Color.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(Color.BLUE, 0.5f)).SetBorderRight(new SolidBorder(Color
                    .BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(Color.BLUE, 2 * i + 1 > 50 ? 50 : 2 * i + 
                    1)).SetBorderTop(new SolidBorder(Color.GREEN, (50 - 2 * i + 1 >= 0) ? 50 - 2 * i + 1 : 0)).Add(new Paragraph
                    ((i + 1).ToString())));
            }
            doc.Add(table);
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest07() {
            String testName = "tableWithHeaderFooterTest07.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A7.Rotate());
            Table table = new Table(2);
            table.AddFooterCell(new Cell(1, 2).SetHeight(30).Add("Footer"));
            table.AddCell(new Cell().Add("0abcdefghijklmnopqrstuvwxyz1abcdefghijklmnopqrstuvwxyz2abcdefghijklmnopq"));
            table.AddCell(new Cell().Add("0bbbbbbbbbbbbbbbbbbbbbbbbbbbb").SetBorderBottom(new SolidBorder(50)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest08() {
            String testName = "tableWithHeaderFooterTest08.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A7.Rotate());
            Table table = new Table(2);
            table.AddFooterCell(new Cell(1, 2).SetHeight(50).Add("Footer"));
            table.AddCell(new Cell().Add("Cell1").SetHeight(50));
            table.AddCell(new Cell().Add("Cell2").SetHeight(50));
            table.SetSkipLastFooter(true);
            table.SetBorderBottom(new SolidBorder(Color.RED, 30));
            doc.Add(table);
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Hello"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest09() {
            String testName = "tableWithHeaderFooterTest09.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)), PageSize.A4.Rotate());
            Cell headerCell1 = new Cell().Add("I am header").SetBorder(new SolidBorder(Color.GREEN, 30)).SetBorderBottom
                (Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell headerCell2 = new Cell().Add("I am header").SetBorder(new SolidBorder(Color.GREEN, 30)).SetBorderBottom
                (Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell tableCell1 = new Cell().Add("I am table").SetBorder(new SolidBorder(Color.RED, 200)).SetBorderBottom(
                Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell tableCell2 = new Cell().Add("I am table").SetBorder(new SolidBorder(Color.RED, 200)).SetBorderBottom(
                Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell footerCell1 = new Cell().Add("I am footer").SetBorder(new SolidBorder(Color.GREEN, 30)).SetBorderBottom
                (Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell footerCell2 = new Cell().Add("I am footer").SetBorder(new SolidBorder(Color.GREEN, 30)).SetBorderBottom
                (Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Table table = new Table(new float[] { 350, 350 }).SetBorder(new SolidBorder(Color.BLUE, 20)).AddHeaderCell
                (headerCell1).AddHeaderCell(headerCell2).AddCell(tableCell1).AddCell(tableCell2).AddFooterCell(footerCell1
                ).AddFooterCell(footerCell2);
            table.GetHeader().SetBorderLeft(new SolidBorder(Color.MAGENTA, 40));
            table.GetFooter().SetBorderRight(new SolidBorder(Color.MAGENTA, 40));
            doc.Add(table);
            doc.Add(new AreaBreak());
            headerCell1 = new Cell().Add("I am header").SetBorder(new SolidBorder(Color.GREEN, 200)).SetBorderBottom(Border
                .NO_BORDER).SetBorderTop(Border.NO_BORDER);
            headerCell2 = new Cell().Add("I am header").SetBorder(new SolidBorder(Color.GREEN, 200)).SetBorderBottom(Border
                .NO_BORDER).SetBorderTop(Border.NO_BORDER);
            tableCell1 = new Cell().Add("I am table").SetBorder(new SolidBorder(Color.RED, 30)).SetBorderBottom(Border
                .NO_BORDER).SetBorderTop(Border.NO_BORDER);
            tableCell2 = new Cell().Add("I am table").SetBorder(new SolidBorder(Color.RED, 30)).SetBorderBottom(Border
                .NO_BORDER).SetBorderTop(Border.NO_BORDER);
            table = new Table(new float[] { 350, 350 }).SetBorder(new SolidBorder(Color.BLUE, 20)).AddHeaderCell(headerCell1
                ).AddHeaderCell(headerCell2).AddCell(tableCell1).AddCell(tableCell2);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest10() {
            String testName = "tableWithHeaderFooterTest10.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A6.Rotate());
            Table table = new Table(3);
            table.AddFooterCell(new Cell(1, 3).SetHeight(70).Add("Footer"));
            table.AddHeaderCell(new Cell(1, 3).SetHeight(30).Add("Header"));
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(i + ": Bazz :").SetBorder(new SolidBorder(Color.BLACK, 10)));
                table.AddCell(new Cell().Add("To infinity").SetBorder(new SolidBorder(Color.YELLOW, 30)));
                table.AddCell(new Cell().Add(" and beyond!").SetBorder(new SolidBorder(Color.RED, 20)));
            }
            table.SetSkipLastFooter(true);
            doc.Add(table);
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest11() {
            String testName = "tableWithHeaderFooterTest11.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(3);
            table.SetBorder(new SolidBorder(90));
            table.AddFooterCell(new Cell(1, 3).SetHeight(150).Add("Footer"));
            table.AddHeaderCell(new Cell(1, 3).SetHeight(30).Add("Header"));
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(i + ": Bazz :").SetBorder(new SolidBorder(Color.BLACK, 10)));
                table.AddCell(new Cell().Add("To infinity").SetBorder(new SolidBorder(Color.YELLOW, 30)));
                table.AddCell(new Cell().Add(" and beyond!").SetBorder(new SolidBorder(Color.RED, 20)));
            }
            table.SetSkipLastFooter(true);
            doc.Add(table);
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void ForcedPlacementTest01() {
            fileName = "forcedPlacementTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(1);
            table.SetWidth(10);
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add("1ORD");
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add("2ORD");
            cell.SetBorderTop(new SolidBorder(Color.YELLOW, 100f));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NoHorizontalBorderTest() {
            fileName = "noHorizontalBorderTest.pdf";
            Document doc = CreateDocument();
            Table mainTable = new Table(1);
            Cell cell = new Cell().SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(Color.BLACK, 0.5f));
            cell.Add("TESCHTINK");
            mainTable.AddCell(cell);
            doc.Add(mainTable);
            doc.Close();
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
