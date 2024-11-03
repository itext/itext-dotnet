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
using System.Text;
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
    //TODO DEVSIX-8689: Change tests after fix.
    [NUnit.Framework.Category("IntegrationTest")]
    public class KeepTogetherDivAndTableTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/KeepTogetherDivAndTableTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/KeepTogetherDivAndTableTest/";

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

        private String CreateBigCellTest(int numRepeats) {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < numRepeats; ++i) {
                buf.Append("Testing. ");
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
