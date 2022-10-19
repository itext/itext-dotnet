/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
