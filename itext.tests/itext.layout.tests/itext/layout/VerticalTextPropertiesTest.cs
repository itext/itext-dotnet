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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Hyphenation;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Splitting;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextPropertiesTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextPropertiesTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextPropertiesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void HyphenationTest() {
            String fileName = "hyphenation";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.SetHyphenation(new HyphenationConfig("en", "EN", 2, 2));
                    Text text = new Text("Hyphen hyphen hyphen hyphen hyphen hyphen hyphen ");
                    paragraph.Add(text).Add("non\u2011breaking").Add("\n").Add(text).Add("non\u2010breaking");
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void HorizontalAlignmentTest() {
            String fileName = "horizontalAlignment";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("first line\nsecond line\nthird line"));
                    paragraph.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    document.Add(paragraph);
                    paragraph.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                    document.Add(paragraph);
                    paragraph.SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TextAlignmentTest() {
            String fileName = "textAlignment";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("first line\nsecond line\nthird line"));
                    paragraph.SetHeight(200).SetBorder(new SolidBorder(1));
                    paragraph.SetTextAlignment(TextAlignment.CENTER);
                    document.Add(paragraph);
                    paragraph.SetTextAlignment(TextAlignment.JUSTIFIED);
                    document.Add(paragraph);
                    paragraph.SetTextAlignment(TextAlignment.RIGHT);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TextRenderingModeTest() {
            String fileName = "textRenderingMode";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("first\nsecond\nthird"));
                    paragraph.SetBorder(new SolidBorder(1)).SetFontSize(20).SetStrokeColor(ColorConstants.CYAN);
                    paragraph.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
                    document.Add(paragraph);
                    paragraph.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL_STROKE);
                    document.Add(paragraph);
                    paragraph.SetBackgroundColor(ColorConstants.PINK);
                    paragraph.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE_CLIP);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RotationAngleTest() {
            String fileName = "rotationAngle";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("first line\nsecond line\nthird line"));
                    paragraph.SetHeight(200).SetBorder(new SolidBorder(1));
                    paragraph.SetRotationAngle(Math.PI / 2);
                    document.Add(paragraph);
                    paragraph.SetRotationAngle(-Math.PI / 2);
                    document.Add(paragraph);
                    paragraph.SetRotationAngle(Math.PI / 4);
                    document.Add(paragraph);
                    paragraph.SetRotationAngle(-Math.PI / 4);
                    document.Add(paragraph);
                    paragraph.SetRotationAngle(Math.PI);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void OrphansControlTest() {
            String fileName = "orphansControl";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    // Add spacing to force page break
                    document.Add(new Div().SetHeight(600));
                    // Paragraph with enough lines to demonstrate orphans behavior
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("Line 1\nLine 2\nLine 3\nLine 4\nLine 5\nLine 6\nLine 7\nLine 8\nLine 9\nLine 10"));
                    paragraph.SetHeight(250).SetWidth(100).SetBorder(new SolidBorder(1));
                    // Control orphans (minimum 8 lines at the start if split)
                    paragraph.SetOrphansControl(new ParagraphOrphansControl(8));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WidowsControlTest() {
            String fileName = "widowsControl";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    // Add spacing to force page break
                    document.Add(new Div().SetHeight(600));
                    // Paragraph with enough lines to demonstrate widows behavior
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("Line 1\nLine 2\nLine 3\nLine 4\nLine 5\nLine 6\nLine 7\nLine 8\nLine 9\nLine 10"));
                    paragraph.SetHeight(250).SetWidth(100).SetBorder(new SolidBorder(1));
                    // Control widows (minimum 4 lines at the end if split)
                    paragraph.SetWidowsControl(new ParagraphWidowsControl(5, 1, true));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, Count = 2)]
        public virtual void FloatWithVerticalTextTest() {
            String fileName = "floatWithVerticalText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    // Create a floated DIV
                    Div floatedDiv = new Div().SetWidth(50).SetHeight(200).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder
                        (new SolidBorder(ColorConstants.BLACK, 1)).Add(new Paragraph("Floated\nElement"));
                    floatedDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
                    document.Add(floatedDiv);
                    // Create a floated VerticalParagraph
                    VerticalParagraph verticalParagraph = new VerticalParagraph(false);
                    verticalParagraph.Add(new Text("Line 1\nLine 2\nLine 3\nLine 4\nLine 5\nLine 6\nLine 7\nLine 8"));
                    verticalParagraph.SetHeight(200).SetWidth(150).SetBorder(new SolidBorder(1));
                    verticalParagraph.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
                    document.Add(verticalParagraph);
                    // Add paragraph between the floated elements
                    Paragraph normalParagraph = new Paragraph("Normal text added after right floated vertical paragraph " + "and div, but before the next left floated elements."
                        );
                    document.Add(normalParagraph);
                    floatedDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
                    document.Add(floatedDiv);
                    verticalParagraph.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
                    document.Add(verticalParagraph);
                    // Add another paragraph after the floated elements.
                    normalParagraph = new Paragraph("Normal text added after vertical paragraphs and divs.");
                    document.Add(normalParagraph);
                    document.Add(new VerticalParagraph("This is a vertical paragraph with a lot of text to demonstrate " + "how it interacts with floated elements. It should wrap around the floated elements "
                         + "and continue on the next line if necessary. " + "The quick brown fox jumps over the lazy dog. 1234567890 ABCDEFG abcdefg."
                        , false).SetHeight(300));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void HorizontalScalingTest() {
            String fileName = "horizontalScaling";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    Text text = new Text("Scaled Text Example");
                    paragraph.Add(text);
                    paragraph.SetHeight(200).SetBorder(new SolidBorder(1));
                    // Test different horizontal scaling values
                    text.SetProperty(Property.HORIZONTAL_SCALING, 0.75f);
                    document.Add(paragraph);
                    text.SetProperty(Property.HORIZONTAL_SCALING, 1.0f);
                    document.Add(paragraph);
                    text.SetProperty(Property.HORIZONTAL_SCALING, 1.25f);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CharacterSpacingTest() {
            String fileName = "characterSpacing";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("Character\nSpacing\nTest"));
                    paragraph.SetHeight(200).SetWidth(100).SetBorder(new SolidBorder(1));
                    // Test different character spacing values
                    paragraph.SetCharacterSpacing(-5);
                    document.Add(paragraph);
                    paragraph.SetCharacterSpacing(2);
                    document.Add(paragraph);
                    paragraph.SetCharacterSpacing(5);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WordSpacingTest() {
            String fileName = "wordSpacing";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("Word spacing test with multiple words here"));
                    paragraph.SetHeight(200).SetWidth(100).SetBorder(new SolidBorder(1));
                    // Test different word spacing values
                    paragraph.SetWordSpacing(-20);
                    document.Add(paragraph);
                    paragraph.SetWordSpacing(0);
                    document.Add(paragraph);
                    paragraph.SetWordSpacing(20);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SpacingRatioTest() {
            String fileName = "spacingRatio";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("Spacing Ratio Test Text"));
                    paragraph.SetHeight(200).SetWidth(100).SetBorder(new SolidBorder(1));
                    paragraph.SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
                    // Test different spacing ratios
                    paragraph.SetProperty(Property.SPACING_RATIO, 0.8f);
                    document.Add(paragraph);
                    paragraph.SetProperty(Property.SPACING_RATIO, 1.0f);
                    document.Add(paragraph);
                    paragraph.SetProperty(Property.SPACING_RATIO, 1.5f);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, Count = 1)]
        public virtual void TabStopsTest() {
            String fileName = "tabStops";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document doc = new Document(pdfDocument)) {
                    doc.Add(new Paragraph("x-coordinate = 100").SetFontColor(ColorConstants.RED).SetFirstLineIndent(100).SetFontSize
                        (8));
                    doc.Add(new Paragraph("x-coordinate = 200").SetFontColor(ColorConstants.GREEN).SetFirstLineIndent(200).SetFontSize
                        (8));
                    doc.Add(new Paragraph("x-coordinate = 300").SetFontColor(ColorConstants.BLUE).SetFirstLineIndent(300).SetFontSize
                        (8));
                    Paragraph p = new Paragraph().Add("Hello, iText!\n").Add(new Tab()).AddTabStops(new TabStop(100, TabAlignment
                        .CENTER, new DashedLine(.5f))).Add("Hi, iText!\n").Add(new Tab()).AddTabStops(new TabStop(200, TabAlignment
                        .RIGHT, new DashedLine(.5f))).Add("Hello, iText!\n").Add(new Tab()).AddTabStops(new TabStop(300, TabAlignment
                        .LEFT, new DashedLine(.5f))).Add("Hello, iText!\n");
                    p.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    p.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    p.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    doc.Add(p);
                    float[] positions = new float[] { 100, 200, 300 };
                    DrawTabStopsPositions(positions, doc, 1, 0, 120);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, Count = 1)]
        public virtual void TextAnchorTest() {
            String fileName = "textAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("Anchor Start\nAnchor Middle\nAnchor End"));
                    paragraph.SetHeight(200).SetWidth(100).SetBorder(new SolidBorder(1));
                    paragraph.SetProperty(Property.TEXT_ANCHOR, TextAnchor.END);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TextRiseTest() {
            String fileName = "textRise";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    Text text1 = new Text("Normal");
                    Text text2 = new Text("Raised").SetTextRise(5);
                    Text text3 = new Text("Lowered").SetTextRise(-5);
                    paragraph.Add(text1).Add(" ").Add(text2).Add(" ").Add(text3);
                    paragraph.SetHeight(300).SetWidth(100).SetBorder(new SolidBorder(1));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SplitCharactersTest() {
            String fileName = "splitCharacters";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    paragraph.Add(new Text("Verylongwordthatmaysplitandcontinueontheline"));
                    paragraph.SetHeight(200).SetWidth(80).SetBorder(new SolidBorder(1));
                    paragraph.SetProperty(Property.SPLIT_CHARACTERS, new BreakAllSplitCharacters());
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SkewTest() {
            String fileName = "skew";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    Text text = new Text("Skewed Text Example");
                    paragraph.Add(text);
                    paragraph.SetHeight(200).SetWidth(100).SetBorder(new SolidBorder(1));
                    // Test different skew angles
                    text.SetSkew(0, 10);
                    document.Add(paragraph);
                    text.SetSkew(10, 0);
                    document.Add(paragraph);
                    text.SetSkew(15, 15);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalScalingTest() {
            String fileName = "verticalScaling";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(false);
                    Text text = new Text("Scaled\nVertically");
                    paragraph.Add(text);
                    paragraph.SetHeight(200).SetWidth(100).SetBorder(new SolidBorder(1));
                    // Test different vertical scaling values
                    text.SetProperty(Property.VERTICAL_SCALING, 0.75f);
                    document.Add(paragraph);
                    text.SetProperty(Property.VERTICAL_SCALING, 1.0f);
                    document.Add(paragraph);
                    text.SetProperty(Property.VERTICAL_SCALING, 1.5f);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        private void DrawTabStopsPositions(float[] positions, Document doc, int pageNum, int yStart, int dy) {
            PdfCanvas canvas = new PdfCanvas(doc.GetPdfDocument().GetPage(pageNum));
            float left = doc.GetLeftMargin();
            float h = doc.GetPdfDocument().GetPage(pageNum).GetCropBox().GetHeight() - yStart;
            canvas.SaveState();
            canvas.SetLineDash(4, 2);
            canvas.SetLineWidth(0.5f);
            canvas.SetLineDash(4, 2);
            foreach (float f in positions) {
                canvas.MoveTo(left + f, h);
                canvas.LineTo(left + f, h - dy);
            }
            canvas.Stroke();
            canvas.RestoreState();
            canvas.Release();
        }
    }
}
