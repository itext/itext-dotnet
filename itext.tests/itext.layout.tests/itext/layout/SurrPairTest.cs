using System;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class SurrPairTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/SurrPairTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/SurrPairTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SurrogatePairFrom2Chars() {
            String outFileName = destinationFolder + "surrogatePairFrom2Chars.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairFrom2Chars.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoEmoji-Regular.ttf", PdfEncodings.IDENTITY_H);
            //😉
            String winkinkSmile = "\uD83D\uDE09";
            Paragraph paragraph = new Paragraph(winkinkSmile);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SurrogatePair2Pairs() {
            String outFileName = destinationFolder + "surrogatePair2Pairs.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePair2Pairs.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoEmoji-Regular.ttf", PdfEncodings.IDENTITY_H);
            //🇧🇾
            String belarusAbbr = "\uD83C\uDDE7\uD83C\uDDFE";
            Paragraph paragraph = new Paragraph(belarusAbbr);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SurrogatePairFullCharacter() {
            String outFileName = destinationFolder + "surrogatePairFullCharacter.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairFullCharacter.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoEmoji-Regular.ttf", PdfEncodings.IDENTITY_H);
            //🛀
            String em = new String(iText.IO.Util.TextUtil.ToChars(0x0001F6C0));
            Paragraph paragraph = new Paragraph(em);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FONT_SUBSET_ISSUE)]
        public virtual void SurrogatePairCombingFullSurrs() {
            //TODO DEVSIX-3307
            String outFileName = destinationFolder + "surrogatePairCombingFullSurrs.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairCombingFullSurrs.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoColorEmoji.ttf", PdfEncodings.IDENTITY_H);
            //🏴󠁧󠁢󠁥󠁮󠁧󠁿
            String firstPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F3F4));
            String secondPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0067));
            String thirdPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0062));
            String forthPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0065));
            String fifthPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E006E));
            String sixthPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0067));
            String seventhPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E007F));
            String blackFlag = firstPair + secondPair + thirdPair + forthPair + fifthPair + sixthPair + seventhPair;
            Paragraph paragraph = new Paragraph(blackFlag);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FONT_SUBSET_ISSUE)]
        public virtual void SurrogatePairCombingFullSurrsWithNoSurrs() {
            //TODO DEVSIX-3307
            String outFileName = destinationFolder + "surrogatePairCombingFullSurrsWithNoSurrs.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairCombingFullSurrsWithNoSurrs.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoColorEmoji.ttf", PdfEncodings.IDENTITY_H);
            //World Map
            String firstPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F5FA));
            String space = "\u0020";
            //🗽
            String secondPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F5FD));
            //Satellite
            String thirdPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F6F0));
            String allPairs = firstPair + space + secondPair + space + thirdPair;
            Paragraph paragraph = new Paragraph(allPairs);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }
    }
}
