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

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicVerticalTextTest() {
            String fileName = "basicVerticalText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("some long vertical text to trigger multiple line breaks. Font size will be also increased to make it easier."
                        ));
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("some long vertical text\nto trigger multiple line breaks.\nFont size will be also increased\nto make it easier."
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.Add(new Text("some long vertical\ntext to trigger multiple line breaks.\nFont size will be also increased to make it easier.\n"
                        ));
                    paragraph.Add(new Text("Additional chunk of text,\n to trigger page break.\nFont size increased even further."
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text text = new Text("some text");
                    text.SetProperty(Property.ITALIC_SIMULATION, true);
                    text.SetProperty(Property.BOLD_SIMULATION, true);
                    text.SetProperty(Property.UNDERLINE, JavaCollectionsUtil.SingletonList(new Underline(ColorConstants.RED, 1
                        , .75F, 0, 0, 1 / 4F, PdfCanvasConstants.LineCapStyle.BUTT)));
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
            String fileName = "verticalTextAndHorizontalText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
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
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Text normalText = new Text("Normal\ntext").SetBackgroundColor(ColorConstants.CYAN);
                    // TODO DEVSIX-10137 double line break is ignored,
                    //  although it's suppose to create a separate line without any content.
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
    }
}
