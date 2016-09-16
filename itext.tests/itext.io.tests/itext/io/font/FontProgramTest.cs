using System;

namespace iText.IO.Font {
    public class FontProgramTest {
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ExceptionMessageTest() {
            String font = "some-font.ttf";
            try {
                FontProgramFactory.CreateFont(font);
            }
            catch (iText.IO.IOException ex) {
                NUnit.Framework.Assert.AreEqual(String.Format(iText.IO.IOException.FontFile1NotFound, font), ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void BoldTest() {
            FontProgram fp = FontProgramFactory.CreateFont(FontConstants.HELVETICA);
            fp.SetBold(true);
            NUnit.Framework.Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) != 0, "Bold expected");
            fp.SetBold(false);
            NUnit.Framework.Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) == 0, "Not Bold expected");
        }
    }
}
