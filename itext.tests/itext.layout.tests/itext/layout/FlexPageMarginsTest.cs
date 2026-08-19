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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Renderer;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FlexPageMarginsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FlexPageMarginsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/FlexPageMarginsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerThenSectionBreakTest() {
            String fileName = "flexSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateRowFlexContainer();
                    flex.Add(ColoredDiv("FLEX ITEM 1", new DeviceRgb(65, 151, 29)));
                    flex.Add(ColoredDiv("FLEX ITEM 2", new DeviceRgb(209, 247, 29)));
                    flex.Add(ColoredDiv("FLEX ITEM 3", new DeviceRgb(78, 151, 205)));
                    document.Add(flex);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Page 2 — margins1 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexThenTwoSectionBreaksInARowTest() {
            String fileName = "flexTwoSectionBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateRowFlexContainer();
                    flex.Add(ColoredDiv("ITEM A", new DeviceRgb(65, 151, 29)));
                    flex.Add(ColoredDiv("ITEM B", new DeviceRgb(209, 247, 29)));
                    document.Add(flex);
                    document.Add(new SectionBreak(PageSize.A4.Rotate(), new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ())));
                    document.Add(new SectionBreak(PageSize.A5, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(new Paragraph("Final page — A5 with margins2."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexAlternatingSectionAndAreaBreaksTest() {
            String fileName = "flexAltBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex1 = CreateRowFlexContainer();
                    flex1.Add(ColoredDiv("S1-A", new DeviceRgb(65, 151, 29)));
                    flex1.Add(ColoredDiv("S1-B", new DeviceRgb(209, 247, 29)));
                    Div flex2 = CreateRowFlexContainer();
                    flex2.Add(ColoredDiv("S3-A", new DeviceRgb(78, 151, 205)));
                    flex2.Add(ColoredDiv("S3-B", new DeviceRgb(255, 165, 0)));
                    Div flex3 = CreateRowFlexContainer();
                    flex3.Add(ColoredDiv("S5-A", new DeviceRgb(200, 100, 100)));
                    flex3.Add(ColoredDiv("S5-B", new DeviceRgb(100, 200, 100)));
                    document.Add(flex1);
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("Page 2 — no special margins."));
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(flex2);
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("Page 4 — still margins1."));
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(flex3);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void MultiPageFlexThenSectionBreakTest() {
            String fileName = "flexMultiPageSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateRowFlexContainer();
                    for (int i = 1; i <= 20; i++) {
                        flex.Add(new Div().Add(new Paragraph("ITEM " + i)).SetWidth(UnitValue.CreatePercentValue(30)).SetHeight(200
                            ).SetBackgroundColor(i % 2 == 0 ? new DeviceRgb(65, 151, 29) : new DeviceRgb(209, 247, 29)).SetMargin(
                            5));
                    }
                    document.Add(flex);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Post-flex section — margins1 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void MultiPageFlexWithDocumentMarginsTest() {
            String fileName = "flexMultiPageDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    Div flex = CreateColumnFlexContainer();
                    for (int i = 0; i < 3; i++) {
                        Div row = CreateRowFlexContainer();
                        for (int j = 0; j < 3; j++) {
                            row.Add(new Div().Add(new Paragraph("R" + i + "C" + j + "\n" + TestResourceUtil.GetByronStanza())).SetWidth
                                (UnitValue.CreatePercentValue(30)).SetBackgroundColor(j % 2 == 0 ? new DeviceRgb(65, 151, 29) : new DeviceRgb
                                (209, 247, 29)).SetMargin(5));
                        }
                        flex.Add(row);
                    }
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexWithPerPageDocumentMarginsTest() {
            String fileName = "flexPerPageDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => {
                        IList<PageMarginContent> margins = new List<PageMarginContent>();
                        margins.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("Page " + pageNum)).SetBackgroundColor
                            (ColorConstants.PINK).SetTextAlignment(TextAlignment.CENTER)));
                        return new PageMarginBoxes(margins);
                    }
                    );
                    Div flex = CreateColumnFlexContainer();
                    for (int i = 0; i < 4; i++) {
                        Div row = CreateRowFlexContainer();
                        for (int j = 0; j < 3; j++) {
                            row.Add(new Div().Add(new Paragraph(TestResourceUtil.GetByronStanza())).SetWidth(UnitValue.CreatePercentValue
                                (30)).SetBackgroundColor(j % 2 == 0 ? new DeviceRgb(65, 151, 29) : new DeviceRgb(209, 247, 29)).SetMargin
                                (4));
                        }
                        flex.Add(row);
                    }
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexDocumentMarginsOverriddenBySectionBreakTest() {
            String fileName = "flexDocMarginsOverriddenBySectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    Div flex1 = CreateRowFlexContainer();
                    flex1.Add(ColoredDiv("S1-A", new DeviceRgb(65, 151, 29)));
                    flex1.Add(ColoredDiv("S1-B", new DeviceRgb(209, 247, 29)));
                    flex1.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 3)));
                    Div flex2 = CreateRowFlexContainer();
                    flex2.Add(ColoredDiv("S2-A", new DeviceRgb(78, 151, 205)));
                    flex2.Add(ColoredDiv("S2-B", new DeviceRgb(255, 165, 0)));
                    document.Add(flex1);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(flex2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerWithElementMarginsAndSectionBreakTest() {
            String fileName = "flexElementMarginsSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateRowFlexContainer();
                    flex.SetMargins(50, 50, 50, 50).SetBackgroundColor(new DeviceRgb(220, 220, 220));
                    flex.Add(ColoredDiv("ITEM 1", new DeviceRgb(65, 151, 29)));
                    flex.Add(ColoredDiv("ITEM 2", new DeviceRgb(209, 247, 29)));
                    flex.Add(ColoredDiv("ITEM 3", new DeviceRgb(78, 151, 205)));
                    document.Add(flex);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Page 2 — section margins1 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexItemsWithElementMarginsAndDocumentPageMarginsTest() {
            String fileName = "flexItemMarginsDocPageMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    Div flex = CreateRowFlexContainer();
                    Div item1 = new Div().Add(new Paragraph("LARGE MARGIN\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                        (new DeviceRgb(65, 151, 29)).SetWidth(UnitValue.CreatePercentValue(28)).SetMargins(30, 20, 30, 20);
                    Div item2 = new Div().Add(new Paragraph("NO MARGIN\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                        (new DeviceRgb(209, 247, 29)).SetWidth(UnitValue.CreatePercentValue(28)).SetMargin(0);
                    Div item3 = new Div().Add(new Paragraph("LARGE PADDING\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                        (new DeviceRgb(78, 151, 205)).SetWidth(UnitValue.CreatePercentValue(28)).SetPaddings(25, 25, 25, 25);
                    flex.Add(item1);
                    flex.Add(item2);
                    flex.Add(item3);
                    document.Add(flex);
                    document.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 8)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexWithStaticDocumentMarginsAndSectionBreakTest() {
            String fileName = "flexStaticMarginsAndSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetMargins(80, 80, 80, 80);
                    Div flex = CreateRowFlexContainer();
                    flex.Add(ColoredDiv("ITEM 1", new DeviceRgb(65, 151, 29)));
                    flex.Add(ColoredDiv("ITEM 2", new DeviceRgb(209, 247, 29)));
                    flex.Add(ColoredDiv("ITEM 3", new DeviceRgb(78, 151, 205)));
                    document.Add(flex);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(new Div().Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 
                        3))).SetBackgroundColor(new DeviceRgb(255, 165, 0)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FlexWithPageNumberSpecificMarginsTest() {
            String fileName = "flexPageNumMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins(1, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1()));
                    Div flex = CreateColumnFlexContainer();
                    for (int i = 0; i < 3; i++) {
                        flex.Add(new Div().Add(new Paragraph("ITEM " + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                            (i % 2 == 0 ? new DeviceRgb(65, 151, 29) : new DeviceRgb(209, 247, 29)));
                    }
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedFlexAroundSectionBreakTest() {
            String fileName = "nestedFlexSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div outer1 = CreateColumnFlexContainer();
                    Div inner1 = CreateRowFlexContainer();
                    inner1.Add(ColoredDiv("S1-A", new DeviceRgb(65, 151, 29)));
                    inner1.Add(ColoredDiv("S1-B", new DeviceRgb(209, 247, 29)));
                    outer1.Add(ColoredDiv("S1 TOP", new DeviceRgb(78, 151, 205)));
                    outer1.Add(inner1);
                    Div outer2 = CreateColumnFlexContainer();
                    Div inner2 = CreateRowFlexContainer();
                    inner2.Add(ColoredDiv("S2-A", new DeviceRgb(200, 100, 100)));
                    inner2.Add(ColoredDiv("S2-B", new DeviceRgb(100, 200, 100)));
                    outer2.Add(ColoredDiv("S2 TOP", new DeviceRgb(255, 165, 0)));
                    outer2.Add(inner2);
                    document.Add(outer1);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(outer2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DeeplyNestedFlexWithDocumentAndSectionMarginsTest() {
            String fileName = "deepNestedFlexMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 != 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    Div outerCol = CreateColumnFlexContainer();
                    for (int row = 0; row < 3; row++) {
                        Div midRow = CreateRowFlexContainer();
                        for (int col = 0; col < 2; col++) {
                            Div innerCol = CreateColumnFlexContainer();
                            innerCol.SetWidth(UnitValue.CreatePercentValue(45)).SetMargin(4);
                            innerCol.Add(new Div().Add(new Paragraph("R" + row + "C" + col + "-TOP\n" + TestResourceUtil.GetByronStanza
                                ())).SetBackgroundColor(col == 0 ? new DeviceRgb(65, 151, 29) : new DeviceRgb(209, 247, 29)));
                            innerCol.Add(new Div().Add(new Paragraph("R" + row + "C" + col + "-BOT\n" + TestResourceUtil.GetByronStanza
                                ())).SetBackgroundColor(col == 0 ? new DeviceRgb(78, 151, 205) : new DeviceRgb(255, 165, 0)));
                            midRow.Add(innerCol);
                        }
                        outerCol.Add(midRow);
                    }
                    document.Add(outerCol);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(new Paragraph("Final section — margins2 override document margins."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.FLEX_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void SectionBreakInsideFlexContainerTest() {
            String fileName = "sectionBreakInsideFlexContainer";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateRowFlexContainer();
                    flex.Add(ColoredDiv("ITEM A", new DeviceRgb(65, 151, 29)));
                    flex.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    flex.Add(ColoredDiv("ITEM B", new DeviceRgb(209, 247, 29)));
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.SECTION_BREAK_IGNORED)]
        public virtual void SectionBreakOnFlexItemChildTest() {
            String fileName = "sectionBreakOnFlexItemChild";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateRowFlexContainer();
                    Div item = new Div().SetBackgroundColor(new DeviceRgb(65, 151, 29)).SetWidth(UnitValue.CreatePercentValue(
                        80));
                    item.Add(new Paragraph("Content before break."));
                    item.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    item.Add(new Paragraph("Content after break."));
                    flex.Add(item);
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.AREA_BREAK_IGNORED)]
        public virtual void AreaBreakOnFlexItemChildTest() {
            String fileName = "flexItemChildAreaBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateColumnFlexContainer();
                    Div item = new Div().SetBackgroundColor(new DeviceRgb(209, 247, 29)).SetWidth(UnitValue.CreatePercentValue
                        (80));
                    item.Add(new Paragraph("Content before area break in item."));
                    item.Add(new AreaBreak());
                    item.Add(new Paragraph("Content after area break in item."));
                    flex.Add(ColoredDiv("ITEM ABOVE", new DeviceRgb(65, 151, 29)));
                    flex.Add(item);
                    flex.Add(ColoredDiv("ITEM BELOW", new DeviceRgb(78, 151, 205)));
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.FLEX_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void AreaBreakInFlexWithDocumentMarginsTest() {
            String fileName = "flexAreaBreakDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    Div flex = CreateColumnFlexContainer();
                    flex.Add(ColoredDiv("Before break.", new DeviceRgb(65, 151, 29)));
                    flex.Add(new AreaBreak());
                    flex.Add(ColoredDiv("After break.", new DeviceRgb(209, 247, 29)));
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.FLEX_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void AreaBreakInFlexThenSectionBreakTest() {
            String fileName = "flexAreaBreakThenSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateColumnFlexContainer();
                    flex.Add(ColoredDiv("Before break.", new DeviceRgb(65, 151, 29)));
                    flex.Add(new AreaBreak());
                    flex.Add(ColoredDiv("After break.", new DeviceRgb(209, 247, 29)));
                    document.Add(flex);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(new Paragraph("Page 2 — margins2 active after section break."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.FLEX_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK, Count = 2)]
        public virtual void MultipleAreaBreaksInNestedFlexWithDocumentMarginsTest() {
            String fileName = "nestedFlexMultiAreaBreakDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 != 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    Div outer = CreateColumnFlexContainer();
                    Div row1 = CreateRowFlexContainer();
                    row1.Add(ColoredDiv("DIV 1 - A", new DeviceRgb(65, 151, 29)));
                    row1.Add(ColoredDiv("DIV 1 - B", new DeviceRgb(209, 247, 29)));
                    Div row2 = CreateRowFlexContainer();
                    row2.Add(ColoredDiv("DIV 2 - A", new DeviceRgb(78, 151, 205)));
                    row2.Add(ColoredDiv("DIV 2 - B", new DeviceRgb(255, 165, 0)));
                    Div row3 = CreateRowFlexContainer();
                    row3.Add(ColoredDiv("DIV 3 - A", new DeviceRgb(200, 100, 100)));
                    row3.Add(ColoredDiv("DIV 3 - B", new DeviceRgb(100, 200, 100)));
                    outer.Add(row1);
                    outer.Add(new AreaBreak());
                    outer.Add(row2);
                    outer.Add(new AreaBreak());
                    outer.Add(row3);
                    document.Add(outer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.AREA_BREAK_IGNORED)]
        public virtual void AreaBreakOnNestedFlexItemWithDocumentMarginsTest() {
            String fileName = "nestedFlexItemAreaBreakDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    Div flex = CreateColumnFlexContainer();
                    Div item = new Div().SetBackgroundColor(new DeviceRgb(220, 220, 255)).SetWidth(UnitValue.CreatePercentValue
                        (80));
                    item.Add(new Paragraph("Item content before nested area break."));
                    item.Add(new Div().Add(new Paragraph("Inner div before break.")).Add(new Paragraph(TestResourceUtil.RepeatString
                        (TestResourceUtil.GetByronStanza(), 5))).Add(new AreaBreak()).Add(new Paragraph("Inner div after break."
                        )).SetBackgroundColor(new DeviceRgb(209, 247, 29)));
                    item.Add(new Paragraph("Item content after nested area break."));
                    flex.Add(ColoredDiv("ABOVE", new DeviceRgb(65, 151, 29)));
                    flex.Add(item);
                    flex.Add(ColoredDiv("BELOW", new DeviceRgb(78, 151, 205)));
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.FLEX_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void AreaBreakWithPageSizeInFlexWithDocumentMarginsTest() {
            String fileName = "flexAreaBreakPageSizeDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    Div flex = CreateColumnFlexContainer();
                    flex.Add(ColoredDiv("A4 PAGE - ITEM", new DeviceRgb(65, 151, 29)));
                    flex.Add(new AreaBreak(PageSize.A5));
                    flex.Add(ColoredDiv("A5 PAGE - ITEM", new DeviceRgb(209, 247, 29)));
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedFlexOuterElementMarginsWithDocumentPageMarginsTest() {
            String fileName = "nestedFlexOuterElemMarginsDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    Div outer = CreateRowFlexContainer();
                    outer.SetMargins(40, 40, 40, 40).SetBackgroundColor(new DeviceRgb(220, 220, 220));
                    Div inner1 = CreateColumnFlexContainer();
                    inner1.SetWidth(UnitValue.CreatePercentValue(45)).SetMargin(5);
                    inner1.Add(ColoredDiv("INNER-1 A", new DeviceRgb(65, 151, 29)));
                    inner1.Add(ColoredDiv("INNER-1 B", new DeviceRgb(209, 247, 29)));
                    Div inner2 = CreateColumnFlexContainer();
                    inner2.SetWidth(UnitValue.CreatePercentValue(45)).SetMargin(5);
                    inner2.Add(ColoredDiv("INNER-2 A", new DeviceRgb(78, 151, 205)));
                    inner2.Add(ColoredDiv("INNER-2 B", new DeviceRgb(255, 165, 0)));
                    outer.Add(inner1);
                    outer.Add(inner2);
                    document.Add(outer);
                    document.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 6)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void AsymmetricNestedFlexWithDocumentMarginsTest() {
            String fileName = "nestedFlexAsymmetricDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    Div outer = CreateRowFlexContainer();
                    Div leftCol = CreateColumnFlexContainer();
                    leftCol.SetWidth(UnitValue.CreatePercentValue(45)).SetMargin(5);
                    Div leftInner = CreateColumnFlexContainer();
                    leftInner.Add(ColoredDiv("L-DEEP A", new DeviceRgb(65, 151, 29)));
                    leftInner.Add(ColoredDiv("L-DEEP B", new DeviceRgb(209, 247, 29)));
                    leftCol.Add(ColoredDiv("L-TOP", new DeviceRgb(78, 151, 205)));
                    leftCol.Add(leftInner);
                    Div rightCol = CreateColumnFlexContainer();
                    rightCol.SetWidth(UnitValue.CreatePercentValue(45)).SetMargin(5);
                    rightCol.Add(ColoredDiv("R A", new DeviceRgb(255, 165, 0)));
                    rightCol.Add(ColoredDiv("R B", new DeviceRgb(200, 100, 100)));
                    rightCol.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 3)));
                    outer.Add(leftCol);
                    outer.Add(rightCol);
                    document.Add(outer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedFlexOverflowWithStaticDocumentMarginsTest() {
            String fileName = "nestedFlexOverflowStaticMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetMargins(80, 80, 80, 80);
                    Div outer = CreateColumnFlexContainer();
                    for (int i = 0; i < 4; i++) {
                        Div row = CreateRowFlexContainer();
                        for (int j = 0; j < 3; j++) {
                            row.Add(new Div().Add(new Paragraph("R" + i + "C" + j + "\n" + TestResourceUtil.GetByronStanza())).SetWidth
                                (UnitValue.CreatePercentValue(30)).SetBackgroundColor(j % 2 == 0 ? new DeviceRgb(65, 151, 29) : new DeviceRgb
                                (209, 247, 29)).SetMargin(4));
                        }
                        outer.Add(row);
                    }
                    document.Add(outer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedFlexOverflowPageNumberSpecificMarginsTest() {
            String fileName = "nestedFlexOverflowPageNumMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins(3, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1()));
                    Div outer = CreateColumnFlexContainer();
                    for (int i = 0; i < 3; i++) {
                        Div row = CreateRowFlexContainer();
                        for (int j = 0; j < 3; j++) {
                            row.Add(new Div().Add(new Paragraph("R" + i + "C" + j + "\n" + TestResourceUtil.GetByronStanza())).SetWidth
                                (UnitValue.CreatePercentValue(30)).SetBackgroundColor(j % 2 == 0 ? new DeviceRgb(65, 151, 29) : new DeviceRgb
                                (209, 247, 29)).SetMargin(4));
                        }
                        outer.Add(row);
                    }
                    document.Add(outer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedFlexSameMarginsAppliedTwiceTest() {
            String fileName = "nestedFlexSameMarginsTwice";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex1 = CreateColumnFlexContainer();
                    Div row1 = CreateRowFlexContainer();
                    row1.Add(ColoredDiv("S1 ITEM A", new DeviceRgb(65, 151, 29)));
                    row1.Add(ColoredDiv("S1 ITEM B", new DeviceRgb(209, 247, 29)));
                    flex1.Add(row1);
                    flex1.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 3)));
                    Div flex2 = CreateColumnFlexContainer();
                    Div row2 = CreateRowFlexContainer();
                    row2.Add(ColoredDiv("S2 ITEM A", new DeviceRgb(78, 151, 205)));
                    row2.Add(ColoredDiv("S2 ITEM B", new DeviceRgb(255, 165, 0)));
                    flex2.Add(row2);
                    flex2.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 3)));
                    document.Add(flex1);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(flex2);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Third section — same margins again."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.SECTION_BREAK_IGNORED, Count = 4)]
        [LogMessage(LayoutLogMessageConstant.AREA_BREAK_IGNORED, Count = 4)]
        public virtual void FlexWithTableHeaderAndFooterWithAreaBreakAndSectionBreakTest() {
            String fileName = "flexWithTableHeaderAndFooter";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div flex = CreateRowFlexContainer();
                    Table table = new Table(3);
                    Cell headerCell = new Cell().Add(new Div().Add(new Paragraph("Before section break")).Add(new SectionBreak
                        (new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1()))).Add(new Paragraph("After section break")
                        ));
                    table.AddHeaderCell(headerCell);
                    table.AddHeaderCell(new Cell());
                    table.AddHeaderCell(new Cell());
                    table.AddCell("Table cell content 1");
                    table.AddCell("Table cell content 2");
                    table.AddCell("Table cell content 3");
                    Cell footerCell = new Cell().Add(new Div().Add(new Paragraph("Before area break")).Add(new AreaBreak()).Add
                        (new Paragraph("After area break")));
                    table.AddFooterCell(footerCell);
                    table.AddFooterCell(new Cell());
                    table.AddFooterCell(new Cell());
                    flex.Add(table);
                    flex.Add(ColoredDiv("Second element", new DeviceRgb(65, 151, 29)));
                    document.Add(flex);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        private static Div CreateRowFlexContainer() {
            Div flex = new Div();
            flex.SetNextRenderer(new FlexContainerRenderer(flex));
            flex.SetProperty(Property.FLEX_DIRECTION, FlexDirectionPropertyValue.ROW);
            flex.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP);
            flex.SetProperty(Property.JUSTIFY_CONTENT, JustifyContent.FLEX_START);
            return flex;
        }

        private static Div CreateColumnFlexContainer() {
            Div flex = new Div();
            flex.SetNextRenderer(new FlexContainerRenderer(flex));
            flex.SetProperty(Property.FLEX_DIRECTION, FlexDirectionPropertyValue.COLUMN);
            flex.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.NOWRAP);
            return flex;
        }

        private static Div ColoredDiv(String label, DeviceRgb color) {
            return new Div().Add(new Paragraph(label)).SetBackgroundColor(color).SetWidth(UnitValue.CreatePercentValue
                (30)).SetMargin(5).SetPadding(8);
        }
    }
}
