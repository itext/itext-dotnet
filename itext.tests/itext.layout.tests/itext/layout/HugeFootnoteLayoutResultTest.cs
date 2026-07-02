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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class HugeFootnoteLayoutResultTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/HugeFootnoteLayoutResultTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/HugeFootnoteLayoutResultTest/";

        private static readonly float A4_HEIGHT = PageSize.A4.GetHeight();

        private static readonly float A4_WIDTH = PageSize.A4.GetWidth();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FootnoteHeightExactlyPageHeightRenderTest() {
            String fileName = "footnoteHeightExactlyPageHeight";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetFootnoteMarginBoxes(A4_HEIGHT)));
                    Div forced = new Div().Add(new Paragraph("Content — footnote height == page height (FORCED).")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteHeightExactlyPageHeightNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    ApplyFootnoteMarginBoxes(document, A4_HEIGHT);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(60);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, 0, A4_HEIGHT, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when the footnote height equals the page height"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FootnoteHeightExceedsPageHeightRenderTest() {
            String fileName = "footnoteHeightExceedsPageHeight";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(new SectionBreak(PageMarginsTestUtil.GetFootnoteMarginBoxes(A4_HEIGHT + 50f)));
                    Div forced = new Div().Add(new Paragraph("Content — footnote height > page height (FORCED).")).SetBackgroundColor
                        (new DeviceRgb(255, 100, 100));
                    document.Add(forced);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteHeightExceedsPageHeightNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    ApplyFootnoteMarginBoxes(document, A4_HEIGHT + 50f);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(60);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, 0, A4_HEIGHT + 50f, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when the footnote height exceeds the page height"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteDoublePageHeightNothingTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document document = new Document(pdfDoc)) {
                    ApplyFootnoteMarginBoxes(document, A4_HEIGHT * 2f);
                    Div element = new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetHeight(60);
                    int status = LayoutResultTestUtil.GetLayoutStatus(element, document, TestResourceUtil.GetAvailableRect(A4_HEIGHT
                        , A4_WIDTH, 36f, 0, A4_HEIGHT * 2f, 0, 0));
                    NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, status, "Element should return NOTHING when the footnote height is 2× the page height"
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 3)]
        public virtual void FootnoteExceedsPageHeightMultiplePagesRenderTest() {
            String fileName = "footnoteExceedsPageHeightMultiPage";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    ApplyFootnoteMarginBoxes(document, A4_HEIGHT + 50f);
                    for (int i = 1; i <= 3; i++) {
                        Div forced = new Div().Add(new Paragraph("PAGE " + i + " — footnote > page height (FORCED).")).SetBackgroundColor
                            (CellColor(i - 1));
                        document.Add(forced);
                        if (i < 3) {
                            document.Add(new SectionBreak(PageMarginsTestUtil.GetFootnoteMarginBoxes(A4_HEIGHT + 50f)));
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void HugeParagraphWithFootnoteAnchorInDivTest() {
            String fileName = "hugeParagraphFontWithFootnoteAnchorInDiv";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    pdfDoc.SetTagged();
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetFontSize
                        (27f)));
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    Paragraph p = new Paragraph().Add("Large paragraph text.").SetFontSize(155f).Add(new FootnoteAnchor("[1]", 
                        footnote));
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void HugeParagraphWithFootnoteAnchorTest() {
            String fileName = "hugeParagraphWithFootnoteAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(105));
                    Paragraph p = new Paragraph().Add("Large paragraph text.").SetFontSize(105f).Add(new FootnoteAnchor(new Text
                        ("[1]").SetFontSize(20).SetTextRise(100), footnote));
                    document.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void HugeFontAnchorFootnoteTest() {
            String fileName = "hugeFontAnchorFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    Paragraph p = new Paragraph().Add("Paragraph.").SetFontSize(30f).Add(new FootnoteAnchor(new Text("Large anchor text."
                        ).SetFontSize(80f), footnote));
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void HugeFontAnchorWithMultipleFootnotesInDivTest() {
            String fileName = "hugeFontAnchorWithMultipleFootnotesInDiv";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    pdfDoc.SetTagged();
                    for (int i = 1; i <= 2; i++) {
                        Footnote footnote = new Footnote("Footnote " + i + ": " + TestResourceUtil.GetByronStanza());
                        footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 2));
                        FootnoteAnchor footnoteAnchor = new FootnoteAnchor(new Text("Anchor " + i + " with huge font.").SetFontSize
                            (47f), footnote);
                        footnoteAnchor.SetProperty(Property.FORCED_PLACEMENT, true);
                        footnoteAnchor.SetProperty(Property.KEEP_TOGETHER, true);
                        Paragraph p = new Paragraph().Add("Paragraph " + i).Add(footnoteAnchor);
                        Div div = new Div().Add(p).SetBorder(new SolidBorder(CellColor(i - 1), 2));
                        document.Add(div);
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void HugeFontAnchorWithMultipleFootnotesInDiv2Test() {
            String fileName = "hugeFontAnchorWithMultipleFootnotesInDiv2";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    pdfDoc.SetTagged();
                    for (int i = 1; i <= 4; i++) {
                        Footnote footnote = new Footnote("Footnote " + i + ": " + TestResourceUtil.GetByronStanza());
                        footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 2));
                        FootnoteAnchor footnoteAnchor = new FootnoteAnchor(new Text("Anchor " + i + " with huge font.").SetFontSize
                            (48f), footnote);
                        footnoteAnchor.SetProperty(Property.FORCED_PLACEMENT, true);
                        footnoteAnchor.SetProperty(Property.KEEP_TOGETHER, true);
                        Paragraph p = new Paragraph().Add("Paragraph " + i).Add(footnoteAnchor);
                        Div div = new Div().Add(p).SetBorder(new SolidBorder(CellColor(i - 1), 2));
                        document.Add(div);
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void HugeFontAnchorWithMultipleFootnotesTest() {
            String fileName = "hugeFontAnchorWithMultipleFootnotes";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    for (int i = 1; i <= 4; i++) {
                        Footnote footnote = new Footnote("Footnote " + i + ": " + TestResourceUtil.GetByronStanza());
                        footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 2));
                        FootnoteAnchor footnoteAnchor = new FootnoteAnchor(new Text("Anchor " + i + " with huge font.").SetFontSize
                            (58f), footnote);
                        footnoteAnchor.SetProperty(Property.FORCED_PLACEMENT, true);
                        footnoteAnchor.SetProperty(Property.KEEP_TOGETHER, true);
                        Paragraph p = new Paragraph().Add("Paragraph " + i).Add(footnoteAnchor);
                        document.Add(p);
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void LargeImageAnchorFootnoteRenderTest() {
            String fileName = "largeImageAnchorFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 4));
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    iText.Layout.Element.Image largeImage = new Image(ImageDataFactory.Create(SOURCE_FOLDER + "bee.png"));
                    largeImage.SetWidth(A4_WIDTH * 0.80f);
                    largeImage.SetHeight(A4_HEIGHT * 0.70f);
                    FootnoteAnchor anchor = new FootnoteAnchor(largeImage, footnote);
                    document.Add(new Div().Add(new Paragraph().Add(anchor)).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)
                        ));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void LargeImageAnchorWithNormalContentRenderTest() {
            String fileName = "largeImageAnchorNormalContent";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote("Footnote for the large image anchor.\n" + TestResourceUtil.GetByronStanza
                        ());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    iText.Layout.Element.Image largeImage = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER
                         + "bee.png"));
                    largeImage.SetWidth(A4_WIDTH * 0.80f);
                    largeImage.SetHeight(A4_HEIGHT * 0.70f);
                    FootnoteAnchor anchor = new FootnoteAnchor(largeImage, footnote);
                    document.Add(new Div().Add(new Paragraph().Add(anchor)).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)
                        ));
                    document.Add(new Paragraph("Normal content after the large image anchor."));
                    document.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void SmallImageAnchorWithHugeTextFootnoteRenderTest() {
            String fileName = "smallImageAnchorHugeTextFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 8));
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png"));
                    image.SetWidth(15);
                    FootnoteAnchor anchor = new FootnoteAnchor(image, footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        private static void ApplyFootnoteMarginBoxes(Document document, float height) {
            document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetFootnoteMarginBoxes(height));
        }

        private static DeviceRgb CellColor(int index) {
            DeviceRgb[] palette = new DeviceRgb[] { new DeviceRgb(65, 151, 29), new DeviceRgb(209, 247, 29), new DeviceRgb
                (78, 151, 205), new DeviceRgb(255, 165, 0) };
            return palette[index % palette.Length];
        }
    }
}
