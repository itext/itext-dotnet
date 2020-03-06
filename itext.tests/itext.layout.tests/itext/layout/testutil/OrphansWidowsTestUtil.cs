using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Testutil {
    public class OrphansWidowsTestUtil {
        public static String paraText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod "
             + "tempor incididunt ut labore et dolore magna aliqua. Nulla at volutpat diam ut " + "venenatis tellus in. Orci porta non pulvinar neque laoreet suspendisse interdum "
             + "consectetur. Ipsum dolor sit amet consectetur adipiscing. Id porta nibh venenatis" + " cras sed felis eget velit. Sapien nec sagittis aliquam malesuada. Cras sed felis"
             + " eget velit aliquet sagittis. Leo a diam sollicitudin tempor id eu nisl nunc." + " Faucibus a pellentesque sit amet porttitor eget dolor morbi. Nisl vel pretium"
             + " lectus quam id leo in vitae. Vehicula ipsum a arcu cursus vitae. Tincidunt praesent" + " semper feugiat nibh sed pulvinar proin gravida hendrerit. Nisl vel pretium lectus"
             + " quam id leo in vitae turpis. Quis hendrerit dolor magna eget est lorem. Diam sit" + " amet nisl suscipit adipiscing bibendum est ultricies. Ultricies mi eget mauris pharetra."
             + " Etiam dignissim diam quis enim. Felis bibendum ut tristique et egestas quis.";

        public static void ProduceOrphansWidowsTestCase(String outPdf, int linesLeft, bool orphans, Paragraph testPara
            , bool marginTestCase) {
            Document doc = new Document(new PdfDocument(new PdfWriter(outPdf)));
            PageSize pageSize = new PageSize(PageSize.A4.GetWidth(), PageSize.A5.GetHeight());
            doc.GetPdfDocument().SetDefaultPageSize(pageSize);
            Rectangle[] columns = InitUniformColumns(pageSize, 2);
            doc.SetRenderer(new ColumnDocumentRenderer(doc, columns));
            testPara.SetMargins(marginTestCase ? 30 : 0, 0, 0, 0).SetBackgroundColor(new DeviceRgb(232, 232, 232)).SetBorder
                (new SolidBorder(1));
            testPara.Add(paraText);
            float linesHeight = CalculateHeightForLinesNum(doc, testPara, columns[0].GetWidth(), linesLeft, orphans);
            float linesSpaceEps = 5;
            String descriptionIntro = "Test " + (orphans ? "orphans" : "widows") + ". ";
            String descriptionBeg = "This block height is adjusted in such way as to leave ";
            String descriptionEnd = " line(s) on area break. Configuration of orphans/widows is identified by the file name. "
                 + "Reference example without orphans/widows control can be found on the next page.";
            float adjustmentHeight = columns[0].GetHeight() - linesHeight - linesSpaceEps;
            doc.Add(new Paragraph().Add(new Text(descriptionIntro).SetFontColor(ColorConstants.RED)).Add(new Text(descriptionBeg
                ).SetFontSize(8)).Add(new Text(linesLeft.ToString()).SetFontColor(ColorConstants.RED)).Add(new Text(descriptionEnd
                ).SetFontSize(8)).SetMargin(0).SetBorder(new SolidBorder(1)).SetHeight(adjustmentHeight));
            if (!marginTestCase) {
                doc.Add(testPara);
            }
            else {
                Div div = new Div().Add(testPara).SetMarginTop(15);
                div.SetProperty(Property.COLLAPSING_MARGINS, true);
                doc.Add(div);
            }
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            doc.Add(new Paragraph("Reference example without orphans/widows control.").SetMargin(0).SetBorder(new SolidBorder
                (1)).SetHeight(adjustmentHeight));
            if (!marginTestCase) {
                doc.Add(new Paragraph(paraText).SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232)).SetBorder(new 
                    SolidBorder(1)));
            }
            else {
                Div div = new Div().Add(new Paragraph(paraText).SetMargins(30, 0, 0, 0).SetBackgroundColor(new DeviceRgb(232
                    , 232, 232)).SetBorder(new SolidBorder(1))).SetMarginTop(15);
                div.SetProperty(Property.COLLAPSING_MARGINS, true);
                doc.Add(div);
            }
            doc.Close();
        }

        public static float CalculateHeightForLinesNum(Document doc, Paragraph p, float width, float linesNum, bool
             orphans) {
            ParagraphRenderer renderer = (ParagraphRenderer)p.CreateRendererSubTree().SetParent(doc.GetRenderer());
            LayoutResult layoutRes = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, 100000))
                ));
            float lineHeight = layoutRes.GetOccupiedArea().GetBBox().GetHeight() / renderer.GetLines().Count;
            float height = lineHeight * linesNum;
            if (orphans) {
                return height;
            }
            else {
                return layoutRes.GetOccupiedArea().GetBBox().GetHeight() - height;
            }
        }

        private static Rectangle[] InitUniformColumns(PageSize pageSize, int columnsNum) {
            Rectangle[] columns = new Rectangle[columnsNum];
            float columnWidth = (pageSize.GetWidth() - 72) / columnsNum;
            for (int i = 0; i < columnsNum; ++i) {
                columns[i] = new Rectangle(36 + i * columnWidth, 36, columnWidth - 36, pageSize.GetHeight() - 72);
            }
            return columns;
        }
    }
}
