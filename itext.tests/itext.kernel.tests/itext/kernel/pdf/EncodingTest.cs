/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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

        [NUnit.Framework.Test]
        public virtual void SurrogatePairTest() {
            String fileName = "surrogatePairTest.pdf";
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
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
