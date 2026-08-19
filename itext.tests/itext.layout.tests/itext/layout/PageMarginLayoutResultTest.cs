/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Renderer;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageMarginLayoutResultTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PageMarginLayoutResultTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/PageMarginLayoutResultTest/";

        private static readonly float A4_HEIGHT = PageSize.A4.GetHeight();

        private static readonly float A4_WIDTH = PageSize.A4.GetWidth();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ShortElementFullResultWithModestMarginsAssertTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    ApplyMarginBoxes(document, 100, 100, 0, 0);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza());
                    int status = LayoutStatus(p, document, AvailableRect(100, 100, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, status, "Short paragraph should fit fully with modest margin boxes"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ShortElementFullResultWithLargeHorizontalMarginsAssertTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    ApplyMarginBoxes(document, 0, 0, 150, 150);
                    Paragraph p = new Paragraph("Short text.");
                    int status = LayoutStatus(p, document, AvailableRect(0, 0, 150, 150));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, status, "Short paragraph should fit fully even with large horizontal margins"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void FullResultWithSectionBreakTest() {
            String fileName = "fullResultSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new Paragraph("Page 1 — no margin boxes.").Add(TestResourceUtil.GetByronStanza()));
                    document.Add(new SectionBreak(MarginBoxes(100, 80, 0, 0)));
                    document.Add(ShortContentDiv());
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void TallElementPartialResultWithLargeMarginsAssertTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    ApplyMarginBoxes(document, 250, 200, 0, 0);
                    Div tall = new Div().Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 4)
                        )).SetBackgroundColor(new DeviceRgb(65, 151, 29));
                    int status = LayoutStatus(tall, document, AvailableRect(250, 200, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should split (PARTIAL) when top/bottom margin boxes are large"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PartialResultWithLargeMarginsTest() {
            String fileName = "partialLargeMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new SectionBreak(MarginBoxes(250, 200, 0, 0)));
                    Div tall = new Div().SetBackgroundColor(new DeviceRgb(209, 247, 29));
                    for (int i = 0; i < 6; i++) {
                        tall.Add(new Paragraph("PARAGRAPH " + i + "\n" + TestResourceUtil.GetByronStanza()));
                    }
                    document.Add(tall);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PartialResultWithAreaBreakAndMarginsTest() {
            String fileName = "partialAreaBreakMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => true, MarginBoxes(200, 150, 0, 0));
                    Div tall = new Div().SetBackgroundColor(new DeviceRgb(78, 151, 205));
                    for (int i = 0; i < 4; i++) {
                        tall.Add(new Paragraph("BLOCK " + i + "\n" + TestResourceUtil.GetByronStanza()));
                    }
                    document.Add(tall);
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("After AreaBreak — same large margins."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ElementTooLargeNothingResultAssertTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeMargin = (A4_HEIGHT - 20f) / 2f;
                    ApplyMarginBoxes(document, hugeMargin, hugeMargin, 0, 0);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(100);
                    int status = LayoutStatus(element, document, AvailableRect(hugeMargin, hugeMargin, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element with explicit height greater than available area should return NOTHING"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void HugeBottomFootnoteMarginsNothingResultAssertTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeBottom = A4_HEIGHT - 30f;
                    ApplyMarginBoxes(document, 0, hugeBottom, 0, 0);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(80);
                    int status = LayoutStatus(element, document, AvailableRect(0, hugeBottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when huge bottom footnote margin leaves no space"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AllFourLargeMarginsNothingResultAssertTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.40f;
                    float left = A4_WIDTH * 0.40f;
                    float right = A4_WIDTH * 0.40f;
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(200).SetKeepTogether
                        (true);
                    int status = LayoutStatus(element, document, AvailableRect(top, bottom, left, right));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Non-splittable element (keepTogether) taller than available area should return NOTHING"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ForcedPlacementWithExtremeMarginBoxesTest() {
            String fileName = "forcedPlacementExtremeMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new SectionBreak(ExtremeMarginBoxes()));
                    Div forced = new Div().Add(new Paragraph("FORCED — margin boxes left almost no room.")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    forced.SetProperty(Property.FORCED_PLACEMENT, true);
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ProgressivelyLargerMarginsTransitionTest() {
            String fileName = "marginsTransition";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new SectionBreak(MarginBoxes(50, 50, 0, 0)));
                    document.Add(ContentDiv("SMALL MARGINS — FULL", new DeviceRgb(65, 151, 29)));
                    document.Add(new SectionBreak(MarginBoxes(200, 180, 0, 0)));
                    document.Add(ContentDiv("MEDIUM MARGINS — PARTIAL", new DeviceRgb(209, 247, 29)));
                    document.Add(new SectionBreak(ExtremeMarginBoxes()));
                    Div forced = ContentDiv("EXTREME MARGINS — FORCED", new DeviceRgb(255, 100, 100));
                    forced.SetProperty(Property.FORCED_PLACEMENT, true);
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ProgressivelyLargerMarginsStatusAssertTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(200).SetBackgroundColor
                        (new DeviceRgb(65, 151, 29));
                    int smallStatus = LayoutStatus(element, document, AvailableRect(50, 50, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, smallStatus, "Expected FULL with small margin boxes (200pt element, ~670pt area)"
                        );
                    float partialTop = 300f;
                    float partialBottom = 300f;
                    int mediumStatus = LayoutStatus(element, document, AvailableRect(partialTop, partialBottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, mediumStatus, "Expected PARTIAL with large margin boxes (200pt element, ~170pt area)"
                        );
                    float hugeMargin = (A4_HEIGHT - 10f) / 2f;
                    int nothingStatus = LayoutStatus(element, document, AvailableRect(hugeMargin, hugeMargin, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, nothingStatus, "Expected NOTHING with extreme margin boxes (~1pt area)"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void NothingThenAreaBreakRestoresMarginsTest() {
            String fileName = "nothingAreaBreakRestoresMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new SectionBreak(ExtremeMarginBoxes()));
                    Div forced = ContentDiv("PAGE 1 — EXTREME MARGINS (FORCED)", new DeviceRgb(255, 100, 100));
                    forced.SetProperty(Property.FORCED_PLACEMENT, true);
                    document.Add(forced);
                    document.Add(new AreaBreak());
                    document.Add(new SectionBreak(MarginBoxes(80, 80, 0, 0)));
                    document.Add(ContentDiv("PAGE 2+ — NORMAL MARGINS", new DeviceRgb(65, 151, 29)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NothingThenFullViaTwoSectionBreaksTest() {
            String fileName = "nothingThenFullTwoSectionBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new SectionBreak(ExtremeMarginBoxes()));
                    Div forced = ContentDiv("EXTREME — FORCED PLACEMENT", new DeviceRgb(255, 100, 100));
                    forced.SetProperty(Property.FORCED_PLACEMENT, true);
                    document.Add(forced);
                    document.Add(new SectionBreak(MarginBoxes(80, 80, 0, 0)));
                    document.Add(ContentDiv("MODEST — FITS FULLY", new DeviceRgb(65, 151, 29)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void AlternatingExtremeAndNormalDocumentMarginsTest() {
            String fileName = "alternatingExtremeNormalMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 != 0 ? ExtremeMarginBoxes() : MarginBoxes(80, 80, 0, 0));
                    for (int page = 1; page <= 4; page++) {
                        Div div = ContentDiv("PAGE " + page, CellColor(page - 1));
                        if (page % 2 != 0) {
                            div.SetProperty(Property.FORCED_PLACEMENT, true);
                        }
                        document.Add(div);
                        if (page < 4) {
                            document.Add(new AreaBreak());
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        private static PageMarginBoxes MarginBoxes(float top, float bottom, float left, float right) {
            IList<PageMarginContent> elements = new List<PageMarginContent>();
            if (top > 0) {
                elements.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("TOP")).SetBackgroundColor
                    (ColorConstants.PINK).SetHeight(top)));
            }
            if (bottom > 0) {
                elements.Add(new PageMarginContent(MarginBoxName.BOTTOM, new Div().Add(new Paragraph("BOTTOM")).SetBackgroundColor
                    (ColorConstants.ORANGE).SetHeight(bottom)));
            }
            if (left > 0) {
                elements.Add(new PageMarginContent(MarginBoxName.LEFT, new Div().Add(new Paragraph("L")).SetBackgroundColor
                    (ColorConstants.BLUE).SetWidth(left)));
            }
            if (right > 0) {
                elements.Add(new PageMarginContent(MarginBoxName.RIGHT, new Div().Add(new Paragraph("R")).SetBackgroundColor
                    (ColorConstants.YELLOW).SetWidth(right)));
            }
            return new PageMarginBoxes(elements);
        }

        private static PageMarginBoxes ExtremeMarginBoxes() {
            float topBottom = (A4_HEIGHT - 10f) / 2f;
            return MarginBoxes(topBottom, topBottom, 0, 0);
        }

        private static void ApplyMarginBoxes(Document document, float top, float bottom, float left, float right) {
            document.SetPageMargins((pageNum) => true, MarginBoxes(top, bottom, left, right));
        }

        private static Rectangle AvailableRect(float top, float bottom, float left, float right) {
            float docMargin = 36f;
            float x = docMargin + left;
            float y = docMargin + bottom;
            float w = A4_WIDTH - 2 * docMargin - left - right;
            float h = A4_HEIGHT - 2 * docMargin - top - bottom;
            return new Rectangle(x, y, Math.Max(w, 1f), Math.Max(h, 1f));
        }

        private static int LayoutStatus(Object element, Document document, Rectangle area) {
            IRenderer renderer;
            if (element is Div) {
                renderer = ((Div)element).CreateRendererSubTree().SetParent(document.GetRenderer());
            }
            else {
                if (element is Paragraph) {
                    renderer = ((Paragraph)element).CreateRendererSubTree().SetParent(document.GetRenderer());
                }
                else {
                    throw new ArgumentException("Unsupported element type");
                }
            }
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, area)));
            return result.GetStatus();
        }

        private static Div ContentDiv(String label, DeviceRgb color) {
            Div div = new Div().SetBackgroundColor(color);
            div.Add(new Paragraph(label));
            div.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 2)));
            return div;
        }

        private static Div ShortContentDiv() {
            return new Div().Add(new Paragraph("Short content — fits fully.")).SetBackgroundColor(new DeviceRgb(65, 151
                , 29));
        }

        private static DeviceRgb CellColor(int index) {
            DeviceRgb[] palette = new DeviceRgb[] { new DeviceRgb(65, 151, 29), new DeviceRgb(209, 247, 29), new DeviceRgb
                (78, 151, 205), new DeviceRgb(255, 165, 0) };
            return palette[index % palette.Length];
        }
    }
}
