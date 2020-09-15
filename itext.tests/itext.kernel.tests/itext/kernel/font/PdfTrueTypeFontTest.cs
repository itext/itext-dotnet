using System;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    public class PdfTrueTypeFontTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/PdfTrueTypeFontTest/";

        [NUnit.Framework.Test]
        public virtual void TestReadingPdfTrueTypeFontWithType1StandardFontProgram() {
            String filePath = SOURCE_FOLDER + "trueTypeFontWithStandardFontProgram.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(filePath));
            PdfDictionary fontDict = pdfDocument.GetPage(1).GetResources().GetResource(PdfName.Font).GetAsDictionary(new 
                PdfName("F1"));
            PdfFont pdfFont = PdfFontFactory.CreateFont(fontDict);
            NUnit.Framework.Assert.AreEqual(542, pdfFont.GetFontProgram().GetAvgWidth());
            NUnit.Framework.Assert.AreEqual(556, pdfFont.GetGlyph('a').GetWidth());
        }
    }
}
