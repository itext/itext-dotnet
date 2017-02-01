using System;
using System.Text;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class EncodingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/EncodingTest/";

        public static readonly String outputFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/kernel/pdf/EncodingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(outputFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CustomSimpleEncodingTimesRomanTest() {
            String fileName = "customSimpleEncodingTimesRomanTest.pdf";
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "# simple 1 0020 041c 0456 0440 044a 0050 0065 0061 0063"
                , true);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 806).SetFontAndSize(font, 12).ShowText("\u041C\u0456\u0440\u044A Peace"
                ).EndText().RestoreState();
            // Міръ Peace
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CustomFullEncodingTimesRomanTest() {
            String fileName = "customFullEncodingTimesRomanTest.pdf";
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.TIMES_ROMAN, "# full 'A' Aring 0041 'E' Egrave 0045 32 space 0020"
                );
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 806).SetFontAndSize(font, 12).ShowText("A E").EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NotdefInStandardFontTest() {
            String fileName = "notdefInStandardFontTest.pdf";
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.HELVETICA, "# full 'A' Aring 0041 'E' abc11 0045 32 space 0020"
                );
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36).ShowText("A E").EndText().RestoreState
                ();
            font = PdfFontFactory.CreateFont(FontConstants.HELVETICA, PdfEncodings.WINANSI);
            canvas.SaveState().BeginText().MoveText(36, 756).SetFontAndSize(font, 36).ShowText("\u0188").EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NotdefInTrueTypeFontTest() {
            String fileName = "notdefInTrueTypeFontTest.pdf";
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "# simple 32 0020 00C5 1987", true
                );
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36).ShowText("\u00C5 \u1987").EndText
                ().RestoreState();
            font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfEncodings.WINANSI, true);
            canvas.SaveState().BeginText().MoveText(36, 756).SetFontAndSize(font, 36).ShowText("\u1987").EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("Should we update built-in font's descriptor in case not standard font encoding?")]
        public virtual void SymbolDefaultFontTest() {
            String fileName = "symbolDefaultFontTest.pdf";
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.SYMBOL, PdfEncodings.WINANSI);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String str = "";
            for (int i = 32; i <= 100; i++) {
                str += (char)i;
            }
            canvas.SaveState().BeginText().MoveText(36, 806).SetFontAndSize(font, 12).ShowText(str).EndText();
            str = "";
            for (int i = 101; i <= 190; i++) {
                str += (char)i;
            }
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 12).ShowText(str).EndText();
            str = "";
            for (int i = 191; i <= 254; i++) {
                str += (char)i;
            }
            canvas.BeginText().MoveText(36, 766).ShowText(str).EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SymbolTrueTypeFontWinAnsiTest() {
            String fileName = "symbolTrueTypeFontWinAnsiTest.pdf";
            PdfWriter writer = new PdfWriter(outputFolder + fileName);
            PdfDocument doc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "Symbols1.ttf", true);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String str = "";
            for (int i = 32; i <= 65; i++) {
                str += (char)i;
            }
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36).ShowText(str).EndText();
            str = "";
            for (int i = 65; i <= 190; i++) {
                str += (char)i;
            }
            canvas.SaveState().BeginText().MoveText(36, 756).SetFontAndSize(font, 36).ShowText(str).EndText();
            str = "";
            for (int i = 191; i <= 254; i++) {
                str += (char)i;
            }
            canvas.BeginText().MoveText(36, 726).SetFontAndSize(font, 36).ShowText(str).EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
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
            String str = builder.ToString();
            canvas.SaveState().BeginText().SetFontAndSize(font, 36).MoveText(36, 786).ShowText(str).EndText().RestoreState
                ();
            str = "";
            for (int i = 101; i <= 190; i++) {
                str += (char)i;
            }
            canvas.SaveState().BeginText().SetFontAndSize(font, 36).MoveText(36, 746).ShowText(str).EndText().RestoreState
                ();
            str = "";
            for (int i = 191; i <= 254; i++) {
                str += (char)i;
            }
            canvas.SaveState().BeginText().SetFontAndSize(font, 36).MoveText(36, 766).ShowText(str).EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + fileName, sourceFolder + "cmp_"
                 + fileName, outputFolder, "diff_"));
        }
    }
}
