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
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextCjkTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextCjkTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextCjkTest/";

        private static readonly String NOTO_SANS_SC = FONTS_FOLDER + "NotoSansCJKsc-Regular.otf";

        private static readonly String NOTO_SANS_SC_BOLD = FONTS_FOLDER + "NotoSansCJKsc-Bold.otf";

        private static readonly String NOTO_SERIF_SC = FONTS_FOLDER + "NotoSerifCJKsc-Regular.otf";

        private static readonly String NOTO_SANS_JP = FONTS_FOLDER + "NotoSansCJKjp-Regular.otf";

        private static readonly String NOTO_SANS_KR = FONTS_FOLDER + "NotoSansCJKkr-Regular.otf";

        private static readonly String NOTO_SANS_MONGOLIAN = FONTS_FOLDER + "NotoSansMongolian-Regular.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextSimplifiedChineseTest() {
            String fileName = "verticalTextSimplifiedChinese";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec("你好，这是一段竖排中文文本。汉字应保持直立。", LoadCjkFont
                (NOTO_SANS_SC), 24);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, spec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, spec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "你好"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "竖排中文文本"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "汉字应保持直立"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextJapaneseWithSmallKanaTest() {
            String fileName = "verticalTextJapaneseWithSmallKana";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec("こんにちは、これは縦書きの日本語のテキストです。ちょっと難しいです。ラーメン。"
                , LoadCjkFont(NOTO_SANS_JP), 24);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, spec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, spec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "こんにちは"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "縦書きの日本語のテキストです"
                ));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ちょっと難しいです"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ラーメン"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextKoreanHangulTest() {
            String fileName = "verticalTextKoreanHangul";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec("안녕하세요, 이것은 세로쓰기 한국어 텍스트입니다.", 
                LoadCjkFont(NOTO_SANS_KR), 24);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, spec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, spec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "안녕하세요"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "세로쓰기 한국어 텍스트입니다"
                ));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextMongolianTest() {
            String fileName = "verticalTextMongolian";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec("ᠮᠣᠩᠭᠣᠯ ᠪᠢᠴᠢᠭ ᠣᠷᠴᠢᠨ ᠴᠠᠭ", LoadCjkFont
                (NOTO_SANS_MONGOLIAN), 24);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, spec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, spec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ᠮᠣᠩᠭᠣᠯ"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ᠪᠢᠴᠢᠭ"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkPunctuationTest() {
            String fileName = "verticalTextCjkPunctuation";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec("彼は「こんにちは」と言った。それから、『さようなら』も言った。"
                , LoadCjkFont(NOTO_SANS_JP), 24);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, spec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, spec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "彼は「こんにちは」と言った")
                );
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "それから、『さようなら』も言った"
                ));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkWithEmbeddedLatinAndDigitsTest() {
            String fileName = "verticalTextCjkWithEmbeddedLatinAndDigits";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec("今日は2026年8月19日、iTextのバージョンは8です。ABC123もテストします。"
                , LoadCjkFont(NOTO_SANS_JP), 20);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, spec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, spec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "2026"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "8"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "19"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "iText"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ABC123"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextVerticalMetricsFontComparisonTest() {
            String fileName = "verticalTextVerticalMetricsFontComparison";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            String sentence = "竖排文字的高度取决于字体的垂直度量。";
            VerticalTextCjkTest.CjkTextSpec sansSpec = new VerticalTextCjkTest.CjkTextSpec(sentence, LoadCjkFont(NOTO_SANS_SC
                ), 24).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkTest.CjkTextSpec serifSpec = new VerticalTextCjkTest.CjkTextSpec(sentence, LoadCjkFont(NOTO_SERIF_SC
                ), 24).BackgroundColor(ColorConstants.CYAN);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, sansSpec));
                    document.Add(BuildParagraph(true, serifSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, sansSpec));
                    document.Add(BuildParagraph(false, serifSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, sentence, 2));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkLineBreakingWithoutWordBoundariesTest() {
            String fileName = "verticalTextCjkLineBreakingWithoutWordBoundaries";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            String text = "这是一段没有任何标点或空格的连续中文文本用来测试竖排换行是否可以在任意汉字之间发生而不需要像西文那样等待单词边界";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec(text, LoadCjkFont(NOTO_SANS_SC)
                , 20).BackgroundColor(ColorConstants.LIGHT_GRAY);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph verticalParagraph = BuildParagraph(true, spec);
                    verticalParagraph.SetHeight(150);
                    document.Add(verticalParagraph);
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    Paragraph horizontalParagraph = BuildParagraph(false, spec);
                    horizontalParagraph.SetWidth(150);
                    document.Add(horizontalParagraph);
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, text));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkUnderlineAndStrikethroughTest() {
            String fileName = "verticalTextCjkUnderlineAndStrikethrough";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec underlinedSpec = new VerticalTextCjkTest.CjkTextSpec("下划线文本", LoadCjkFont(
                NOTO_SANS_SC), 24).BackgroundColor(ColorConstants.LIGHT_GRAY).Underline(new Underline(ColorConstants.RED
                , 1, .75F, 0, 0, 1 / 4F, PdfCanvasConstants.LineCapStyle.BUTT));
            VerticalTextCjkTest.CjkTextSpec strikethroughSpec = new VerticalTextCjkTest.CjkTextSpec("删除线文本", LoadCjkFont
                (NOTO_SANS_SC), 24).BackgroundColor(ColorConstants.CYAN).Underline(new Underline(ColorConstants.BLUE, 
                1, .75F, 0, 0, 1 / 2F, PdfCanvasConstants.LineCapStyle.BUTT));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, underlinedSpec, strikethroughSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, underlinedSpec, strikethroughSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "下划线文本"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "删除线文本"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkBoldItalicSimulationTest() {
            String fileName = "verticalTextCjkBoldItalicSimulation";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec regularSpec = new VerticalTextCjkTest.CjkTextSpec("常规字体", LoadCjkFont(NOTO_SANS_SC
                ), 24).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkTest.CjkTextSpec simulatedBoldItalicSpec = new VerticalTextCjkTest.CjkTextSpec("模拟粗斜体", LoadCjkFont
                (NOTO_SANS_SC), 24).BackgroundColor(ColorConstants.CYAN).BoldSimulation().ItalicSimulation();
            VerticalTextCjkTest.CjkTextSpec realBoldSpec = new VerticalTextCjkTest.CjkTextSpec("真实粗体字体", LoadCjkFont(NOTO_SANS_SC_BOLD
                ), 24).BackgroundColor(ColorConstants.LIGHT_GRAY);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, regularSpec, simulatedBoldItalicSpec, realBoldSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, regularSpec, simulatedBoldItalicSpec, realBoldSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "常规字体"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "模拟粗斜体"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "真实粗体字体"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        //TODO DEVSIX-10167: Update test after fix
        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkIdeographicSpaceVsRegularSpaceTest() {
            String fileName = "verticalTextCjkIdeographicSpaceVsRegularSpace";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec ideographicSpec = new VerticalTextCjkTest.CjkTextSpec("文字\u3000文字", LoadCjkFont
                (NOTO_SANS_SC), 24).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkTest.CjkTextSpec regularSpaceSpec = new VerticalTextCjkTest.CjkTextSpec("文字 文字", LoadCjkFont
                (NOTO_SANS_SC), 24).BackgroundColor(ColorConstants.CYAN);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, ideographicSpec));
                    document.Add(BuildParagraph(true, regularSpaceSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, ideographicSpec));
                    document.Add(BuildParagraph(false, regularSpaceSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "文字", 4));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkCustomLeadingTest() {
            String fileName = "verticalTextCjkCustomLeading";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec defaultLeadingSpec = new VerticalTextCjkTest.CjkTextSpec("默认行距\n默认行距", LoadCjkFont
                (NOTO_SANS_SC), 20).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkTest.CjkTextSpec customLeadingSpec = new VerticalTextCjkTest.CjkTextSpec("自定义行距\n自定义行距", LoadCjkFont
                (NOTO_SANS_SC), 20).BackgroundColor(ColorConstants.CYAN);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, defaultLeadingSpec));
                    Paragraph customLeadingVertical = BuildParagraph(true, customLeadingSpec);
                    customLeadingVertical.SetMultipliedLeading(2.5F);
                    document.Add(customLeadingVertical);
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, defaultLeadingSpec));
                    Paragraph customLeadingHorizontal = BuildParagraph(false, customLeadingSpec);
                    customLeadingHorizontal.SetMultipliedLeading(2.5F);
                    document.Add(customLeadingHorizontal);
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "默认行距", 2));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "自定义行距", 2));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkFullWidthVsHalfWidthFormsTest() {
            String fileName = "verticalTextCjkFullWidthVsHalfWidthForms";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkTest.CjkTextSpec spec = new VerticalTextCjkTest.CjkTextSpec("全角：１２３ＡＢＣ 半角：123ABC", LoadCjkFont
                (NOTO_SANS_SC), 20);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.Add(BuildParagraph(true, spec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, spec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "全角"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "１２３ＡＢＣ"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "半角"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "123ABC"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        private static PdfFont LoadCjkFont(String path) {
            return PdfFontFactory.CreateFont(path, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
        }

        private static Paragraph VerticalParagraph() {
            Paragraph paragraph = new Paragraph();
            paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
            paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            paragraph.SetBorder(new SolidBorder(1));
            return paragraph;
        }

        private static Paragraph HorizontalParagraph() {
            Paragraph paragraph = new Paragraph();
            paragraph.SetBorder(new SolidBorder(1));
            return paragraph;
        }

        private static Paragraph BuildParagraph(bool vertical, params VerticalTextCjkTest.CjkTextSpec[] specs) {
            Paragraph paragraph = vertical ? VerticalParagraph() : HorizontalParagraph();
            foreach (VerticalTextCjkTest.CjkTextSpec spec in specs) {
                Text text = new Text(spec.content);
                text.SetFont(spec.font);
                text.SetFontSize(spec.fontSize);
                if (spec.backgroundColor != null) {
                    text.SetBackgroundColor(spec.backgroundColor);
                }
                if (spec.underline != null) {
                    text.SetProperty(Property.UNDERLINE, JavaCollectionsUtil.SingletonList(spec.underline));
                }
                if (spec.boldSimulation) {
                    text.SetProperty(Property.BOLD_SIMULATION, true);
                }
                if (spec.italicSimulation) {
                    text.SetProperty(Property.ITALIC_SIMULATION, true);
                }
                paragraph.Add(text);
            }
            return paragraph;
        }

        private sealed class CjkTextSpec {
            protected internal readonly String content;

            protected internal readonly PdfFont font;

            protected internal readonly float fontSize;

            protected internal Color backgroundColor;

            protected internal iText.Layout.Properties.Underline underline;

            protected internal bool boldSimulation;

            protected internal bool italicSimulation;

            protected internal CjkTextSpec(String content, PdfFont font, float fontSize) {
                this.content = content;
                this.font = font;
                this.fontSize = fontSize;
            }

            protected internal VerticalTextCjkTest.CjkTextSpec BackgroundColor(Color color) {
                this.backgroundColor = color;
                return this;
            }

            protected internal VerticalTextCjkTest.CjkTextSpec Underline(iText.Layout.Properties.Underline underline) {
                this.underline = underline;
                return this;
            }

            protected internal VerticalTextCjkTest.CjkTextSpec BoldSimulation() {
                this.boldSimulation = true;
                return this;
            }

            protected internal VerticalTextCjkTest.CjkTextSpec ItalicSimulation() {
                this.italicSimulation = true;
                return this;
            }
        }
    }
}
