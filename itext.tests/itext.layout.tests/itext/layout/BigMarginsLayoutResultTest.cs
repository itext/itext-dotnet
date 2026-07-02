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
using System.IO;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BigMarginsLayoutResultTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BigMarginsLayoutResultTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/BigMarginsLayoutResultTest/";

        private static readonly float A4_HEIGHT = PageSize.A4.GetHeight();

        private static readonly float A4_WIDTH = PageSize.A4.GetWidth();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void StaticLargeTopBottomPartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.35f;
                    float bottom = A4_HEIGHT * 0.35f;
                    document.SetMargins(top, 36, bottom, 36);
                    Div tall = TestResourceUtil.GetTallDiv(4);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 0, top, bottom, 36, 36));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be PARTIAL when static top+bottom margins are large"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StaticExtremeTopBottomNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float each = (A4_HEIGHT - 10f) / 2f;
                    document.SetMargins(each, 36, each, 36);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(80);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 0, each, each, 36, 36));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when static top+bottom margins "
                         + "leave virtually no vertical space");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StaticLargeTopBottomMarginsPartialRenderTest() {
            String fileName = "staticLargeTopBottomPartial";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.35f;
                    float bottom = A4_HEIGHT * 0.35f;
                    document.SetMargins(top, 36, bottom, 36);
                    document.Add(TestResourceUtil.GetTallDiv(5));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void StaticExtremeTopBottomMarginsForcedPlacementRenderTest() {
            String fileName = "staticExtremeTopBottomForced";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float each = (A4_HEIGHT - 10f) / 2f;
                    document.SetMargins(each, 36, each, 36);
                    Div forced = new Div().Add(new Paragraph("FORCED — almost no vertical space left.")).SetBackgroundColor(new 
                        DeviceRgb(255, 100, 100));
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void StaticAllFourLargePartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float v = A4_HEIGHT * 0.30f;
                    float h = A4_WIDTH * 0.30f;
                    document.SetMargins(v, h, v, h);
                    Div tall = TestResourceUtil.GetTallDiv(4);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 0, v, v, h, h));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be PARTIAL when all four static margins are large"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StaticAllFourExtremeNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float v = (A4_HEIGHT - 10f) / 2f;
                    float h = (A4_WIDTH - 10f) / 2f;
                    document.SetMargins(v, h, v, h);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(80);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 0, v, v, h, h));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when all four static margins are extreme"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StaticAllFourLargeMarginsPartialRenderTest() {
            String fileName = "staticAllFourLargeMarginsPartial";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float v = A4_HEIGHT * 0.30f;
                    float h = A4_WIDTH * 0.30f;
                    document.SetMargins(v, h, v, h);
                    document.Add(TestResourceUtil.GetTallDiv(5));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void StaticAllFourLargeMarginsWithSectionBreakRenderTest() {
            String fileName = "staticAllFourLargeMarginsSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float v = A4_HEIGHT * 0.30f;
                    float h = A4_WIDTH * 0.30f;
                    document.SetMargins(v, h, v, h);
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetMarginBoxes(60, 60, 0, 0)));
                    document.Add(new Div().Add(new Paragraph("After section break — smaller margin boxes.")).SetBackgroundColor
                        (new DeviceRgb(65, 151, 29)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginLargeTopBottomPartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.30f;
                    document.SetPageMargins(1, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    Div tall = TestResourceUtil.GetTallDiv(4);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, top, bottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be PARTIAL when per-page margin boxes on page 1 are large"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginExtremeTopBottomNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float each = (A4_HEIGHT - 10f) / 2f;
                    document.SetPageMargins(1, PageMarginsTestUtil.GetMarginBoxes(each, each, 0, 0));
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(80);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, each, each, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when per-page margin boxes on page 1 are extreme"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginLargeOnSpecificPagePartialRenderTest() {
            String fileName = "pageMarginLargeOnPage2Partial";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.45f;
                    float bottom = A4_HEIGHT * 0.25f;
                    document.SetPageMargins(2, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    document.Add(new Paragraph("Page 1 — no special margin boxes."));
                    document.Add(new AreaBreak());
                    document.Add(TestResourceUtil.GetTallDiv(2));
                    document.Add(new Paragraph("Page 3 — no special margin boxes."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginAllFourSidesLargeOnPage1PartialRenderTest() {
            String fileName = "pageMarginAllFourLargeOnPage1";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float v = A4_HEIGHT * 0.30f;
                    float h = A4_WIDTH * 0.28f;
                    document.SetPageMargins(1, PageMarginsTestUtil.GetMarginBoxes(v, v, h, h));
                    document.Add(TestResourceUtil.GetTallDiv(4));
                    document.Add(new Paragraph("Page 2 — no margin boxes."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginPredicateLargeTopBottomPartialTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.30f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    Div tall = TestResourceUtil.GetTallDiv(4);
                    int status = LayoutResultTestUtil.GetLayoutStatus(tall, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, top, bottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.PARTIAL, status, "Tall element should be PARTIAL when predicate-based top+bottom margin boxes "
                         + "are large on all pages");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginPredicateExtremeTopBottomNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float each = (A4_HEIGHT - 10f) / 2f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(each, each, 0, 0));
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(80);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, each, each, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when predicate-based top+bottom margin boxes "
                         + "are extreme on all pages");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginPredicateLargeTopBottomPartialRenderTest() {
            String fileName = "pageMarginPredicateLargeTopBottomPartial";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.30f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    document.Add(TestResourceUtil.GetTallDiv(5));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginPredicateAllFourSidesLargePartialRenderTest() {
            String fileName = "pageMarginPredicateAllFourLargePartial";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float v = A4_HEIGHT * 0.30f;
                    float h = A4_WIDTH * 0.28f;
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, PageMarginsTestUtil.GetMarginBoxes(v, v, h, h));
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new AreaBreak());
                    document.Add(TestResourceUtil.GetTallDiv(3));
                    document.Add(new AreaBreak());
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.PAGE_CONTENT_CANNOT_BE_DRAWN, Count = 2)]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void PageMarginPredicateAllFourExtremeForcedRenderTest() {
            String fileName = "pageMarginPredicateAllFourExtremeForced";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float v = (A4_HEIGHT - 10f) / 2f;
                    float h = (A4_WIDTH - 10f) / 2f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(v, v, h, h));
                    Div forced = new Div().Add(new Paragraph("FORCED — all four margin boxes are extreme.")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PageMarginPredicateLargeThenSectionBreakRenderTest() {
            String fileName = "pageMarginPredicateLargeThenSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.30f;
                    document.SetPageMargins((pageNum) => pageNum % 2 != 0, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 
                        0));
                    document.Add(TestResourceUtil.GetTallDiv(4));
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetMarginBoxes(60, 60, 0, 0)));
                    document.Add(new Div().Add(new Paragraph("Section 2 — comfortable margin boxes.")).SetBackgroundColor(new 
                        DeviceRgb(65, 151, 29)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void MixedStaticAndPageMarginRenderTest() {
            String fileName = "mixedStaticAndPageMargin";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetMargins(60, 60, 60, 60);
                    document.SetPageMargins(2, PageMarginsTestUtil.GetMarginBoxes(A4_HEIGHT * 0.30f, A4_HEIGHT * 0.25f, 0, 0));
                    document.Add(TestResourceUtil.GetTallDiv(6));
                    document.Add(new AreaBreak());
                    document.Add(TestResourceUtil.GetTallDiv(4));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ExtremePageMarginsUnsplittableImageNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.40f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(SOURCE_FOLDER + "bee.png"));
                    int status = LayoutResultTestUtil.GetLayoutStatusForImage(image, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 36f, top, bottom, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Unsplittable image should return NOTHING when extreme dynamic margins "
                         + "leave less space than the image height");
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ExtremePageMarginsUnsplittableImageNothingRenderTest() {
            String fileName = "extremePageMarginsUnsplittableImageNothing";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.40f;
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxes(top, bottom, 0, 0));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png"));
                    document.Add(image);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ExtremeStaticMarginsUnsplittableImageNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    float top = A4_HEIGHT * 0.40f;
                    float bottom = A4_HEIGHT * 0.40f;
                    document.SetMargins(top, 36, bottom, 36);
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png"));
                    int status = LayoutResultTestUtil.GetLayoutStatusForImage(image, document, TestResourceUtil.GetAvailableRect
                        (A4_HEIGHT, A4_WIDTH, 0, top, bottom, 36, 36));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Unsplittable image should return NOTHING when extreme static margins "
                         + "leave less space than the image height");
                }
            }
        }
    }
}
