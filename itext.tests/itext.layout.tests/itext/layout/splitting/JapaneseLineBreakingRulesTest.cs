/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout.Splitting {
    [NUnit.Framework.Category("IntegrationTest")]
    public class JapaneseLineBreakingRulesTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/splitting/JapaneseLineBreakingRulesTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/splitting/JapaneseLineBreakingRulesTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        // ---------------- Tests for https://www.w3.org/TR/jlreq/?lang=en#characters_not_starting_a_line section
        [NUnit.Framework.Test]
        public virtual void ClosingBracketsNotStartingLineTest() {
            CreatePdfAndCompare("closingBracketsNotStartingLine", JavaUtil.ArraysAsList("’", "”", ")", "〕", "]", "}", 
                "〉", "》", "」", "』", "】", "⦆", "〙", "〗", "»", "〟"), null);
        }

        [NUnit.Framework.Test]
        public virtual void HyphensNotStartingLineTest() {
            CreatePdfAndCompare("hyphensNotStartingLine", JavaUtil.ArraysAsList("‐", "〜", "゠", "–"), null);
        }

        [NUnit.Framework.Test]
        public virtual void DividingPunctuationMarksNotStartingLineTest() {
            CreatePdfAndCompare("dividingPunctuationMarksNotStartingLine", JavaUtil.ArraysAsList("!", "?", "‼", "⁇", "⁈"
                , "⁉"), null);
        }

        [NUnit.Framework.Test]
        public virtual void MiddleDotsNotStartingLineTest() {
            CreatePdfAndCompare("middleDotsNotStartingLine", JavaUtil.ArraysAsList("・", ":", ";"), null);
        }

        [NUnit.Framework.Test]
        public virtual void FullStopsNotStartingLineTest() {
            CreatePdfAndCompare("fullStopsNotStartingLine", JavaUtil.ArraysAsList("。", "."), null);
        }

        [NUnit.Framework.Test]
        public virtual void CommasNotStartingLineTest() {
            CreatePdfAndCompare("commasNotStartingLine", JavaUtil.ArraysAsList("、", ","), null);
        }

        [NUnit.Framework.Test]
        public virtual void IterationMarksNotStartingLineTest() {
            CreatePdfAndCompare("iterationMarksNotStartingLine", JavaUtil.ArraysAsList("ヽ", "ヾ", "ゝ", "ゞ", "々", "〻"), 
                null);
        }

        [NUnit.Framework.Test]
        public virtual void ProlongedSoundMarkNotStartingLineTest() {
            CreatePdfAndCompare("prolongedSoundMarkNotStartingLine", JavaUtil.ArraysAsList("ー"), null);
        }

        [NUnit.Framework.Test]
        public virtual void SmallKanaNotStartingLineTest() {
            CreatePdfAndCompare("smallKanaNotStartingLine", JavaUtil.ArraysAsList(
                        // ㇷ゚	<31F7, 309A> is missing, because 31F7 is already in the list
                        "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "ァ", "ィ", "ゥ", "ェ", "ォ", "っ", "ゃ", "ゅ", "ょ", "ゎ", "ゕ", "ゖ", "ッ", "ャ", "ュ", "ョ", "ヮ"
                , "ヵ", "ヶ", "ㇰ", "ㇱ", "ㇲ", "ㇳ", "ㇴ", "ㇵ", "ㇶ", "ㇷ", "ㇸ", "ㇹ", "ㇺ", "ㇻ", "ㇼ", "ㇽ", "ㇾ", "ㇿ"), null);
        }

        [NUnit.Framework.Test]
        public virtual void WarichuClosingBracketsNotStartingLineTest() {
            CreatePdfAndCompare("warichuClosingBracketsNotStartingLine", JavaUtil.ArraysAsList(")", "〕", "]"), null);
        }

        // ---------------- Tests for https://www.w3.org/TR/jlreq/?lang=en#characters_not_ending_a_line section
        [NUnit.Framework.Test]
        public virtual void OpeningBracketsNotEndingLineTest() {
            CreatePdfAndCompare("openingBracketsNotEndingLine", null, JavaUtil.ArraysAsList("‘", "“", "(", "〔", "[", "{"
                , "〈", "《", "「", "『", "【", "⦅", "〘", "〖", "«", "〝"));
        }

        [NUnit.Framework.Test]
        public virtual void WarichuOpeningBracketsNotEndingLineTest() {
            CreatePdfAndCompare("warichuOpeningBracketsNotEndingLine", null, JavaUtil.ArraysAsList("(", "〔", "["));
        }

        // ---------------- Tests for https://www.w3.org/TR/jlreq/?lang=en#unbreakable_character_sequences
        [NUnit.Framework.Test]
        public virtual void InseparableCharsSequenceTest() {
            // 5th point from https://www.w3.org/TR/jlreq/?lang=en#notes_a3
            CreatePdfAndCompare("inseparableCharsSequence", JavaUtil.ArraysAsList("—", "…", "‥", "〵", "〵"), JavaUtil.ArraysAsList
                ("—", "…", "‥", "〳", "〴"));
        }

        [NUnit.Framework.Test]
        public virtual void EuropeanNumeralsSequenceTest() {
            // TODO DEVSIX-4863 Layout splitting logic handles negative values incorrectly if they are not in the very beginning of Text element
            CreatePdfAndCompare("europeanNumeralsSequence", JavaUtil.ArraysAsList("2", "9", "9"), JavaUtil.ArraysAsList
                ("1", "-", "+"));
        }

        [NUnit.Framework.Test]
        public virtual void PrefixedAbbreviationsSequenceTest() {
            CreatePdfAndCompare("prefixedAbbreviationsSequence", JavaUtil.ArraysAsList("9", "9", "9", "9", "9", "9"), 
                JavaUtil.ArraysAsList("¥", "$", "£", "#", "€", "№"));
        }

        [NUnit.Framework.Test]
        public virtual void PostfixedAbbreviationsSequenceTest() {
            CreatePdfAndCompare("postfixedAbbreviationsSequence", JavaUtil.ArraysAsList("°", "′", "″", "℃", "¢", "%", 
                "‰", "㏋", "ℓ", "㌃", "㌍", "㌔", "㌘", "㌢", "㌣", "㌦", "㌧", "㌫", "㌶", "㌻", "㍉", "㍊", "㍍", "㍑", "㍗", "㎎", "㎏"
                , "㎜", "㎝", "㎞", "㎡", "㏄"), JavaUtil.ArraysAsList("9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9"
                , "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", 
                "9"));
        }

        [NUnit.Framework.Test]
        public virtual void UnbreakableCharsWithoutSequenceEndedLineTest() {
            CreatePdfAndCompare("unbreakableCharsWithoutSequenceEndedLine", null, JavaUtil.ArraysAsList("—", "…", "‥", 
                "〳", "〴", "¥", "$", "£", "#", "€", "№", "°", "′", "″", "℃", "¢", "%", "‰", "㏋", "ℓ", "㌃", "㌍", "㌔", "㌘"
                , "㌢", "㌣", "㌦", "㌧", "㌫", "㌶", "㌻", "㍉", "㍊", "㍍", "㍑", "㍗", "㎎", "㎏", "㎜", "㎝", "㎞", "㎡", "㏄"));
        }

        [NUnit.Framework.Test]
        public virtual void UnbreakableCharsWithoutSequenceStartedLineTest() {
            CreatePdfAndCompare("unbreakableCharsWithoutSequenceStartedLine", JavaUtil.ArraysAsList("—", "…", "‥", "〵"
                , "〵", "¥", "$", "£", "#", "€", "№", "°", "′", "″", "℃", "¢", "%", "‰", "㏋", "ℓ", "㌃", "㌍", "㌔", "㌘", 
                "㌢", "㌣", "㌦", "㌧", "㌫", "㌶", "㌻", "㍉", "㍊", "㍍", "㍑", "㍗", "㎎", "㎏", "㎜", "㎝", "㎞", "㎡", "㏄"), null);
        }

        private static void CreatePdfAndCompare(String pdfName, IList<String> startLineChars, IList<String> endLineChars
            ) {
            String outFileName = DESTINATION_FOLDER + pdfName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + pdfName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetFont(PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoSansJP-Regular.ttf"));
                    if (startLineChars != null && endLineChars != null) {
                        if (startLineChars.Count != endLineChars.Count) {
                            NUnit.Framework.Assert.Fail("startLineChars size not equal endLineChars size");
                        }
                        for (int i = 0; i < startLineChars.Count; i++) {
                            Div div = CreateDiv(startLineChars[i], endLineChars[i]);
                            document.Add(div);
                        }
                    }
                    else {
                        if (startLineChars != null) {
                            foreach (String startLineChar in startLineChars) {
                                Div div = CreateDiv(startLineChar, null);
                                document.Add(div);
                            }
                        }
                        else {
                            if (endLineChars != null) {
                                foreach (String endLineChar in endLineChars) {
                                    Div div = CreateDiv(null, endLineChar);
                                    document.Add(div);
                                }
                            }
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private static Div CreateDiv(String startLineChar, String endLineChar) {
            Div parentDiv = new Div();
            Div div = new Div();
            div.SetBorder(new SolidBorder(ColorConstants.RED, 1));
            div.SetWidth(159);
            Paragraph p = null;
            if (startLineChar != null && endLineChar != null) {
                parentDiv.Add(new Paragraph("End line char is '" + endLineChar + "', start line char is '" + startLineChar
                     + "'"));
                p = new Paragraph("に関連する主要なステーク" + endLineChar + startLineChar + "以下の項目");
            }
            else {
                if (endLineChar != null && endLineChar.Length == 1) {
                    parentDiv.Add(new Paragraph("End line character for check is '" + endLineChar + "'"));
                    p = new Paragraph("に関連する主要なステーク" + endLineChar + "以下の項目");
                }
                else {
                    if (startLineChar != null && startLineChar.Length == 1) {
                        parentDiv.Add(new Paragraph("Start line character for check is '" + startLineChar + "'"));
                        p = new Paragraph("に関連する主要なステークホ" + startLineChar + "以下の項目");
                    }
                    else {
                        NUnit.Framework.Assert.Fail("Wrong start and/or end line");
                    }
                }
            }
            div.Add(p);
            parentDiv.Add(div);
            parentDiv.SetKeepTogether(true);
            return parentDiv;
        }
    }
}
