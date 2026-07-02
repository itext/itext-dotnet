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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties.Margins;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class DynamicMarginsFootnoteLayoutResultTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/DynamicMarginsFootnoteLayoutResultTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/DynamicMarginsFootnoteLayoutResultTest/";

        private static readonly float A4_HEIGHT = PageSize.A4.GetHeight();

        private static readonly float A4_WIDTH = PageSize.A4.GetWidth();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void HugeTopMarginPartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float largeTop = A4_HEIGHT * 0.60f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(largeTop, 0, 0, 0));
                    Div tall = TestResourceUtil.GetTallDiv(4);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, largeTop, 0, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be split (PARTIAL) when a "
                         + "large dynamic top margin leaves only a small usable area");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void HugeTopAndBottomMarginsPartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.35f;
                    float bottom = A4_HEIGHT * 0.35f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    Div tall = TestResourceUtil.GetTallDiv(4);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, top, bottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be split (PARTIAL) when both "
                         + "dynamic top and bottom margins are large but a small usable strip remains");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DynamicMarginsEvenPagesRenderTest() {
            String fileName = "partialDynamicMarginsEvenPages";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float largeTop = A4_HEIGHT * 0.55f;
                    float largeBottom = A4_HEIGHT * 0.20f;
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, PageMarginsTestUtil.GetMarginBoxes(largeTop, largeBottom
                        , 0, 0));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                    document.Add(new AreaBreak());
                    document.Add(TestResourceUtil.GetTallDiv(3));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DynamicMarginsOddPagesRenderTest() {
            String fileName = "partialDynamicMarginsOddPages";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float largeTop = A4_HEIGHT * 0.55f;
                    float largeBottom = A4_HEIGHT * 0.15f;
                    document.SetPageMargins((pageNum) => pageNum % 2 != 0, PageMarginsTestUtil.GetMarginBoxes(largeTop, largeBottom
                        , 0, 0));
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new AreaBreak());
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DynamicMarginsSectionBreakRenderTest() {
            String fileName = "partialDynamicMarginsSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float largeTop = A4_HEIGHT * 0.50f;
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, PageMarginsTestUtil.GetMarginBoxes(largeTop, 0, 0, 
                        0));
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetMarginBoxes(0, A4_HEIGHT * 0.50f, 0, 0)));
                    document.Add(TestResourceUtil.GetTallDiv(3));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PerPageDynamicMarginsRenderTest() {
            String fileName = "partialPerPageDynamicMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => {
                        float top = Math.Min(pageNum * 60f, A4_HEIGHT * 0.55f);
                        IList<PageMarginContent> list = new List<PageMarginContent>();
                        list.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("Header p." + pageNum)).SetBackgroundColor
                            (ColorConstants.PINK).SetHeight(top)));
                        return new PageMarginBoxes(list);
                    }
                    );
                    document.Add(TestResourceUtil.GetTallDiv(6));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void LargeFootnotePartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float footnoteHeight = A4_HEIGHT * 0.55f;
                    ApplyMarginBoxes(document, 0, footnoteHeight, 0, 0);
                    Div tall = TestResourceUtil.GetTallDiv(3);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, 0, footnoteHeight, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be split (PARTIAL) when a "
                         + "large footnote margin leaves less than half the page for content");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void LargeFootnotePartialRenderTest() {
            String fileName = "partialLargeFootnoteMargin";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float footnoteHeight = A4_HEIGHT * 0.55f;
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetFootnoteMarginBoxes(footnoteHeight)));
                    Div tall = new Div().SetBackgroundColor(new DeviceRgb(78, 151, 205));
                    for (int i = 0; i < 5; i++) {
                        tall.Add(new Paragraph("BLOCK " + i + "\n" + TestResourceUtil.GetByronStanza()));
                    }
                    document.Add(tall);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void LargeFootnoteAreaBreakRenderTest() {
            String fileName = "partialFootnoteAreaBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float footnoteHeight = A4_HEIGHT * 0.55f;
                    ApplyMarginBoxes(document, 0, footnoteHeight, 0, 0);
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("Second page — same large footnote margin."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void LargeFootnoteAndHeaderPartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float headerHeight = A4_HEIGHT * 0.30f;
                    float footnoteHeight = A4_HEIGHT * 0.35f;
                    ApplyMarginBoxes(document, headerHeight, footnoteHeight, 0, 0);
                    Div tall = TestResourceUtil.GetTallDiv(4);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, headerHeight, footnoteHeight, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be split (PARTIAL) when both "
                         + "header and large footnote margins leave only a small usable band");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DynamicFootnoteMarginsEvenPagesRenderTest() {
            String fileName = "partialDynamicFootnoteEvenPages";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float footnoteHeight = A4_HEIGHT * 0.55f;
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, PageMarginsTestUtil.GetFootnoteMarginBoxes(footnoteHeight
                        ));
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new AreaBreak());
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("Page 4 - Footnote margins."));
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("Page 5 - No margins."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ExtremeTopMarginNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeTop = A4_HEIGHT - 30f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(hugeTop, 0, 0, 0));
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(80);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, hugeTop, 0, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when a dynamic top "
                         + "margin occupies virtually the entire page");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExtremeTopAndBottomMarginsNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeMargin = (A4_HEIGHT - 10f) / 2f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(hugeMargin, hugeMargin, 0, 0
                        ));
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(80);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, hugeMargin, hugeMargin, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when combined "
                         + "dynamic top and bottom margins leave virtually no usable area");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExtremeFootnoteNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeFootnote = A4_HEIGHT - 25f;
                    ApplyMarginBoxes(document, 0, hugeFootnote, 0, 0);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(60);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, 0, hugeFootnote, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Short element with explicit height should "
                         + "return NOTHING when a huge footnote margin leaves essentially no vertical room");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExtremeFootnoteKeepTogetherNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeMargin = (A4_HEIGHT - 8f) / 2f;
                    ApplyMarginBoxes(document, hugeMargin, hugeMargin, 0, 0);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(150).SetKeepTogether
                        (true);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, hugeMargin, hugeMargin, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "keepTogether element taller than available area should return NOTHING"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void HugeDynamicMarginForcedPlacementRenderTest() {
            String fileName = "hugeDynamicMarginForcedPlacement";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeTop = A4_HEIGHT - 30f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(hugeTop, 0, 0, 0));
                    Div forced = new Div().Add(new Paragraph("FORCED — huge dynamic top margin, almost no space.")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void HugeFootnoteForcedPlacementRenderTest() {
            String fileName = "hugeFootnoteForcedPlacement";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeFootnote = A4_HEIGHT - 30f;
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetFootnoteMarginBoxes(hugeFootnote)));
                    Div forced = new Div().Add(new Paragraph("FORCED — huge footnote margin, almost no space.")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ExtremeMarginsRecoveryViaSectionBreakRenderTest() {
            String fileName = "extremeMarginsRecoverySectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeMargin = (A4_HEIGHT - 10f) / 2f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(hugeMargin, hugeMargin, 0, 0
                        ));
                    Div forced = new Div().Add(new Paragraph("PAGE 1 — extreme dynamic margins (FORCED).")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    document.Add(forced);
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetMarginBoxes(80, 80, 0, 0)));
                    document.Add(new Div().Add(new Paragraph("PAGE 2 — normal margins, content fits fully.")).SetBackgroundColor
                        (new DeviceRgb(65, 151, 29)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void ExtremeFootnoteMarginsRecoveryRenderTest() {
            String fileName = "extremeFootnoteMarginsRecovery";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float hugeFootnote = A4_HEIGHT - 30f;
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetFootnoteMarginBoxes(hugeFootnote)));
                    Div forced = new Div().Add(new Paragraph("PAGE 1 — extreme footnote margin (FORCED).")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    document.Add(forced);
                    document.Add(new AreaBreak());
                    Div also = new Div().Add(new Paragraph("PAGE 2 — same extreme footnote margin (FORCED).")).SetBackgroundColor
                        (new DeviceRgb(255, 180, 80));
                    document.Add(also);
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetMarginBoxes(60, 60, 0, 0)));
                    document.Add(new Div().Add(new Paragraph("PAGE 3 — normal margins, content fits.")).SetBackgroundColor(new 
                        DeviceRgb(65, 151, 29)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DynamicMarginStatusProgressionTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(350).SetBackgroundColor
                        (new DeviceRgb(65, 151, 29));
                    int fullStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, 50, 0, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, fullStatus, "Expected FULL with small dynamic top margin"
                        );
                    int partialStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, A4_HEIGHT * 0.55f, 0, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, partialStatus, "Expected PARTIAL with large dynamic top margin"
                        );
                    float hugeTop = A4_HEIGHT - 30f;
                    int nothingStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, hugeTop, 0, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, nothingStatus, "Expected NOTHING with extreme dynamic top margin"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteMarginStatusProgressionTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(350).SetBackgroundColor
                        (new DeviceRgb(78, 151, 205));
                    int fullStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, 0, 50, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, fullStatus, "Expected FULL with a small footnote margin"
                        );
                    int partialStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, 0, A4_HEIGHT * 0.55f, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, partialStatus, "Expected PARTIAL with a large footnote margin"
                        );
                    float hugeBottom = A4_HEIGHT - 30f;
                    int nothingStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, 0, hugeBottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, nothingStatus, "Expected NOTHING with an extreme footnote margin"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CombinedDynamicAndFootnoteMarginStatusProgressionTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(350).SetBackgroundColor
                        (new DeviceRgb(209, 247, 29));
                    int fullStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, 50, 50, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, fullStatus, "Expected FULL when both margins are small"
                        );
                    float medTop = A4_HEIGHT * 0.30f;
                    float medBottom = A4_HEIGHT * 0.30f;
                    int partialStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, medTop, medBottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, partialStatus, "Expected PARTIAL when both margins are medium-large"
                        );
                    float hugeMargin = (A4_HEIGHT - 10f) / 2f;
                    int nothingStatus = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, hugeMargin, hugeMargin, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, nothingStatus, "Expected NOTHING when both margins are extreme"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void HugeTopDynamicMarginWithFootnoteAnchorTest() {
            String fileName = "hugeTopDynamicMarginWithFootnoteAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float largeTop = A4_HEIGHT * 0.55f;
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(largeTop, 0, 0, 0));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void HugeBottomDynamicMarginWithFootnoteAnchorTest() {
            String fileName = "hugeBottomDynamicMarginWithFootnoteAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float largeBottom = A4_HEIGHT * 0.55f;
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(0, largeBottom, 0, 0));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void HugeTopAndBottomMarginsWithFootnoteAnchorRenderTest() {
            String fileName = "hugeTopAndBottomMarginsWithFootnoteAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.35f;
                    float bottom = A4_HEIGHT * 0.35f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        private static void ApplyMarginBoxes(Document document, float top, float bottom, float left, float right) {
            document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(top, bottom, left, right));
        }
    }
}
