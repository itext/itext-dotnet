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
using iText.Layout.Properties.Grid;
using iText.Layout.Properties.Margins;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GridPageMarginsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/GridPageMarginsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/GridPageMarginsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void GridContainerThenSectionBreakTest() {
            String fileName = "gridSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer grid = CreateThreeColumnGrid();
                    for (int i = 1; i <= 6; i++) {
                        grid.Add(ColoredDiv("ITEM " + i, CellColor(i)));
                    }
                    document.Add(grid);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Page 2 — margins1 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridThenTwoSectionBreaksInARowTest() {
            String fileName = "gridTwoSectionBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer grid = CreateThreeColumnGrid();
                    for (int i = 1; i <= 6; i++) {
                        grid.Add(ColoredDiv("ITEM " + i, CellColor(i)));
                    }
                    document.Add(grid);
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
        public virtual void MultiPageGridThenSectionBreakTest() {
            String fileName = "gridMultiPageSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer grid = CreateThreeColumnGrid();
                    for (int i = 1; i <= 18; i++) {
                        grid.Add(new Div().Add(new Paragraph("ITEM " + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                            (CellColor(i)));
                    }
                    document.Add(grid);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Post-grid section — margins1 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridSameMarginsAppliedTwiceTest() {
            String fileName = "gridSameMarginsTwice";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer grid1 = CreateThreeColumnGrid();
                    for (int i = 1; i <= 6; i++) {
                        grid1.Add(ColoredDiv("S1-" + i, CellColor(i)));
                    }
                    GridContainer grid2 = CreateThreeColumnGrid();
                    for (int i = 1; i <= 6; i++) {
                        grid2.Add(ColoredDiv("S2-" + i, CellColor(i + 3)));
                    }
                    document.Add(grid1);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(grid2);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Third section — same margins again."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridAlternatingSectionAndAreaBreaksTest() {
            String fileName = "gridAltBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.Add(BuildSmallGrid("S1", 1));
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("Page 2 — no special margins."));
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(BuildSmallGrid("S3", 2));
                    document.Add(new AreaBreak());
                    document.Add(new Paragraph("Page 4 — still margins1."));
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(BuildSmallGrid("S5", 3));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void MultiPageGridWithDocumentMarginsTest() {
            String fileName = "gridMultiPageDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    GridContainer grid = CreateThreeColumnGrid();
                    for (int i = 1; i <= 18; i++) {
                        grid.Add(new Div().Add(new Paragraph("CELL " + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                            (CellColor(i)));
                    }
                    document.Add(grid);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridWithPerPageDocumentMarginsTest() {
            String fileName = "gridPerPageDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => {
                        IList<PageMarginContent> margins = new List<PageMarginContent>();
                        margins.Add(new PageMarginContent(MarginBoxName.TOP, new Div().Add(new Paragraph("Page " + pageNum)).SetBackgroundColor
                            (ColorConstants.PINK).SetTextAlignment(TextAlignment.CENTER)));
                        return new PageMarginBoxes(margins);
                    }
                    );
                    GridContainer grid = CreateThreeColumnGrid();
                    for (int i = 1; i <= 15; i++) {
                        grid.Add(new Div().Add(new Paragraph("CELL " + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                            (CellColor(i)));
                    }
                    document.Add(grid);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridDocumentMarginsOverriddenBySectionBreakTest() {
            String fileName = "gridDocMarginsOverriddenBySectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    GridContainer grid1 = CreateThreeColumnGrid();
                    for (int i = 1; i <= 6; i++) {
                        grid1.Add(new Div().Add(new Paragraph("S1-" + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                            (CellColor(i)));
                    }
                    GridContainer grid2 = CreateTwoColumnGrid();
                    for (int i = 1; i <= 4; i++) {
                        grid2.Add(ColoredDiv("S2-" + i, CellColor(i + 2)));
                    }
                    document.Add(grid1);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(grid2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridWithStaticDocumentMarginsAndSectionBreakTest() {
            String fileName = "gridStaticMarginsAndSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetMargins(80, 80, 80, 80);
                    GridContainer grid1 = CreateThreeColumnGrid();
                    for (int i = 1; i <= 6; i++) {
                        grid1.Add(ColoredDiv("BEFORE-" + i, CellColor(i)));
                    }
                    GridContainer grid2 = CreateTwoColumnGrid();
                    for (int i = 1; i <= 4; i++) {
                        grid2.Add(ColoredDiv("AFTER-" + i, CellColor(i + 3)));
                    }
                    document.Add(grid1);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(grid2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridWithPageNumberSpecificMarginsTest() {
            String fileName = "gridPageNumMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins(2, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1()));
                    GridContainer grid = CreateThreeColumnGrid();
                    for (int i = 1; i <= 18; i++) {
                        grid.Add(new Div().Add(new Paragraph("CELL " + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                            (CellColor(i)));
                    }
                    document.Add(grid);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridContainerWithElementMarginsAndSectionBreakTest() {
            String fileName = "gridElemMarginsSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer grid = CreateThreeColumnGrid();
                    grid.SetMargins(40, 40, 40, 40).SetBackgroundColor(new DeviceRgb(220, 220, 220));
                    for (int i = 1; i <= 6; i++) {
                        grid.Add(ColoredDiv("ITEM " + i, CellColor(i)));
                    }
                    document.Add(grid);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1())));
                    document.Add(new Paragraph("Page 2 — section margins1 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridItemsWithElementMarginsAndDocumentPageMarginsTest() {
            String fileName = "gridItemMarginsDocPageMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    GridContainer grid = CreateThreeColumnGrid();
                    grid.Add(new Div().Add(new Paragraph("LARGE MARGIN\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                        (new DeviceRgb(65, 151, 29)).SetMargins(20, 15, 20, 15));
                    grid.Add(new Div().Add(new Paragraph("NO MARGIN\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                        (new DeviceRgb(209, 247, 29)).SetMargin(0));
                    grid.Add(new Div().Add(new Paragraph("LARGE PADDING\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                        (new DeviceRgb(78, 151, 205)).SetPaddings(20, 20, 20, 20));
                    grid.Add(new Div().Add(new Paragraph("MIXED\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor(new 
                        DeviceRgb(255, 165, 0)).SetMarginTop(30).SetPaddingBottom(30));
                    grid.Add(new Div().Add(new Paragraph("DEFAULT\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor(
                        new DeviceRgb(200, 100, 100)));
                    grid.Add(new Div().Add(new Paragraph("SMALL PADDING\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                        (new DeviceRgb(100, 200, 100)).SetPadding(5));
                    document.Add(grid);
                    document.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 5)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridFractionColumnsThenSectionBreakTest() {
            String fileName = "gridFractionColsSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer grid = new GridContainer();
                    IList<TemplateValue> columns = new List<TemplateValue>();
                    columns.Add(new FlexValue(1));
                    columns.Add(new FlexValue(2));
                    grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, columns);
                    grid.SetProperty(Property.GRID_FLOW, GridFlow.ROW);
                    for (int i = 1; i <= 6; i++) {
                        grid.Add(ColoredDiv("ITEM " + i, CellColor(i)));
                    }
                    document.Add(grid);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(new Paragraph("Page 2 — margins2 active."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridMixedColumnSizingWithDocumentMarginsTest() {
            String fileName = "gridMixedColsDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    GridContainer grid = new GridContainer();
                    IList<TemplateValue> columns = new List<TemplateValue>();
                    columns.Add(new FlexValue(1));
                    columns.Add(new FlexValue(2));
                    columns.Add(new FlexValue(1));
                    grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, columns);
                    grid.SetProperty(Property.GRID_FLOW, GridFlow.ROW);
                    for (int i = 1; i <= 9; i++) {
                        grid.Add(new Div().Add(new Paragraph("CELL " + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                            (CellColor(i)));
                    }
                    document.Add(grid);
                    document.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 4)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void GridPercentColumnsWithPageSizeSectionBreakTest() {
            String fileName = "gridPercentColsPageSizeSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer grid1 = new GridContainer();
                    IList<TemplateValue> columns1 = new List<TemplateValue>();
                    columns1.Add(new PercentValue(50));
                    columns1.Add(new PercentValue(50));
                    grid1.SetProperty(Property.GRID_TEMPLATE_COLUMNS, columns1);
                    grid1.SetProperty(Property.GRID_FLOW, GridFlow.ROW);
                    for (int i = 1; i <= 4; i++) {
                        grid1.Add(ColoredDiv("A4-" + i, CellColor(i)));
                    }
                    GridContainer grid2 = new GridContainer();
                    IList<TemplateValue> columns2 = new List<TemplateValue>();
                    columns2.Add(new PercentValue(50));
                    columns2.Add(new PercentValue(50));
                    grid2.SetProperty(Property.GRID_TEMPLATE_COLUMNS, columns2);
                    grid2.SetProperty(Property.GRID_FLOW, GridFlow.ROW);
                    for (int i = 1; i <= 4; i++) {
                        grid2.Add(ColoredDiv("A4R-" + i, CellColor(i + 2)));
                    }
                    document.Add(grid1);
                    document.Add(new SectionBreak(PageSize.A4.Rotate(), new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ())));
                    document.Add(grid2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.GRID_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void AreaBreakInsideNestedGridCellWithDocumentMarginsTest() {
            String fileName = "nestedGridCellAreaBreakDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    GridContainer outer = CreateTwoColumnGrid();
                    outer.SetBackgroundColor(new DeviceRgb(220, 220, 220));
                    GridContainer innerLeft = CreateTwoColumnGrid();
                    innerLeft.Add(ColoredDiv("LEFT-1", new DeviceRgb(65, 151, 29)));
                    innerLeft.Add(ColoredDiv("LEFT-2", new DeviceRgb(209, 247, 29)));
                    Div breakCell = new Div().Add(new Paragraph("Before break.")).Add(new AreaBreak()).Add(new Paragraph("After break."
                        ));
                    innerLeft.Add(breakCell);
                    innerLeft.Add(ColoredDiv("LEFT-4", new DeviceRgb(78, 151, 205)));
                    GridContainer innerRight = CreateTwoColumnGrid();
                    innerRight.Add(ColoredDiv("RIGHT-1", new DeviceRgb(255, 165, 0)));
                    innerRight.Add(ColoredDiv("RIGHT-2", new DeviceRgb(200, 100, 100)));
                    innerRight.Add(ColoredDiv("RIGHT-3", new DeviceRgb(100, 200, 100)));
                    innerRight.Add(ColoredDiv("RIGHT-4", new DeviceRgb(65, 151, 29)));
                    outer.Add(innerLeft);
                    outer.Add(innerRight);
                    document.Add(outer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedGridsWithDocumentMarginsTest() {
            String fileName = "nestedGridsDocMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    GridContainer outer = CreateThreeColumnGrid();
                    for (int col = 0; col < 3; col++) {
                        GridContainer inner = CreateTwoColumnGrid();
                        for (int i = 1; i <= 4; i++) {
                            inner.Add(new Div().Add(new Paragraph("C" + col + "-" + i + "\n" + TestResourceUtil.GetByronStanza())).SetBackgroundColor
                                (CellColor(col * 2 + i)));
                        }
                        outer.Add(inner);
                    }
                    document.Add(outer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedGridsAroundAreaBreakTest() {
            String fileName = "nestedGridsAreaBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer outer1 = CreateTwoColumnGrid();
                    for (int col = 0; col < 2; col++) {
                        GridContainer inner = CreateThreeColumnGrid();
                        for (int i = 1; i <= 3; i++) {
                            inner.Add(ColoredDiv("P1-C" + col + "-" + i, CellColor(col * 3 + i)));
                        }
                        outer1.Add(inner);
                    }
                    GridContainer outer2 = CreateTwoColumnGrid();
                    for (int col = 0; col < 2; col++) {
                        GridContainer inner = CreateThreeColumnGrid();
                        for (int i = 1; i <= 3; i++) {
                            inner.Add(ColoredDiv("P2-C" + col + "-" + i, CellColor(col * 2 + i + 1)));
                        }
                        outer2.Add(inner);
                    }
                    document.Add(outer1);
                    document.Add(new AreaBreak(PageSize.A5));
                    document.Add(outer2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedGridsWithPageSizeSectionBreakTest() {
            String fileName = "nestedGridsPageSizeSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer outer1 = CreateThreeColumnGrid();
                    for (int col = 0; col < 3; col++) {
                        GridContainer inner = CreateTwoColumnGrid();
                        for (int i = 1; i <= 2; i++) {
                            inner.Add(ColoredDiv("S1-C" + col + "-" + i, CellColor(col + i)));
                        }
                        outer1.Add(inner);
                    }
                    GridContainer outer2 = CreateThreeColumnGrid();
                    for (int col = 0; col < 3; col++) {
                        GridContainer inner = CreateTwoColumnGrid();
                        for (int i = 1; i <= 2; i++) {
                            inner.Add(ColoredDiv("S2-C" + col + "-" + i, CellColor(col + i + 2)));
                        }
                        outer2.Add(inner);
                    }
                    document.Add(outer1);
                    document.Add(new SectionBreak(PageSize.A4.Rotate(), new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ())));
                    document.Add(outer2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DeeplyNestedGridsWithDocumentAndSectionMarginsTest() {
            String fileName = "deepNestedGridsMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 != 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    GridContainer outer = CreateTwoColumnGrid();
                    outer.SetBackgroundColor(new DeviceRgb(240, 240, 240));
                    for (int o = 0; o < 2; o++) {
                        GridContainer mid = CreateTwoColumnGrid();
                        for (int m = 0; m < 2; m++) {
                            GridContainer inner = CreateTwoColumnGrid();
                            for (int i = 1; i <= 2; i++) {
                                inner.Add(new Div().Add(new Paragraph("O" + o + "M" + m + "I" + i + "\n" + TestResourceUtil.GetByronStanza
                                    ())).SetBackgroundColor(CellColor(o * 4 + m * 2 + i)));
                            }
                            mid.Add(inner);
                        }
                        outer.Add(mid);
                    }
                    document.Add(outer);
                    document.Add(new SectionBreak(new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2())));
                    document.Add(new Paragraph("Final section — margins2 override document margins."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.GRID_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void AreaBreakDirectlyInsideGridContainerTest() {
            String fileName = "areaBreakDirectlyInsideGridContainer";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    GridContainer grid = CreateThreeColumnGrid();
                    grid.Add(ColoredDiv("BEFORE-1", new DeviceRgb(65, 151, 29)));
                    grid.Add(ColoredDiv("BEFORE-2", new DeviceRgb(209, 247, 29)));
                    grid.Add(ColoredDiv("BEFORE-3", new DeviceRgb(78, 151, 205)));
                    grid.Add(new AreaBreak());
                    grid.Add(ColoredDiv("AFTER-1", new DeviceRgb(255, 165, 0)));
                    grid.Add(ColoredDiv("AFTER-2", new DeviceRgb(200, 100, 100)));
                    grid.Add(ColoredDiv("AFTER-3", new DeviceRgb(100, 200, 100)));
                    document.Add(grid);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.GRID_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void SectionBreakDirectlyInsideGridContainerTest() {
            String fileName = "sectionBreakDirectlyInsideGridContainer";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins2
                        ()));
                    GridContainer grid = CreateThreeColumnGrid();
                    grid.Add(ColoredDiv("BEFORE-1", new DeviceRgb(65, 151, 29)));
                    grid.Add(ColoredDiv("BEFORE-2", new DeviceRgb(209, 247, 29)));
                    grid.Add(ColoredDiv("BEFORE-3", new DeviceRgb(78, 151, 205)));
                    grid.Add(new SectionBreak(PageSize.A5));
                    grid.Add(ColoredDiv("AFTER-1", new DeviceRgb(255, 165, 0)));
                    grid.Add(ColoredDiv("AFTER-2", new DeviceRgb(200, 100, 100)));
                    grid.Add(ColoredDiv("AFTER-3", new DeviceRgb(100, 200, 100)));
                    document.Add(grid);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.GRID_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void SectionBreakInsideNestedGridCellTest() {
            String fileName = "sectionBreakInNestedGrid";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer outer = CreateTwoColumnGrid();
                    GridContainer inner = CreateTwoColumnGrid();
                    inner.Add(ColoredDiv("BEFORE", new DeviceRgb(65, 151, 29)));
                    Div breakCell = new Div().Add(new Paragraph("Before section break.")).Add(new SectionBreak(new PageMarginBoxes
                        (PageMarginsTestUtil.GetPageMargins1()))).Add(new Paragraph("After section break."));
                    inner.Add(breakCell);
                    inner.Add(ColoredDiv("AFTER", new DeviceRgb(209, 247, 29)));
                    outer.Add(inner);
                    outer.Add(ColoredDiv("OTHER CELL", new DeviceRgb(78, 151, 205)));
                    document.Add(outer);
                    document.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 6)));
                    document.Add(new Paragraph("Page 2 — PageMargins1 should be active here if SectionBreak was honoured."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.GRID_CONTAINER_SHOULD_NOT_CONTAIN_AREA_OR_SECTION_BREAK)]
        public virtual void AreaBreakInsideNestedGridCellTest() {
            String fileName = "areaBreakInNestedGrid";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer outer = CreateTwoColumnGrid();
                    GridContainer inner = CreateTwoColumnGrid();
                    inner.Add(ColoredDiv("BEFORE", new DeviceRgb(65, 151, 29)));
                    Div breakCell = new Div().Add(new Paragraph("Before area break.")).Add(new AreaBreak()).Add(new Paragraph(
                        "After area break."));
                    inner.Add(breakCell);
                    inner.Add(ColoredDiv("AFTER", new DeviceRgb(209, 247, 29)));
                    outer.Add(inner);
                    outer.Add(ColoredDiv("OTHER CELL", new DeviceRgb(78, 151, 205)));
                    document.Add(outer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedGridInnerElementMarginsWithDocumentPageMarginsTest() {
            String fileName = "nestedGridInnerElemMarginsDocPageMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    document.SetPageMargins((pageNum) => pageNum % 2 == 0, new PageMarginBoxes(PageMarginsTestUtil.GetPageMargins1
                        ()));
                    GridContainer outer = CreateTwoColumnGrid();
                    outer.SetBackgroundColor(new DeviceRgb(220, 220, 220));
                    GridContainer innerLeft = CreateTwoColumnGrid();
                    innerLeft.SetMargins(20, 20, 20, 20).SetBackgroundColor(new DeviceRgb(200, 200, 255));
                    innerLeft.Add(ColoredDiv("L-1", new DeviceRgb(65, 151, 29)));
                    innerLeft.Add(ColoredDiv("L-2", new DeviceRgb(209, 247, 29)));
                    innerLeft.Add(ColoredDiv("L-3", new DeviceRgb(78, 151, 205)));
                    innerLeft.Add(ColoredDiv("L-4", new DeviceRgb(255, 165, 0)));
                    GridContainer innerRight = CreateTwoColumnGrid();
                    innerRight.Add(new Div().Add(new Paragraph("R-1 LARGE MARGIN")).SetBackgroundColor(new DeviceRgb(65, 151, 
                        29)).SetMargins(15, 10, 15, 10));
                    innerRight.Add(new Div().Add(new Paragraph("R-2 NO MARGIN")).SetBackgroundColor(new DeviceRgb(209, 247, 29
                        )).SetMargin(0));
                    innerRight.Add(new Div().Add(new Paragraph("R-3 LARGE PADDING")).SetBackgroundColor(new DeviceRgb(78, 151, 
                        205)).SetPaddings(15, 15, 15, 15));
                    innerRight.Add(new Div().Add(new Paragraph("R-4 DEFAULT")).SetBackgroundColor(new DeviceRgb(255, 165, 0)));
                    outer.Add(innerLeft);
                    outer.Add(innerRight);
                    document.Add(outer);
                    document.Add(new Paragraph(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 4)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        // TODO DEVSIX-10004: Update test after fix.
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [LogMessage(LayoutLogMessageConstant.SECTION_BREAK_UNEXPECTED, Count = 5)]
        [LogMessage(LayoutLogMessageConstant.AREA_BREAK_UNEXPECTED, Count = 21)]
        public virtual void GridWithTableHeaderAndFooterWithAreaBreakAndSectionBreakTest() {
            String fileName = "gridWithTableHeaderAndFooter";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    GridContainer gridContainer = CreateTwoColumnGrid();
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
                    gridContainer.Add(table);
                    gridContainer.Add(ColoredDiv("Second column div", new DeviceRgb(65, 151, 29)));
                    document.Add(gridContainer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        private static GridContainer CreateThreeColumnGrid() {
            GridContainer grid = new GridContainer();
            IList<TemplateValue> columns = new List<TemplateValue>();
            columns.Add(new FlexValue(1));
            columns.Add(new FlexValue(1));
            columns.Add(new FlexValue(1));
            grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, columns);
            grid.SetProperty(Property.GRID_FLOW, GridFlow.ROW);
            return grid;
        }

        private static GridContainer CreateTwoColumnGrid() {
            GridContainer grid = new GridContainer();
            IList<TemplateValue> columns = new List<TemplateValue>();
            columns.Add(new FlexValue(1));
            columns.Add(new FlexValue(1));
            grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, columns);
            grid.SetProperty(Property.GRID_FLOW, GridFlow.ROW);
            return grid;
        }

        private static GridContainer BuildSmallGrid(String prefix, int colorOffset) {
            GridContainer grid = CreateThreeColumnGrid();
            for (int i = 1; i <= 3; i++) {
                grid.Add(ColoredDiv(prefix + "-" + i, CellColor(i + colorOffset)));
            }
            return grid;
        }

        private static DeviceRgb CellColor(int index) {
            DeviceRgb[] palette = new DeviceRgb[] { new DeviceRgb(65, 151, 29), new DeviceRgb(209, 247, 29), new DeviceRgb
                (78, 151, 205), new DeviceRgb(255, 165, 0), new DeviceRgb(200, 100, 100), new DeviceRgb(100, 200, 100)
                 };
            return palette[(index - 1) % palette.Length];
        }

        private static Div ColoredDiv(String label, DeviceRgb color) {
            return new Div().Add(new Paragraph(label)).SetBackgroundColor(color).SetMargin(4).SetPadding(6);
        }
    }
}
