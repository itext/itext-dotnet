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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextCjkMixedFontsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextCjkMixedFontsTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextCjkMixedFontsTest/";

        private static readonly String NOTO_SANS_SC = FONTS_FOLDER + "NotoSansCJKsc-Regular.otf";

        private static readonly String NOTO_SANS_TC = FONTS_FOLDER + "NotoSansCJKtc-Regular.otf";

        private static readonly String NOTO_SANS_JP = FONTS_FOLDER + "NotoSansCJKjp-Regular.otf";

        private static readonly String NOTO_SANS_KR = FONTS_FOLDER + "NotoSansCJKkr-Regular.otf";

        private static readonly String NOTO_SANS_MONGOLIAN = FONTS_FOLDER + "NotoSansMongolian-Regular.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextChineseJapaneseKoreanInSameParagraphTest() {
            String fileName = "verticalTextChineseJapaneseKoreanInSameParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkMixedFontsTest.CjkTextSpec chineseSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("中文：你好世界。\n"
                , LoadCjkFont(NOTO_SANS_SC), 20).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkMixedFontsTest.CjkTextSpec japaneseSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("日本語：こんにちは世界。\n"
                , LoadCjkFont(NOTO_SANS_JP), 20).BackgroundColor(ColorConstants.CYAN);
            VerticalTextCjkMixedFontsTest.CjkTextSpec koreanSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("한국어: 안녕하세요 세계."
                , LoadCjkFont(NOTO_SANS_KR), 20).BackgroundColor(ColorConstants.LIGHT_GRAY);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.Add(BuildParagraph(true, chineseSpec, japaneseSpec, koreanSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, chineseSpec, japaneseSpec, koreanSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "中文：你好世界。"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "日本語：こんにちは世界。"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "한국어: 안녕하세요 세계."
                ));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextChineseAndLatinSameLineDifferentFontsTest() {
            String fileName = "verticalTextChineseAndLatinSameLineDifferentFonts";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkMixedFontsTest.CjkTextSpec chineseSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("产品名称："
                , LoadCjkFont(NOTO_SANS_SC), 20).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkMixedFontsTest.CjkTextSpec latinSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("iText Core"
                , PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 20).BackgroundColor(ColorConstants.CYAN);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.Add(BuildParagraph(true, chineseSpec, latinSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, chineseSpec, latinSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "产品名称："));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "iText Core"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextMongolianAndChineseSameParagraphTest() {
            String fileName = "verticalTextMongolianAndChineseSameParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkMixedFontsTest.CjkTextSpec mongolianSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("ᠮᠣᠩᠭᠣᠯ ᠬᠡᠯᠡ\n"
                , LoadCjkFont(NOTO_SANS_MONGOLIAN), 22).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkMixedFontsTest.CjkTextSpec chineseSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("蒙古语与中文并排书写。"
                , LoadCjkFont(NOTO_SANS_SC), 22).BackgroundColor(ColorConstants.CYAN);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.Add(BuildParagraph(true, mongolianSpec, chineseSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, mongolianSpec, chineseSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ᠮᠣᠩᠭᠣᠯ"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ᠬᠡᠯᠡ"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "蒙古语与中文并排书写。"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextFourScriptsMultipleFontSizesTest() {
            String fileName = "verticalTextFourScriptsMultipleFontSizes";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkMixedFontsTest.CjkTextSpec chineseSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("中文 12pt。\n"
                , LoadCjkFont(NOTO_SANS_SC), 12);
            VerticalTextCjkMixedFontsTest.CjkTextSpec japaneseSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("日本語 18pt。\n"
                , LoadCjkFont(NOTO_SANS_JP), 18);
            VerticalTextCjkMixedFontsTest.CjkTextSpec koreanSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("한국어 24pt.\n"
                , LoadCjkFont(NOTO_SANS_KR), 24);
            VerticalTextCjkMixedFontsTest.CjkTextSpec mongolianSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("ᠮᠣᠩᠭᠣᠯ 30pt"
                , LoadCjkFont(NOTO_SANS_MONGOLIAN), 30);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.Add(BuildParagraph(true, chineseSpec, japaneseSpec, koreanSpec, mongolianSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, chineseSpec, japaneseSpec, koreanSpec, mongolianSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "中文 12pt。"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "日本語 18pt。"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "한국어 24pt."));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "ᠮᠣᠩᠭᠣᠯ 30pt"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextCjkMultipleFontSizesSameLineTest() {
            String fileName = "verticalTextCjkMultipleFontSizesSameLine";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkMixedFontsTest.CjkTextSpec smallSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("小字", 
                LoadCjkFont(NOTO_SANS_SC), 12).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkMixedFontsTest.CjkTextSpec mediumSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("中字", 
                LoadCjkFont(NOTO_SANS_SC), 24).BackgroundColor(ColorConstants.CYAN);
            VerticalTextCjkMixedFontsTest.CjkTextSpec largeSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("大字", 
                LoadCjkFont(NOTO_SANS_SC), 36).BackgroundColor(ColorConstants.LIGHT_GRAY);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.Add(BuildParagraph(true, smallSpec, mediumSpec, largeSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, smallSpec, mediumSpec, largeSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "小字"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "中字"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "大字"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalTextSimplifiedAndTraditionalChineseSameParagraphTest() {
            String fileName = "verticalTextSimplifiedAndTraditionalChineseSameParagraph";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            VerticalTextCjkMixedFontsTest.CjkTextSpec simplifiedSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec("简体：汉字 国\n"
                , LoadCjkFont(NOTO_SANS_SC), 20).BackgroundColor(ColorConstants.LIGHT_GRAY);
            VerticalTextCjkMixedFontsTest.CjkTextSpec traditionalSpec = new VerticalTextCjkMixedFontsTest.CjkTextSpec(
                "繁體：漢字 國", LoadCjkFont(NOTO_SANS_TC), 20).BackgroundColor(ColorConstants.CYAN);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    document.Add(BuildParagraph(true, simplifiedSpec, traditionalSpec));
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    document.Add(BuildParagraph(false, simplifiedSpec, traditionalSpec));
                }
            }
            IDictionary<char, int?> extractedCounts = VerticalTextTestUtil.ExtractPageCharacterCounts(outFileName);
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "简体：汉字 国"));
            NUnit.Framework.Assert.IsTrue(VerticalTextTestUtil.ContainsAllCharacters(extractedCounts, "繁體：漢字 國"));
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

        private static Paragraph BuildParagraph(bool vertical, params VerticalTextCjkMixedFontsTest.CjkTextSpec[] 
            specs) {
            Paragraph paragraph = vertical ? VerticalParagraph() : HorizontalParagraph();
            foreach (VerticalTextCjkMixedFontsTest.CjkTextSpec spec in specs) {
                Text text = new Text(spec.content);
                text.SetFont(spec.font);
                text.SetFontSize(spec.fontSize);
                if (spec.backgroundColor != null) {
                    text.SetBackgroundColor(spec.backgroundColor);
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

            protected internal CjkTextSpec(String content, PdfFont font, float fontSize) {
                this.content = content;
                this.font = font;
                this.fontSize = fontSize;
            }

            protected internal VerticalTextCjkMixedFontsTest.CjkTextSpec BackgroundColor(Color color) {
                this.backgroundColor = color;
                return this;
            }
        }
    }
}
