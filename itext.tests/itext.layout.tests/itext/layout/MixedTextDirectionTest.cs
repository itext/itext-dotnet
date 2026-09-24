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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class MixedTextDirectionTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/MixedTextDirectionTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/MixedTextDirectionTest/";

        public static ICollection<WritingMode> MixedVertical() {
            return JavaUtil.ArraysAsList(WritingMode.HORIZONTAL_TB, WritingMode.VERTICAL_LR, WritingMode.VERTICAL_RL);
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphMixedTextTest() {
            String fileName = "paragraphMixedTextTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(200);
                    Text text1 = new Text("vertical text chunk");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    paragraph.Add(text2);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.TestCaseSource("MixedVertical")]
        public virtual void ParagraphMixedVerticalTextTest(WritingMode? paragraphWritingMode) {
            // TODO DEVSIX-10200 Consider text elements with different writing-mode as inline-blocks,
            //  after that vertical RTL text chunks in vertical LTR paragraphs and vice versa will be fixed.
            String fileName = "paragraphMixedVerticalText_" + paragraphWritingMode.ToString();
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(200);
                    paragraph.SetProperty(Property.WRITING_MODE, paragraphWritingMode);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                    Text text1 = new Text("vertical text chunk left-to-right ");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("vertical text chunk right-to-left ");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_RL);
                    text2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("one more vertical text chunk left-to-right ");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    Text text4 = new Text("horizontal text ");
                    text4.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text4.SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    paragraph.Add(text4);
                    paragraph.Add(text2);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.TestCaseSource("MixedVertical")]
        public virtual void ParagraphMixedVerticalTextNoHeightTest(WritingMode? paragraphWritingMode) {
            // TODO DEVSIX-10200 Consider text elements with different writing-mode as inline-blocks,
            //  after that vertical RTL text chunks in vertical LTR paragraphs and vice versa should be fixed.
            // No line breaks in vertical text with different writing-mode looks like workaround for horizontal text.
            String fileName = "paragraphMixedVerticalTextNoHeight_" + paragraphWritingMode.ToString();
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetProperty(Property.WRITING_MODE, paragraphWritingMode);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                    Text text1 = new Text("vertical text chunk\nleft-to-right ");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("vertical\ntext chunk\nright-to-left ");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_RL);
                    text2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("one more\nvertical text chunk\nleft-to-right ");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    Text text4 = new Text("horizontal\ntext ");
                    text4.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text4.SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    paragraph.Add(text4);
                    paragraph.Add(text2);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphMixedTextWithLineBreaksTest() {
            String fileName = "paragraphMixedTextWithLineBreaksTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    Text text1 = new Text("vertical text chunk\n");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk\n");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    paragraph.Add(text2);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphMixedTextNoEnoughHorizontalSpaceTest() {
            String fileName = "paragraphMixedTextNoEnoughHorizontalSpaceTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetWidth(300);
                    Text text1 = new Text("vertical text chunk");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    for (int i = 0; i < 4; ++i) {
                        paragraph.Add(text1);
                        paragraph.Add(text2);
                        paragraph.Add(text3);
                    }
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphMixedTextWithPageBreakTest() {
            String fileName = "paragraphMixedTextWithPageBreakTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetWidth(500);
                    paragraph.SetFontSize(20);
                    Text text1 = new Text("vertical text chunk");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    for (int i = 0; i < 5; ++i) {
                        paragraph.Add(text1);
                        paragraph.Add(text2);
                        paragraph.Add(text3);
                    }
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalParagraphMixedTest() {
            String fileName = "verticalParagraphMixedTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(200);
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text text1 = new Text("vertical text chunk");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalParagraphMixedWithLineBreaksTest() {
            String fileName = "verticalParagraphMixedWithLineBreaksTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetHeight(200);
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text text1 = new Text("vertical text chunk\n");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MixedTextWithAlignmentTest() {
            String fileName = "mixedTextWithAlignmentTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
                    Text text1 = new Text("vertical text chunk");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalMixedTextWithAlignmentTest() {
            String fileName = "verticalMixedTextWithAlignmentTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph.SetHeight(700);
                    Text text1 = new Text("vertical text");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("horizontal text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("second vertical text");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalWritingAtTextLevelTest() {
            String fileName = "verticalWritingAtTextLevelTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    Text text1 = new Text("first text chunk ");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("second text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text(" third text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalWritingAtTextLevelTwoLinesTest() {
            String fileName = "verticalWritingAtTextLevelTwoLinesTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetFontSize(20);
                    Text text1 = new Text("text chunk 1");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("text chunk 2");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("text chunk 3");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    for (int i = 0; i < 10; ++i) {
                        paragraph.Add(text1);
                        paragraph.Add(text2);
                        paragraph.Add(text3);
                    }
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalWritingAtTextLevelPageBreakTest() {
            String fileName = "verticalWritingAtTextLevelPageBreakTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetFontSize(20);
                    Text text1 = new Text("text chunk 1");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("text chunk 2");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text("text chunk 3");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    for (int i = 0; i < 30; ++i) {
                        paragraph.Add(text1);
                        paragraph.Add(text2);
                        paragraph.Add(text3);
                    }
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalWritingAtTextLevelLongTextTest() {
            String fileName = "verticalWritingAtTextLevelLongTextTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetHeight(200);
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    Text text1 = new Text("some very long text\nchunk with\nvertical writing");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text1.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("second text chunk ");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text(" small chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    text3.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalParagraphWithHorizontalTextTest() {
            String fileName = "verticalParagraphWithHorizontalTextTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    Text text1 = new Text("first text chunk ");
                    text1.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text1.SetBackgroundColor(ColorConstants.MAGENTA);
                    Text text2 = new Text("second text chunk");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text2.SetBackgroundColor(ColorConstants.CYAN);
                    Text text3 = new Text(" third text chunk");
                    text3.SetProperty(Property.WRITING_MODE, WritingMode.HORIZONTAL_TB);
                    text3.SetBackgroundColor(ColorConstants.ORANGE);
                    paragraph.Add(text1);
                    paragraph.Add(text2);
                    paragraph.Add(text3);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }
    }
}
