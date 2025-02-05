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
using System.Text;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EncodingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/EncodingTest/";

        public static readonly String outputFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/kernel/pdf/EncodingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(outputFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(outputFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SurrogatePairTest() {
            String fileName = "surrogatePairTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "DejaVuSans.ttf", PdfEncodings.IDENTITY_H);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(font, 72).ShowText("\uD835\uDD59\uD835\uDD56\uD835\uDD5D\uD835\uDD5D\uD835\uDD60\uD83D\uDE09\uD835\uDD68"
                 + "\uD835\uDD60\uD835\uDD63\uD835\uDD5D\uD835\uDD55").EndText().RestoreState();
            canvas.Release();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomSimpleEncodingTimesRomanTest() {
            String fileName = "customSimpleEncodingTimesRomanTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "# simple 1 0020 041c 0456 0440 044a 0050 0065 0061 0063"
                , PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 806).SetFontAndSize(font, 12)
                        // Міръ Peace
                        .ShowText("\u041C\u0456\u0440\u044A Peace").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomFullEncodingTimesRomanTest() {
            String fileName = "customFullEncodingTimesRomanTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN, "# full 'A' Aring 0041 'E' Egrave 0045 32 space 0020"
                );
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 806).SetFontAndSize(font, 12).ShowText("A E").EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void NotdefInStandardFontTest() {
            String fileName = "notdefInStandardFontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, "# full 'A' Aring 0041 'E' abc11 0045 32 space 0020"
                );
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36).ShowText("A E").EndText().RestoreState
                ();
            font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.WINANSI);
            canvas.SaveState().BeginText().MoveText(36, 756).SetFontAndSize(font, 36).ShowText("\u0188").EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void NotdefInTrueTypeFontTest() {
            String fileName = "notdefInTrueTypeFontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "# simple 32 0020 00C5 1987", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36).ShowText("\u00C5 \u1987").EndText
                ().RestoreState();
            font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            canvas.SaveState().BeginText().MoveText(36, 756).SetFontAndSize(font, 36).ShowText("\u1987").EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void NotdefInType0Test() {
            String fileName = "notdefInType0Test.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfEncodings.IDENTITY_H);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36).ShowText("\u00C5 \u1987").EndText
                ().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SymbolDefaultFontTest() {
            String fileName = "symbolDefaultFontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.SYMBOL);
            FillSymbolDefaultPage(font, doc.AddNewPage());
            //WinAnsi encoding doesn't support special symbols
            font = PdfFontFactory.CreateFont(StandardFonts.SYMBOL, PdfEncodings.WINANSI);
            FillSymbolDefaultPage(font, doc.AddNewPage());
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        private void FillSymbolDefaultPage(PdfFont font, PdfPage page) {
            PdfCanvas canvas = new PdfCanvas(page);
            StringBuilder builder = new StringBuilder();
            for (int i = 32; i <= 100; i++) {
                builder.Append((char)i);
            }
            canvas.SaveState().BeginText().SetFontAndSize(font, 12).MoveText(36, 806).ShowText(builder.ToString()).EndText
                ().RestoreState();
            builder = new StringBuilder();
            for (int i = 101; i <= 190; i++) {
                builder.Append((char)i);
            }
            canvas.SaveState().BeginText().SetFontAndSize(font, 12).MoveText(36, 786).ShowText(builder.ToString()).EndText
                ();
            builder = new StringBuilder();
            for (int i = 191; i <= 254; i++) {
                builder.Append((char)i);
            }
            canvas.BeginText().MoveText(36, 766).ShowText(builder.ToString()).EndText().RestoreState();
        }

        [NUnit.Framework.Test]
        public virtual void SymbolTrueTypeFontWinAnsiTest() {
            String fileName = "symbolTrueTypeFontWinAnsiTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "Symbols1.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            StringBuilder str = new StringBuilder();
            for (int i = 32; i <= 65; i++) {
                str.Append((char)i);
            }
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36).ShowText(str.ToString()).EndText
                ();
            str = new StringBuilder();
            for (int i = 65; i <= 190; i++) {
                str.Append((char)i);
            }
            canvas.SaveState().BeginText().MoveText(36, 756).SetFontAndSize(font, 36).ShowText(str.ToString()).EndText
                ();
            str = new StringBuilder();
            for (int i = 191; i <= 254; i++) {
                str.Append((char)i);
            }
            canvas.BeginText().MoveText(36, 726).SetFontAndSize(font, 36).ShowText(str.ToString()).EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SymbolTrueTypeFontIdentityTest() {
            String fileName = "symbolTrueTypeFontIdentityTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "Symbols1.ttf", PdfEncodings.IDENTITY_H);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            StringBuilder builder = new StringBuilder();
            for (int i = 32; i <= 100; i++) {
                builder.Append((char)i);
            }
            StringBuilder str = new StringBuilder(builder.ToString());
            canvas.SaveState().BeginText().SetFontAndSize(font, 36).MoveText(36, 786).ShowText(str.ToString()).EndText
                ().RestoreState();
            str = new StringBuilder();
            for (int i = 101; i <= 190; i++) {
                str.Append((char)i);
            }
            canvas.SaveState().BeginText().SetFontAndSize(font, 36).MoveText(36, 746).ShowText(str.ToString()).EndText
                ().RestoreState();
            str = new StringBuilder();
            for (int i = 191; i <= 254; i++) {
                str.Append((char)i);
            }
            canvas.SaveState().BeginText().SetFontAndSize(font, 36).MoveText(36, 766).ShowText(str.ToString()).EndText
                ().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SymbolTrueTypeFontSameCharsIdentityTest() {
            String fileName = "symbolTrueTypeFontSameCharsIdentityTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "Symbols1.ttf", PdfEncodings.IDENTITY_H);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String line = "AABBCCDDEEFFGGHHIIJJ";
            canvas.SaveState().BeginText().SetFontAndSize(font, 36).MoveText(36, 786).ShowText(line).EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void EncodingStreamExtractionTest() {
            String fileName = sourceFolder + "encodingStream01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(fileName));
            String extractedText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1));
            NUnit.Framework.Assert.AreEqual("abc", extractedText);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentCodeSpaceRangeLengthsExtractionTest() {
            String fileName = sourceFolder + "differentCodeSpaceRangeLengths01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(fileName));
            String extractedText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1));
            NUnit.Framework.Assert.AreEqual("Hello\u7121\u540dworld\u6b98\u528d", extractedText);
        }
    }
}
