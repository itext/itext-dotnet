using System;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Font {
    public class TrueTypeFontTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/TrueTypeFontTest/";

        [NUnit.Framework.Test]
        public virtual void NotoSansJpCmapTest() {
            // 信
            char jpChar = '\u4FE1';
            FontProgram fontProgram = FontProgramFactory.CreateFont(sourceFolder + "NotoSansJP-Regular.otf");
            Glyph glyph = fontProgram.GetGlyph(jpChar);
            NUnit.Framework.Assert.AreEqual(new char[] { jpChar }, glyph.GetUnicodeChars());
            NUnit.Framework.Assert.AreEqual(20449, glyph.GetUnicode());
            // TODO DEVSIX-5767 actual expected value is 0x27d3
            NUnit.Framework.Assert.AreEqual(0x0a72, glyph.GetCode());
        }
    }
}
