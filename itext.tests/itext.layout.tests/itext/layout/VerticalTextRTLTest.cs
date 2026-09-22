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
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextRTLTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextRTLTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextRTLTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static ICollection<float> WidthValues() {
            return JavaUtil.ArraysAsList(0F, 400F, 60F);
        }

        [NUnit.Framework.TestCaseSource("WidthValues")]
        public virtual void BasicVerticalRtlTest(float? width) {
            String fileName = "basicVerticalRtl" + (width == 0F ? "" : ("_" + width));
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(true);
                    paragraph.SetHeight(300).SetFontSize(16).SetBorder(new SolidBorder(1));
                    if (width != 0F) {
                        paragraph.SetWidth((float)width);
                    }
                    paragraph.Add(new Text("The quick brown fox jumps over the lazy dog. 1234567890 ABCDEFG abcdefg."));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PageSplitTest() {
            String fileName = "pageSplit";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(new Div().SetHeight(600));
                    VerticalParagraph paragraph = new VerticalParagraph(true);
                    paragraph.SetHeight(300).SetWidth(80).SetFontSize(16).SetBorder(new SolidBorder(1)).SetBackgroundColor(ColorConstants
                        .YELLOW);
                    paragraph.Add(new Text("The quick brown fox jumps over the lazy dog. 1234567890 ABCDEFG abcdefg."));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DirectionRtlTest() {
            String fileName = "directionRtl";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    VerticalParagraph paragraph = new VerticalParagraph(true);
                    paragraph.SetProperty(Property.BASE_DIRECTION, BaseDirection.RIGHT_TO_LEFT);
                    paragraph.SetProperty(Property.TEXT_ALIGNMENT, TextAlignment.RIGHT);
                    paragraph.SetHeight(300).SetFontSize(16).SetBorder(new SolidBorder(1)).SetBackgroundColor(ColorConstants.YELLOW
                        );
                    paragraph.Add(new Text("The quick brown fox jumps over the lazy dog. 1234567890 ABCDEFG abcdefg."));
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void InnerTextVerticalRlTest() {
            String fileName = "innerTextVerticalRl";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph.SetHeight(300).SetFontSize(16).SetBorder(new SolidBorder(1));
                    Text text = new Text("The quick brown fox jumps over the lazy dog. 1234567890 ABCDEFG abcdefg.");
                    text.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_RL);
                    text.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text.SetBackgroundColor(ColorConstants.YELLOW);
                    paragraph.Add(text);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SeveralInnerTextVerticalRlTest() {
            String fileName = "severalInnerTextVerticalRl";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph.SetHeight(300).SetFontSize(16).SetBorder(new SolidBorder(1));
                    Text text = new Text("The quick brown fox jumps over the lazy dog. 1234567890 ABCDEFG abcdefg.");
                    text.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_RL);
                    text.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text.SetBackgroundColor(ColorConstants.YELLOW);
                    Text text2 = new Text("One more\nvertical\nRTL text\nwith line breaks.");
                    text2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_RL);
                    text2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    text2.SetBackgroundColor(ColorConstants.PINK);
                    paragraph.Add(text).Add(text2);
                    document.Add(paragraph);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FloatWithVerticalRlTextTest() {
            // Float + vertical-rl writing mode is not supported.
            String fileName = "floatWithVerticalRlText";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    // Create a floated DIV
                    Div floatedDiv = new Div().SetWidth(200).SetHeight(200).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder
                        (new SolidBorder(ColorConstants.BLACK, 1)).Add(new Paragraph("Floated\nElement"));
                    floatedDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
                    document.Add(floatedDiv);
                    // Add paragraph between the floated elements
                    Paragraph normalParagraph = new Paragraph("Normal text added after right floated div, " + "but before the next left floated div."
                        );
                    document.Add(normalParagraph);
                    floatedDiv.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
                    document.Add(floatedDiv);
                    // Add another paragraph after the floated elements.
                    normalParagraph = new Paragraph("Normal text added after vertical paragraphs and divs.");
                    document.Add(normalParagraph);
                    document.Add(new VerticalTextRTLTest.CustomVerticalParagraph("This is a vertical paragraph with a lot of text to "
                         + "demonstrate how it interacts with floated elements. It should wrap around the floated elements " +
                         "and continue on the next line if necessary. " + "The quick brown fox jumps over the lazy dog. 1234567890 ABCDEFG abcdefg."
                        , true).SetHeight(300));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        private class CustomVerticalParagraph : VerticalParagraph {
            public CustomVerticalParagraph(String text, bool rightToLeftProgression)
                : base(text, rightToLeftProgression) {
            }

            public override IDictionary<int, String> GetUnsupportedProperties() {
                return JavaCollectionsUtil.EmptyMap<int, String>();
            }
        }
    }
}
