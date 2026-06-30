using System;
using System.Collections.Generic;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties.Margins;
using iText.Pdfua;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FootnotesTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/FootnotesTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private const String PART1_1 = "Tiger! Tiger! burning bright\n" + "In the forests of the night,\n" + "What immortal";

        private const String PART1_2 = " hand or eye\n" + "Could frame thy fearful symmetry?\n\n";

        private const String PART2_1 = "In what distant deeps";

        private const String PART2_2 = " or skies\n" + "Burnt the fire of thine eyes?\n" + "On what wings dare he aspire?\n"
             + "What the hand dare seize the fire?";

        private const String PART3 = "And what shoulder, and what art,\n" + "Could twist the sinews of thy heart?\n"
             + "And when thy heart began to beat,\n" + "What dread hand? and what dread feet?";

        private const String NOTE1 = "immortal (adjective): never dying";

        private const String NOTE2 = "deeps (noun): seas";

        private static readonly String IMG1 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        private static readonly String IMG2 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/FOX.bmp";

        public static IList<PdfConformance> Conformances() {
            return UaValidationTestFramework.GetConformanceList(true);
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void TestFootnoteTagging(PdfConformance conformance) {
            PdfConformance pdfConformance = conformance;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, pdfConformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                try {
                    Document document = new Document(pdfDoc);
                    PdfFont font = null;
                    font = PdfFontFactory.CreateFont(FONT, "WinAnsi", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                    document.SetFont(font);
                    Div div1 = new Div();
                    Footnote footnote = new Footnote(NOTE1);
                    footnote.SetBackgroundColor(ColorConstants.CYAN);
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Footnote footnote2 = new Footnote(new Paragraph(NOTE2).SetMargin(0));
                    footnote2.SetBackgroundColor(ColorConstants.ORANGE);
                    FootnoteAnchor anchor2 = new FootnoteAnchor("[2]", footnote2);
                    Paragraph p = new Paragraph(PART1_1);
                    p.Add(anchor);
                    p.Add(PART1_2);
                    div1.Add(p);
                    p = new Paragraph(PART2_1);
                    p.Add(PART2_1);
                    p.Add(anchor2);
                    p.Add(PART2_2);
                    div1.Add(p);
                    div1.Add(new Paragraph(PART3));
                    div1.Add(p).SetBorder(new SolidBorder(ColorConstants.MAGENTA, 5));
                    document.Add(div1);
                }
                catch (System.IO.IOException e) {
                    throw new Exception("Error creating test document", e);
                }
            }
            );
            framework.AssertBothValid("footnotes");
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void TestFootnoteTaggingImages(PdfConformance conformance) {
            PdfConformance pdfConformance = conformance;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, pdfConformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                try {
                    Document document = new Document(pdfDoc);
                    PdfFont font = null;
                    font = PdfFontFactory.CreateFont(FONT, "WinAnsi", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                    document.SetFont(font);
                    Div div1 = new Div();
                    Footnote footnote = new Footnote(NOTE1);
                    footnote.SetBackgroundColor(ColorConstants.CYAN);
                    iText.Layout.Element.Image anchorImage = new Image(ImageDataFactory.Create(IMG1));
                    anchorImage.SetWidth(10);
                    anchorImage.GetAccessibilityProperties().SetAlternateDescription("dog");
                    FootnoteAnchor anchor = new FootnoteAnchor(anchorImage, footnote);
                    Footnote footnote2 = new Footnote(new Paragraph(NOTE2).SetMargin(0));
                    footnote2.SetBackgroundColor(ColorConstants.ORANGE);
                    anchorImage = new iText.Layout.Element.Image(ImageDataFactory.Create(IMG2));
                    anchorImage.SetWidth(10);
                    anchorImage.GetAccessibilityProperties().SetAlternateDescription("fox");
                    FootnoteAnchor anchor2 = new FootnoteAnchor(anchorImage, footnote2);
                    Paragraph p = new Paragraph(PART1_1);
                    p.Add(anchor);
                    p.Add(PART1_2);
                    div1.Add(p);
                    p = new Paragraph(PART2_1);
                    p.Add(PART2_1);
                    p.Add(anchor2);
                    p.Add(PART2_2);
                    div1.Add(p);
                    div1.Add(new Paragraph(PART3));
                    div1.Add(p).SetBorder(new SolidBorder(ColorConstants.MAGENTA, 5));
                    document.Add(div1);
                }
                catch (System.IO.IOException e) {
                    throw new Exception("Error creating test document", e);
                }
            }
            );
            framework.AssertBothValid("UA2IMG");
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void TestFootnoteTableTagging(PdfConformance conformance) {
            PdfConformance pdfConformance = conformance;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, pdfConformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                try {
                    Document document = new Document(pdfDoc);
                    PdfFont font = null;
                    font = PdfFontFactory.CreateFont(FONT, "WinAnsi", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                    document.SetFont(font);
                    Footnote footnote = new Footnote("Footnote text");
                    footnote.SetBackgroundColor(ColorConstants.PINK);
                    FootnoteAnchor anchor = new FootnoteAnchor(new Text("1").SetFontSize(6).SetTextRise(7), footnote);
                    Footnote footnote2 = new Footnote("Footnote text 2");
                    footnote2.SetBackgroundColor(ColorConstants.YELLOW);
                    FootnoteAnchor anchor2 = new FootnoteAnchor(new Text("2").SetFontSize(6).SetTextRise(7), footnote2);
                    Footnote footnote3 = new Footnote("Footnote text 3");
                    footnote.SetBackgroundColor(ColorConstants.PINK);
                    FootnoteAnchor anchor3 = new FootnoteAnchor(new Text("3").SetFontSize(6).SetTextRise(7), footnote3);
                    Footnote footnote4 = new Footnote("Footnote text 4");
                    footnote2.SetBackgroundColor(ColorConstants.YELLOW);
                    FootnoteAnchor anchor4 = new FootnoteAnchor(new Text("4").SetFontSize(6).SetTextRise(7), footnote4);
                    Table table = new Table(4);
                    for (int i = 0; i < 120; ++i) {
                        Paragraph paragraph = new Paragraph("Cell " + i);
                        if (i == 1) {
                            paragraph.Add(anchor).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 5) {
                            paragraph.Add(anchor2).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 119) {
                            paragraph.Add(anchor4).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i == 100) {
                            paragraph.Add(anchor3).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        }
                        if (i < 4) {
                            table.AddHeaderCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)));
                        }
                        else {
                            if (i > 115) {
                                table.AddFooterCell(new Cell().Add(paragraph).SetBorder(new SolidBorder(ColorConstants.BLUE, 2)));
                            }
                            else {
                                table.AddCell(paragraph);
                            }
                        }
                    }
                    document.Add(table);
                }
                catch (System.IO.IOException e) {
                    throw new Exception("Error creating test document", e);
                }
            }
            );
            framework.AssertBothValid("footnotesTables");
        }
    }
}
