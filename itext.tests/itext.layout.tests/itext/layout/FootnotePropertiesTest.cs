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
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FootnotePropertiesTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FootnotePropertiesTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/FootnotePropertiesTest/";

        public static IEnumerable<Object[]> NumberingType() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { FootnoteNumberingType.DECIMAL }, new Object[]
                 { FootnoteNumberingType.ROMAN_LOWER }, new Object[] { FootnoteNumberingType.ROMAN_UPPER }, new Object
                [] { FootnoteNumberingType.ENGLISH_LOWER }, new Object[] { FootnoteNumberingType.ENGLISH_UPPER }, new 
                Object[] { FootnoteNumberingType.GREEK_LOWER }, new Object[] { FootnoteNumberingType.GREEK_UPPER } });
        }

        public static IEnumerable<Object[]> NumberingConfig() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { FootnoteNumberingConfig.PER_PAGE }, new Object
                [] { FootnoteNumberingConfig.PER_SECTION }, new Object[] { FootnoteNumberingConfig.PER_DOCUMENT } });
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.TestCaseSource("NumberingType")]
        public virtual void FootnoteNumberingTypeTest(FootnoteNumberingType? numberingType) {
            String fileName = "footnoteNumberingType_" + numberingType.ToString();
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnoteNumberingType(numberingType).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_DOCUMENT));
                    Footnote footnote = new Footnote("Footnote text");
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Footnote footnote2 = new Footnote("Footnote text 2");
                    FootnoteAnchor anchor2 = new FootnoteAnchor(footnote2);
                    Table table = new Table(4);
                    for (int i = 0; i < 24; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 5) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 19) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        table.AddCell(paragraph);
                    }
                    document.Add(table);
                    footnote = new Footnote("Footnote text 3");
                    anchor = new FootnoteAnchor(footnote);
                    footnote2 = new Footnote("Footnote text 5");
                    anchor2 = new FootnoteAnchor(footnote2);
                    table = new Table(4);
                    for (int i = 0; i < 24; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 1) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 5) {
                            paragraph.Add(new FootnoteAnchor(new Footnote("Footnote text 4")));
                        }
                        if (i == 23) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i < 4) {
                            table.AddHeaderCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)));
                        }
                        else {
                            if (i > 19) {
                                table.AddFooterCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.BLUE, 2)));
                            }
                            else {
                                table.AddCell(paragraph);
                            }
                        }
                    }
                    document.Add(new Paragraph(TestResourceUtil.GetByronStanza() + "\n\n" + TestResourceUtil.GetByronStanza() 
                        + "\n\n" + "Two more \nlines"));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteTableFooterNewPageTest() {
            String fileName = "footnoteTableFooterNewPage";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            FootnoteNumberingType? numberingType = FootnoteNumberingType.DECIMAL;
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnoteNumberingType(numberingType).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_DOCUMENT));
                    Footnote footnote = new Footnote("Footnote text");
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Footnote footnote2 = new Footnote("Footnote text 2");
                    FootnoteAnchor anchor2 = new FootnoteAnchor(footnote2);
                    Table table = new Table(4);
                    for (int i = 0; i < 24; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 5) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 19) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        table.AddCell(paragraph);
                    }
                    document.Add(table);
                    footnote = new Footnote("Footnote text 3");
                    anchor = new FootnoteAnchor(footnote);
                    footnote2 = new Footnote("Footnote text 5");
                    anchor2 = new FootnoteAnchor(footnote2);
                    table = new Table(4);
                    for (int i = 0; i < 24; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 1) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 15) {
                            paragraph.Add(new FootnoteAnchor(new Footnote("Footnote text 4\n\n")));
                        }
                        if (i == 23) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i < 4) {
                            table.AddHeaderCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)));
                        }
                        else {
                            if (i > 19) {
                                table.AddFooterCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.BLUE, 2)));
                            }
                            else {
                                table.AddCell(paragraph);
                            }
                        }
                    }
                    document.Add(new Paragraph(TestResourceUtil.GetByronStanza() + "\n\n" + TestResourceUtil.GetByronStanza() 
                        + "\n\n" + "Two more \nlines"));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteTableFooterNewPage3Test() {
            String fileName = "footnoteTableFooterNewPage3";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            FootnoteNumberingType? numberingType = FootnoteNumberingType.DECIMAL;
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnoteNumberingType(numberingType).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_DOCUMENT));
                    Footnote footnote = new Footnote("Footnote text");
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Footnote footnote2 = new Footnote("Footnote text 2");
                    FootnoteAnchor anchor2 = new FootnoteAnchor(footnote2);
                    Table table = new Table(4);
                    for (int i = 0; i < 24; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 5) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 19) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        table.AddCell(paragraph);
                    }
                    document.Add(table);
                    footnote = new Footnote("Footnote text 3");
                    anchor = new FootnoteAnchor(footnote);
                    footnote2 = new Footnote("Footnote text 5");
                    anchor2 = new FootnoteAnchor(footnote2);
                    table = new Table(4);
                    for (int i = 0; i < 24; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 1) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 15) {
                            paragraph.Add(new FootnoteAnchor(new Footnote("Footnote\ntext\n4"))).SetBorder(new SolidBorder(ColorConstants
                                .GREEN, 1));
                        }
                        if (i == 23) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        table.AddCell(paragraph);
                    }
                    document.Add(new Paragraph(TestResourceUtil.GetByronStanza() + "\n\n" + TestResourceUtil.GetByronStanza() 
                        + "\n\n" + "Two more \nlines"));
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteTableFooterNewPage2Test() {
            String fileName = "footnoteTableFooterNewPage2";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            FootnoteNumberingType? numberingType = FootnoteNumberingType.DECIMAL;
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnoteNumberingType(numberingType).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_DOCUMENT));
                    document.Add(new Div().SetHeight(580).SetWidth(500).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                    Footnote footnote = new Footnote("Footnote text 1");
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Footnote footnote2 = new Footnote("Footnote text 3");
                    FootnoteAnchor anchor2 = new FootnoteAnchor(footnote2);
                    Table table = new Table(4);
                    for (int i = 0; i < 24; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 1) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 15) {
                            paragraph.Add(new FootnoteAnchor(new Footnote("Footnote\ntext\n2"))).SetBorder(new SolidBorder(ColorConstants
                                .GREEN, 1));
                        }
                        if (i == 23) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i < 4) {
                            table.AddHeaderCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)));
                        }
                        else {
                            if (i > 19) {
                                table.AddFooterCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.BLUE, 2)));
                            }
                            else {
                                table.AddCell(paragraph);
                            }
                        }
                    }
                    document.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.TestCaseSource("NumberingConfig")]
        public virtual void FootnoteNumberingConfigTest(FootnoteNumberingConfig numberingConfig) {
            String fileName = "footnoteNumberingConfig_" + numberingConfig.ToString();
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnoteNumberingType(FootnoteNumberingType.DECIMAL
                        ).SetFootnoteNumberingConfig(numberingConfig));
                    for (int i = 1; i < 50; ++i) {
                        Paragraph paragraph = new Paragraph("Paragraph " + i);
                        Footnote footnote = new Footnote("Footnote text " + i);
                        FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                        paragraph.Add(anchor);
                        document.Add(paragraph);
                        if (i % 15 == 0) {
                            document.Add(new Paragraph("SECTION BREAK").SetBackgroundColor(ColorConstants.GREEN));
                            document.Add(new SectionBreak());
                            document.Add(new Paragraph("NEW SECTION").SetBackgroundColor(ColorConstants.GREEN));
                        }
                        if (i % 10 == 0) {
                            document.Add(new Paragraph("PAGE BREAK").SetBackgroundColor(ColorConstants.CYAN));
                            document.Add(new AreaBreak());
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteCustomStyleTest() {
            String fileName = "footnoteCustomStyle";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Style footnoteAnchorLabelStyle = new Style().SetMarginLeft(10).SetMarginRight(10).SetBackgroundColor(ColorConstants
                        .YELLOW);
                    footnoteAnchorLabelStyle.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(12));
                    footnoteAnchorLabelStyle.SetProperty(Property.TEXT_RISE, 0);
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetBackgroundColor
                        (ColorConstants.LIGHT_GRAY).SetBorder(new DashedBorder(ColorConstants.GREEN, 3))).SetFootnoteAnchorLabelStyle
                        (footnoteAnchorLabelStyle).SetFootnoteNumberingType(FootnoteNumberingType.DECIMAL).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_DOCUMENT));
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(new FootnoteAnchor("dummy", 
                        new Footnote("One more"))).Add(new FootnoteAnchor(new Footnote("Two more"))).Add("\n" + TestResourceUtil
                        .GetByronStanza());
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FootnoteCustomStyleSectionBreakTest() {
            String fileName = "footnoteCustomStyleSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    Style footnoteAnchorStyle = new Style().SetMarginLeft(10).SetMarginRight(10).SetBackgroundColor(ColorConstants
                        .YELLOW);
                    footnoteAnchorStyle.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(12));
                    footnoteAnchorStyle.SetProperty(Property.TEXT_RISE, 0);
                    Div div = new Div().Add(new SectionBreak().SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle
                        (new Style().SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new DashedBorder(ColorConstants.GREEN
                        , 3))).SetFootnoteAnchorLabelStyle(footnoteAnchorStyle).SetFootnoteNumberingType(FootnoteNumberingType
                        .DECIMAL).SetFootnoteNumberingConfig(FootnoteNumberingConfig.PER_SECTION))).Add(p).SetBorder(new SolidBorder
                        (ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CustomStyleTest() {
            String fileName = "customStyle";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Style footnoteAnchorStyle = new Style().SetMarginLeft(10).SetMarginRight(10).SetBackgroundColor(ColorConstants
                        .YELLOW);
                    footnoteAnchorStyle.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(12));
                    footnoteAnchorStyle.SetProperty(Property.TEXT_RISE, 0);
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetBackgroundColor
                        (ColorConstants.LIGHT_GRAY).SetBorder(new DashedBorder(ColorConstants.GREEN, 3))).SetFootnoteAnchorStyle
                        (footnoteAnchorStyle).SetFootnoteNumberingType(FootnoteNumberingType.DECIMAL).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_DOCUMENT));
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(new FootnoteAnchor("dummy", 
                        new Footnote("One more"))).Add(new FootnoteAnchor(new Footnote("Two more"))).Add("\n" + TestResourceUtil
                        .GetByronStanza());
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        // TODO: DEVSIX-10143 fix border taking the text rise space
        // TODO DEVSIX-10135 - Big text rise leads to a footnote not being drawn / laid out
        [NUnit.Framework.Test]
        public virtual void NoninheritablePropertyAndStyleFromFootnoteAnchorAppliedTest() {
            String fileName = "noninheritablePropertyAndStyleApplied";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Style footnoteAnchorStyle = new Style().SetMarginLeft(10).SetMarginRight(10);
                    footnoteAnchorStyle.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(12));
                    footnoteAnchorStyle.SetProperty(Property.TEXT_RISE, 5);
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetBackgroundColor
                        (ColorConstants.LIGHT_GRAY).SetBorder(new DashedBorder(ColorConstants.GREEN, 3))).SetFootnoteAnchorStyle
                        (footnoteAnchorStyle).SetFootnoteNumberingType(FootnoteNumberingType.DECIMAL).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_DOCUMENT));
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    anchor.SetBackgroundColor(ColorConstants.GREEN);
                    Style anchorStyle = new Style().SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
                    anchor.AddStyle(anchorStyle);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).SetBackgroundColor(ColorConstants.LIGHT_GRAY
                        ).Add(anchor).Add(new FootnoteAnchor("dummy", new Footnote("One more"))).Add(new FootnoteAnchor(new Footnote
                        ("Two more"))).Add("\n" + TestResourceUtil.GetByronStanza());
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CustomStyleSectionBreakTest() {
            String fileName = "customStyleSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    Style anchorStyle = new Style().SetMarginLeft(10).SetMarginRight(10).SetBackgroundColor(ColorConstants.YELLOW
                        );
                    anchorStyle.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(12));
                    anchorStyle.SetProperty(Property.TEXT_RISE, 0);
                    Div div = new Div().Add(new SectionBreak().SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle
                        (new Style().SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new DashedBorder(ColorConstants.GREEN
                        , 3))).SetFootnoteAnchorStyle(anchorStyle).SetFootnoteNumberingType(FootnoteNumberingType.DECIMAL).SetFootnoteNumberingConfig
                        (FootnoteNumberingConfig.PER_SECTION))).Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PropertyNotOverriddenByCustomStyleInFootnoteTest() {
            String fileName = "propertyNotOverriddenByStyleInFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Text anchorText = new Text("property").SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants.GREEN
                );
            Style customStyleInFootnote = new Style().SetMarginLeft(5).SetMarginRight(5).SetFontSize(5).SetTextRise(3)
                .SetBackgroundColor(ColorConstants.YELLOW);
            RenderDocumentWithCustomAnchor(outFileName, anchorText, customStyleInFootnote, null, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CustomStyleInFootnoteNotOverriddenByDefaultsTest() {
            // TODO DEVSIX-10135 - Big text rise leads to a footnote not being drawn / laid out
            String fileName = "customStyleInFootnoteNotOverriddenByDefaults";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Style customStyleInFootnote = new Style().SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants
                .YELLOW);
            Text anchorText = new Text("style");
            RenderDocumentWithCustomAnchor(outFileName, anchorText, customStyleInFootnote, null, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CustomStylesInTextAndFootnoteTest() {
            String fileName = "customStylesInTextAndFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Text anchorText = new Text("style");
            Style customStyleInFootnote = new Style().SetFontSize(10).SetTextRise(5).SetBackgroundColor(ColorConstants
                .MAGENTA);
            Style customStyle = new Style().SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants.CYAN);
            RenderDocumentWithCustomAnchor(outFileName, anchorText, customStyleInFootnote, customStyle, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PropertyNotOverriddenByCustomStyleTest() {
            String fileName = "propertyNotOverriddenByCustomStyle";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Text anchorText = new Text("property").SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants.GREEN
                );
            Style customStyle = new Style().SetMarginLeft(5).SetMarginRight(5).SetFontSize(5).SetTextRise(3);
            customStyle.SetBackgroundColor(ColorConstants.YELLOW);
            RenderDocumentWithCustomAnchor(outFileName, anchorText, null, customStyle, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void PropertyNotOverriddenByDefaultsTest() {
            String fileName = "propertyNotOverriddenByDefaults";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Text anchorText = new Text("property").SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants.YELLOW
                );
            RenderDocumentWithCustomAnchor(outFileName, anchorText, null, null, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ElementStyleNotOverriddenByDefaultsTest() {
            String fileName = "elementStyleNotOverriddenByDefaults";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Style textStyle = new Style().SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants.YELLOW);
            Text anchorText = new Text("style").AddStyle(textStyle);
            RenderDocumentWithCustomAnchor(outFileName, anchorText, null, null, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ElementStyleNotOverriddenByCustomStyleTest() {
            String fileName = "elementStyleNotOverriddenByCustomStyle";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Style textStyle = new Style().SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants.YELLOW);
            Text anchorText = new Text("style").AddStyle(textStyle);
            Style customStyle = new Style().SetMarginLeft(5).SetMarginRight(5).SetFontSize(5).SetTextRise(3);
            customStyle.SetBackgroundColor(ColorConstants.YELLOW);
            RenderDocumentWithCustomAnchor(outFileName, anchorText, null, customStyle, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CustomStyleNotOverriddenByDefaultsTest() {
            // TODO DEVSIX-10135 - Big text rise leads to a footnote not being drawn / laid out
            String fileName = "customStyleNotOverriddenByDefaults";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Style customStyle = new Style().SetFontSize(15).SetTextRise(10).SetBackgroundColor(ColorConstants.YELLOW);
            Text anchorText = new Text("style");
            RenderDocumentWithCustomAnchor(outFileName, anchorText, null, customStyle, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultStylesInMainTextTest() {
            String fileName = "defaultStylesInMainText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            Text anchorText = new Text("default");
            RenderDocumentWithCustomAnchor(outFileName, anchorText, null, null, 20f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultsInFootnoteOverrideDefaultsFromMainTextTest() {
            String fileName = "defaultsInFootnoteOverrideDefaultsFromMainText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    FootnotesProperties footnotesProperties = new FootnotesProperties().SetFootnotesContainerStyle(new Style()
                        .SetFontSize(30).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new DashedBorder(ColorConstants
                        .GREEN, 3)));
                    document.SetFootnotesProperties(footnotesProperties);
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).SetFontSize(20f);
                    Paragraph firstFootnoteParagraph = new Paragraph(TestResourceUtil.GetByronStanza()).SetFontSize(7f);
                    paragraph.Add(new FootnoteAnchor(new Text("default"), new Footnote(firstFootnoteParagraph))).Add("\n" + TestResourceUtil
                        .GetByronStanza()).Add(new FootnoteAnchor("dummy", new Footnote("One more").SetFontColor(ColorConstants
                        .RED))).Add(new FootnoteAnchor(new Footnote("Two more")));
                    Div div = new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultFootnoteStyleTest() {
            String fileName = "defaultFootnoteStyle";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor(new Footnote(TestResourceUtil
                        .GetByronStanza()))).Add("\n" + TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor("custom", new 
                        Footnote("One more"))).Add("\nOne more line.").Add(new FootnoteAnchor(new Text("text").SetFontSize(5).
                        SetTextRise(3).SetBackgroundColor(ColorConstants.GREEN), new Footnote("Two more"))).Add("\nTwo more lines."
                        ).Add(new FootnoteAnchor(new Footnote("Three more"))).Add("\nThree more lines.").Add(new FootnoteAnchor
                        (new Footnote("Four more")));
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAnchorStyleInFootnoteFromParagraphTest() {
            String fileName = "defaultAnchorStyleFromParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Footnote footnote = new Footnote(new Paragraph(TestResourceUtil.GetByronStanza()).SetFontSize(22f));
                    footnote.SetFontSize(9f);
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor(new Text("first"
                        ), footnote)).Add(new FootnoteAnchor(new Footnote("unstyled"))).Add("\n" + TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAnchorStyleInFootnoteFromFootnoteTest() {
            String fileName = "defaultStyleFromFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    Footnote footnote = new Footnote(new Paragraph(TestResourceUtil.GetByronStanza()));
                    footnote.SetFontSize(22f);
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor(new Text("first"
                        ), footnote)).Add(new FootnoteAnchor(new Footnote("unstyled"))).Add("\n" + TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAnchorStyleInFootnoteFromFootnotesContainerTest() {
            String fileName = "defaultStyleFromFootnotesContainer";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetFontSize
                        (22f)));
                    Footnote footnote = new Footnote(new Paragraph(TestResourceUtil.GetByronStanza()));
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor(new Text("first"
                        ), footnote)).Add(new FootnoteAnchor(new Footnote("unstyled"))).Add("\n" + TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAnchorStyleInFootnoteFromDocumentTest() {
            String fileName = "defaultAnchorStyleInFootnoteFromDocument";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    document.SetFontSize(22f);
                    Footnote footnote = new Footnote(new Paragraph(TestResourceUtil.GetByronStanza()));
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).SetFontSize(12f).Add(new FootnoteAnchor
                        (new Text("first"), footnote)).Add(new FootnoteAnchor(new Footnote("unstyled"))).Add("\n" + TestResourceUtil
                        .GetByronStanza());
                    document.Add(new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void InheritedPropertyInFootnoteFromParagraphTest() {
            String fileName = "inheritedPropertyFromParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetFontColor
                        (ColorConstants.BLUE)));
                    Footnote footnote = new Footnote(new Paragraph(TestResourceUtil.GetByronStanza()).SetFontColor(ColorConstants
                        .GREEN));
                    footnote.SetFontColor(ColorConstants.RED);
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor(new Text("paragraph"
                        ), footnote)).Add(new FootnoteAnchor(new Footnote("unstyled"))).Add("\n" + TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void InheritedTextColorInFootnoteFromFootnoteTest() {
            String fileName = "inheritedPropertyFromFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetFontColor
                        (ColorConstants.BLUE)));
                    Footnote footnote = new Footnote(new Paragraph(TestResourceUtil.GetByronStanza()));
                    footnote.SetFontColor(ColorConstants.GREEN);
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor(new Text("footnote"
                        ), footnote)).Add(new FootnoteAnchor(new Footnote("unstyled"))).Add("\n" + TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void InheritedTextColorInFootnoteFromFootnotesContainerTest() {
            String fileName = "inheritedPropertyFromFootnotesContainer";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnotesContainerStyle(new Style().SetFontColor
                        (ColorConstants.GREEN)));
                    Footnote footnote = new Footnote(new Paragraph(TestResourceUtil.GetByronStanza()));
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor(new Text("container"
                        ), footnote)).Add(new FootnoteAnchor(new Footnote("unstyled"))).Add("\n" + TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.FOOTNOTE_NUM_PER_DOCUMENT_SHOULD_BE_FIRST)]
        public virtual void FootnotePropertiesSectionBreakTest() {
            String fileName = "footnotePropertiesSectionBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            FootnoteNumberingConfig initialNumConfig = FootnoteNumberingConfig.PER_PAGE;
            SetFootnotePropertiesForFootnotes(outFileName, initialNumConfig);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.FOOTNOTE_NUM_PER_DOCUMENT_CANNOT_BE_CHANGED, Count = 2)]
        public virtual void FootnotePropertiesPerDocumentTest() {
            String fileName = "footnotePropertiesPerDocument";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            FootnoteNumberingConfig initialNumConfig = FootnoteNumberingConfig.PER_DOCUMENT;
            SetFootnotePropertiesForFootnotes(outFileName, initialNumConfig);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        private static void SetFootnotePropertiesForFootnotes(String outFileName, FootnoteNumberingConfig initialNumConfig
            ) {
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    document.SetFootnotesProperties(new FootnotesProperties().SetFootnoteNumberingType(FootnoteNumberingType.DECIMAL
                        ).SetFootnoteNumberingConfig(initialNumConfig).SetFootnotesContainerStyle(new Style().SetBackgroundColor
                        (ColorConstants.GREEN, 0.1F)).SetFootnoteAnchorLabelStyle(new Style().SetBorderBottom(new SolidBorder(
                        ColorConstants.GREEN, 1)).SetMarginRight(5)));
                    for (int i = 1; i < 24; ++i) {
                        Footnote footnote = new Footnote("Footnote " + i);
                        FootnoteAnchor anchor = new FootnoteAnchor(footnote);
                        Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor);
                        document.Add(p);
                        SectionBreak sectionBreak = new SectionBreak();
                        if (i % 6 == 0) {
                            FootnotesProperties footnotesProperties = new FootnotesProperties();
                            if (i == 6) {
                                footnotesProperties.SetFootnotesContainerStyle(new Style().SetBackgroundColor(ColorConstants.RED, 0.1F)).SetFootnoteAnchorLabelStyle
                                    (new Style().SetBorderBottom(new SolidBorder(ColorConstants.RED, 1)).SetMarginRight(5)).SetFootnoteNumberingType
                                    (FootnoteNumberingType.ENGLISH_LOWER).SetFootnoteNumberingConfig(FootnoteNumberingConfig.PER_SECTION);
                            }
                            if (i == 12) {
                                footnotesProperties.SetFootnotesContainerStyle(new Style().SetBackgroundColor(ColorConstants.BLUE, 0.1F)).
                                    SetFootnoteAnchorLabelStyle(new Style().SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 1)).SetMarginRight
                                    (5)).SetFootnoteNumberingType(FootnoteNumberingType.DECIMAL).SetFootnoteNumberingConfig(FootnoteNumberingConfig
                                    .PER_DOCUMENT);
                            }
                            if (i == 18) {
                                footnotesProperties.SetFootnotesContainerStyle(new Style().SetBackgroundColor(ColorConstants.YELLOW, 0.1F)
                                    ).SetFootnoteAnchorLabelStyle(new Style().SetBorderBottom(new SolidBorder(ColorConstants.YELLOW, 1)).SetMarginRight
                                    (5)).SetFootnoteNumberingType(FootnoteNumberingType.ENGLISH_UPPER).SetFootnoteNumberingConfig(FootnoteNumberingConfig
                                    .PER_PAGE);
                            }
                            sectionBreak.SetFootnotesProperties(footnotesProperties);
                            document.Add(sectionBreak);
                            document.Add(new Paragraph("NEW SECTION").SetBackgroundColor(ColorConstants.GREEN));
                        }
                    }
                }
            }
        }

        private static void RenderDocumentWithCustomAnchor(String outFileName, Text anchorText, Style customStyleInFootnote
            , Style customStyle, float? paragraphFontSize) {
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    pdfDocument.SetTagged();
                    FootnotesProperties footnotesProperties = new FootnotesProperties().SetFootnotesContainerStyle(new Style()
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new DashedBorder(ColorConstants.GREEN, 3)));
                    if (customStyle != null) {
                        footnotesProperties.SetFootnoteAnchorStyle(customStyle);
                    }
                    if (customStyleInFootnote != null) {
                        footnotesProperties.SetFootnoteAnchorLabelStyle(customStyleInFootnote);
                    }
                    document.SetFootnotesProperties(footnotesProperties);
                    Paragraph paragraph = new Paragraph(TestResourceUtil.GetByronStanza());
                    if (paragraphFontSize != null) {
                        paragraph.SetFontSize(paragraphFontSize.Value);
                    }
                    paragraph.Add(new FootnoteAnchor(anchorText, new Footnote(TestResourceUtil.GetByronStanza()))).Add("\n" + 
                        TestResourceUtil.GetByronStanza()).Add(new FootnoteAnchor("dummy", new Footnote("One more"))).Add(new 
                        FootnoteAnchor(new Footnote("Two more")));
                    Div div = new Div().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.GREEN, 3));
                    document.Add(div);
                }
            }
        }
    }
}
