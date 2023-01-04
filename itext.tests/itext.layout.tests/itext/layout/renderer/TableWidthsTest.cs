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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class TableWidthsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestprocessCellRemainWidth01() {
            TableRenderer tableRenderer = CreateTableRendererWithDiffColspan(150);
            TableWidths tableWidths = new TableWidths(tableRenderer, 150, true, 15, 15);
            IList<TableWidths.CellInfo> cells = tableWidths.AutoLayoutCustom();
            foreach (TableWidths.CellInfo cell in cells) {
                tableWidths.ProcessCell(cell);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestProcessCellsRemainWidth02() {
            TableRenderer tableRenderer = CreateTableRendererWithDiffColspan(320);
            TableWidths tableWidths = new TableWidths(tableRenderer, 150, true, 15, 15);
            IList<TableWidths.CellInfo> cells = tableWidths.AutoLayoutCustom();
            foreach (TableWidths.CellInfo cell in cells) {
                tableWidths.ProcessCell(cell);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SUM_OF_TABLE_COLUMNS_IS_GREATER_THAN_100, Count = 2)]
        public virtual void TestSumOfColumnsIsGreaterThan100() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 1 }));
            table.SetWidth(UnitValue.CreatePercentValue(80));
            table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            Cell c1 = new Cell(1, 3);
            c1.SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(200));
            c1.Add(new Paragraph("Cell with colspan 3"));
            table.AddCell(c1);
            table.AddCell(new Cell(2, 1).Add(new Paragraph("Cell with rowspan 2")));
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 1")).SetMinWidth(200));
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 2")).SetMaxWidth(50));
            table.AddCell("row 2; cell 1");
            table.AddCell("row 2; cell 2");
            TableRenderer tableRenderer = new TableRenderer(table);
            CellRenderer[] row1 = new CellRenderer[] { new CellRenderer(table.GetCell(0, 0)), null, null };
            CellRenderer[] row2 = new CellRenderer[] { null, new CellRenderer(table.GetCell(1, 1)), new CellRenderer(table
                .GetCell(1, 2)) };
            CellRenderer[] row3 = new CellRenderer[] { new CellRenderer(table.GetCell(1, 0)), new CellRenderer(table.GetCell
                (2, 1)), new CellRenderer(table.GetCell(2, 2)) };
            tableRenderer.rows[0] = row1;
            tableRenderer.rows[1] = row2;
            tableRenderer.rows[2] = row3;
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            tableRenderer.bordersHandler = new SeparatedTableBorders(tableRenderer.rows, table.GetNumberOfColumns(), tableRenderer
                .GetBorders());
            TableWidths tableWidths = new TableWidths(tableRenderer, 150, true, 15, 15);
            IList<TableWidths.CellInfo> cells = tableWidths.AutoLayoutCustom();
            foreach (TableWidths.CellInfo cell in cells) {
                tableWidths.ProcessCell(cell);
            }
            doc.Add(table);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestProcessCellPointWidthValue() {
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetMarginTop(5);
            for (int i = 0; i < 4; i++) {
                Cell cell = new Cell().Add(new Paragraph("smth" + i));
                cell.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(250));
                table.AddCell(cell);
            }
            TableRenderer tableRenderer = new TableRenderer(table);
            CellRenderer[] row1 = new CellRenderer[] { new CellRenderer(table.GetCell(0, 0)), new CellRenderer(table.GetCell
                (0, 1)) };
            CellRenderer[] row2 = new CellRenderer[] { new CellRenderer(table.GetCell(1, 0)), new CellRenderer(table.GetCell
                (1, 1)) };
            tableRenderer.rows[0] = row1;
            tableRenderer.rows[1] = row2;
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            tableRenderer.bordersHandler = new SeparatedTableBorders(tableRenderer.rows, table.GetNumberOfColumns(), tableRenderer
                .GetBorders());
            TableWidths tableWidths = new TableWidths(tableRenderer, 150, true, 15, 15);
            IList<TableWidths.CellInfo> cells = tableWidths.AutoLayoutCustom();
            foreach (TableWidths.CellInfo cell in cells) {
                tableWidths.ProcessCell(cell);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestProcessCellsWithPercentWidth01() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("With 2 columns:"));
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetMarginTop(5);
            for (int i = 0; i < 4; i++) {
                Cell cell = new Cell().Add(new Paragraph("smth" + i));
                cell.SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(50));
                table.AddCell(cell);
            }
            TableRenderer tableRenderer = new TableRenderer(table);
            CellRenderer[] row1 = new CellRenderer[] { new CellRenderer(table.GetCell(0, 0)), new CellRenderer(table.GetCell
                (0, 1)) };
            CellRenderer[] row2 = new CellRenderer[] { new CellRenderer(table.GetCell(1, 0)), new CellRenderer(table.GetCell
                (1, 1)) };
            tableRenderer.rows[0] = row1;
            tableRenderer.rows[1] = row2;
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            tableRenderer.bordersHandler = new SeparatedTableBorders(tableRenderer.rows, table.GetNumberOfColumns(), tableRenderer
                .GetBorders());
            TableWidths tableWidths = new TableWidths(tableRenderer, 150, true, 15, 15);
            IList<TableWidths.CellInfo> cells = tableWidths.AutoLayoutCustom();
            foreach (TableWidths.CellInfo cell in cells) {
                tableWidths.ProcessCell(cell);
            }
            tableWidths.Recalculate(25);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestProcessCellsWithPercentWidth02() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 1 }));
            table.SetWidth(UnitValue.CreatePercentValue(80));
            table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            Cell c1 = new Cell(1, 3);
            c1.SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(90));
            c1.Add(new Paragraph("Cell with colspan 3"));
            table.AddCell(c1);
            Cell c2 = new Cell(2, 1);
            c2.Add(new Paragraph("Cell with rowspan 2"));
            c2.SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(50));
            table.AddCell(c2);
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 1")).SetMinWidth(200));
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 2")).SetMaxWidth(50));
            table.AddCell("row 2; cell 1");
            table.AddCell("row 2; cell 2");
            TableRenderer tableRenderer = new TableRenderer(table);
            CellRenderer[] row1 = new CellRenderer[] { new CellRenderer(table.GetCell(0, 0)), null, null };
            CellRenderer[] row2 = new CellRenderer[] { null, new CellRenderer(table.GetCell(1, 1)), new CellRenderer(table
                .GetCell(1, 2)) };
            CellRenderer[] row3 = new CellRenderer[] { new CellRenderer(table.GetCell(1, 0)), new CellRenderer(table.GetCell
                (2, 1)), new CellRenderer(table.GetCell(2, 2)) };
            tableRenderer.rows[0] = row1;
            tableRenderer.rows[1] = row2;
            tableRenderer.rows[2] = row3;
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            tableRenderer.bordersHandler = new SeparatedTableBorders(tableRenderer.rows, table.GetNumberOfColumns(), tableRenderer
                .GetBorders());
            TableWidths tableWidths = new TableWidths(tableRenderer, 150, true, 15, 15);
            IList<TableWidths.CellInfo> cells = tableWidths.AutoLayoutCustom();
            foreach (TableWidths.CellInfo cell in cells) {
                tableWidths.ProcessCell(cell);
            }
            tableWidths.Recalculate(200);
            doc.Add(table);
            doc.Close();
        }

        private Table CreateTableWithDiffColspan(int maxWidth) {
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 1 }));
            table.SetWidth(UnitValue.CreatePercentValue(80));
            table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            Cell c1 = new Cell(1, 3);
            c1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(maxWidth));
            c1.Add(new Paragraph("Cell with colspan 3"));
            table.AddCell(c1);
            table.AddCell(new Cell(2, 1).Add(new Paragraph("Cell with rowspan 2")));
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 1")).SetMinWidth(200));
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 2")).SetMaxWidth(50));
            table.AddCell("row 2; cell 1");
            table.AddCell("row 2; cell 2");
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 5));
            return table;
        }

        private TableRenderer CreateTableRendererWithDiffColspan(int maxWidth) {
            Table table = CreateTableWithDiffColspan(maxWidth);
            TableRenderer tableRenderer = new TableRenderer(table);
            CellRenderer[] row1 = new CellRenderer[] { new CellRenderer(table.GetCell(0, 0)), null, null };
            CellRenderer[] row2 = new CellRenderer[] { null, new CellRenderer(table.GetCell(1, 1)), new CellRenderer(table
                .GetCell(1, 2)) };
            CellRenderer[] row3 = new CellRenderer[] { new CellRenderer(table.GetCell(1, 0)), new CellRenderer(table.GetCell
                (2, 1)), new CellRenderer(table.GetCell(2, 2)) };
            tableRenderer.rows[0] = row1;
            tableRenderer.rows[1] = row2;
            tableRenderer.rows[2] = row3;
            tableRenderer.bordersHandler = new SeparatedTableBorders(tableRenderer.rows, table.GetNumberOfColumns(), tableRenderer
                .GetBorders());
            return tableRenderer;
        }
    }
}
