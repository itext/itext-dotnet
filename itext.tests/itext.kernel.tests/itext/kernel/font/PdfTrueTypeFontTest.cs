using System;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    public class PdfTrueTypeFontTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/PdfTrueTypeFontTest/";

        [NUnit.Framework.Test]
        public virtual void TestReadingPdfTrueTypeFontWithType1StandardFontProgram() {
            // We deliberately use an existing PDF in this test and not simplify the test to create the
            // PDF object structure on the fly to be able to easily inspect the PDF with other processors
            String filePath = SOURCE_FOLDER + "trueTypeFontWithStandardFontProgram.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(filePath));
            PdfDictionary fontDict = pdfDocument.GetPage(1).GetResources().GetResource(PdfName.Font).GetAsDictionary(new 
                PdfName("F1"));
            PdfFont pdfFont = PdfFontFactory.CreateFont(fontDict);
            NUnit.Framework.Assert.AreEqual(542, pdfFont.GetFontProgram().GetAvgWidth());
            NUnit.Framework.Assert.AreEqual(556, pdfFont.GetGlyph('a').GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void IsBuiltInTest() {
            PdfFont font = PdfFontFactory.CreateFont(CreateTrueTypeFontDictionaryWithStandardHelveticaFont());
            NUnit.Framework.Assert.IsTrue(font is PdfTrueTypeFont);
            NUnit.Framework.Assert.IsTrue(((PdfTrueTypeFont)font).IsBuiltInFont());
        }

        [NUnit.Framework.Test]
        public virtual void IsNotBuiltInTest() {
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSans-Regular_v.1.8.2.ttf");
            NUnit.Framework.Assert.IsTrue(font is PdfTrueTypeFont);
            NUnit.Framework.Assert.IsFalse(((PdfTrueTypeFont)font).IsBuiltInFont());
        }

        private static PdfDictionary CreateTrueTypeFontDictionaryWithStandardHelveticaFont() {
            PdfDictionary fontDictionary = new PdfDictionary();
            fontDictionary.Put(PdfName.Type, PdfName.Font);
            fontDictionary.Put(PdfName.Subtype, PdfName.TrueType);
            fontDictionary.Put(PdfName.Encoding, PdfName.WinAnsiEncoding);
            fontDictionary.Put(PdfName.BaseFont, new PdfName(StandardFonts.HELVETICA));
            return fontDictionary;
        }
    }
}
