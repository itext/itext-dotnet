/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Text;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class KeepTogetherDivAndTableTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/KeepTogetherDivAndTableTest/";

        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/KeepTogetherDivAndTableTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 4)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RENDERER_WAS_NOT_ABLE_TO_PROCESS_KEEP_WITH_NEXT)]
        public virtual void CombineKeepTogetherDivWithTableTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_combineKeepTogetherDivWithTable.pdf";
            String destPdf = DESTINATION_FOLDER + "combineKeepTogetherDivWithTable.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destPdf))) {
                Document doc = new Document(pdfDoc);
                for (int i = 0; i < 10; i++) {
                    doc.Add(new Paragraph("").SetFontSize(10).SetMarginBottom(0).SetBorder(new SolidBorder(ColorConstants.PINK
                        , 1f)));
                }
                Div div = new Div().SetKeepTogether(true).SetKeepWithNext(true).SetBorder(new SolidBorder(ColorConstants.BLUE
                    , 1f));
                div.Add(new Paragraph("Moved title").SetFontSize(12).SetFontColor(ColorConstants.BLUE).SetMarginTop(10).SetMarginBottom
                    (0));
                doc.Add(div);
                doc.Add(CreateTableWithData(CreateBigCellTest(55)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CombineKeepTogetherDivWithTablePossibleLayout2pageTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_combineKeepTogetherDivWithTablePossibleLayout2page.pdf";
            String destPdf = DESTINATION_FOLDER + "combineKeepTogetherDivWithTablePossibleLayout2page.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destPdf))) {
                Document doc = new Document(pdfDoc);
                for (int i = 0; i < 10; i++) {
                    doc.Add(new Paragraph("").SetFontSize(10).SetMarginBottom(0).SetBorder(new SolidBorder(ColorConstants.PINK
                        , 1f)));
                }
                Div div = new Div().SetKeepTogether(true).SetKeepWithNext(true).SetBorder(new SolidBorder(ColorConstants.BLUE
                    , 1f));
                div.Add(new Paragraph("Moved title").SetFontSize(12).SetFontColor(ColorConstants.BLUE).SetMarginTop(10).SetMarginBottom
                    (0));
                doc.Add(div);
                doc.Add(CreateTableWithData(CreateBigCellTest(45)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RENDERER_WAS_NOT_ABLE_TO_PROCESS_KEEP_WITH_NEXT)]
        public virtual void CombineKeepTogetherDivWithTextTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_combineKeepTogetherDivWithText.pdf";
            String destPdf = DESTINATION_FOLDER + "combineKeepTogetherDivWithText.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destPdf))) {
                Document doc = new Document(pdfDoc);
                for (int i = 0; i < 1; i++) {
                    doc.Add(new Paragraph("").SetFontSize(10).SetMarginBottom(0).SetBorder(new SolidBorder(ColorConstants.PINK
                        , 1f)));
                }
                Div div = new Div().SetKeepTogether(true).SetKeepWithNext(true).SetBorder(new SolidBorder(ColorConstants.BLUE
                    , 1f));
                StringBuilder divText = new StringBuilder();
                for (int i = 0; i < 369; i++) {
                    divText.Append("some text ");
                }
                div.Add(new Paragraph(divText.ToString()).SetFontSize(12).SetFontColor(ColorConstants.BLUE).SetMarginTop(10
                    ).SetMarginBottom(0));
                doc.Add(div);
                doc.Add(new Paragraph().Add(new iText.Layout.Element.Text("BIG TEXT").SetFontSize(72)).Add(divText.ToString
                    ()));
                doc.Flush();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RENDERER_WAS_NOT_ABLE_TO_PROCESS_KEEP_WITH_NEXT)]
        public virtual void CombineKeepTogetherDivWithTableWithImageTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_combineKeepTogetherDivWithTableWithImage.pdf";
            String destPdf = DESTINATION_FOLDER + "combineKeepTogetherDivWithTableWithImage.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destPdf))) {
                Document doc = new Document(pdfDoc);
                for (int i = 0; i < 1; i++) {
                    doc.Add(new Paragraph("").SetFontSize(10).SetMarginBottom(0).SetBorder(new SolidBorder(ColorConstants.PINK
                        , 1f)));
                }
                Div div = new Div().SetKeepTogether(true).SetKeepWithNext(true).SetBorder(new SolidBorder(ColorConstants.BLUE
                    , 1f));
                StringBuilder divText = new StringBuilder();
                for (int i = 0; i < 350; i++) {
                    divText.Append("some text ");
                }
                div.Add(new Paragraph(divText.ToString()).SetFontSize(12).SetFontColor(ColorConstants.BLUE).SetMarginTop(10
                    ).SetMarginBottom(0));
                doc.Add(div);
                doc.Add(CreateTableWithImages());
                doc.Flush();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ParagraphTableSameDivKeepNextTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_paragraphTableSameDivKeepNext.pdf";
            String destPdf = DESTINATION_FOLDER + "paragraphTableSameDivKeepNext.pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(destPdf))) {
                Document document = new Document(pdf, pdf.GetDefaultPageSize(), false);
                Div div = new Div().SetKeepTogether(true).SetKeepWithNext(true);
                div.Add(CreateTableWithData(CreateBigCellTest(40)));
                document.Add(div);
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ParagraphTableWithAnotherParagraphKeepNextTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_paragraphTableWithAnotherParagraphKeepNext.pdf";
            String destPdf = DESTINATION_FOLDER + "paragraphTableWithAnotherParagraphKeepNext.pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(destPdf))) {
                Document document = new Document(pdf, pdf.GetDefaultPageSize(), false);
                Div div = new Div().SetKeepTogether(true).SetKeepWithNext(true);
                div.Add(CreateTableWithData(CreateBigCellTest(40)));
                document.Add(div);
                document.Add(new Paragraph("some text here"));
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RENDERER_WAS_NOT_ABLE_TO_PROCESS_KEEP_WITH_NEXT)]
        public virtual void CombineKeepTogether2TablesTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_combineKeepTogether2Tables.pdf";
            String destPdf = DESTINATION_FOLDER + "combineKeepTogether2Tables.pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(destPdf))) {
                Document document = new Document(pdf, pdf.GetDefaultPageSize(), false);
                Div div1 = new Div().SetKeepTogether(true).SetKeepWithNext(true);
                div1.Add(CreateTableWithData(CreateBigCellTest(20)));
                document.Add(div1);
                Div div2 = new Div().SetKeepTogether(true).SetKeepWithNext(true);
                div2.Add(CreateTableWithData(CreateBigCellTest(20)));
                document.Add(div2);
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RENDERER_WAS_NOT_ABLE_TO_PROCESS_KEEP_WITH_NEXT)]
        public virtual void CombineKeepTogether2LargeTablesTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_combineKeepTogether2LargeTables.pdf";
            String destPdf = DESTINATION_FOLDER + "combineKeepTogether2LargeTables.pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(destPdf))) {
                Document document = new Document(pdf, pdf.GetDefaultPageSize(), false);
                Div div1 = new Div().SetKeepTogether(true).SetKeepWithNext(true);
                div1.Add(CreateTableWithData(CreateBigCellTest(40)));
                document.Add(div1);
                Div div2 = new Div().SetKeepTogether(true).SetKeepWithNext(true);
                div2.Add(CreateTableWithData(CreateBigCellTest(40)));
                document.Add(div2);
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 5)]
        public virtual void DivWithLargeTableTest() {
            String cmpFile = SOURCE_FOLDER + "cmp_divWithLargeTable.pdf";
            String destPdf = DESTINATION_FOLDER + "divWithLargeTable.pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(destPdf))) {
                Document document = new Document(pdf, pdf.GetDefaultPageSize(), false);
                document.immediateFlush = false;
                Div div2 = new Div().SetKeepTogether(true).SetKeepWithNext(true);
                div2.Add(CreateTableWithData(CreateBigCellTest(150)));
                document.Add(div2);
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpFile, DESTINATION_FOLDER, "diff"
                ));
        }

        private String CreateBigCellTest(int numRepeats) {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < numRepeats; ++i) {
                buf.Append("Test ").Append(i).Append(" ");
            }
            String bigText = buf.ToString();
            return bigText;
        }

        private static Table CreateTableWithData(String mainCellText) {
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 5, 25, 25 }));
            table.SetWidth(UnitValue.CreatePercentValue(100)).SetMarginTop(0).SetMarginBottom(5);
            table.SetBorder(Border.NO_BORDER);
            table.SetVerticalAlignment(VerticalAlignment.BOTTOM).SetPadding(0);
            table.SetFixedLayout();
            String[] headerTitles = new String[] { "Title1", "Title2", "Title3" };
            foreach (String headerTitle in headerTitles) {
                table.AddHeaderCell(CreateCell(headerTitle, 8, Border.NO_BORDER));
            }
            Border grayBorder = new SolidBorder(ColorConstants.LIGHT_GRAY, 0.75f);
            for (int i = 0; i < 2; i++) {
                table.AddCell(CreateCell(mainCellText, 10, grayBorder));
                table.AddCell(CreateCell("Col2_" + i, 10, grayBorder));
                table.AddCell(CreateCell("Col3_" + i, 10, grayBorder));
            }
            return table;
        }

        private static Table CreateTableWithImages() {
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 25, 5, 5 }));
            table.SetWidth(UnitValue.CreatePercentValue(100)).SetMarginTop(0).SetMarginBottom(5);
            table.SetBorder(Border.NO_BORDER);
            table.SetVerticalAlignment(VerticalAlignment.BOTTOM).SetPadding(0);
            table.SetFixedLayout();
            String[] headerTitles = new String[] { "Title1", "Title2", "Title3" };
            foreach (String headerTitle in headerTitles) {
                table.AddHeaderCell(CreateCell(headerTitle, 8, Border.NO_BORDER));
            }
            Border grayBorder = new SolidBorder(ColorConstants.LIGHT_GRAY, 0.75f);
            for (int i = 0; i < 2; i++) {
                Cell cell = new Cell().Add(new Image(ImageDataFactory.Create(iText.Test.TestUtil.GetParentProjectDirectory
                    (NUnit.Framework.TestContext.CurrentContext.TestDirectory) + "/resources/itext/layout/ImageTest/itis.jpg"
                    )));
                cell.SetBorder(grayBorder);
                table.AddCell(cell);
                table.AddCell(CreateCell("Col2_" + i, 10, grayBorder));
                table.AddCell(CreateCell("Col3_" + i, 10, grayBorder));
            }
            return table;
        }

        private static Cell CreateCell(String cellText, float fontSize, Border border) {
            Paragraph p = new Paragraph(cellText);
            p.SetFontSize(fontSize);
            p.SetPaddingBottom(0);
            p.SetKeepTogether(true);
            Cell c = new Cell();
            c.SetBorder(border);
            c.SetPaddingLeft(0);
            c.SetPaddingBottom(0);
            c.SetKeepTogether(true);
            c.Add(p);
            return c;
        }
    }
}
