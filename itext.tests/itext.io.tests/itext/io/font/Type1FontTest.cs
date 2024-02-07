using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class Type1FontTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FillUsingEncodingTest() {
            FontEncoding fontEncoding = FontEncoding.CreateFontEncoding("WinAnsiEncoding");
            Type1Font type1StdFont = (Type1Font)FontProgramFactory.CreateFont("Helvetica", true);
            NUnit.Framework.Assert.AreEqual(149, type1StdFont.codeToGlyph.Count);
            type1StdFont.InitializeGlyphs(fontEncoding);
            NUnit.Framework.Assert.AreEqual(217, type1StdFont.codeToGlyph.Count);
            NUnit.Framework.Assert.AreEqual(0x2013, type1StdFont.codeToGlyph.Get(150).GetUnicode());
            NUnit.Framework.Assert.AreEqual(new char[] { (char)0x2013 }, type1StdFont.codeToGlyph.Get(150).GetChars());
        }
    }
}
