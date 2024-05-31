/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Exceptions;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GridContainerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/GridContainerTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/GridContainerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicThreeColumnsTest() {
            String filename = DESTINATION_FOLDER + "basicThreeColumnsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicThreeColumnsTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(150.0f));
            templateColumns.Add(GridValue.CreatePointValue(150.0f));
            templateColumns.Add(GridValue.CreatePointValue(150.0f));
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                for (int i = 0; i < 5; ++i) {
                    grid.Add(new Paragraph("Test" + i).SetBorder(border));
                }
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BasicAutoColumnsTest() {
            String filename = DESTINATION_FOLDER + "basicAutoColumnsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicAutoColumnsTest.pdf";
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_AUTO_COLUMNS, GridValue.CreatePointValue(150.0f));
                for (int i = 0; i < 5; ++i) {
                    grid.Add(new Paragraph("Test" + i).SetBorder(border));
                }
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BasicAutoRowsTest() {
            String filename = DESTINATION_FOLDER + "basicAutoRowsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicAutoRowsTest.pdf";
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_AUTO_ROWS, GridValue.CreatePointValue(70.0f));
                grid.Add(new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                     + "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " + "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. "
                     + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim " +
                     "id est laborum.").SetBorder(border));
                grid.Add(new Paragraph("test").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BasicThreeColumnsWithCustomColumnIndexesTest() {
            String filename = DESTINATION_FOLDER + "basicThreeColumnsWithCustomColumnIndexesTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicThreeColumnsWithCustomColumnIndexesTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One").SetBorder(border);
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 3);
                grid.Add(paragraph1);
                grid.Add(new Paragraph("Two").SetBorder(border));
                Paragraph paragraph3 = new Paragraph("Three").SetBorder(border);
                paragraph3.SetProperty(Property.GRID_COLUMN_START, 2);
                paragraph3.SetProperty(Property.GRID_COLUMN_END, 4);
                grid.Add(paragraph3);
                grid.Add(new Paragraph("Four").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ThreeColumnsWithAdjacentWideCellsTest() {
            String filename = DESTINATION_FOLDER + "threeColumnsWithAdjacentWideCellsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_threeColumnsWithAdjacentWideCellsTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph1.SetBorder(border);
                grid.Add(paragraph1);
                Paragraph paragraph2 = new Paragraph("Two");
                paragraph2.SetProperty(Property.GRID_COLUMN_START, 3);
                paragraph2.SetProperty(Property.GRID_COLUMN_END, 5);
                paragraph2.SetBorder(border);
                grid.Add(paragraph2);
                Paragraph paragraph3 = new Paragraph("Three");
                paragraph3.SetProperty(Property.GRID_COLUMN_START, 2);
                paragraph3.SetProperty(Property.GRID_COLUMN_END, 4);
                paragraph3.SetBorder(border);
                grid.Add(paragraph3);
                grid.Add(new Paragraph("Four").SetBorder(border));
                grid.Add(new Paragraph("Five").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BasicThreeColumnsWithCustomRowIndexesTest() {
            String filename = DESTINATION_FOLDER + "basicThreeColumnsWithCustomRowIndexesTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicThreeColumnsWithCustomRowIndexesTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One").SetBorder(border);
                paragraph1.SetProperty(Property.GRID_ROW_START, 1);
                paragraph1.SetProperty(Property.GRID_ROW_END, 3);
                grid.Add(paragraph1);
                grid.Add(new Paragraph("Two").SetBorder(border));
                Paragraph paragraph3 = new Paragraph("Three").SetBorder(border);
                paragraph3.SetProperty(Property.GRID_ROW_START, 3);
                paragraph3.SetProperty(Property.GRID_ROW_END, 4);
                grid.Add(paragraph3);
                grid.Add(new Paragraph("Four").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BasicThreeColumnsWithCustomColumnAndRowIndexesTest() {
            String filename = DESTINATION_FOLDER + "basicThreeColumnsWithCustomColumnAndRowIndexesTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicThreeColumnsWithCustomColumnAndRowIndexesTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph1.SetProperty(Property.GRID_ROW_START, 1);
                paragraph1.SetProperty(Property.GRID_ROW_END, 3);
                paragraph1.SetBorder(border);
                grid.Add(paragraph1);
                grid.Add(new Paragraph("Two").SetBorder(border));
                grid.Add(new Paragraph("Three").SetBorder(border));
                grid.Add(new Paragraph("Four").SetBorder(border));
                grid.Add(new Paragraph("Five").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BasicThreeColumnsWithReversedIndexesTest() {
            String filename = DESTINATION_FOLDER + "basicThreeColumnsWithReversedIndexesTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicThreeColumnsWithReversedIndexesTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 3);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 1);
                paragraph1.SetProperty(Property.GRID_ROW_START, 3);
                paragraph1.SetProperty(Property.GRID_ROW_END, 1);
                paragraph1.SetBorder(border);
                grid.Add(paragraph1);
                Paragraph paragraph2 = new Paragraph("Two");
                paragraph2.SetProperty(Property.GRID_ROW_START, 3);
                paragraph2.SetProperty(Property.GRID_ROW_END, 1);
                paragraph2.SetBorder(border);
                grid.Add(paragraph2);
                Paragraph paragraph3 = new Paragraph("Three");
                paragraph3.SetProperty(Property.GRID_COLUMN_START, 3);
                paragraph3.SetProperty(Property.GRID_COLUMN_END, 1);
                paragraph3.SetBorder(border);
                grid.Add(paragraph3);
                grid.Add(new Paragraph("Four").SetBorder(border));
                grid.Add(new Paragraph("Five").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BasicThreeColumnsWithoutColumnEndTest() {
            String filename = DESTINATION_FOLDER + "basicThreeColumnsWithoutColumnEndTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicThreeColumnsWithoutColumnEndTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_ROW_END, 2);
                paragraph1.SetBorder(border);
                grid.Add(paragraph1);
                Paragraph paragraph2 = new Paragraph("Two");
                paragraph2.SetProperty(Property.GRID_ROW_START, 2);
                paragraph2.SetBorder(border);
                grid.Add(paragraph2);
                Paragraph paragraph3 = new Paragraph("Three");
                paragraph3.SetProperty(Property.GRID_COLUMN_START, 3);
                paragraph3.SetBorder(border);
                grid.Add(paragraph3);
                Paragraph paragraph4 = new Paragraph("Four");
                paragraph4.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph4.SetBorder(border);
                grid.Add(paragraph4);
                grid.Add(new Paragraph("Five").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FixedColumnRowGoesFirstTest() {
            String filename = DESTINATION_FOLDER + "fixedColumnRowGoesFirstTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_fixedColumnRowGoesFirstTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_ROW_END, 3);
                paragraph1.SetBorder(border);
                grid.Add(paragraph1);
                Paragraph paragraph2 = new Paragraph("Two\nTwo");
                paragraph2.SetProperty(Property.GRID_ROW_START, 1);
                paragraph2.SetProperty(Property.GRID_ROW_END, 3);
                paragraph2.SetBorder(border);
                grid.Add(paragraph2);
                grid.Add(new Paragraph("Three").SetBorder(border));
                grid.Add(new Paragraph("Four").SetBorder(border));
                grid.Add(new Paragraph("Five").SetBorder(border));
                Paragraph paragraph6 = new Paragraph("Six");
                paragraph6.SetProperty(Property.GRID_COLUMN_START, 3);
                paragraph6.SetBorder(border);
                grid.Add(paragraph6);
                Paragraph paragraph7 = new Paragraph("Seven");
                paragraph7.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph7.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph7.SetBorder(border);
                grid.Add(paragraph7);
                Paragraph paragraph8 = new Paragraph("Eight\nEight");
                paragraph8.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph8.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph8.SetProperty(Property.GRID_ROW_START, 4);
                paragraph8.SetProperty(Property.GRID_ROW_END, 6);
                paragraph8.SetBorder(border);
                grid.Add(paragraph8);
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void OverlapWithExistingColumnTest() {
            String filename = DESTINATION_FOLDER + "overlapWithExistingColumnTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("Two");
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 2);
                paragraph1.SetProperty(Property.GRID_ROW_START, 1);
                paragraph1.SetProperty(Property.GRID_ROW_END, 2);
                paragraph1.SetBorder(border);
                grid.Add(paragraph1);
                grid.Add(new Paragraph("Three").SetBorder(border));
                grid.Add(new Paragraph("Four").SetBorder(border));
                grid.Add(new Paragraph("Five").SetBorder(border));
                Paragraph paragraph2 = new Paragraph("One");
                paragraph2.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph2.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph2.SetProperty(Property.GRID_ROW_START, 1);
                paragraph2.SetProperty(Property.GRID_ROW_END, 3);
                paragraph2.SetBorder(border);
                grid.Add(paragraph2);
                Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => document.Add(grid));
                NUnit.Framework.Assert.AreEqual(LayoutExceptionMessageConstant.INVALID_CELL_INDEXES, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BasicThreeColumnsWithPtAndPercentTest() {
            String filename = DESTINATION_FOLDER + "basicThreeColumnsWithPtAndPercentTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_basicThreeColumnsWithPtAndPercentTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePercentValue(15.0f));
            templateColumns.Add(GridValue.CreatePercentValue(50.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                for (int i = 0; i < 5; ++i) {
                    grid.Add(new Paragraph("Test" + i).SetBorder(border));
                }
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ThirdColumnNotLayoutedTest() {
            String filename = DESTINATION_FOLDER + "thirdColumnNotLayoutedTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_thirdColumnNotLayoutedTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(200.0f));
            templateColumns.Add(GridValue.CreatePointValue(200.0f));
            templateColumns.Add(GridValue.CreatePointValue(200.0f));
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                for (int i = 0; i < 5; ++i) {
                    grid.Add(new Paragraph("Test" + i).SetBorder(border));
                }
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ThreeColumnsWithSquareAndVerticalCellsTest() {
            String filename = DESTINATION_FOLDER + "threeColumnsWithSquareAndVerticalCellsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_threeColumnsWithSquareAndVerticalCellsTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph1.SetProperty(Property.GRID_ROW_START, 1);
                paragraph1.SetProperty(Property.GRID_ROW_END, 3);
                paragraph1.SetBorder(border);
                grid.Add(paragraph1);
                grid.Add(new Paragraph("Two").SetBorder(border));
                Paragraph paragraph3 = new Paragraph("Three");
                paragraph3.SetProperty(Property.GRID_ROW_START, 2);
                paragraph3.SetProperty(Property.GRID_ROW_END, 4);
                paragraph3.SetBorder(border);
                grid.Add(paragraph3);
                grid.Add(new Paragraph("Four").SetBorder(border));
                grid.Add(new Paragraph("Five").SetBorder(border));
                grid.Add(new Paragraph("Six").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ThreeColumnsWithSquareCellAndCellWithExplicitHeightTest() {
            String filename = DESTINATION_FOLDER + "threeColumnsWithSquareCellAndCellWithExplicitHeightTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_threeColumnsWithSquareCellAndCellWithExplicitHeightTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetBackgroundColor(ColorConstants.GREEN);
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph1.SetProperty(Property.GRID_ROW_START, 1);
                paragraph1.SetProperty(Property.GRID_ROW_END, 3);
                paragraph1.SetBorder(border).SetBackgroundColor(ColorConstants.RED);
                grid.Add(paragraph1);
                grid.Add(new Paragraph("Two").SetBorder(border).SetBackgroundColor(ColorConstants.RED));
                grid.Add(new Paragraph("Three").SetBorder(border).SetBackgroundColor(ColorConstants.RED).SetHeight(100.0f)
                    );
                grid.Add(new Paragraph("Four").SetBorder(border).SetBackgroundColor(ColorConstants.RED));
                grid.Add(new Paragraph("Five").SetBorder(border).SetBackgroundColor(ColorConstants.RED));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ThreeColumnsWithVerticalCellWithSeveralNeighboursToTheLeftTest() {
            String filename = DESTINATION_FOLDER + "threeColumnsWithVerticalCellWithSeveralNeighboursToTheLeftTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_threeColumnsWithVerticalCellWithSeveralNeighboursToTheLeftTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            templateColumns.Add(GridValue.CreatePointValue(100.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                Paragraph paragraph1 = new Paragraph("One");
                paragraph1.SetProperty(Property.GRID_COLUMN_START, 1);
                paragraph1.SetProperty(Property.GRID_COLUMN_END, 3);
                paragraph1.SetProperty(Property.GRID_ROW_START, 1);
                paragraph1.SetProperty(Property.GRID_ROW_END, 3);
                paragraph1.SetBorder(border).SetBackgroundColor(ColorConstants.RED);
                grid.Add(paragraph1);
                grid.Add(new Paragraph("Two").SetBorder(border).SetBackgroundColor(ColorConstants.RED));
                Paragraph paragraph3 = new Paragraph("Three");
                paragraph3.SetProperty(Property.GRID_ROW_START, 2);
                paragraph3.SetProperty(Property.GRID_ROW_END, 4);
                paragraph3.SetBorder(border).SetBackgroundColor(ColorConstants.RED);
                grid.Add(paragraph3);
                grid.Add(new Paragraph("Four").SetBorder(border).SetBackgroundColor(ColorConstants.RED));
                grid.Add(new Paragraph("Five").SetBorder(border).SetBackgroundColor(ColorConstants.RED));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BigCellMinContentTest() {
            String filename = DESTINATION_FOLDER + "bigCellMinContentTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_bigCellMinContentTest.pdf";
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreateMinContentValue());
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                grid.Add(new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore"
                    ).SetBorder(border));
                grid.Add(new Paragraph("test").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ColumnRowGapTest() {
            String filename = DESTINATION_FOLDER + "columnRowGapTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_columnRowGapTest.pdf";
            IList<GridValue> template = new List<GridValue>();
            template.Add(GridValue.CreatePointValue(50.0f));
            template.Add(GridValue.CreatePointValue(50.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, template);
                grid.SetProperty(Property.GRID_TEMPLATE_ROWS, template);
                grid.SetProperty(Property.GRID_AUTO_ROWS, GridValue.CreatePointValue(70.0f));
                grid.SetProperty(Property.COLUMN_GAP, 20.0f);
                grid.SetProperty(Property.ROW_GAP, 20.0f);
                grid.Add(new Paragraph("One").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Two").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Tree").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Four").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Five").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Six").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Seven").SetBackgroundColor(ColorConstants.CYAN));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FewBigCellsWithGapTest() {
            String filename = DESTINATION_FOLDER + "fewBigCellsWithGapTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_fewBigCellsWithGapTest.pdf";
            IList<GridValue> template = new List<GridValue>();
            template.Add(GridValue.CreatePointValue(50.0f));
            template.Add(GridValue.CreatePointValue(50.0f));
            template.Add(GridValue.CreatePointValue(50.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, template);
                grid.SetProperty(Property.GRID_TEMPLATE_ROWS, template);
                grid.SetProperty(Property.COLUMN_GAP, 10.0f);
                grid.SetProperty(Property.ROW_GAP, 10.0f);
                Paragraph one = new Paragraph("One").SetBackgroundColor(ColorConstants.CYAN);
                one.SetProperty(Property.GRID_COLUMN_START, 1);
                one.SetProperty(Property.GRID_COLUMN_END, 3);
                one.SetProperty(Property.GRID_ROW_START, 1);
                one.SetProperty(Property.GRID_ROW_END, 3);
                grid.Add(one);
                grid.Add(new Paragraph("Two").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Tree").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Four").SetBackgroundColor(ColorConstants.CYAN));
                Paragraph five = new Paragraph("Five").SetBackgroundColor(ColorConstants.CYAN);
                five.SetProperty(Property.GRID_COLUMN_START, 1);
                five.SetProperty(Property.GRID_COLUMN_END, 4);
                five.SetProperty(Property.GRID_ROW_START, 3);
                five.SetProperty(Property.GRID_ROW_END, 5);
                grid.Add(five);
                grid.Add(new Paragraph("Six").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Seven").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Eight").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Nine").SetBackgroundColor(ColorConstants.CYAN));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ColumnFlowWithBigCellsTest() {
            String filename = DESTINATION_FOLDER + "columnFlowWithBigCellsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_columnFlowWithBigCellsTest.pdf";
            IList<GridValue> template = new List<GridValue>();
            template.Add(GridValue.CreatePointValue(50.0f));
            template.Add(GridValue.CreatePointValue(50.0f));
            template.Add(GridValue.CreatePointValue(50.0f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_FLOW, GridFlow.COLUMN);
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, template);
                grid.SetProperty(Property.GRID_TEMPLATE_ROWS, template);
                grid.SetProperty(Property.COLUMN_GAP, 10.0f);
                grid.SetProperty(Property.ROW_GAP, 10.0f);
                Paragraph one = new Paragraph("One").SetBackgroundColor(ColorConstants.CYAN);
                one.SetProperty(Property.GRID_COLUMN_START, 1);
                one.SetProperty(Property.GRID_COLUMN_END, 3);
                one.SetProperty(Property.GRID_ROW_START, 1);
                one.SetProperty(Property.GRID_ROW_END, 3);
                grid.Add(one);
                grid.Add(new Paragraph("Two").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Tree").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Four").SetBackgroundColor(ColorConstants.CYAN));
                Paragraph five = new Paragraph("Five").SetBackgroundColor(ColorConstants.CYAN);
                five.SetProperty(Property.GRID_COLUMN_START, 1);
                five.SetProperty(Property.GRID_COLUMN_END, 4);
                grid.Add(five);
                grid.Add(new Paragraph("Six").SetBackgroundColor(ColorConstants.CYAN));
                Paragraph seven = new Paragraph("Seven").SetBackgroundColor(ColorConstants.CYAN);
                seven.SetProperty(Property.GRID_ROW_START, 1);
                seven.SetProperty(Property.GRID_ROW_END, 4);
                grid.Add(seven);
                grid.Add(new Paragraph("Eight").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Nine").SetBackgroundColor(ColorConstants.CYAN));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FrInRowsTest() {
            String filename = DESTINATION_FOLDER + "frInRowsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_frInRowsTest.pdf";
            IList<GridValue> template = new List<GridValue>();
            template.Add(GridValue.CreateFlexValue(1f));
            template.Add(GridValue.CreateFlexValue(3f));
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_ROWS, template);
                grid.SetProperty(Property.ROW_GAP, 20.0f);
                grid.Add(new Paragraph("One").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Two").SetBackgroundColor(ColorConstants.CYAN));
                grid.Add(new Paragraph("Tree").SetBackgroundColor(ColorConstants.CYAN));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FrColumnsTest() {
            String filename = DESTINATION_FOLDER + "frColumnsTest.pdf";
            String cmpName = SOURCE_FOLDER + "cmp_frColumnsTest.pdf";
            IList<GridValue> templateColumns = new List<GridValue>();
            templateColumns.Add(GridValue.CreateFlexValue(1f));
            templateColumns.Add(GridValue.CreateAutoValue());
            templateColumns.Add(GridValue.CreateFlexValue(3f));
            SolidBorder border = new SolidBorder(ColorConstants.BLUE, 1);
            using (Document document = new Document(new PdfDocument(new PdfWriter(filename)))) {
                GridContainer grid = new GridContainer();
                grid.SetProperty(Property.GRID_TEMPLATE_COLUMNS, templateColumns);
                grid.Add(new Paragraph("Test1").SetBorder(border));
                grid.Add(new Paragraph("Test2").SetBorder(border));
                grid.Add(new Paragraph("Test3").SetBorder(border));
                Paragraph test4 = new Paragraph("Test4Test4Test4Test4Test4 Test4 Test4 Test4 Test4 Test4 Test4 Test4").SetBorder
                    (border);
                test4.SetProperty(Property.GRID_COLUMN_START, 1);
                test4.SetProperty(Property.GRID_COLUMN_END, 3);
                grid.Add(test4);
                grid.Add(new Paragraph("Test5").SetBorder(border));
                document.Add(grid);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
