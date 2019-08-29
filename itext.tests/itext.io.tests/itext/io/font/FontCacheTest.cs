using System;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Font {
    public class FontCacheTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ClearFontCacheTest() {
            String fontName = "FreeSans.ttf";
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(fontName));
            FontProgram fontProgram = new FontCacheTest.FontProgramMock();
            FontCache.SaveFont(fontProgram, fontName);
            NUnit.Framework.Assert.AreEqual(fontProgram, FontCache.GetFont(fontName));
            FontCache.ClearSavedFonts();
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(fontName));
        }

        private class FontProgramMock : FontProgram {
            public override int GetPdfFontFlags() {
                return 0;
            }

            public override int GetKerning(Glyph first, Glyph second) {
                return 0;
            }
        }
    }
}
