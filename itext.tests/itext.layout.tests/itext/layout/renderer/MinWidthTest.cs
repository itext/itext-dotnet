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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class MinWidthTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/MinWidthTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/MinWidthTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphTest() {
            String outFileName = destinationFolder + "paragraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_paragraphTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str).SetBorder(new SolidBorder(ColorConstants.BLACK, 5))).SetBorder(new 
                SolidBorder(ColorConstants.BLUE, 5));
            MinMaxWidth result = ((AbstractRenderer)p.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            p.SetWidth(ToEffectiveWidth(p, result.GetMinWidth()));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivTest() {
            String outFileName = destinationFolder + "divTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(ColorConstants.BLACK, 
                2)).SetMargin(3).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(ColorConstants.GREEN, 5)).SetMargin(6);
            d.Add(p);
            MinMaxWidth result = ((AbstractRenderer)d.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            d.SetWidth(ToEffectiveWidth(d, result.GetMinWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivWithSmallRotatedParagraph() {
            String outFileName = destinationFolder + "divSmallRotatedParagraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divSmallRotatedParagraphTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(ColorConstants.BLACK, 
                2)).SetMargin(3).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(ColorConstants.GREEN, 5)).SetMargin(6);
            d.Add(new Paragraph(("iText")).SetRotationAngle(Math.PI / 8).SetBorder(new SolidBorder(ColorConstants.BLUE
                , 2f)));
            d.Add(p);
            MinMaxWidth result = ((AbstractRenderer)d.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            d.SetWidth(ToEffectiveWidth(d, result.GetMinWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void DivWithBigRotatedParagraph() {
            String outFileName = destinationFolder + "divBigRotatedParagraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divBigRotatedParagraphTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(ColorConstants.BLACK, 
                2)).SetMargin(3).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetRotationAngle(Math.PI / 8);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(ColorConstants.GREEN, 5)).SetMargin(6);
            d.Add(p);
            d.Add(new Paragraph(("iText")));
            MinMaxWidth result = ((AbstractRenderer)d.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            d.SetWidth(ToEffectiveWidth(d, result.GetMinWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivWithSmallRotatedDiv() {
            String outFileName = destinationFolder + "divSmallRotatedDivTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divSmallRotatedDivTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(ColorConstants.BLACK, 
                2)).SetMargin(3).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(ColorConstants.GREEN, 5)).SetMargin(6);
            d.Add(p);
            Div dRotated = new Div().SetRotationAngle(Math.PI / 8).SetBorder(new SolidBorder(ColorConstants.BLUE, 2f));
            d.Add(dRotated.Add(new Paragraph(("iText"))));
            MinMaxWidth result = ((AbstractRenderer)d.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            d.SetWidth(ToEffectiveWidth(d, result.GetMinWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivWithBigRotatedDiv() {
            String outFileName = destinationFolder + "divBigRotatedDivTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divBigRotatedDivTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(ColorConstants.BLACK, 
                2)).SetMargin(3).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Div dRotated = new Div().SetPadding(4f).SetBorder(new SolidBorder(ColorConstants.GREEN, 5)).SetMargin(6);
            dRotated.Add(p).SetRotationAngle(Math.PI * 3 / 8);
            Div d = new Div().Add(new Paragraph(("iText"))).Add(dRotated).SetBorder(new SolidBorder(ColorConstants.BLUE
                , 2f));
            MinMaxWidth result = ((AbstractRenderer)d.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            d.SetWidth(ToEffectiveWidth(d, result.GetMinWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivWithPercentImage() {
            String outFileName = destinationFolder + "divPercentImage.pdf";
            String cmpFileName = sourceFolder + "cmp_divPercentImage.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(imageXObject);
            Div d = new Div().Add(img).SetBorder(new SolidBorder(ColorConstants.BLUE, 2f)).SetMarginBottom(10);
            iText.Layout.Element.Image imgPercent = new iText.Layout.Element.Image(imageXObject).SetWidth(UnitValue.CreatePercentValue
                (50));
            Div dPercent = new Div().Add(imgPercent).SetBorder(new SolidBorder(ColorConstants.BLUE, 2f));
            MinMaxWidth result = ((AbstractRenderer)d.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            d.SetWidth(ToEffectiveWidth(d, result.GetMinWidth()));
            MinMaxWidth resultPercent = ((AbstractRenderer)dPercent.CreateRendererSubTree().SetParent(doc.GetRenderer(
                ))).GetMinMaxWidth();
            dPercent.SetWidth(ToEffectiveWidth(dPercent, resultPercent.GetMaxWidth()));
            doc.Add(d);
            doc.Add(dPercent);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivWithRotatedPercentImage() {
            String outFileName = destinationFolder + "divRotatedPercentImage.pdf";
            String cmpFileName = sourceFolder + "cmp_divRotatedPercentImage.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(imageXObject).SetRotationAngle(Math.PI * 3
                 / 8);
            Div d = new Div().Add(img).SetBorder(new SolidBorder(ColorConstants.BLUE, 2f)).SetMarginBottom(10);
            iText.Layout.Element.Image imgPercent = new iText.Layout.Element.Image(imageXObject).SetWidth(UnitValue.CreatePercentValue
                (50)).SetRotationAngle(Math.PI * 3 / 8);
            Div dPercent = new Div().Add(imgPercent).SetBorder(new SolidBorder(ColorConstants.BLUE, 2f));
            MinMaxWidth result = ((AbstractRenderer)d.CreateRendererSubTree().SetParent(doc.GetRenderer())).GetMinMaxWidth
                ();
            d.SetWidth(ToEffectiveWidth(d, result.GetMinWidth()));
            MinMaxWidth resultPercent = ((AbstractRenderer)dPercent.CreateRendererSubTree().SetParent(doc.GetRenderer(
                ))).GetMinMaxWidth();
            dPercent.SetWidth(ToEffectiveWidth(dPercent, resultPercent.GetMaxWidth()));
            doc.Add(d);
            doc.Add(dPercent);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultipleDivTest() {
            String outFileName = destinationFolder + "multipleDivTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleDivTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Border[] borders = new Border[] { new SolidBorder(ColorConstants.BLUE, 2f), new SolidBorder(ColorConstants
                .RED, 2f), new SolidBorder(ColorConstants.GREEN, 2f) };
            Div externalDiv = new Div().SetPadding(2f).SetBorder(borders[2]);
            Div curr = externalDiv;
            for (int i = 0; i < 100; ++i) {
                Div d = new Div().SetBorder(borders[i % 3]);
                curr.Add(d);
                curr = d;
            }
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(ColorConstants.BLACK, 
                2)).SetMargin(3).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            curr.Add(p);
            MinMaxWidth result = ((AbstractRenderer)externalDiv.CreateRendererSubTree().SetParent(doc.GetRenderer())).
                GetMinMaxWidth();
            externalDiv.SetWidth(ToEffectiveWidth(externalDiv, result.GetMinWidth()));
            doc.Add(externalDiv);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void SimpleTableTest() {
            String outFileName = destinationFolder + "simpleTableTest.pdf";
            String cmpFileName = sourceFolder + "cmp_simpleTableTest.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Cell cell1 = new Cell().Add(new Paragraph("I am table")).SetBorder(new SolidBorder(ColorConstants.RED, 60)
                ).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Cell cell2 = new Cell().Add(new Paragraph("I am table")).SetBorder(new SolidBorder(ColorConstants.YELLOW, 
                10)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().SetBorder(new SolidBorder(
                ColorConstants.BLUE, 20)).AddCell(cell1.Clone(true)).AddCell(cell2.Clone(true)).AddCell(cell1.Clone(true
                )).AddCell(cell2.Clone(true));
            Table minTable = new Table(new float[] { -1, -1 }).SetWidth(UnitValue.CreatePointValue(1)).SetMarginTop(10
                ).SetBorder(new SolidBorder(ColorConstants.BLUE, 20)).AddCell(cell1.Clone(true)).AddCell(cell2.Clone(true
                )).AddCell(cell1.Clone(true)).AddCell(cell2.Clone(true));
            Table maxTable = new Table(new float[] { -1, -1 }).SetMarginTop(10).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 20)).AddCell(cell1.Clone(true)).AddCell(cell2.Clone(true)).AddCell(cell1.Clone(true)).AddCell(cell2
                .Clone(true));
            doc.Add(table);
            doc.Add(minTable);
            doc.Add(maxTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void ColspanTableTest() {
            String outFileName = destinationFolder + "colspanTableTest.pdf";
            String cmpFileName = sourceFolder + "cmp_colspanTableTest.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Cell bigCell = new Cell(1, 2).Add(new Paragraph("I am veryveryvery big cell")).SetBorder(new SolidBorder(ColorConstants
                .RED, 60)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Cell cell = new Cell().Add(new Paragraph("I am cell")).SetBorder(new SolidBorder(ColorConstants.YELLOW, 10
                )).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetBorder(new SolidBorder(
                ColorConstants.BLUE, 20)).AddCell(cell.Clone(true)).AddCell(bigCell.Clone(true)).AddCell(cell.Clone(true
                )).AddCell(cell.Clone(true)).AddCell(cell.Clone(true));
            Table minTable = new Table(new float[] { -1, -1, -1 }).SetWidth(UnitValue.CreatePointValue(1)).SetMarginTop
                (10).SetBorder(new SolidBorder(ColorConstants.BLUE, 20)).AddCell(cell.Clone(true)).AddCell(bigCell.Clone
                (true)).AddCell(cell.Clone(true)).AddCell(cell.Clone(true)).AddCell(cell.Clone(true));
            Table maxTable = new Table(new float[] { -1, -1, -1 }).SetMarginTop(10).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 20)).AddCell(cell.Clone(true)).AddCell(bigCell.Clone(true)).AddCell(cell.Clone(true)).AddCell(cell
                .Clone(true)).AddCell(cell.Clone(true));
            doc.Add(table);
            doc.Add(minTable);
            doc.Add(maxTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void ColspanRowspanTableTest() {
            String outFileName = destinationFolder + "colspanRowspanTableTest.pdf";
            String cmpFileName = sourceFolder + "cmp_colspanRowspanTableTest.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Cell colspanCell = new Cell(1, 2).Add(new Paragraph("I am veryveryvery big cell")).SetBorder(new SolidBorder
                (ColorConstants.RED, 60)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(
                0);
            Cell rowspanCell = new Cell(2, 1).Add(new Paragraph("I am very very very long cell")).SetBorder(new SolidBorder
                (ColorConstants.GREEN, 60)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding
                (0);
            Cell cell = new Cell().Add(new Paragraph("I am cell")).SetBorder(new SolidBorder(ColorConstants.BLUE, 10))
                .SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetBorder(new SolidBorder(
                ColorConstants.BLACK, 20)).AddCell(cell.Clone(true)).AddCell(cell.Clone(true)).AddCell(rowspanCell.Clone
                (true)).AddCell(colspanCell.Clone(true));
            Table minTable = new Table(new float[] { -1, -1, -1 }).SetWidth(UnitValue.CreatePointValue(1)).SetMarginTop
                (10).SetBorder(new SolidBorder(ColorConstants.BLACK, 20)).AddCell(cell.Clone(true)).AddCell(cell.Clone
                (true)).AddCell(rowspanCell.Clone(true)).AddCell(colspanCell.Clone(true));
            Table maxTable = new Table(new float[] { -1, -1, -1 }).SetMarginTop(10).SetBorder(new SolidBorder(ColorConstants
                .BLACK, 20)).AddCell(cell.Clone(true)).AddCell(cell.Clone(true)).AddCell(rowspanCell.Clone(true)).AddCell
                (colspanCell.Clone(true));
            doc.Add(table);
            doc.Add(minTable);
            doc.Add(maxTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void HeaderFooterTableTest() {
            String outFileName = destinationFolder + "headerFooterTableTest.pdf";
            String cmpFileName = sourceFolder + "cmp_headerFooterTableTest.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Cell bigCell = new Cell().Add(new Paragraph("veryveryveryvery big cell")).SetBorder(new SolidBorder(ColorConstants
                .RED, 40)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Cell mediumCell = new Cell().Add(new Paragraph("mediumsize cell")).SetBorder(new SolidBorder(ColorConstants
                .GREEN, 30)).SetBorderBottom(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Cell cell = new Cell().Add(new Paragraph("cell")).SetBorder(new SolidBorder(ColorConstants.BLUE, 10)).SetBorderBottom
                (Border.NO_BORDER).SetBorderTop(Border.NO_BORDER).SetPadding(0);
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetBorder(new SolidBorder(
                ColorConstants.BLACK, 20)).AddCell(mediumCell.Clone(true)).AddCell(mediumCell.Clone(true)).AddCell(mediumCell
                .Clone(true)).AddFooterCell(cell.Clone(true)).AddFooterCell(cell.Clone(true)).AddFooterCell(bigCell.Clone
                (true)).AddHeaderCell(bigCell.Clone(true)).AddHeaderCell(cell.Clone(true)).AddHeaderCell(cell.Clone(true
                ));
            TableRenderer renderer = (TableRenderer)table.CreateRendererSubTree().SetParent(doc.GetRenderer());
            MinMaxWidth minMaxWidth = renderer.GetMinMaxWidth();
            Table minTable = new Table(new float[] { -1, -1, -1 }).SetWidth(UnitValue.CreatePointValue(1)).SetBorder(new 
                SolidBorder(ColorConstants.BLACK, 20)).SetMarginTop(20).AddCell(mediumCell.Clone(true)).AddCell(mediumCell
                .Clone(true)).AddCell(mediumCell.Clone(true)).AddFooterCell(cell.Clone(true)).AddFooterCell(cell.Clone
                (true)).AddFooterCell(bigCell.Clone(true)).AddHeaderCell(bigCell.Clone(true)).AddHeaderCell(cell.Clone
                (true)).AddHeaderCell(cell.Clone(true));
            Table maxTable = new Table(new float[] { -1, -1, -1 }).SetBorder(new SolidBorder(ColorConstants.BLACK, 20)
                ).SetMarginTop(20).AddCell(mediumCell.Clone(true)).AddCell(mediumCell.Clone(true)).AddCell(mediumCell.
                Clone(true)).AddFooterCell(cell.Clone(true)).AddFooterCell(cell.Clone(true)).AddFooterCell(bigCell.Clone
                (true)).AddHeaderCell(bigCell.Clone(true)).AddHeaderCell(cell.Clone(true)).AddHeaderCell(cell.Clone(true
                ));
            doc.Add(table);
            doc.Add(minTable);
            doc.Add(maxTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private static float ToEffectiveWidth(IBlockElement b, float fullWidth) {
            if (b is Table) {
                return fullWidth + ((Table)b).GetNumberOfColumns() * MinMaxWidthUtils.GetEps();
            }
            else {
                return fullWidth - MinMaxWidthUtils.GetBorderWidth(b) - MinMaxWidthUtils.GetMarginsWidth(b) - MinMaxWidthUtils
                    .GetPaddingWidth(b) + MinMaxWidthUtils.GetEps();
            }
        }

        private static float[] ToEffectiveTableColumnWidth(float[] tableColumnWidth) {
            float[] result = new float[tableColumnWidth.Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = tableColumnWidth[i] + MinMaxWidthUtils.GetEps();
            }
            return result;
        }
    }
}
