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
    [NUnit.Framework.Category("Integration test")]
    public class LeadingHeightTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LeadingHeightTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LeadingHeightTest/";

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
            // This is how table looks like if no height property is set
            AddTable(doc, 504, "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.", -1);
            // Here we set value from pre layout as height. We expect that this table shall be equal to the previous one
            AddTable(doc, 360, "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.", 0);
            // Here we set 100 as height. We expect that this will be enough and all text will be placed
            AddTable(doc, 216, "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.", 100);
            // Here we set 100 as height. We expect that this will be enough to place 3 lines
            AddTable(doc, 216, "RETIREMENT PLANNING: BECAUSE ***SOME TEST TEXT IS PLACED*** YOU CAN’T BE A FINANCIAL PLANNER FOREVER."
                , 100);
            // Here we set value from pre layout minus 0.5f as height. We expect that this table shall not be equal to the previous one
            AddTable(doc, 50, "RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.", -2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PageHeightParagraphTest() {
            String outPdf = DESTINATION_FOLDER + "pageHeightParagraphTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_pageHeightParagraphTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            //176 = 104 + 36 + 36 (page margins)
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PageHeightParagraphWithWithWorkaroundTest() {
            String outPdf = DESTINATION_FOLDER + "pageHeightParagraphWorkAroundTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_pageHeightParagraphWorkAroundTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            //176 = 104 + 36 + 36 (page margins)
            Document doc = new Document(pdfDoc, new PageSize(700, 176));
            Paragraph ph = new Paragraph();
            Text txt = new Text("RETIREMENT PLANNING: BECAUSE YOU CAN’T BE A FINANCIAL PLANNER FOREVER.");
            txt.SetFontSize(32f);
            ph.Add(txt);
            ph.SetFixedLeading(32f);
            ph.SetPaddingTop(0f);
            ph.SetPaddingBottom(0f);
            ph.SetWidth(585f);
            Paragraph ph2 = new Paragraph();
            ph2.SetHeight(104);
            ph2.SetMargin(0);
            ph2.SetPadding(0);
            ph2.Add(ph);
            ph2.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            doc.Add(ph2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        public virtual void AddTable(Document doc, int y, String text, int heightParam) {
            float width = 585f;
            float fontSize = 32f;
            Table table = new Table(1);
            table.SetWidth(width);
            table.SetFixedLayout();
            Paragraph ph = new Paragraph();
            Text txt = new Text(text);
            txt.SetFontSize(fontSize);
            ph.Add(txt);
            ph.SetFixedLeading(fontSize);
            Cell cell = new Cell();
            cell.SetPaddingTop(0f);
            cell.SetPaddingBottom(0f);
            cell.Add(ph);
            cell.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            cell.SetBorder(null);
            table.AddCell(cell);
            // find out how tall the cell is we just added
            LayoutResult result = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(
                new LayoutArea(1, new Rectangle(0, 0, width, 10000.0F))));
            String heightStr = "Natural";
            if (heightParam == -2) {
                float rowHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                cell.SetHeight(rowHeight - 0.5f);
                heightStr = "Calculated " + (rowHeight - 0.5f);
            }
            if (heightParam == 0) {
                float rowHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                cell.SetHeight(rowHeight + 1f);
                heightStr = "Calculated " + rowHeight;
            }
            else {
                if (heightParam > 0) {
                    cell.SetHeight(heightParam);
                    heightStr = "Explicit " + heightParam;
                }
            }
            table.SetFixedPosition((float)36, (float)y, width);
            doc.Add(table);
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
    }
}
