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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TableBorderTest : AbstractTableTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TableBorderTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/TableBorderTest/";

        public const String cmpPrefix = "cmp_";

//\cond DO_NOT_DOCUMENT
        internal String fileName;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal String outFileName;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal String cmpFileName;
//\endcond

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CellWithBigRowspanOnThreePagesTest() {
            fileName = "cellWithBigRowspanOnThreePagesTest.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().SetBorderCollapse(BorderCollapsePropertyValue
                .SEPARATE);
            table.AddCell(new Cell(2, 1));
            table.AddCell(new Cell(1, 1).SetHeight(2000).SetBackgroundColor(ColorConstants.RED));
            table.AddCell(new Cell(1, 1));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 2)]
        public virtual void IncompleteTableTest01() {
            fileName = "incompleteTableTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add(new Paragraph("One"));
            table.AddCell(cell);
            // row 1 and 2, cell 2
            cell = new Cell(2, 1).Add(new Paragraph("Two"));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add(new Paragraph("Three"));
            table.AddCell(cell);
            // row 3, cell 1
            cell = new Cell().Add(new Paragraph("Four"));
            table.AddCell(cell);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void IncompleteTableTest02() {
            fileName = "incompleteTableTest02.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add(new Paragraph("One"));
            table.AddCell(cell);
            table.StartNewRow();
            // row 2, cell 1
            cell = new Cell().Add(new Paragraph("Two"));
            table.AddCell(cell);
            // row 2, cell 2
            cell = new Cell().Add(new Paragraph("Three"));
            table.AddCell(cell);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        public virtual void IncompleteTableTest03() {
            fileName = "incompleteTableTest03.pdf";
            Document doc = CreateDocument();
            Table innerTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            Cell cell = new Cell().Add(new Paragraph("Inner"));
            innerTable.AddCell(cell);
            innerTable.StartNewRow();
            Table outerTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            outerTable.AddCell(innerTable);
            doc.Add(outerTable);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 2)]
        public virtual void IncompleteTableTest04() {
            fileName = "incompleteTableTest04.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("Liberte")).SetBorderBottom(new SolidBorder(ColorConstants.BLUE
                , 10)).SetHeight(40));
            table.StartNewRow();
            table.AddCell(new Cell().Add(new Paragraph("Fraternite")).SetBorderTop(new SolidBorder(ColorConstants.BLUE
                , 15)).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 15)).SetHeight(40));
            table.StartNewRow();
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest02() {
            fileName = "simpleBorderTest02.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add(new Paragraph("One"));
            cell.SetBorderTop(new SolidBorder(20));
            cell.SetBorderBottom(new SolidBorder(20));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add(new Paragraph("Two"));
            cell.SetBorderTop(new SolidBorder(30));
            cell.SetBorderBottom(new SolidBorder(40));
            table.AddCell(cell);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest03() {
            fileName = "simpleBorderTest03.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("1")));
            table.AddCell(new Cell(2, 1).Add(new Paragraph("2")));
            table.AddCell(new Cell().Add(new Paragraph("3")));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest04() {
            fileName = "simpleBorderTest04.pdf";
            Document doc = CreateDocument();
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            String textHelloWorld = "Hello World\n" + "Hello World\n" + "Hello World\n" + "Hello World\n" + "Hello World\n";
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.RED, 2f));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(textHelloWorld)));
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(new Paragraph(textByron)));
            }
            table.AddCell(new Cell(1, 2).Add(new Paragraph(textByron)));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void NoVerticalBorderTest() {
            fileName = "noVerticalBorderTest.pdf";
            Document doc = CreateDocument();
            Table mainTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            Cell cell = new Cell().SetBorder(Border.NO_BORDER).SetBorderTop(new SolidBorder(ColorConstants.BLACK, 0.5f
                ));
            cell.Add(new Paragraph("TESCHTINK"));
            mainTable.AddCell(cell);
            doc.Add(mainTable);
            doc.Add(new AreaBreak());
            mainTable.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(mainTable);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void WideBorderTest01() {
            fileName = "wideBorderTest01.pdf";
            Document doc = CreateDocument();
            doc.Add(new Paragraph("ROWS SHOULD BE THE SAME"));
            Table table = new Table(new float[] { 1, 3 });
            table.SetWidth(UnitValue.CreatePercentValue(50));
            Cell cell;
            // row 21, cell 1
            cell = new Cell().Add(new Paragraph("BORDERS"));
            table.AddCell(cell);
            // row 1, cell 2
            cell = new Cell().Add(new Paragraph("ONE"));
            cell.SetBorderLeft(new SolidBorder(ColorConstants.RED, 16f));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add(new Paragraph("BORDERS"));
            table.AddCell(cell);
            // row 2, cell 2
            cell = new Cell().Add(new Paragraph("TWO"));
            cell.SetBorderLeft(new SolidBorder(ColorConstants.RED, 16f));
            table.AddCell(cell);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void WideBorderTest02() {
            fileName = "wideBorderTest02.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, new PageSize(902, 842));
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 91f));
            Cell cell;
            cell = new Cell(1, 2).Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 70f));
            table.AddCell(cell);
            cell = new Cell(2, 1).Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 70f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 70f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.BLUE, 20f));
            table.AddCell(cell);
            for (int i = 0; i < 6; i++) {
                cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
                cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
                table.AddCell(cell);
            }
            cell = new Cell(1, 2).Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
            table.AddCell(cell);
            cell = new Cell(2, 1).Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
            table.AddCell(cell);
            for (int i = 0; i < 1 + 6; i++) {
                cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
                cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
                table.AddCell(cell);
            }
            cell = new Cell(1, 2).Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 45f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 40f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 35f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.BLUE, 5f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 45f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 64f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 102f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 11f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 12f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 44f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 27f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 16f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 59));
            table.AddCell(cell);
            for (int i = 0; i < 9; i++) {
                cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
                cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
                table.AddCell(cell);
            }
            for (int i = 0; i < 3; i++) {
                cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
                cell.SetBorder(new SolidBorder(ColorConstants.RED, 20f));
                table.AddCell(cell);
            }
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void WideBorderTest03() {
            fileName = "wideBorderTest03.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, new PageSize(842, 400));
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 90f));
            Cell cell;
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.BLUE, 20f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 120f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Borders shouldn't be layouted outside the layout area."));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 50f));
            table.AddCell(cell);
            doc.Add(table);
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(842, 520));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void WideBorderTest04() {
            fileName = "wideBorderTest04.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, new PageSize(200, 150));
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.RED, 5));
            for (int i = 0; i < 5; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell " + i)));
            }
            table.AddCell(new Cell().Add(new Paragraph("Cell 5")).SetBorderTop(new SolidBorder(ColorConstants.GREEN, 20
                )));
            doc.Add(table);
            pdfDocument.SetDefaultPageSize(new PageSize(250, 170));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BorderCollapseTest01() {
            fileName = "borderCollapseTest01.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.RED, 5));
            Cell cell;
            table.AddCell(new Cell(1, 2).Add(new Paragraph("first")).SetBorder(Border.NO_BORDER));
            cell = new Cell(1, 2).Add(new Paragraph("second"));
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BorderCollapseTest02() {
            fileName = "borderCollapseTest02.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Cell cell;
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            // first row
            // column 1
            cell = new Cell().Add(new Paragraph("1"));
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add(new Paragraph("2"));
            table.AddCell(cell);
            // second row
            // column 1
            cell = new Cell().Add(new Paragraph("3"));
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add(new Paragraph("4"));
            table.AddCell(cell);
            cell = new Cell(1, 2).Add(new Paragraph("5"));
            cell.SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BorderCollapseTest02A() {
            fileName = "borderCollapseTest02A.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Cell cell;
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            for (int i = 0; i < 3; i++) {
                // column 1
                cell = new Cell().Add(new Paragraph("1"));
                cell.SetBorder(Border.NO_BORDER);
                cell.SetPadding(0);
                cell.SetMargin(0);
                cell.SetHeight(50);
                cell.SetBackgroundColor(ColorConstants.RED);
                table.AddCell(cell);
                // column 2
                cell = new Cell().Add(new Paragraph("2"));
                cell.SetPadding(0);
                cell.SetMargin(0);
                cell.SetBackgroundColor(ColorConstants.RED);
                cell.SetHeight(50);
                cell.SetBorder(i % 2 == 1 ? Border.NO_BORDER : new SolidBorder(20));
                table.AddCell(cell);
            }
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BorderCollapseTest03() {
            fileName = "borderCollapseTest03.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Cell cell;
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            // first row
            // column 1
            cell = new Cell().Add(new Paragraph("1"));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.RED, 4));
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add(new Paragraph("2"));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.YELLOW, 5));
            table.AddCell(cell);
            // second row
            // column 1
            cell = new Cell().Add(new Paragraph("3"));
            cell.SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
            table.AddCell(cell);
            // column 2
            cell = new Cell().Add(new Paragraph("4"));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.MAGENTA, 2));
            table.AddCell(cell);
            cell = new Cell(1, 2).Add(new Paragraph("5"));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SeparatedBorderTest01A() {
            fileName = "separatedBorderTest01A.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.SEPARATE);
            table.SetBorder(new SolidBorder(ColorConstants.RED, 50));
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + i)).SetBorder(new SolidBorder(new DeviceRgb(100 * i %
                     255, 60 * i % 255, 20 * i % 255), 10 * (i % 5) + 10)));
            }
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SeparatedBorderTest01B() {
            fileName = "separatedBorderTest01B.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.SEPARATE);
            // table.setBorder(new SolidBorder(ColorConstants.RED, 5));
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + i)).SetBorder(new SolidBorder(new DeviceRgb(100 * i %
                     255, 60 * i % 255, 20 * i % 255), 10 * (i % 5) + 10)));
            }
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SeparatedBorderTest01C() {
            fileName = "separatedBorderTest01C.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.SEPARATE);
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + i)).SetBorder(new SolidBorder(new DeviceRgb(100 * i %
                     255, 60 * i % 255, 20 * i % 255), 10)));
            }
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void InfiniteLoopTest01() {
            fileName = "infiniteLoopTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3 }));
            table.SetWidth(UnitValue.CreatePercentValue(50)).SetProperty(Property.TABLE_LAYOUT, "fixed");
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add(new Paragraph("1ORD"));
            cell.SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 5));
            table.AddCell(cell);
            // row 1, cell 2
            cell = new Cell().Add(new Paragraph("ONE"));
            cell.SetBorderLeft(new SolidBorder(ColorConstants.RED, 100f));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add(new Paragraph("2ORD"));
            cell.SetBorderTop(new SolidBorder(ColorConstants.YELLOW, 100f));
            table.AddCell(cell);
            // row 2, cell 2
            cell = new Cell().Add(new Paragraph("TWO"));
            cell.SetBorderLeft(new SolidBorder(ColorConstants.RED, 0.5f));
            table.AddCell(cell);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

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
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorderTop(new DottedBorder(ColorConstants.MAGENTA, 3f));
            table.SetBorderRight(new DottedBorder(ColorConstants.RED, 3f));
            table.SetBorderBottom(new DottedBorder(ColorConstants.BLUE, 3f));
            table.SetBorderLeft(new DottedBorder(ColorConstants.GRAY, 3f));
            Cell cell;
            cell = new Cell().Add(new Paragraph("Some text"));
            cell.SetBorderRight(new SolidBorder(ColorConstants.RED, 2f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Some text"));
            cell.SetBorderLeft(new SolidBorder(ColorConstants.GREEN, 4f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph(longText));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.RED, 5f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Hello"));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 5f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Some text."));
            cell.SetBorderTop(new SolidBorder(ColorConstants.GREEN, 6f));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("World"));
            cell.SetBorderTop(new SolidBorder(ColorConstants.YELLOW, 6f));
            table.AddCell(cell);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest02() {
            fileName = "splitCellsTest02.pdf";
            Document doc = CreateDocument();
            String text = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n";
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            Cell cell;
            for (int i = 0; i < 38; i++) {
                cell = new Cell().Add(new Paragraph(text));
                cell.SetBorder(new SolidBorder(ColorConstants.RED, 2f));
                table.AddCell(cell);
            }
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorder(new SolidBorder(ColorConstants.YELLOW, 3));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.DeleteOwnProperty(Property.BORDER);
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorder(new SolidBorder(ColorConstants.YELLOW, 3));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void SplitCellsTest03() {
            fileName = "splitCellsTest03.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(100, 160));
            String textAlphabet = "ABCDEFGHIJKLMNOPQRSTUWXYZ";
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(textAlphabet)).SetBorder(new SolidBorder(4)));
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")));
            doc.Add(table);
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(140, 200));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest04() {
            // TODO (DEVSIX-1734 Run commented snippet to produce a bug.)
            fileName = "splitCellsTest04.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 100 + 72));
            String text = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "A\n" + "B\n" + "C\n" + "D";
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            Cell cell;
            cell = new Cell().Add(new Paragraph(text));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.RED, 20));
            cell.SetBorderTop(new SolidBorder(ColorConstants.GREEN, 20));
            table.AddCell(cell);
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetBorderTop(new SolidBorder(ColorConstants.YELLOW
                , 20)));
            doc.Add(table);
            //        doc.add(new AreaBreak());
            //        table.setBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            //        doc.add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1734")]
        public virtual void SplitCellsTest04A() {
            fileName = "splitCellsTest04A.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 80 + 72));
            String text = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "A\n" + "B\n" + "C\n" + "D";
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            Cell cell;
            cell = new Cell().Add(new Paragraph(text));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.RED, 20));
            cell.SetBorderTop(new SolidBorder(ColorConstants.GREEN, 20));
            table.AddCell(cell);
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetBorderTop(new SolidBorder(ColorConstants.YELLOW
                , 20)));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest05() {
            fileName = "splitCellsTest05.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(130, 150));
            String text = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(text)));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(text)));
            table.AddCell(new Cell().Add(new Paragraph(text)));
            table.AddCell(new Cell().Add(new Paragraph(text)));
            table.AddCell(new Cell().Add(new Paragraph(text)));
            doc.Add(table);
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(196, 192));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest06() {
            fileName = "splitCellsTest06.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(300, 150));
            doc.Add(new Paragraph("No more"));
            doc.Add(new Paragraph("place"));
            doc.Add(new Paragraph("here"));
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            Cell cell = new Cell(3, 1);
            cell.Add(new Paragraph("G"));
            cell.Add(new Paragraph("R"));
            cell.Add(new Paragraph("P"));
            table.AddCell(cell);
            table.AddCell("middle row 1");
            cell = new Cell(3, 1);
            cell.Add(new Paragraph("A"));
            cell.Add(new Paragraph("B"));
            cell.Add(new Paragraph("C"));
            table.AddCell(cell);
            table.AddCell("middle row 2");
            table.AddCell("middle row 3");
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("No more"));
            doc.Add(new Paragraph("place"));
            doc.Add(new Paragraph("here"));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest07() {
            fileName = "splitCellsTest07.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(133, 180));
            String text = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(text + "1")));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(text + "222")));
            table.AddCell(new Cell().Add(new Paragraph(text + "3")));
            table.AddCell(new Cell().Add(new Paragraph(text + "4")).SetKeepTogether(true));
            table.AddCell(new Cell().Add(new Paragraph(text + "5")).SetKeepTogether(true));
            table.SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 1));
            doc.Add(table);
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(193, 240));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest08() {
            fileName = "splitCellsTest08.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(134, 140));
            String text = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(text + "1")));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(text + "2")).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 1)));
            table.AddCell(new Cell().Add(new Paragraph(text + "3")));
            table.AddCell(new Cell().Add(new Paragraph(text + "4")));
            table.AddCell(new Cell().Add(new Paragraph(text + "5")));
            doc.Add(table);
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(204, 160));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest09() {
            fileName = "splitCellsTest09.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 160));
            String text = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("Make Gretzky great again! Make Gretzky great again! Make Gretzky great again! Make Gretzky great again! Make Gretzky great again! Make Gretzky great again!"
                )));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(text + "3")));
            table.AddCell(new Cell().Add(new Paragraph(text + "4")).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)
                ));
            table.AddCell(new Cell().Add(new Paragraph(text + "5")));
            table.AddCell(new Cell().Add(new Paragraph(text + "5")));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest10() {
            fileName = "splitCellsTest10.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(136, 142));
            String text = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(text + "1")).SetBackgroundColor(ColorConstants.YELLOW));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(text + "222222222")).SetBackgroundColor(ColorConstants.YELLOW
                ));
            table.AddCell(new Cell().Add(new Paragraph(text + "3")).SetBackgroundColor(ColorConstants.YELLOW));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(text + "4")).SetKeepTogether
                (true));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(text + "5")).SetKeepTogether
                (true));
            table.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest10A() {
            fileName = "splitCellsTest10A.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(130, 140));
            String textAlphabet = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(textAlphabet + "1")).SetBackgroundColor(ColorConstants.YELLOW));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(textAlphabet + "222222222")).SetBackgroundColor(ColorConstants
                .YELLOW));
            table.AddCell(new Cell().Add(new Paragraph(textAlphabet + "3")).SetBackgroundColor(ColorConstants.YELLOW));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(textAlphabet + "4")).
                SetKeepTogether(true));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(textAlphabet + "5")).
                SetKeepTogether(true));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1736")]
        public virtual void SplitCellsTest10B() {
            fileName = "splitCellsTest10B.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(130, 110));
            String textAlphabet = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(textAlphabet + "1")).SetBackgroundColor(ColorConstants.YELLOW));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(textAlphabet + "222222222")).SetBackgroundColor(ColorConstants
                .YELLOW));
            table.AddCell(new Cell().Add(new Paragraph(textAlphabet + "3")).SetBackgroundColor(ColorConstants.YELLOW));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(textAlphabet + "4")).
                SetKeepTogether(true));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(textAlphabet + "5")).
                SetKeepTogether(true));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellsTest10C() {
            fileName = "splitCellsTest10C.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(136, 142));
            String text = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddCell(new Cell().Add(new Paragraph(text + "1")).SetBackgroundColor(ColorConstants.YELLOW).SetBorderBottom
                (new SolidBorder(ColorConstants.BLACK, 10)));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(text + "222222222")).SetBackgroundColor(ColorConstants.YELLOW
                ).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 10)));
            table.AddCell(new Cell().Add(new Paragraph(text + "3")).SetBackgroundColor(ColorConstants.YELLOW).SetBorderBottom
                (new SolidBorder(ColorConstants.BLACK, 10)));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(text + "4")).SetKeepTogether
                (true).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 10)));
            table.AddCell(new Cell().SetBackgroundColor(ColorConstants.YELLOW).Add(new Paragraph(text + "5")).SetKeepTogether
                (true).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 10)));
            table.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            doc.Add(table);
            // TODO DEVSIX-1736: Set pagesize as 236x162 to produce a NPE
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(236, 222));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableAndCellBordersCollapseTest01() {
            // TODO DEVSIX-5834 Consider this test when deciding on the strategy:
            //  left-bottom corner could be magenta as in Chrome
            fileName = "tableAndCellBordersCollapseTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 100));
            table.AddCell(new Cell().Add(new Paragraph("Hello World")).SetBorderBottom(new SolidBorder(ColorConstants.
                MAGENTA, 100)));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest01() {
            fileName = "tableWithHeaderFooterTest01.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(1000, 1000));
            String text = "Cell";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(new Paragraph(text + "1")).SetHeight(40).SetBorderBottom(new SolidBorder(ColorConstants
                    .MAGENTA, 100)));
                table.AddCell(new Cell().Add(new Paragraph(text + "4")).SetHeight(40).SetBorderBottom(new SolidBorder(ColorConstants
                    .MAGENTA, 100)));
                table.AddCell(new Cell().Add(new Paragraph(text + "5")).SetHeight(40).SetBorderBottom(new SolidBorder(ColorConstants
                    .MAGENTA, 100)));
            }
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header")).SetHeight(40));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetHeight(40));
            }
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 100));
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                ("Hello"))).SetBorder(new SolidBorder(ColorConstants.BLACK, 10)));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                ("Hello"))).SetBorder(new SolidBorder(ColorConstants.BLACK, 10)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest02() {
            // TODO DEVSIX-5864 footer's top border / body's bottom border should be drawn by footer
            fileName = "tableWithHeaderFooterTest02.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 1500));
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().SetHeight(30).Add(new Paragraph("Header1")).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 100)));
            table.AddHeaderCell(new Cell().SetHeight(30).Add(new Paragraph("Header2")).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 200)));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer1")).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 100)));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer2")).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 200)));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer3")));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer4")));
            for (int i = 1; i < 43; i += 2) {
                table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Cell" + i)).SetBorderBottom(new SolidBorder(ColorConstants
                    .BLUE, 400)).SetBorderRight(new SolidBorder(20)));
                table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Cell" + (i + 1))).SetBorderBottom(new SolidBorder
                    (ColorConstants.BLUE, 100)).SetBorderLeft(new SolidBorder(20)));
            }
            table.SetSkipLastFooter(true);
            table.SetSkipFirstHeader(true);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.ORANGE, 2)));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.ORANGE, 2)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest02A() {
            // TODO DEVSIX-5864 footer's top border / body's bottom border should be drawn by footer
            fileName = "tableWithHeaderFooterTest02A.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(595, 1500));
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            for (int i = 1; i < 2; i += 2) {
                table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Cell" + i)).SetBorderBottom(new SolidBorder(ColorConstants
                    .BLUE, 400)).SetBorderRight(new SolidBorder(20)));
                table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Cell" + (i + 1))).SetBorderBottom(new SolidBorder
                    (ColorConstants.BLUE, 100)).SetBorderLeft(new SolidBorder(20)));
            }
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer1")).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 100)));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer2")).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 200)));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest03() {
            fileName = "tableWithHeaderFooterTest03.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header")).SetHeight(400).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 40)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 100));
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.MAGENTA, 5)));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.MAGENTA, 5)));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetHeight(400).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 40)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 100));
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.MAGENTA, 5)));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.MAGENTA, 5)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest04() {
            fileName = "tableWithHeaderFooterTest04.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header")).SetBorder(new SolidBorder(ColorConstants.BLUE, 
                40)));
            table.AddCell(new Cell().Add(new Paragraph("Cell")).SetBorder(new SolidBorder(ColorConstants.MAGENTA, 30))
                );
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetBorder(new SolidBorder(ColorConstants.BLUE, 
                20)));
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.MAGENTA, 5)));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.MAGENTA, 5)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest05() {
            fileName = "tableWithHeaderFooterTest05.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("Cell")).SetBorder(new SolidBorder(ColorConstants.MAGENTA, 30))
                .SetHeight(30));
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetBorder(new SolidBorder(ColorConstants.BLUE, 
                50)).SetHeight(30));
            table.SetBorder(new SolidBorder(100));
            table.SetSkipLastFooter(true);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.ORANGE, 5)));
            table.DeleteOwnProperty(Property.BORDER);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.ORANGE, 5)));
            doc.Add(new AreaBreak());
            table.SetBorder(new SolidBorder(100));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.ORANGE, 5)));
            table.DeleteOwnProperty(Property.BORDER);
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell("Hello").SetBorder(new SolidBorder
                (ColorConstants.ORANGE, 5)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 10)]
        public virtual void TableWithHeaderFooterTest06() {
            fileName = "tableWithHeaderFooterTest06.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(PageSize.A6.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (ColorConstants.RED, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 2 * i 
                    + 1 > 50 ? 50 : 2 * i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
            }
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 10)]
        public virtual void TableWithHeaderFooterTest06A() {
            fileName = "tableWithHeaderFooterTest06A.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, PageSize.A6.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (ColorConstants.RED, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 2 * i 
                    + 1 > 50 ? 50 : 2 * i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
            }
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void VerticalBordersInfluenceHorizontalTopAndBottomBordersTest() {
            fileName = "verticalBordersInfluenceHorizontalTopAndbottomBordersTest.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, PageSize.A6.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (ColorConstants.RED, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 1));
            table.AddFooterCell(cell);
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 20)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 20)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.RED, 50 - 2 * 
                    i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, 50 - 2 * i + 1)).Add(new Paragraph((i + 1).
                    ToString())));
            }
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 10)]
        public virtual void TableWithHeaderFooterTest06B() {
            fileName = "tableWithHeaderFooterTest06B.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument, PageSize.A6.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (ColorConstants.RED, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderTop(new SolidBorder(ColorConstants.BLUE, 2 * i + 1
                     > 50 ? 50 : 2 * i + 1)).SetBorderBottom(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
            }
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest07() {
            fileName = "tableWithHeaderFooterTest07.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + "cmp_" + fileName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A7.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            table.AddFooterCell(new Cell(1, 2).SetHeight(30).Add(new Paragraph("Footer")));
            table.AddCell(new Cell().Add(new Paragraph("0abcdefghijklmnopqrstuvwxyz1abcdefghijklmnopqrstuvwxyz2abcdefghijklmnopq"
                )));
            table.AddCell(new Cell().Add(new Paragraph("0bbbbbbbbbbbbbbbbbbbbbbbbbbbb")).SetBorderBottom(new SolidBorder
                (50)));
            doc.Add(table);
            pdfDoc.SetDefaultPageSize(new PageSize(298, 250));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest08() {
            fileName = "tableWithHeaderFooterTest08.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + "cmp_" + fileName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A7.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.AddFooterCell(new Cell(1, 2).SetHeight(50).Add(new Paragraph("Footer")));
            table.AddCell(new Cell().Add(new Paragraph("Cell1")).SetHeight(50));
            table.AddCell(new Cell().Add(new Paragraph("Cell2")).SetHeight(50));
            table.SetSkipLastFooter(true);
            table.SetBorderBottom(new SolidBorder(ColorConstants.RED, 30));
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest09() {
            fileName = "tableWithHeaderFooterTest09.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + "cmp_" + fileName;
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)), PageSize.A3.Rotate());
            Cell headerCell1 = new Cell().Add(new Paragraph("I am header")).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 30)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell headerCell2 = new Cell().Add(new Paragraph("I am header")).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 30)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell tableCell1 = new Cell().Add(new Paragraph("I am table")).SetBorder(new SolidBorder(ColorConstants.RED
                , 200)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell tableCell2 = new Cell().Add(new Paragraph("I am table")).SetBorder(new SolidBorder(ColorConstants.RED
                , 200)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell footerCell1 = new Cell().Add(new Paragraph("I am footer")).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 30)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Cell footerCell2 = new Cell().Add(new Paragraph("I am footer")).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 30)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            Table table = new Table(new float[] { 350, 350 }).SetBorder(new SolidBorder(ColorConstants.BLUE, 20)).AddHeaderCell
                (headerCell1).AddHeaderCell(headerCell2).AddCell(tableCell1).AddCell(tableCell2).AddFooterCell(footerCell1
                ).AddFooterCell(footerCell2);
            table.GetHeader().SetBorderLeft(new SolidBorder(ColorConstants.MAGENTA, 40));
            table.GetFooter().SetBorderRight(new SolidBorder(ColorConstants.MAGENTA, 40));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            doc.Add(new AreaBreak());
            headerCell1 = new Cell().Add(new Paragraph("I am header")).SetBorder(new SolidBorder(ColorConstants.GREEN, 
                200)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            headerCell2 = new Cell().Add(new Paragraph("I am header")).SetBorder(new SolidBorder(ColorConstants.GREEN, 
                200)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            tableCell1 = new Cell().Add(new Paragraph("I am table")).SetBorder(new SolidBorder(ColorConstants.RED, 30)
                ).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            tableCell2 = new Cell().Add(new Paragraph("I am table")).SetBorder(new SolidBorder(ColorConstants.RED, 30)
                ).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            table = new Table(new float[] { 350, 350 }).SetBorder(new SolidBorder(ColorConstants.BLUE, 20)).AddHeaderCell
                (headerCell1).AddHeaderCell(headerCell2).AddCell(tableCell1).AddCell(tableCell2);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest10() {
            fileName = "tableWithHeaderFooterTest10.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + "cmp_" + fileName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(380, 300));
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.AddFooterCell(new Cell(1, 3).SetHeight(70).Add(new Paragraph("Footer")));
            table.AddHeaderCell(new Cell(1, 3).SetHeight(30).Add(new Paragraph("Header")));
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i + ": Bazz :")).SetBorder(new SolidBorder(ColorConstants.BLACK
                    , 10)));
                table.AddCell(new Cell().Add(new Paragraph("To infinity")).SetBorder(new SolidBorder(ColorConstants.YELLOW
                    , 30)));
                table.AddCell(new Cell().Add(new Paragraph(" and beyond!")).SetBorder(new SolidBorder(ColorConstants.RED, 
                    20)));
            }
            table.SetSkipLastFooter(true);
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            pdfDoc.SetDefaultPageSize(new PageSize(480, 350));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest11() {
            fileName = "tableWithHeaderFooterTest11.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(90));
            table.AddFooterCell(new Cell(1, 3).SetHeight(150).Add(new Paragraph("Footer")));
            table.AddHeaderCell(new Cell(1, 3).SetHeight(30).Add(new Paragraph("Header")));
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i + ": Bazz :")).SetBorder(new SolidBorder(ColorConstants.BLACK
                    , 10)));
                table.AddCell(new Cell().Add(new Paragraph("To infinity")).SetBorder(new SolidBorder(ColorConstants.YELLOW
                    , 30)));
                table.AddCell(new Cell().Add(new Paragraph(" and beyond!")).SetBorder(new SolidBorder(ColorConstants.RED, 
                    20)));
            }
            table.SetSkipLastFooter(true);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(695, 842));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest11A() {
            String testName = "tableWithHeaderFooterTest11A.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(90));
            table.AddFooterCell(new Cell(1, 3).SetHeight(150).Add(new Paragraph("Footer")));
            table.AddHeaderCell(new Cell(1, 3).SetHeight(30).Add(new Paragraph("Header")));
            for (int i = 0; i < 11; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i + ": Bazz :")).SetBorder(new SolidBorder(ColorConstants.BLACK
                    , 10)));
                table.AddCell(new Cell().Add(new Paragraph("To infinity")).SetBorder(new SolidBorder(ColorConstants.YELLOW
                    , 30)));
                table.AddCell(new Cell().Add(new Paragraph(" and beyond!")).SetBorder(new SolidBorder(ColorConstants.RED, 
                    20)));
            }
            table.SetSkipLastFooter(true);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(695, 842));
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest12() {
            fileName = "tableWithHeaderFooterTest12.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().SetHeight(30).Add(new Paragraph("Header")).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 5)));
            table.AddHeaderCell(new Cell().SetHeight(30).Add(new Paragraph("Header")).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 35)));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer")).SetBorder(new SolidBorder(ColorConstants
                .YELLOW, 20)));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer")).SetBorder(new SolidBorder(ColorConstants
                .YELLOW, 20)));
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Ignore("DEVSIX-1219")]
        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest13() {
            fileName = "tableWithHeaderFooterTest13.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().SetHeight(30).Add(new Paragraph("Header")).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 5)));
            table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Make Gretzky great again!")).SetBorder(Border.NO_BORDER
                ));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer")).SetBorder(new SolidBorder(ColorConstants
                .YELLOW, 5)));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Make Gretzky great again!")).SetBorderLeft(Border
                .NO_BORDER).SetBorderRight(Border.NO_BORDER));
            table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Make Gretzky great again!")).SetBorderLeft(new SolidBorder
                (ColorConstants.GREEN, 0.5f)).SetBorderRight(new SolidBorder(ColorConstants.RED, 0.5f)));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest14() {
            fileName = "tableWithHeaderFooterTest14.pdf";
            Document doc = CreateDocument();
            Table table = new Table(new float[3]);
            for (int r = 0; r < 1; r++) {
                for (int c = 0; c < 3; c++) {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(MessageFormatUtil.Format("header row {0} col {1}", r, c))
                        ).SetBorder(Border.NO_BORDER));
                }
            }
            for (int r = 0; r < 3; r++) {
                for (int c = 0; c < 3; c++) {
                    table.AddCell(new Cell().Add(new Paragraph(MessageFormatUtil.Format("row {0} col {1}", r, c))).SetBorder(Border
                        .NO_BORDER));
                }
            }
            for (int r = 0; r < 1; r++) {
                for (int c = 0; c < 3; c++) {
                    table.AddFooterCell(new Cell().Add(new Paragraph(MessageFormatUtil.Format("footer row {0} col {1}", r, c))
                        ).SetBorder(Border.NO_BORDER));
                }
            }
            table.GetHeader().SetBorderTop(new SolidBorder(2)).SetBorderBottom(new SolidBorder(1));
            table.GetFooter().SetBold().SetBorderTop(new SolidBorder(10)).SetBorderBottom(new SolidBorder(1)).SetBackgroundColor
                (ColorConstants.LIGHT_GRAY);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest15() {
            fileName = "tableWithHeaderFooterTest15.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().SetHeight(30).Add(new Paragraph("Header")).SetBorder(new DottedBorder(ColorConstants
                .RED, 20)));
            table.AddCell(new Cell().SetHeight(30).Add(new Paragraph("Body")).SetBorder(new DottedBorder(ColorConstants
                .GREEN, 20)));
            table.AddFooterCell(new Cell().SetHeight(30).Add(new Paragraph("Footer")).SetBorder(new DottedBorder(ColorConstants
                .BLUE, 20)));
            table.SetBackgroundColor(ColorConstants.MAGENTA);
            table.GetHeader().SetBackgroundColor(ColorConstants.ORANGE);
            table.GetFooter().SetBackgroundColor(ColorConstants.ORANGE);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest16() {
            fileName = "tableWithHeaderFooterTest16.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header 1")).SetBorderBottom(new SolidBorder(ColorConstants
                .RED, 25)).SetBorderTop(new SolidBorder(ColorConstants.ORANGE, 27)));
            table.GetHeader().AddHeaderCell("Header 2");
            table.AddCell(new Cell().Add(new Paragraph("Body 1")).SetBorderTop(new SolidBorder(ColorConstants.GREEN, 20
                )));
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer 1")).SetBorderTop(new SolidBorder(ColorConstants.
                RED, 25)).SetBorderBottom(new SolidBorder(ColorConstants.ORANGE, 27)));
            table.GetFooter().AddFooterCell("Footer 2");
            table.SetBorderTop(new SolidBorder(ColorConstants.BLUE, 30)).SetBorderBottom(new SolidBorder(ColorConstants
                .BLUE, 30));
            table.GetFooter().SetBorderBottom(new SolidBorder(ColorConstants.YELLOW, 50));
            table.GetHeader().SetBorderTop(new SolidBorder(ColorConstants.YELLOW, 50));
            table.SetBackgroundColor(ColorConstants.MAGENTA);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest17() {
            fileName = "tableWithHeaderFooterTest17.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(300, 300));
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(40));
            table.AddFooterCell(new Cell().SetHeight(50).Add(new Paragraph("Footer")));
            for (int i = 0; i < 3; i++) {
                table.AddCell(new Cell().SetHeight(50).Add(new Paragraph("Cell" + i)));
            }
            table.SetSkipLastFooter(true);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest18() {
            fileName = "tableWithHeaderFooterTest18.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(300, 400));
            // only footer
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(50));
            table.AddFooterCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Footer")));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            // only header
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(10));
            table.AddHeaderCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Header")).SetBackgroundColor(ColorConstants
                .YELLOW));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            // only header and footer
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorderTop(new SolidBorder(ColorConstants.BLUE, 40));
            table.SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 40));
            table.AddHeaderCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Header")));
            table.AddFooterCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Footer")));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderFooterTest19() {
            fileName = "tableWithHeaderFooterTest19.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(300, 400));
            // footer and body
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Cell")));
            table.AddFooterCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Footer")));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            // header and body
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Cell")));
            table.AddHeaderCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Header")));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            // header, footer and body
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Cell")));
            table.AddHeaderCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Header")));
            table.AddFooterCell(new Cell(1, 1).SetHeight(50).Add(new Paragraph("Footer")));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void TableWithHeaderFooterTest20() {
            fileName = "tableWithHeaderFooterTest20.pdf";
            Document doc = CreateDocument();
            doc.GetPdfDocument().SetDefaultPageSize(new PageSize(300, 115 + 72));
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(10));
            table.AddFooterCell(new Cell().SetHeight(50).Add(new Paragraph("Footer")));
            table.AddCell(new Cell().Add(new Paragraph("Cell")).SetHeight(50));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void CellBorderPriorityTest() {
            fileName = "cellBorderPriorityTest.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            Cell cell = new Cell().Add(new Paragraph("Hello"));
            cell.SetBorderTop(new SolidBorder(ColorConstants.RED, 50));
            cell.SetBorderRight(new SolidBorder(ColorConstants.GREEN, 50));
            cell.SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 50));
            cell.SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 50));
            cell.SetHeight(100).SetWidth(100);
            for (int i = 0; i < 9; i++) {
                table.AddCell(cell.Clone(true));
            }
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void CellBorderPriorityTest02() {
            fileName = "cellBorderPriorityTest02.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            Color[] array = new Color[] { ColorConstants.RED, ColorConstants.GREEN, ColorConstants.BLUE, ColorConstants
                .RED, ColorConstants.GREEN, ColorConstants.BLUE };
            for (int i = 0; i < 3; i++) {
                Cell cell = new Cell().Add(new Paragraph("Hello"));
                cell.SetBorder(new SolidBorder(array[i], 50));
                table.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Hello"));
                cell.SetBorder(new SolidBorder(array[i + 1], 50));
                table.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Hello"));
                cell.SetBorder(new SolidBorder(array[i + 2], 50));
                table.AddCell(cell);
            }
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void CellsBorderPriorityTest() {
            fileName = "cellsBorderPriorityTest.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(2));
            Cell cell = new Cell().Add(new Paragraph("1"));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 20));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("2"));
            cell.SetBorder(new SolidBorder(ColorConstants.GREEN, 20));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("3"));
            cell.SetBorder(new SolidBorder(ColorConstants.BLUE, 20));
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("4"));
            cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 20));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void TableBorderPriorityTest() {
            fileName = "tableBorderPriorityTest.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorderTop(new SolidBorder(ColorConstants.RED, 20));
            table.SetBorderRight(new SolidBorder(ColorConstants.GREEN, 20));
            table.SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 20));
            table.SetBorderLeft(new SolidBorder(ColorConstants.BLACK, 20));
            Cell cell = new Cell().Add(new Paragraph("Hello"));
            table.AddCell(cell);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void SplitRowspanKeepTogetherTest() {
            fileName = "splitRowspanKeepTogetherTest.pdf";
            Document doc = CreateDocument();
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetKeepTogether(true);
            int bigRowspan = 8;
            table.AddCell(new Cell(bigRowspan, 1).Add(new Paragraph("Big cell")).SetBorder(new SolidBorder(ColorConstants
                .GREEN, 20)));
            for (int i = 0; i < bigRowspan; i++) {
                table.AddCell(i + " " + textByron);
            }
            doc.Add(new Paragraph("Try to break me!"));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 4)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RECTANGLE_HAS_NEGATIVE_SIZE, Count = 2)]
        public virtual void ForcedPlacementTest01() {
            fileName = "forcedPlacementTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetWidth(10).SetProperty(Property.TABLE_LAYOUT, "fixed");
            Cell cell;
            // row 1, cell 1
            cell = new Cell().Add(new Paragraph("1ORD"));
            table.AddCell(cell);
            // row 2, cell 1
            cell = new Cell().Add(new Paragraph("2ORD"));
            cell.SetBorderTop(new SolidBorder(ColorConstants.YELLOW, 100f));
            table.AddCell(cell);
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void NoHorizontalBorderTest() {
            fileName = "noHorizontalBorderTest.pdf";
            Document doc = CreateDocument();
            Table mainTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            Cell cell = new Cell().SetBorder(Border.NO_BORDER).SetBorderRight(new SolidBorder(ColorConstants.BLACK, 0.5f
                ));
            cell.Add(new Paragraph("TESCHTINK"));
            mainTable.AddCell(cell);
            doc.Add(mainTable);
            doc.Add(new AreaBreak());
            mainTable.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            mainTable.SetHorizontalBorderSpacing(20);
            mainTable.SetVerticalBorderSpacing(20);
            doc.Add(mainTable);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BordersWithSpansTest01() {
            fileName = "bordersWithSpansTest01.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(10)).UseAllAvailableWidth();
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.AddCell(new Cell(1, 3).Add(new Paragraph(1 + "_" + 3 + "_")));
            table.AddCell(new Cell(1, 7).Add(new Paragraph(1 + "_" + 7 + "_")));
            table.AddCell(new Cell(6, 1).Add(new Paragraph(6 + "_" + 1 + "_")));
            table.AddCell(new Cell(6, 9).Add(new Paragraph(6 + "_" + 9 + "_")));
            table.FlushContent();
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BordersWithSpansTest02() {
            fileName = "bordersWithSpansTest02.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("Liberte")).SetBorder(new SolidBorder(ColorConstants.MAGENTA, 1
                )));
            table.AddCell(new Cell().Add(new Paragraph("Egalite")));
            table.AddCell(new Cell(3, 1).Add(new Paragraph("Fra")).SetBorder(new SolidBorder(ColorConstants.GREEN, 2))
                );
            table.AddCell(new Cell(2, 1).Add(new Paragraph("ter")).SetBorder(new SolidBorder(ColorConstants.YELLOW, 2)
                ));
            table.AddCell(new Cell().Add(new Paragraph("nite")).SetBorder(new SolidBorder(ColorConstants.CYAN, 5)));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void BordersWithSpansTest03() {
            fileName = "bordersWithSpansTest03.pdf";
            Document doc = CreateDocument();
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.AddCell(new Cell(6, 1).Add(new Paragraph("Fra")).SetBorder(new SolidBorder(ColorConstants.ORANGE, 10
                )));
            table.AddCell(new Cell().Add(new Paragraph("Liberte")).SetBorder(new SolidBorder(ColorConstants.MAGENTA, 1
                )));
            table.AddCell(new Cell().Add(new Paragraph("Egalite")));
            table.AddCell(new Cell(5, 1).Add(new Paragraph("ter")).SetBorder(new SolidBorder(ColorConstants.GREEN, 2))
                );
            table.AddCell(new Cell(2, 1).Add(new Paragraph("ni")).SetBorder(new SolidBorder(ColorConstants.YELLOW, 2))
                );
            table.AddCell(new Cell(3, 1).Add(new Paragraph("te")).SetBorder(new SolidBorder(ColorConstants.CYAN, 5)));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void HeaderTopBorderTest01() {
            fileName = "headerTopBorderTest01.pdf";
            Document doc = CreateDocument();
            for (int i = 0; i < 29; ++i) {
                doc.Add(new Paragraph("aaaaaaaaaaaa"));
            }
            Table table = new Table(new float[] { 50, 50 }).SetBorder(new SolidBorder(1));
            table.AddHeaderCell(new Cell().Add(new Paragraph("h")).SetBorderTop(Border.NO_BORDER));
            table.AddHeaderCell(new Cell().Add(new Paragraph("h")).SetBorderTop(Border.NO_BORDER));
            for (int i = 0; i < 4; ++i) {
                table.AddCell(new Cell().Add(new Paragraph("aa")).SetBorder(Border.NO_BORDER));
            }
            doc.Add(table);
            doc.Add(new Paragraph("Correct result:"));
            doc.Add(table);
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void EqualBordersSameInstancesTest() {
            fileName = "equalBordersSameInstancesTest.pdf";
            Document doc = CreateDocument();
            Border border = new SolidBorder(ColorConstants.RED, 20);
            int colNum = 4;
            Table table = new Table(UnitValue.CreatePercentArray(colNum)).UseAllAvailableWidth();
            int rowNum = 4;
            for (int i = 0; i < rowNum; i++) {
                for (int j = 0; j < colNum; j++) {
                    table.AddCell(new Cell().Add(new Paragraph("Cell: " + i + ", " + j)).SetBorder(border));
                }
            }
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                ("Hello"))).SetBorder(new SolidBorder(ColorConstants.BLACK, 10)));
            CloseDocumentAndCompareOutputs(doc);
        }

        [NUnit.Framework.Test]
        public virtual void VerticalMiddleBorderTest() {
            String testName = "verticalMiddleBorderTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            for (int i = 0; i < 4; i++) {
                if (i % 2 == 1) {
                    Cell cell = new Cell().Add(new Paragraph("Left Cell " + i)).SetBorderLeft(new SolidBorder(ColorConstants.GREEN
                        , 20));
                    table.AddCell(cell);
                }
                else {
                    table.AddCell("Right cell " + i);
                }
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
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
    }
}
