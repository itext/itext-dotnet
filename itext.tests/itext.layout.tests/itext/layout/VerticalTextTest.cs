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
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextTest/";

        private static readonly String EXPANDED_FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/BioRhymeExpanded-Regular.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicVerticalTextTest() {
            String fileName = "basicVerticalText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("some text"));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextDifferentFontsInParagraphTest() {
            String fileName = "verticalTextDifferentFontsInParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text text1 = new Text("some text in courier font.\nFont size is 25.\n");
                    PdfFont courier = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                    text1.SetFont(courier);
                    text1.SetFontSize(25);
                    paragraph.Add(text1);
                    Text text2 = new Text("some text in times new roman font.\nFont size is 20.");
                    PdfFont timesRoman = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    text2.SetFont(timesRoman);
                    text2.SetFontSize(20);
                    paragraph.Add(text2);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextDifferentFontsInLineTest() {
            String fileName = "verticalTextDifferentFontsInLineTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text text1 = new Text("some text in courier font. Font size is 25.");
                    PdfFont courier = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                    text1.SetFont(courier);
                    text1.SetFontSize(25);
                    text1.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.Add(text1);
                    Text text2 = new Text("some text in times new roman font. Font size is 40.");
                    PdfFont timesRoman = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    text2.SetFont(timesRoman);
                    text2.SetFontSize(40);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    paragraph.Add(text2);
                    Text text3 = new Text("some text in helvetica bold font. Font size is 10.");
                    PdfFont helvetica = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    text3.SetFont(helvetica);
                    text3.SetFontSize(10);
                    text3.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SeveralTextChunksVerticalTextTest() {
            String fileName = "severalTextChunksVerticalText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("first text chunk "));
                    paragraph.Add(new Text("second text chunk "));
                    paragraph.Add(new Text("third text chunk "));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LongVerticalTextTest() {
            String fileName = "longVerticalText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("some long vertical text to trigger multiple line breaks. Font size will be also " 
                        + "increased to make it easier."));
                    paragraph.SetFontSize(25);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LongVerticalTextWithLineBreaksTest() {
            String fileName = "longVerticalTextWithLineBreaks";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("some long vertical text\nto trigger multiple line breaks.\nFont size will be " + "also increased\nto make it easier."
                        ));
                    paragraph.SetFontSize(25);
                    paragraph.SetBorder(new SolidBorder(1));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LongVerticalTextWithPageBreakTest() {
            String fileName = "longVerticalTextWithPageBreak";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("some long vertical\ntext to trigger multiple line breaks.\nFont size will be " + "also increased to make it easier.\n"
                        ));
                    paragraph.Add(new Text("Additional chunk of text,\n to trigger page break.\n" + "Font size increased even further."
                        ));
                    paragraph.SetFontSize(35);
                    paragraph.SetBorder(new SolidBorder(1));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextWithStyleAdjustmentsTest() {
            String fileName = "verticalTextWithStyleAdjustments";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    document.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Paragraph paragraph = new Paragraph();
                    Text text = new Text("some text");
                    text.SetProperty(Property.ITALIC_SIMULATION, true);
                    text.SetProperty(Property.BOLD_SIMULATION, true);
                    text.SetProperty(Property.UNDERLINE, JavaCollectionsUtil.SingletonList(new Underline(ColorConstants.RED, 1
                        , .75F, 0, 0, 1 / 2F, PdfCanvasConstants.LineCapStyle.BUTT)));
                    text.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.Add(text);
                    paragraph.Add(new Text("Normal some text\nsome text").SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                    paragraph.SetBorder(new SolidBorder(1));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextAndHorizontalTextTest() {
            // TODO DEVSIX-10183 vertical and horizontal text in one paragraph.
            String fileName = "verticalTextAndHorizontalText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBorder(new SolidBorder(ColorConstants.BLACK, 2));
                    Text verticalText = new Text("vertical text.");
                    verticalText.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    verticalText.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    verticalText.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                    paragraph.Add(verticalText);
                    paragraph.Add(new Text("horizontal text.").SetBorder(new SolidBorder(ColorConstants.BLUE, 1)));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextWithLongWordsTest() {
            String fileName = "verticalTextWithLongWords";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.Add(new Paragraph("Long word, first line and first word:"));
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text longWordText = new Text("Tooooooooolongword");
                    longWordText.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetBorder(new SolidBorder(1));
                    paragraph.SetHeight(100);
                    paragraph.Add(longWordText);
                    paragraph.Add(" and usual words length now");
                    document.Add(paragraph);
                    document.Add(new Paragraph("Long word, first line and not first word:"));
                    paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    longWordText = new Text("Tooooooooolongword");
                    longWordText.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetBorder(new SolidBorder(1));
                    paragraph.SetHeight(100);
                    paragraph.Add("Abc ");
                    paragraph.Add(longWordText);
                    paragraph.Add(" and usual words length now");
                    document.Add(paragraph);
                    document.Add(new Paragraph("Long word, not first line:"));
                    paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    longWordText = new Text("Tooooooooolongword");
                    longWordText.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetBorder(new SolidBorder(1));
                    paragraph.SetHeight(100);
                    paragraph.Add("Abc\n");
                    paragraph.Add(longWordText);
                    paragraph.Add(" and usual words length now");
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void VerticalTextWithMaxHeightWidthParagraphTest() {
            String fileName = "verticalTextWithMaxHeightParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text longText = new Text("Pretty long text example is provided here, " + "especially given its font-size is set to bigger value"
                        );
                    longText.SetFontSize(32);
                    longText.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetBorder(new SolidBorder(1));
                    paragraph.Add(longText);
                    paragraph.SetHeight(100);
                    paragraph.SetWidth(100);
                    document.Add(paragraph);
                    paragraph.SetHeight(500);
                    paragraph.SetWidth(300);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextSpaceTrimmingTest() {
            String fileName = "verticalTextSpaceTrimming";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Text normalText = new Text("Normal\ntext").SetBackgroundColor(ColorConstants.CYAN);
                    Text whitespacesRiddenText = new Text("     Hello     \n \n World    \n        \n  ").SetBackgroundColor(ColorConstants
                        .LIGHT_GRAY);
                    Text threeMSpaceWrappedText = new Text(" MMM ").SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    Paragraph vParagraph = new Paragraph();
                    vParagraph.SetBorder(new SolidBorder(1));
                    vParagraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    vParagraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    vParagraph.Add(whitespacesRiddenText);
                    vParagraph.Add(normalText);
                    document.Add(vParagraph);
                    Paragraph hParagraph = new Paragraph();
                    hParagraph.SetBorder(new SolidBorder(1));
                    hParagraph.Add(whitespacesRiddenText);
                    hParagraph.Add(normalText);
                    document.Add(hParagraph);
                    vParagraph = new Paragraph();
                    vParagraph.SetBorder(new SolidBorder(1));
                    vParagraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    vParagraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    // fine-tune height to fit threeMSpaceWrappedText characters
                    vParagraph.SetFontSize(12);
                    vParagraph.SetHeight(12 * 5 + 12);
                    // fully fits
                    vParagraph.Add(threeMSpaceWrappedText).Add("\n");
                    // trailing text elem space shouldn't fit on the line
                    vParagraph.Add("M").Add(threeMSpaceWrappedText).Add("M").Add("\n");
                    vParagraph.Add("ABC                      ");
                    vParagraph.Add(normalText);
                    document.Add(vParagraph);
                    hParagraph = new Paragraph();
                    hParagraph.SetBorder(new SolidBorder(1));
                    // fine-tune height to fit threeMSpaceWrappedText characters
                    hParagraph.SetFontSize(12);
                    hParagraph.SetWidth((float)(12 * 3 + 12 / 2.4 * 2));
                    // fully fits
                    hParagraph.Add(threeMSpaceWrappedText).Add("\n");
                    // trailing text elem space shouldn't fit on the line
                    hParagraph.Add("M").Add(threeMSpaceWrappedText).Add("M").Add("\n");
                    hParagraph.Add("ABC                      ");
                    hParagraph.Add(normalText);
                    document.Add(hParagraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LineThroughWithTextRiseTest() {
            // TODO DEVSIX-10180 Support text rise in html mode for vertical text
            String outFileName = DESTINATION_FOLDER + "lineThroughWithTextRise.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_lineThroughWithTextRise.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Text textUp = new Text("textRise10f_with_lineThrough");
                    textUp.SetTextRise(-10f);
                    textUp.SetLineThrough();
                    textUp.SetFontColor(ColorConstants.GREEN);
                    Text textDown = new Text("textRise-10f_with_lineThrough");
                    textDown.SetTextRise(-10f);
                    textDown.SetLineThrough();
                    textDown.SetFontColor(ColorConstants.RED);
                    Paragraph n = new Paragraph("baseline");
                    n.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    n.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    n.Add(textUp).Add(textDown);
                    document.Add(n);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void UnderlineTest() {
            String outFileName = DESTINATION_FOLDER + "underline.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_underline.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    document.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Underline underline = new Underline(null, 0, 0.1f, 0, -0.1f, PdfCanvasConstants.LineCapStyle.BUTT).SetStrokeWidth
                        (2).SetStrokeColor(new TransparentColor(ColorConstants.PINK, 0.5f)).SetDashPattern(new float[] { 5, 5, 
                        10, 5 }, 5);
                    Paragraph p = new Paragraph("Yellow text with pink stroked dashed underline.").SetFontSize(45).SetFontColor
                        (ColorConstants.YELLOW).SetUnderline(underline);
                    TransparentColor strokeColor = new TransparentColor(ColorConstants.GREEN, 0.5f);
                    Underline underline2 = new Underline(ColorConstants.DARK_GRAY, 0, 0.1f, 0, 0.3f, PdfCanvasConstants.LineCapStyle
                        .BUTT).SetStrokeWidth(1).SetStrokeColor(strokeColor);
                    Paragraph p2 = new Paragraph("Text with line-through and default underline.").SetFontSize(50).SetStrokeWidth
                        (1).SetFontColor(ColorConstants.DARK_GRAY).SetStrokeColor(strokeColor).SetUnderline(underline2).SetUnderline
                        ();
                    Underline underline3 = new Underline(null, 0, 0.1f, 0, 0.9f, PdfCanvasConstants.LineCapStyle.BUTT);
                    Paragraph p3 = new Paragraph("Text with null font color and default overline.").SetFontSize(50).SetFontColor
                        ((TransparentColor)null).SetUnderline(underline3);
                    // This line should be around the middle of the text compared to horizontal text.
                    Underline underline4 = new Underline(null, 0, 0.1f, 15, 0f, PdfCanvasConstants.LineCapStyle.BUTT);
                    Paragraph p4 = new Paragraph("Text with custom yPosition (15).").SetFontSize(50).SetUnderline(underline4);
                    document.Add(p).Add(p2).Add(p3).Add(p4);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FontStyleSimulationTest01() {
            String outFileName = DESTINATION_FOLDER + "fontStyleSimulationTest01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_fontStyleSimulationTest01.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    document.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Paragraph p = new Paragraph("I'm underlined").SetUnderline();
                    document.Add(p);
                    p = new Paragraph("I'm strikethrough").SetLineThrough();
                    document.Add(p);
                    p = new Paragraph(new Text("I'm a bold simulation font").SetBackgroundColor(ColorConstants.GREEN)).SimulateBold
                        ();
                    document.Add(p);
                    p = new Paragraph(new Text("I'm an italic simulation font").SetBackgroundColor(ColorConstants.GREEN)).SimulateItalic
                        ();
                    document.Add(p);
                    p = new Paragraph(new Text("I'm a super bold italic underlined linethrough piece of text and no one " + "can be better than me, even if such a long description will cause me to occupy two lines"
                        ).SetBackgroundColor(ColorConstants.GREEN)).SimulateItalic().SimulateBold().SetUnderline().SetLineThrough
                        ();
                    document.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextLineWidthTest() {
            String fileName = "verticalTextLineWidth";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div();
                    div.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    div.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    PdfFont helvetica = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    div.SetFont(helvetica);
                    Paragraph paragraph = new Paragraph().SetHeight(500).SetFontSize(50).SetBackgroundColor(new DeviceRgb(187, 
                        187, 255));
                    Text text1 = new Text("WWWWWWW").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text1);
                    Text text2 = new Text("aaaaaaaa").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(20);
                    paragraph.Add(text2);
                    Text text3 = new Text("iiiiii").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text3);
                    Text text4 = new Text("jjjj").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(80);
                    paragraph.Add(text4);
                    Text text5 = new Text("......").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text5);
                    Text text6 = new Text("Wow!").SetBackgroundColor(new DeviceRgb(173, 255, 47));
                    paragraph.Add(text6);
                    paragraph.Add("Hello World");
                    Paragraph paragraph2 = new Paragraph().SetHeight(500).SetFontSize(20).SetBackgroundColor(new DeviceRgb(255
                        , 0, 204));
                    text1 = new Text("WWWWWWWwwwwWWWWW").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph2.Add(text1);
                    text2 = new Text("Waaaaaaaa").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(20);
                    paragraph2.Add(text2);
                    text3 = new Text("i").SetBackgroundColor(ColorConstants.YELLOW).SetFontSize(80);
                    paragraph2.Add(text3);
                    text4 = new Text("Wjjj").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(80);
                    paragraph2.Add(text4);
                    text5 = new Text("....").SetBackgroundColor(ColorConstants.YELLOW).SetFontSize(80);
                    paragraph2.Add(text5);
                    text6 = new Text("Wow!").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(80);
                    paragraph2.Add(text6);
                    paragraph2.Add("Hello World");
                    div.Add(paragraph).Add(new AreaBreak()).Add(paragraph2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextExpandedFontTest() {
            String fileName = "verticalTextExpandedFont";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div();
                    div.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    div.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    PdfFont bioRhyme = PdfFontFactory.CreateFont(EXPANDED_FONT);
                    div.SetFont(bioRhyme);
                    Paragraph paragraph = new Paragraph().SetHeight(600).SetFontSize(40).SetBackgroundColor(new DeviceRgb(187, 
                        187, 255));
                    Text text1 = new Text("WWWWWWW").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text1);
                    Text text2 = new Text("aaaaaaaa").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(20);
                    paragraph.Add(text2);
                    Text text3 = new Text("iiiiii").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text3);
                    Text text4 = new Text("jjjj").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(80);
                    paragraph.Add(text4);
                    Text text5 = new Text("......").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text5);
                    Text text6 = new Text("Wow!").SetBackgroundColor(new DeviceRgb(173, 255, 47));
                    paragraph.Add(text6);
                    paragraph.Add("Hello World");
                    Paragraph paragraph2 = new Paragraph().SetHeight(600).SetFontSize(20).SetBackgroundColor(new DeviceRgb(255
                        , 0, 204));
                    text1 = new Text("WWWWWWWwwwwWWWWW").SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph2.Add(text1);
                    text2 = new Text("Waaaaaaaa").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(20);
                    paragraph2.Add(text2);
                    text3 = new Text("i").SetBackgroundColor(ColorConstants.YELLOW).SetFontSize(40);
                    paragraph2.Add(text3);
                    text4 = new Text("Wjjj").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(40);
                    paragraph2.Add(text4);
                    text5 = new Text("....").SetBackgroundColor(ColorConstants.YELLOW).SetFontSize(50);
                    paragraph2.Add(text5);
                    text6 = new Text("Wow!").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(50);
                    paragraph2.Add(text6);
                    paragraph2.Add("Hello World");
                    div.Add(paragraph).Add(new AreaBreak()).Add(paragraph2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalAlignTextRiseTest() {
            // TODO DEVSIX-10180 Support text rise in html mode for vertical text
            String fileName = "verticalAlignTextRise";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div();
                    div.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    div.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    PdfFont helvetica = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    div.SetFont(helvetica);
                    Paragraph paragraph = new Paragraph().SetFontSize(30).SetBackgroundColor(new DeviceRgb(187, 187, 255));
                    Text text1 = new Text("Text").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    Text text2 = new Text("rise").SetBackgroundColor(new DeviceRgb(229, 235, 253)).SetTextRise(20);
                    Text text3 = new Text("1").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    text3.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(InlineVerticalAlignmentType
                        .FIXED, -20));
                    Text text4 = new Text("2").SetBackgroundColor(new DeviceRgb(229, 235, 253));
                    text4.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(InlineVerticalAlignmentType
                        .FIXED, 20));
                    Text text5 = new Text("check").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    paragraph.Add(text1).Add(text2).Add(text3).Add(text4).Add(text5);
                    document.Add(div.Add(paragraph));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentTest() {
            String fileName = "inlineVerticalAlignment";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div();
                    div.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    div.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    PdfFont helvetica = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    div.SetFont(helvetica);
                    Paragraph paragraph = new Paragraph().SetFontSize(60).SetBackgroundColor(new DeviceRgb(187, 187, 255));
                    Text text = new Text("Text").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    paragraph.Add(text);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.BASELINE);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.TEXT_TOP);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.TEXT_BOTTOM);
                    paragraph.Add("\n");
                    paragraph.Add(text);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.SUB);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.SUPER);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.FIXED);
                    paragraph.Add("\n");
                    paragraph.Add(text);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.FRACTION);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.MIDDLE);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.TOP);
                    AddAlignedElement(paragraph, InlineVerticalAlignmentType.BOTTOM);
                    paragraph.Add("\n");
                    // Property.LEADING is not supported for vertical text for now.
                    paragraph.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateMultipliedValue(2));
                    document.Add(div.Add(paragraph));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void OccupiedAreaDivTest() {
            String fileName = "occupiedAreaDiv";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div().SetFontSize(50).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new SolidBorder
                        (ColorConstants.DARK_GRAY, 1));
                    PdfFont bioRhyme = PdfFontFactory.CreateFont(EXPANDED_FONT);
                    div.SetFont(bioRhyme);
                    div.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateMultipliedValue(2));
                    Div div1 = new Div().SetBackgroundColor(new DeviceRgb(210, 250, 179)).SetBorder(new SolidBorder(new DeviceRgb
                        (0, 128, 0), 1));
                    div1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    div1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Div div2 = new Div().SetBackgroundColor(new DeviceRgb(210, 250, 179)).SetBorder(new SolidBorder(new DeviceRgb
                        (0, 128, 0), 1));
                    div2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    Text text1 = new Text("W").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    Text text2 = new Text("j").SetBackgroundColor(new DeviceRgb(229, 235, 253)).SetBorder(new SolidBorder(ColorConstants
                        .GREEN, 1));
                    Text text3 = new Text("50").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    Text text4 = new Text("10").SetBackgroundColor(new DeviceRgb(229, 235, 253)).SetFontSize(10);
                    Text text5 = new Text("30").SetBackgroundColor(new DeviceRgb(255, 255, 211)).SetBorder(new SolidBorder(ColorConstants
                        .RED, 1)).SetFontSize(30);
                    div1.Add(new Paragraph().Add(text1).Add(text2).Add(text3).Add(text4).Add(text5));
                    div2.Add(new Paragraph().Add(text1).Add(text2).Add(text3).Add(text4).Add(text5));
                    div.Add(div1).Add(div2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void OccupiedAreaParagraphTest() {
            String fileName = "occupiedAreaParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div().SetFontSize(50).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(new SolidBorder
                        (ColorConstants.DARK_GRAY, 1));
                    PdfFont bioRhyme = PdfFontFactory.CreateFont(EXPANDED_FONT);
                    div.SetFont(bioRhyme);
                    div.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateMultipliedValue(2));
                    Paragraph paragraph1 = new Paragraph().SetBackgroundColor(new DeviceRgb(210, 250, 179)).SetBorder(new SolidBorder
                        (new DeviceRgb(0, 128, 0), 1));
                    paragraph1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Paragraph paragraph2 = new Paragraph().SetBackgroundColor(new DeviceRgb(210, 250, 179)).SetBorder(new SolidBorder
                        (new DeviceRgb(0, 128, 0), 1));
                    paragraph2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    Text text1 = new Text("W").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    Text text2 = new Text("j").SetBackgroundColor(new DeviceRgb(229, 235, 253)).SetBorder(new SolidBorder(ColorConstants
                        .GREEN, 1));
                    Text text3 = new Text("50").SetBackgroundColor(new DeviceRgb(255, 255, 211));
                    Text text4 = new Text("10").SetBackgroundColor(new DeviceRgb(229, 235, 253)).SetFontSize(10);
                    Text text5 = new Text("30").SetBackgroundColor(new DeviceRgb(255, 255, 211)).SetBorder(new SolidBorder(ColorConstants
                        .RED, 1)).SetFontSize(30);
                    paragraph1.Add(text1).Add(text2).Add(text3).Add(text4).Add(text5);
                    paragraph2.Add(text1).Add(text2).Add(text3).Add(text4).Add(text5);
                    div.Add(paragraph1).Add(paragraph2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextWithWordSpaceTest() {
            String fileName = "verticalTextWithWordSpaceTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(500);
                    Text wordSpacing30 = new Text("word-spacing 30pt with line\nbreak.\n\n\n");
                    wordSpacing30.SetBackgroundColor(ColorConstants.GREEN);
                    wordSpacing30.SetWordSpacing(30);
                    paragraph.Add(wordSpacing30);
                    Text wordSpacing10 = new Text("word-spacing 10pt with line\nbreak.\n\n\n");
                    wordSpacing10.SetBackgroundColor(ColorConstants.ORANGE);
                    wordSpacing10.SetWordSpacing(10);
                    paragraph.Add(wordSpacing10);
                    Text wordSpacingMinus15 = new Text("word-spacing minus 15pt with line\nbreak.\n\n\n");
                    wordSpacingMinus15.SetBackgroundColor(ColorConstants.RED);
                    wordSpacingMinus15.SetWordSpacing(-15);
                    paragraph.Add(wordSpacingMinus15);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextWithCharacterSpaceTest() {
            String fileName = "verticalTextWithCharacterSpaceTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(500);
                    Text characterSpacing30 = new Text("character-spacing 30pt with line\nbreak.\n\n\n");
                    characterSpacing30.SetBackgroundColor(ColorConstants.GREEN);
                    characterSpacing30.SetCharacterSpacing(30);
                    paragraph.Add(characterSpacing30);
                    Text characterSpacing10 = new Text("character-spacing 10pt with line\nbreak.\n\n\n");
                    characterSpacing10.SetBackgroundColor(ColorConstants.ORANGE);
                    characterSpacing10.SetCharacterSpacing(10);
                    paragraph.Add(characterSpacing10);
                    Text characterSpacingMinus5 = new Text("character-spacing minus 5pt with line\nbreak.\n\n\n");
                    characterSpacingMinus5.SetBackgroundColor(ColorConstants.RED);
                    characterSpacingMinus5.SetCharacterSpacing(-5);
                    paragraph.Add(characterSpacingMinus5);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void VerticalTextWithCharacterAndWordSpaceExtremeValuesTest() {
            String fileName = "verticalTextWithCharacterAndWordSpaceExtremeValuesTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(500);
                    paragraph.SetPaddingTop(400);
                    Text characterSpacingMinus30 = new Text("character-spacing minus 30pt with line\nbreak.\n\n\n");
                    characterSpacingMinus30.SetBackgroundColor(ColorConstants.GREEN);
                    characterSpacingMinus30.SetCharacterSpacing(-30);
                    paragraph.Add(characterSpacingMinus30);
                    Text wordSpacingMinus300 = new Text("word-spacing minus 300pt with line\nbreak.\n\n\n");
                    wordSpacingMinus300.SetBackgroundColor(ColorConstants.ORANGE);
                    wordSpacingMinus300.SetWordSpacing(-300);
                    paragraph.Add(wordSpacingMinus300);
                    Text characterSpacing300 = new Text("character-spacing 300pt with line\nbreak.\n\n\n");
                    characterSpacing300.SetBackgroundColor(ColorConstants.RED);
                    characterSpacing300.SetCharacterSpacing(300);
                    paragraph.Add(characterSpacing300);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void JustifiedAlignmentTest() {
            String fileName = "justifiedAlignmentTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph container = new Paragraph();
                    Text alignedText1 = new Text("text to be aligned with line\nbreak.");
                    alignedText1.SetBackgroundColor(ColorConstants.GREEN);
                    Text alignedText2 = new Text("text to be aligned with line\nbreak.");
                    alignedText2.SetBackgroundColor(ColorConstants.ORANGE);
                    Text alignedText3 = new Text("text to be aligned with line\nbreak.");
                    alignedText3.SetBackgroundColor(ColorConstants.RED);
                    Paragraph paragraph0 = new Paragraph();
                    paragraph0.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph0.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph0.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph0.SetTextAlignment(TextAlignment.LEFT);
                    paragraph0.SetHeight(700);
                    paragraph0.SetMargin(10);
                    paragraph0.Add(alignedText1);
                    paragraph0.Add(alignedText2);
                    paragraph0.Add(alignedText3);
                    container.Add(paragraph0);
                    Paragraph paragraph1 = new Paragraph();
                    paragraph1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph1.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph1.SetHeight(700);
                    paragraph1.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
                    paragraph1.SetMargin(10);
                    // Extremely large ratio results in word spacing being negative. However, any further increase doesn't affect word spacing.
                    paragraph1.SetSpacingRatio(10000f);
                    paragraph1.Add(alignedText1);
                    paragraph1.Add(alignedText2);
                    paragraph1.Add(alignedText3);
                    container.Add(paragraph1);
                    Paragraph paragraph2 = new Paragraph();
                    paragraph2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph2.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph2.SetHeight(700);
                    paragraph2.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
                    paragraph2.SetMargin(10);
                    paragraph2.SetSpacingRatio(1f);
                    paragraph2.Add(alignedText1);
                    paragraph2.Add(alignedText2);
                    paragraph2.Add(alignedText3);
                    container.Add(paragraph2);
                    Paragraph paragraph3 = new Paragraph();
                    paragraph3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph3.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph3.SetHeight(700);
                    paragraph3.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
                    paragraph3.SetMargin(10);
                    // Extremely low ration results in word-spacing being close to zero, and character spacing taking over.
                    paragraph3.SetSpacingRatio(0.0001f);
                    paragraph3.Add(alignedText1);
                    paragraph3.Add(alignedText2);
                    paragraph3.Add(alignedText3);
                    container.Add(paragraph3);
                    document.Add(container);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DifferentAlignmentValuesTogetherTest() {
            String fileName = "differentAlignmentValuesTogetherTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph container = new Paragraph();
                    Text alignedText1 = new Text("text to be aligned with line\nbreak.");
                    alignedText1.SetBackgroundColor(ColorConstants.GREEN);
                    Text alignedText2 = new Text("text to be aligned with line\nbreak.");
                    alignedText2.SetBackgroundColor(ColorConstants.ORANGE);
                    Text alignedText3 = new Text("text to be aligned with line\nbreak.");
                    alignedText3.SetBackgroundColor(ColorConstants.RED);
                    Paragraph paragraph0 = new Paragraph();
                    paragraph0.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph0.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph0.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph0.SetTextAlignment(TextAlignment.LEFT);
                    paragraph0.SetHeight(700);
                    paragraph0.SetMargin(10);
                    paragraph0.Add(alignedText1);
                    paragraph0.Add(alignedText2);
                    paragraph0.Add(alignedText3);
                    container.Add(paragraph0);
                    Paragraph paragraph1 = new Paragraph();
                    paragraph1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph1.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph1.SetHeight(700);
                    paragraph1.SetTextAlignment(TextAlignment.CENTER);
                    paragraph1.SetMargin(10);
                    paragraph1.Add(alignedText1);
                    paragraph1.Add(alignedText2);
                    paragraph1.Add(alignedText3);
                    container.Add(paragraph1);
                    Paragraph paragraph2 = new Paragraph();
                    paragraph2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph2.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph2.SetHeight(700);
                    paragraph2.SetTextAlignment(TextAlignment.RIGHT);
                    paragraph2.SetMargin(10);
                    paragraph2.Add(alignedText1);
                    paragraph2.Add(alignedText2);
                    paragraph2.Add(alignedText3);
                    container.Add(paragraph2);
                    Paragraph paragraph3 = new Paragraph();
                    paragraph3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph3.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph3.SetHeight(700);
                    paragraph3.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
                    paragraph3.SetMargin(10);
                    paragraph3.Add(alignedText1);
                    paragraph3.Add(alignedText2);
                    paragraph3.Add(alignedText3);
                    container.Add(paragraph3);
                    document.Add(container);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.TestCaseSource("AlignmentValues")]
        public virtual void VerticalTextWithAlignmentTest(TextAlignment? alignment) {
            String fileName = "verticalTextWithAlignment" + alignment;
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(500);
                    paragraph.SetTextAlignment(alignment);
                    // TextAlignment.JUSTIFIED does nothing since line ends with a line break.
                    Text alignedText1 = new Text("text to be aligned with line\nbreak.");
                    alignedText1.SetBackgroundColor(ColorConstants.GREEN);
                    paragraph.Add(alignedText1);
                    // TextAlignment.JUSTIFIED does nothing since line ends with a line break.
                    Text alignedText2 = new Text("text to be aligned with line\nbreak.");
                    alignedText2.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(alignedText2);
                    // TextAlignment.JUSTIFIED justifies the content.
                    Text alignedText3 = new Text("text to be aligned without line break.");
                    alignedText3.SetBackgroundColor(ColorConstants.RED);
                    paragraph.Add(alignedText3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        public static ICollection<TextAlignment> AlignmentValues() {
            return JavaUtil.ArraysAsList(TextAlignment.LEFT, TextAlignment.CENTER, TextAlignment.RIGHT, TextAlignment.
                JUSTIFIED, TextAlignment.JUSTIFIED_ALL);
        }

        private void AddAlignedElement(Paragraph p, InlineVerticalAlignmentType? verticalAlignment) {
            Text text1 = new Text(" " + verticalAlignment + " ");
            text1.SetFontSize(12).SetBackgroundColor(new DeviceRgb(229, 235, 253));
            if (verticalAlignment == InlineVerticalAlignmentType.FIXED) {
                text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment, 25));
            }
            else {
                if (verticalAlignment == InlineVerticalAlignmentType.FRACTION) {
                    text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment, 0.20F
                        ));
                }
                else {
                    text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment));
                }
            }
            p.Add(text1);
        }
    }
}
