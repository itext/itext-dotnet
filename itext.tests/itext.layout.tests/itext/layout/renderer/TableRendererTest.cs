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
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TableRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 6)]
        public virtual void CalculateColumnWidthsNotPointValue() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Document doc = new Document(pdfDoc);
            Rectangle layoutBox = new Rectangle(0, 0, 1000, 100);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            // Set margins and paddings in percents, which is not expected
            table.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePercentValue(7));
            table.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePercentValue(7));
            table.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePercentValue(7));
            table.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePercentValue(7));
            // Fill the table somehow. The layout area is wide enough to calculate the widths as expected
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            // Create a TableRenderer, the instance of which will be used to test the application of margins and paddings
            TableRenderer tableRenderer = (TableRenderer)table.CreateRendererSubTree().SetParent(doc.GetRenderer());
            tableRenderer.bordersHandler = (TableBorders)new SeparatedTableBorders(tableRenderer.rows, 3, tableRenderer
                .GetBorders(), 0);
            tableRenderer.ApplyMarginsAndPaddingsAndCalculateColumnWidths(layoutBox);
            // Specify that the render is not original in order not to recalculate the column widths
            tableRenderer.isOriginalNonSplitRenderer = false;
            MinMaxWidth minMaxWidth = tableRenderer.GetMinMaxWidth();
            // TODO DEVSIX-3676: currently margins and paddings are still applied as if they are in points. After the mentioned ticket is fixed, the expected values should be updated.
            NUnit.Framework.Assert.AreEqual(327.46f, minMaxWidth.GetMaxWidth(), 0.001);
            NUnit.Framework.Assert.AreEqual(327.46f, minMaxWidth.GetMinWidth(), 0.001);
        }

        [NUnit.Framework.Test]
        public virtual void TestIsOriginalNonSplitRenderer() {
            Table table = new Table(1);
            table.AddCell(new Cell());
            table.AddCell(new Cell());
            table.AddCell(new Cell());
            TableRenderer original = (TableRenderer)table.CreateRendererSubTree();
            TableRenderer[] children = original.Split(1);
            TableRenderer[] grandChildren = children[1].Split(1);
            NUnit.Framework.Assert.IsFalse(grandChildren[0].isOriginalNonSplitRenderer);
        }
    }
}
