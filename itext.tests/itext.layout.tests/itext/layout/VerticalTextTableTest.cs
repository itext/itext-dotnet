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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextTableTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextTableTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextTableTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<bool> IsRightToLeftProgression() {
            return JavaUtil.ArraysAsList(false, true);
        }

        [NUnit.Framework.TestCaseSource("IsRightToLeftProgression")]
        public virtual void VerticalTextInTableVerticalAlignmentTest(bool rightToLeftProgression) {
            String fileName = "verticalTextInTableVerticalAlignment" + (rightToLeftProgression ? "_rtl" : "_ltr");
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Table table = new Table(UnitValue.CreatePointArray(new float[] { 150, 150, 150 }));
                    table.AddCell(CreateCell("top\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .LEFT, false, rightToLeftProgression));
                    table.AddCell(CreateCell("middle\nseveral\nlines", VerticalAlignment.MIDDLE, new DeviceRgb(222, 243, 222), 
                        TextAlignment.LEFT, false, rightToLeftProgression));
                    table.AddCell(CreateCell("bottom\nseveral\nlines", VerticalAlignment.BOTTOM, new DeviceRgb(222, 243, 222), 
                        TextAlignment.LEFT, false, rightToLeftProgression));
                    table.AddCell(CreateCell("top\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .LEFT, true, rightToLeftProgression));
                    table.AddCell(CreateCell("middle\nseveral\nlines", VerticalAlignment.MIDDLE, new DeviceRgb(222, 243, 222), 
                        TextAlignment.LEFT, true, rightToLeftProgression));
                    table.AddCell(CreateCell("bottom\nseveral\nlines", VerticalAlignment.BOTTOM, new DeviceRgb(222, 243, 222), 
                        TextAlignment.LEFT, true, rightToLeftProgression));
                    table.AddCell(CreateCell("top\nseveral\nlines\ntop\nseveral\nlines\ntop\nseveral\nlines\ntop", VerticalAlignment
                        .TOP, new DeviceRgb(255, 249, 220), TextAlignment.LEFT, true, rightToLeftProgression));
                    table.AddCell(CreateCell("middle\nseveral\nlines\nmiddle\nseveral\nlines\nmiddle\nseveral\nlines\nmiddle", 
                        VerticalAlignment.MIDDLE, new DeviceRgb(222, 243, 222), TextAlignment.LEFT, true, rightToLeftProgression
                        ));
                    table.AddCell(CreateCell("bottom\nseveral\nlines\nbottom\nseveral\nlines\nbottom\nseveral\nlines\nbottom", 
                        VerticalAlignment.BOTTOM, new DeviceRgb(254, 243, 255), TextAlignment.LEFT, true, rightToLeftProgression
                        ));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextInTableTextAlignmentTest() {
            String fileName = "verticalTextInTableTextAlignment";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Table table = new Table(UnitValue.CreatePointArray(new float[] { 150, 150, 150 }));
                    table.AddCell(CreateCell("top\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .LEFT, false, false));
                    table.AddCell(CreateCell("middle\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .CENTER, false, false));
                    table.AddCell(CreateCell("bottom\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .RIGHT, false, false));
                    table.AddCell(CreateCell("top\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .LEFT, true, false));
                    table.AddCell(CreateCell("middle\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .CENTER, true, false));
                    table.AddCell(CreateCell("bottom\nseveral\nlines", VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment
                        .RIGHT, true, false));
                    table.AddCell(CreateCell("top\nseveral\nlines\ntop\nseveral\nlines\ntop\nseveral\nlines\ntop", VerticalAlignment
                        .TOP, new DeviceRgb(255, 249, 220), TextAlignment.LEFT, true, false));
                    table.AddCell(CreateCell("middle\nseveral\nlines\nmiddle\nseveral\nlines\nmiddle\nseveral\nlines\nmiddle", 
                        VerticalAlignment.TOP, new DeviceRgb(222, 243, 222), TextAlignment.CENTER, true, false));
                    table.AddCell(CreateCell("bottom\nseveral\nlines\nbottom\nseveral\nlines\nbottom\nseveral\nlines\nbottom", 
                        VerticalAlignment.TOP, new DeviceRgb(254, 243, 255), TextAlignment.RIGHT, true, false));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextTableCellAlignWithHeightTest() {
            String fileName = "verticalTextTableCellAlignWithHeight";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Table table = new Table(UnitValue.CreatePointArray(new float[] { 120, 120, 120, 120 }));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Left", TextAlignment.LEFT, VerticalAlignment.TOP, null, 50
                        , new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Center", TextAlignment.CENTER, VerticalAlignment.MIDDLE, 
                        null, 50, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Right", TextAlignment.RIGHT, VerticalAlignment.BOTTOM, null
                        , 50, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalTextCell("Middle vertical", TextAlignment.CENTER, VerticalAlignment.MIDDLE
                        ));
                    table.AddCell(CreateHorizontalCell("Row horizontal value A"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value B"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value C"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value D"));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Right", TextAlignment.RIGHT, VerticalAlignment.TOP, 200f, 
                        null, new DeviceRgb(255, 222, 239)).SetBackgroundColor(new DeviceRgb(253, 253, 244)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Center", TextAlignment.CENTER, VerticalAlignment.MIDDLE, 
                        null, 400, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Right", TextAlignment.RIGHT, VerticalAlignment.BOTTOM, null
                        , 200, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalTextCell("Middle vertical", TextAlignment.CENTER, VerticalAlignment.MIDDLE
                        ));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextTableCellHorizontalAlignTest() {
            String fileName = "verticalTextTableCellHorizontalAlign";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Table table = new Table(UnitValue.CreatePointArray(new float[] { 105, 105, 105, 105, 105 }));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Left\nseveral\nlines", TextAlignment.LEFT, VerticalAlignment
                        .MIDDLE, null, null, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Center\nseveral\nlines", TextAlignment.CENTER, VerticalAlignment
                        .MIDDLE, null, null, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Right\nseveral\nlines", TextAlignment.RIGHT, VerticalAlignment
                        .MIDDLE, null, null, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalTextCell("Middle vertical", TextAlignment.CENTER, VerticalAlignment.MIDDLE
                        ));
                    table.AddHeaderCell(CreateHorizontalCell("Horizontal").SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment
                        (VerticalAlignment.MIDDLE));
                    table.AddCell(CreateHorizontalCell("Row horizontal value A"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value B"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value C"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value D"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value E"));
                    table.AddFooterCell(CreateVerticalTextCell("Left", TextAlignment.LEFT, VerticalAlignment.MIDDLE));
                    table.AddFooterCell(CreateVerticalTextCell("Center", TextAlignment.CENTER, VerticalAlignment.MIDDLE));
                    table.AddFooterCell(CreateVerticalTextCell("Right", TextAlignment.RIGHT, VerticalAlignment.MIDDLE));
                    table.AddFooterCell(CreateVerticalTextCell("Middle vertical", TextAlignment.LEFT, VerticalAlignment.MIDDLE
                        ));
                    table.AddFooterCell(CreateHorizontalCell("Horizontal").SetTextAlignment(TextAlignment.LEFT).SetVerticalAlignment
                        (VerticalAlignment.MIDDLE));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextTableCellVerticalAlignTest() {
            String fileName = "verticalTextTableCellVerticalAlign";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Table table = new Table(UnitValue.CreatePointArray(new float[] { 105, 105, 105, 105, 105 }));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Top", TextAlignment.CENTER, VerticalAlignment.TOP, null, 
                        null, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Middle", TextAlignment.CENTER, VerticalAlignment.MIDDLE, 
                        null, null, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalParagraphCell("Bottom", TextAlignment.CENTER, VerticalAlignment.BOTTOM, 
                        null, null, new DeviceRgb(255, 222, 239)));
                    table.AddHeaderCell(CreateVerticalTextCell("Middle vertical", TextAlignment.CENTER, VerticalAlignment.MIDDLE
                        ));
                    table.AddHeaderCell(CreateHorizontalCell("Horizontal").SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment
                        (VerticalAlignment.MIDDLE));
                    table.AddCell(CreateHorizontalCell("Row horizontal value A"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value B"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value C"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value D"));
                    table.AddCell(CreateHorizontalCell("Row horizontal value E"));
                    table.AddFooterCell(CreateVerticalTextCell("Top", TextAlignment.LEFT, VerticalAlignment.TOP));
                    table.AddFooterCell(CreateVerticalTextCell("Middle", TextAlignment.LEFT, VerticalAlignment.MIDDLE));
                    table.AddFooterCell(CreateVerticalTextCell("Bottom", TextAlignment.LEFT, VerticalAlignment.BOTTOM));
                    table.AddFooterCell(CreateVerticalTextCell("Middle vertical", TextAlignment.LEFT, VerticalAlignment.MIDDLE
                        ));
                    table.AddFooterCell(CreateHorizontalCell("Horizontal").SetTextAlignment(TextAlignment.LEFT).SetVerticalAlignment
                        (VerticalAlignment.MIDDLE));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        private static Cell CreateCell(String text, VerticalAlignment? verticalAlignment, DeviceRgb borderColor, TextAlignment?
             textAlignment, bool verticalCell, bool rightToLeftProgression) {
            Cell cell = new Cell().SetHeight(200).SetVerticalAlignment(verticalAlignment).SetBorder(new SolidBorder(borderColor
                , 3)).SetBackgroundColor(new DeviceRgb(255, 252, 254)).SetPadding(0);
            if (verticalCell) {
                cell.SetProperty(Property.WRITING_MODE, rightToLeftProgression ? WritingMode.VERTICAL_RL : WritingMode.VERTICAL_LR
                    );
                cell.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            }
            VerticalParagraph paragraph = new VerticalParagraph(text, rightToLeftProgression).SetMargin(0).SetBackgroundColor
                (new DeviceRgb(222, 243, 222)).SetBorder(new SolidBorder(new DeviceRgb(153, 153, 153), 1)).SetTextAlignment
                (textAlignment);
            cell.Add(paragraph);
            return cell;
        }

        private static Cell CreateHorizontalCell(String text) {
            return new Cell().Add(new Paragraph(text).SetMargin(0));
        }

        private static Cell CreateVerticalTextCell(String text, TextAlignment? textAlignment, VerticalAlignment? verticalAlignment
            ) {
            Cell cell = new Cell().SetVerticalAlignment(verticalAlignment).SetPadding(8).SetBorder(new SolidBorder(new 
                DeviceRgb(153, 153, 153), 1));
            cell.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
            cell.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            Paragraph paragraph = new Paragraph(text).SetMargin(0).SetTextAlignment(textAlignment);
            cell.Add(paragraph);
            return cell;
        }

        private static Cell CreateVerticalParagraphCell(String text, TextAlignment? textAlignment, VerticalAlignment?
             verticalAlignment, float? cellHeight, int? paragraphHeight, DeviceRgb paragraphBackground) {
            Cell cell = new Cell().SetVerticalAlignment(verticalAlignment).SetPadding(8).SetBorder(new SolidBorder(new 
                DeviceRgb(153, 153, 153), 1));
            cell.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
            cell.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            if (cellHeight != null) {
                cell.SetHeight((float)cellHeight);
            }
            VerticalParagraph paragraph = new VerticalParagraph(text, false).SetMargin(0).SetTextAlignment(textAlignment
                );
            if (paragraphHeight != null) {
                paragraph.SetHeight((float)paragraphHeight);
            }
            if (paragraphBackground != null) {
                paragraph.SetBackgroundColor(paragraphBackground);
            }
            cell.Add(paragraph);
            return cell;
        }
    }
}
