/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LeadingHeightTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LeadingHeightTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LeadingHeightTest/";

        private const int HEIGHT_LESS_THAN_REQUIRED = -2;

        private const int HEIGHT_IS_NOT_SET = -1;

        private const int HEIGHT_EXACT_THAT_REQUIRED = 0;

        private const int HEIGHT_MORE_THAN_REQUIRED = 100;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void ClippedHeightParagraphTest() {
            String outPdf = DESTINATION_FOLDER + "leadingTestHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_leadingTestHeight.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            Document doc = new Document(pdfDoc, new PageSize(700, 700));
            AddDescription(doc, 600, "This is how table looks like if no height property is set");
            AddTable(doc, 504, "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.", HEIGHT_IS_NOT_SET
                );
            AddDescription(doc, 456, "Here we set value from pre layout as height. We expect that this table shall be equal to the previous one"
                );
            AddTable(doc, 360, "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.", HEIGHT_EXACT_THAT_REQUIRED
                );
            AddDescription(doc, 312, "Here we set 100 as height. We expect that this will be enough to place 3 lines");
            AddTable(doc, 216, "RETIREMENT PLANNING: BECAUSE ***SOME TEST TEXT IS PLACED*** YOU CAN’T BE A FINANCIAL PLANNER FOREVER."
                , HEIGHT_MORE_THAN_REQUIRED);
            AddDescription(doc, 146, "Here we set value from pre layout minus 0.5f as height. We expect that this table shall not be equal to the previous one"
                );
            AddTable(doc, 50, "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.", HEIGHT_LESS_THAN_REQUIRED
                );
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphTest() {
            String outPdf = DESTINATION_FOLDER + "pageHeightParagraphTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_pageHeightParagraphTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            //Document height = 176 = 104 + 36 + 36, where 104 - is exact size of paragraph after layout and 34 + 34 - page margins
            Document doc = new Document(pdfDoc, new PageSize(700, 176));
            Paragraph ph = new Paragraph();
            Text txt = new Text("RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.");
            txt.SetFontSize(32f);
            ph.Add(txt);
            ph.SetFixedLeading(32f);
            ph.SetPaddingTop(0f);
            ph.SetPaddingBottom(0f);
            ph.SetWidth(585f);
            doc.Add(ph);
            doc.Close();
            //Partial text expected to be present in the document
            //There should be only "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL"
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PageHeightDivWithNestedParagraphTest() {
            String outPdf = DESTINATION_FOLDER + "pageHeightParagraphWorkAroundTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_pageHeightParagraphWorkAroundTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            //Document height = 176 = 104 + 36 + 36, where 104 - is exact size of paragraph after layout and 34 + 34 - page margins
            Document doc = new Document(pdfDoc, new PageSize(700, 176));
            Paragraph ph = new Paragraph();
            Text txt = new Text("RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.");
            txt.SetFontSize(32f);
            ph.Add(txt);
            ph.SetFixedLeading(32f);
            ph.SetPaddingTop(0f);
            ph.SetPaddingBottom(0f);
            ph.SetWidth(585f);
            Div ph2 = new Div();
            ph2.SetHeight(104);
            ph2.SetMargin(0);
            ph2.SetPadding(0);
            ph2.Add(ph);
            ph2.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            doc.Add(ph2);
            doc.Close();
            //Full text expected to be present on the first page
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        private void AddTable(Document doc, int y, String text, int heightParam) {
            float width = 585f;
            Table table = new Table(1);
            table.SetWidth(width);
            table.SetFixedLayout();
            Cell cell = AddCell(table, text);
            // find out how tall the cell is we just added
            LayoutResult result = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(
                new LayoutArea(1, new Rectangle(0, 0, width, 10000.0F))));
            String heightStr = "Natural";
            if (heightParam == HEIGHT_LESS_THAN_REQUIRED) {
                float rowHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                cell.SetHeight(rowHeight - 0.5f);
                heightStr = "Calculated " + (rowHeight - 0.5f);
            }
            if (heightParam == HEIGHT_EXACT_THAT_REQUIRED) {
                float rowHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                cell.SetHeight(rowHeight);
                heightStr = "Calculated " + rowHeight;
                if (heightStr.EndsWith(".0")) {
                    heightStr = heightStr.JSubstring(0, heightStr.Length - 2);
                }
            }
            else {
                if (heightParam > 0) {
                    cell.SetHeight(heightParam);
                    heightStr = "Explicit " + heightParam;
                }
            }
            table.SetFixedPosition((float)36, (float)y, width);
            doc.Add(table);
            AddCellFooter(doc, y, width, heightStr);
        }

        private Cell AddCell(Table table, String text) {
            Paragraph ph = new Paragraph();
            Text txt = new Text(text);
            txt.SetFontSize(32f);
            ph.Add(txt);
            ph.SetFixedLeading(32f);
            Cell cell = new Cell();
            cell.SetPaddingTop(0f);
            cell.SetPaddingBottom(0f);
            cell.Add(ph);
            cell.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            cell.SetBorder(null);
            table.AddCell(cell);
            return cell;
        }

        private void AddCellFooter(Document doc, float y, float width, String heightStr) {
            Table t2 = new Table(1);
            t2.SetWidth(width);
            t2.SetFixedLayout();
            Cell c2 = new Cell();
            c2.SetTextAlignment(TextAlignment.CENTER);
            c2.SetWidth(width);
            c2.SetBorder(Border.NO_BORDER);
            c2.Add(new Paragraph("Row Height: " + heightStr));
            t2.AddCell(c2);
            t2.SetFixedPosition((float)36, (float)y - 18, width);
            doc.Add(t2);
        }

        private void AddDescription(Document doc, float y, String text) {
            Paragraph ph = new Paragraph();
            Text txt = new Text(text);
            txt.SetFontSize(12f);
            ph.Add(txt);
            ph.SetFontColor(ColorConstants.RED);
            ph.SetFixedPosition(1, 40, y, 585f);
            doc.Add(ph);
        }
    }
}
