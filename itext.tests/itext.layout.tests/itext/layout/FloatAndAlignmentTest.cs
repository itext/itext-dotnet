/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FloatAndAlignmentTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FloatAndAlignmentTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FloatAndAlignmentTest/";

        private static String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor "
             + "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco "
             + "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit "
             + "esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa "
             + "qui officia deserunt mollit anim id est laborum.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BlocksInsideDiv() {
            /* this test shows different combinations of 3 float values blocks  within divParent containers
            */
            String testName = "blocksInsideDiv";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div div1 = CreateDiv(ColorConstants.RED, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE, UnitValue.CreatePercentValue(80));
            Div div2 = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE, UnitValue.CreatePercentValue(80));
            Div div3 = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE, UnitValue.CreatePercentValue(80));
            Div divParent1 = CreateParentDiv(HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (80));
            divParent1.Add(div3);
            divParent1.Add(div2);
            divParent1.Add(div1);
            document.Add(divParent1);
            Div divParent2 = CreateParentDiv(HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (80));
            divParent2.Add(div2);
            divParent2.Add(div1);
            divParent2.Add(div3);
            document.Add(divParent2);
            Div divParent3 = CreateParentDiv(HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (80));
            divParent3.Add(div1);
            divParent3.Add(div2);
            divParent3.Add(div3);
            document.Add(divParent3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void BlocksInsideDivFloat() {
            /* this test shows different combinations of 3 float values blocks  within divParent containers
            */
            String testName = "blocksInsideDivFloat";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div div1 = CreateDiv(ColorConstants.RED, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE, UnitValue.CreatePercentValue(80));
            Div div2 = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, FloatPropertyValue
                .RIGHT, UnitValue.CreatePercentValue(80));
            Div div3 = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, FloatPropertyValue
                .LEFT, UnitValue.CreatePercentValue(80));
            Div divParent1 = CreateParentDiv(HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (75));
            divParent1.Add(div3);
            divParent1.Add(div2);
            divParent1.Add(div1);
            document.Add(divParent1);
            Div divParent2 = CreateParentDiv(HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (75));
            divParent2.Add(div2);
            divParent2.Add(div1);
            divParent2.Add(div3);
            document.Add(divParent2);
            Div divParent3 = CreateParentDiv(HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (75));
            divParent3.Add(div1);
            divParent3.Add(div2);
            divParent3.Add(div3);
            document.Add(divParent3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void BlocksInsideEachOther() {
            /* this test shows different combinations of float blocks  inside each other
            */
            String testName = "blocksInsideEachOther";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div divRed = CreateDiv(ColorConstants.RED, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE, UnitValue.CreatePercentValue(80));
            Div divBlue = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, FloatPropertyValue
                .RIGHT, UnitValue.CreatePercentValue(80));
            Div divGreen = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, FloatPropertyValue
                .LEFT, UnitValue.CreatePercentValue(80));
            Div divYellow = CreateDiv(ColorConstants.YELLOW, HorizontalAlignment.RIGHT, ClearPropertyValue.NONE, FloatPropertyValue
                .RIGHT, UnitValue.CreatePercentValue(80));
            Div divOrange = CreateDiv(ColorConstants.ORANGE, HorizontalAlignment.LEFT, ClearPropertyValue.NONE, FloatPropertyValue
                .LEFT, UnitValue.CreatePercentValue(80));
            Div divParent1 = CreateParentDiv(HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (85));
            divParent1.Add(divRed);
            divRed.Add(divBlue);
            divBlue.Add(divGreen);
            document.Add(divParent1);
            Div divParent2 = CreateParentDiv(HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (85));
            divParent2.Add(divYellow);
            divYellow.Add(divRed);
            document.Add(divParent2);
            Div divParent3 = CreateParentDiv(HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (85));
            divParent3.Add(divOrange);
            divOrange.Add(divYellow);
            document.Add(divParent3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff02_"));
        }

        [NUnit.Framework.Test]
        public virtual void BlocksInsideEachOther_sameFixedWidthsNesting() {
            /* this test shows different combinations of float blocks inside each other with blocks nested inside each other that have the same fixed width
            */
            String testName = "blocksInsideEachOther_sameFixedWidthsNesting";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div divRed = CreateDiv(ColorConstants.RED, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE, UnitValue.CreatePointValue(300));
            Div divBlue = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, FloatPropertyValue
                .RIGHT, UnitValue.CreatePointValue(300));
            Div divGreen = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, FloatPropertyValue
                .LEFT, UnitValue.CreatePointValue(300));
            Div divYellow = CreateDiv(ColorConstants.YELLOW, HorizontalAlignment.RIGHT, ClearPropertyValue.NONE, FloatPropertyValue
                .RIGHT, UnitValue.CreatePointValue(300));
            Div divOrange = CreateDiv(ColorConstants.ORANGE, HorizontalAlignment.LEFT, ClearPropertyValue.NONE, FloatPropertyValue
                .LEFT, UnitValue.CreatePointValue(300));
            Div divParent1 = CreateParentDiv(HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (70));
            divParent1.Add(divRed);
            divRed.Add(divBlue);
            divBlue.Add(divGreen);
            document.Add(divParent1);
            Div divParent2 = CreateParentDiv(HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (70));
            divParent2.Add(divYellow);
            divYellow.Add(divRed);
            document.Add(divParent2);
            Div divParent3 = CreateParentDiv(HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, UnitValue.CreatePercentValue
                (70));
            divParent3.Add(divOrange);
            divOrange.Add(divYellow);
            document.Add(divParent3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff02_sameFixedWidth_"));
        }

        [NUnit.Framework.Test]
        public virtual void BlocksNotInDivCenter() {
            /* this test shows different combinations of 3 float values blocks
            * NOTE, that div1 text is partly overlapped
            */
            String testName = "blocksNotInDivCenter";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            CreateDocumentWithBlocks(outFileName, HorizontalAlignment.CENTER);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff03_"));
        }

        [NUnit.Framework.Test]
        public virtual void BlocksNotInDivLeft() {
            /* this test shows different combinations of 3 float values blocks
            * NOTE, that div1 text is partly overlapped
            */
            String testName = "blocksNotInDivLeft";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            CreateDocumentWithBlocks(outFileName, HorizontalAlignment.LEFT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff04_"));
        }

        [NUnit.Framework.Test]
        public virtual void BlocksNotInDivRight() {
            /* this test shows different combinations of 3 float values blocks
            * NOTE, that div1 text is partly overlapped
            */
            String testName = "blocksNotInDivRight";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            /*
            * Please, NOTE: in current example HorizontalAlignment values are ignored, if FloatPropertyValue !=NONE
            * So, only FloatPropertyValue defines the position of element in such cases
            */
            CreateDocumentWithBlocks(outFileName, HorizontalAlignment.RIGHT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff05_"));
        }

        private void CreateDocumentWithBlocks(String outFileName, HorizontalAlignment? horizontalAlignment) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div divRed = CreateDiv(ColorConstants.RED, horizontalAlignment, ClearPropertyValue.NONE, FloatPropertyValue
                .NONE, UnitValue.CreatePointValue(300));
            Div divBlue = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.NONE, FloatPropertyValue
                .RIGHT, UnitValue.CreatePointValue(300));
            Div divGreen = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.NONE, FloatPropertyValue
                .LEFT, UnitValue.CreatePointValue(300));
            Div divYellow = CreateDiv(ColorConstants.YELLOW, HorizontalAlignment.RIGHT, ClearPropertyValue.NONE, FloatPropertyValue
                .RIGHT, UnitValue.CreatePointValue(300));
            Div divOrange = CreateDiv(ColorConstants.ORANGE, HorizontalAlignment.LEFT, ClearPropertyValue.NONE, FloatPropertyValue
                .LEFT, UnitValue.CreatePointValue(300));
            document.Add(divOrange);
            document.Add(divYellow);
            document.Add(divGreen);
            document.Add(divBlue);
            document.Add(divRed);
            document.Add(divOrange);
            document.Add(divYellow);
            document.Add(divGreen);
            document.Add(divBlue);
            document.Add(divRed);
            document.Add(divRed);
            document.Add(divRed);
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void InlineBlocksAndFloatsWithTextAlignmentTest01() {
            String testName = "inlineBlocksAndFloatsWithTextAlignmentTest01";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph parentPara = new Paragraph().SetTextAlignment(TextAlignment.RIGHT);
            Div floatingDiv = new Div();
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parentPara.Add("Text begin").Add(new Div().Add(new Paragraph("div text").SetBorder(new SolidBorder(2)))).Add
                ("More text").Add(floatingDiv.Add(new Paragraph("floating div text")).SetBorder(new SolidBorder(ColorConstants
                .GREEN, 2)));
            document.Add(parentPara);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diffTextAlign01_"));
        }

        [NUnit.Framework.Test]
        public virtual void InlineBlocksAndFloatsWithTextAlignmentTest02() {
            String testName = "inlineBlocksAndFloatsWithTextAlignmentTest02";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph parentPara = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            Div floatingDiv = new Div();
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parentPara.Add("Text begin").Add(new Div().Add(new Paragraph("div text").SetBorder(new SolidBorder(2)))).Add
                (floatingDiv.Add(new Paragraph("floating div text")).SetBorder(new SolidBorder(ColorConstants.GREEN, 2
                ))).Add("MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. "
                );
            document.Add(parentPara.SetBorder(new DashedBorder(2)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diffTextAlign02_"));
        }

        [NUnit.Framework.Test]
        public virtual void InlineBlocksAndFloatsWithTextAlignmentTest03() {
            String testName = "inlineBlocksAndFloatsWithTextAlignmentTest03";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            // making an inline float a last element in the line
            Paragraph parentPara = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            Div floatingDiv = new Div();
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parentPara.Add("Text begin").Add(new Div().Add(new Paragraph("div text").SetBorder(new SolidBorder(2)))).Add
                ("MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. "
                ).Add(floatingDiv.Add(new Paragraph("floating div text")).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 2))).Add("MoretextMoretextMoretext.");
            document.Add(parentPara.SetBorder(new DashedBorder(2)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diffTextAlign03_"));
        }

        [NUnit.Framework.Test]
        public virtual void InlineBlocksAndFloatsWithTextAlignmentTest04() {
            String testName = "inlineBlocksAndFloatsWithTextAlignmentTest04";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            try {
                using (PdfWriter writer = new PdfWriter(outFileName)) {
                    using (PdfDocument pdfDocument = new PdfDocument(writer)) {
                        pdfDocument.SetDefaultPageSize(PageSize.A5);
                        using (Document document = new Document(pdfDocument)) {
                            Table table2 = new Table(1).SetWidth(150f).SetBorder(new SolidBorder(1f)).SetMargin(5f).SetHorizontalAlignment
                                (HorizontalAlignment.LEFT).AddCell(new Cell().Add(new Paragraph(text.JSubstring(0, text.Length / 2))));
                            table2.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
                            document.Add(table2);
                            document.Add(new Paragraph(text).SetTextAlignment(TextAlignment.JUSTIFIED));
                            Table table3 = new Table(1).SetWidth(150f).SetBorder(new SolidBorder(1f)).SetMargin(5f).SetHorizontalAlignment
                                (HorizontalAlignment.RIGHT).AddCell(new Cell().Add(new Paragraph(text.JSubstring(0, text.Length / 2)))
                                );
                            table3.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
                            document.Add(table3);
                            document.Add(new Paragraph(text).SetTextAlignment(TextAlignment.JUSTIFIED));
                        }
                    }
                }
            }
            catch (Exception) {
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diffTextAlign04_"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatsOnlyJustificationTest01() {
            String testName = "floatsOnlyJustificationTest01";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph parentPara = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            Div floatingDiv = new Div();
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parentPara.Add(floatingDiv.Add(new Paragraph("floating div text")).SetBorder(new SolidBorder(ColorConstants
                .GREEN, 2)));
            document.Add(parentPara.SetBorder(new DashedBorder(2)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithAlignmentNextToRightFloatTest() {
            //TODO DEVSIX-4021 update cmp file after fix
            String outFileName = destinationFolder + "tableWithAlignmentNextToRightFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_tableWithAlignmentNextToRightFloat.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table1 = CreateTable(HorizontalAlignment.RIGHT);
            Table table2 = CreateTable(HorizontalAlignment.LEFT);
            Table table3 = CreateTable(HorizontalAlignment.CENTER);
            Div div = CreateDiv(ColorConstants.GREEN, ClearPropertyValue.NONE, FloatPropertyValue.RIGHT, UnitValue.CreatePointValue
                (200));
            Div spaceDiv = new Div();
            spaceDiv.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            spaceDiv.Add(new Paragraph("Space Div").SetFontColor(ColorConstants.BLUE));
            document.Add(div);
            document.Add(table1);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table2);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithAlignmentNextToLeftFloatTest() {
            //TODO DEVSIX-4021 update cmp file after fix
            String outFileName = destinationFolder + "tableWithAlignmentNextToLeftFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_tableWithAlignmentNextToLeftFloat.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table1 = CreateTable(HorizontalAlignment.RIGHT);
            Table table2 = CreateTable(HorizontalAlignment.LEFT);
            Table table3 = CreateTable(HorizontalAlignment.CENTER);
            Div div = CreateDiv(ColorConstants.GREEN, ClearPropertyValue.NONE, FloatPropertyValue.LEFT, UnitValue.CreatePointValue
                (200));
            Div spaceDiv = new Div();
            spaceDiv.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            spaceDiv.Add(new Paragraph("Space Div").SetFontColor(ColorConstants.BLUE));
            document.Add(div);
            document.Add(table1);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table2);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithAlignmentBetweenFloatsTest() {
            //TODO DEVSIX-4021 update cmp file after fix
            String outFileName = destinationFolder + "tableWithAlignmentBetweenFloats.pdf";
            String cmpFileName = sourceFolder + "cmp_tableWithAlignmentBetweenFloats.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table1 = CreateTable(HorizontalAlignment.RIGHT).SetWidth(250);
            Table table2 = CreateTable(HorizontalAlignment.LEFT).SetWidth(250);
            Table table3 = CreateTable(HorizontalAlignment.CENTER).SetWidth(250);
            Div div1 = CreateDiv(ColorConstants.GREEN, ClearPropertyValue.NONE, FloatPropertyValue.LEFT, UnitValue.CreatePointValue
                (100));
            Div div2 = CreateDiv(ColorConstants.BLUE, ClearPropertyValue.NONE, FloatPropertyValue.RIGHT, UnitValue.CreatePointValue
                (100));
            Div spaceDiv = new Div();
            spaceDiv.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            spaceDiv.Add(new Paragraph("Space Div").SetFontColor(ColorConstants.BLUE));
            document.Add(div1);
            document.Add(div2);
            document.Add(table1);
            document.Add(spaceDiv);
            document.Add(div1);
            document.Add(div2);
            document.Add(table2);
            document.Add(spaceDiv);
            document.Add(div1);
            document.Add(div2);
            document.Add(table3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithBigLeftMarginAfterFloatTest() {
            //TODO DEVSIX-4021 update cmp file after fix
            String outFileName = destinationFolder + "tableWithBigLeftMarginAfterFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_tableWithBigLeftMarginAfterFloat.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table1 = CreateTable(HorizontalAlignment.RIGHT);
            table1.SetMarginLeft(300);
            Table table2 = CreateTable(HorizontalAlignment.LEFT);
            table2.SetMarginLeft(300);
            Table table3 = CreateTable(HorizontalAlignment.CENTER);
            table3.SetMarginLeft(300);
            Div div = CreateDiv(ColorConstants.GREEN, ClearPropertyValue.NONE, FloatPropertyValue.RIGHT, UnitValue.CreatePointValue
                (200));
            Div spaceDiv = new Div();
            spaceDiv.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            spaceDiv.Add(new Paragraph("Space Div").SetFontColor(ColorConstants.BLUE));
            document.Add(div);
            document.Add(table1);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table2);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithBigRightMarginAfterFloatTest() {
            //TODO DEVSIX-4021 update cmp file after fix
            String outFileName = destinationFolder + "tableWithBigRightMarginAfterFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_tableWithBigRightMarginAfterFloat.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table1 = CreateTable(HorizontalAlignment.RIGHT);
            table1.SetMarginRight(300);
            Table table2 = CreateTable(HorizontalAlignment.LEFT);
            table2.SetMarginRight(300);
            Table table3 = CreateTable(HorizontalAlignment.CENTER);
            table3.SetMarginRight(300);
            Div div = CreateDiv(ColorConstants.GREEN, ClearPropertyValue.NONE, FloatPropertyValue.LEFT, UnitValue.CreatePointValue
                (200));
            Div spaceDiv = new Div();
            spaceDiv.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            spaceDiv.Add(new Paragraph("Space Div").SetFontColor(ColorConstants.BLUE));
            document.Add(div);
            document.Add(table1);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table2);
            document.Add(spaceDiv);
            document.Add(div);
            document.Add(table3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithSideMarginsBetweenFloatTest() {
            //TODO DEVSIX-4021 update cmp file after fix
            String outFileName = destinationFolder + "tableWithSideMarginsBetweenFloat.pdf";
            String cmpFileName = sourceFolder + "cmp_tableWithSideMarginsBetweenFloat.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table1 = CreateTable(HorizontalAlignment.RIGHT);
            table1.SetMarginRight(150).SetMarginLeft(150);
            Table table2 = CreateTable(HorizontalAlignment.LEFT);
            table2.SetMarginRight(300);
            Table table3 = CreateTable(HorizontalAlignment.CENTER);
            table3.SetMarginLeft(300);
            Div div1 = CreateDiv(ColorConstants.GREEN, ClearPropertyValue.NONE, FloatPropertyValue.LEFT, UnitValue.CreatePointValue
                (100));
            Div div2 = CreateDiv(ColorConstants.BLUE, ClearPropertyValue.NONE, FloatPropertyValue.RIGHT, UnitValue.CreatePointValue
                (100));
            Div spaceDiv = new Div();
            spaceDiv.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            spaceDiv.Add(new Paragraph("Space Div").SetFontColor(ColorConstants.BLUE));
            document.Add(div1);
            document.Add(div2);
            document.Add(table1);
            document.Add(spaceDiv);
            document.Add(div1);
            document.Add(div2);
            document.Add(table2);
            document.Add(spaceDiv);
            document.Add(div1);
            document.Add(div2);
            document.Add(table3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        private Div CreateParentDiv(HorizontalAlignment? horizontalAlignment, ClearPropertyValue? clearPropertyValue
            , UnitValue width) {
            Div divParent1 = new Div().SetBorder(new SolidBorder(5)).SetWidth(width);
            divParent1.SetHorizontalAlignment(horizontalAlignment);
            divParent1.SetProperty(Property.CLEAR, clearPropertyValue);
            divParent1.Add(new Paragraph("Div with HorizontalAlignment." + horizontalAlignment + ", ClearPropertyValue."
                 + clearPropertyValue));
            return divParent1;
        }

        private static Div CreateDiv(Color color, HorizontalAlignment? horizontalAlignment, ClearPropertyValue? clearPropertyValue
            , FloatPropertyValue? floatPropertyValue, UnitValue width) {
            Div div = new Div().SetBorder(new SolidBorder(color, 1)).SetBackgroundColor(color, 0.3f).SetMargins(10, 10
                , 10, 10).SetWidth(width);
            div.SetHorizontalAlignment(horizontalAlignment);
            div.SetProperty(Property.CLEAR, clearPropertyValue);
            div.SetProperty(Property.FLOAT, floatPropertyValue);
            String cont = "Div with HorizontalAlignment." + horizontalAlignment + ", ClearPropertyValue." + clearPropertyValue
                 + ", FloatPropertyValue." + floatPropertyValue;
            div.Add(new Paragraph(cont));
            return div;
        }

        private static Div CreateDiv(Color color, ClearPropertyValue? clearPropertyValue, FloatPropertyValue? floatPropertyValue
            , UnitValue width) {
            Div div = new Div().SetBorder(new SolidBorder(color, 1)).SetBackgroundColor(color, 0.3f).SetMargins(10, 10
                , 10, 10).SetWidth(width);
            div.SetProperty(Property.CLEAR, clearPropertyValue);
            div.SetProperty(Property.FLOAT, floatPropertyValue);
            String cont = "Div with ClearPropertyValue." + clearPropertyValue + ", FloatPropertyValue." + floatPropertyValue;
            div.Add(new Paragraph(cont));
            return div;
        }

        private static Table CreateTable(HorizontalAlignment? horizontalAlignment) {
            Table table = new Table(3);
            table.AddCell("Align" + horizontalAlignment.ToString());
            table.AddCell("Cell number two");
            table.AddCell("Cell number three");
            table.SetHorizontalAlignment(horizontalAlignment);
            return table;
        }
    }
}
