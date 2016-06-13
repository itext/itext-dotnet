namespace iText.IO.Font {
    public class FontProgramTest {
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ExceptionMessageTest() {
            try {
                FontProgramFactory.CreateFont("some-font.ttf");
            }
            catch (iText.IO.IOException ex) {
                NUnit.Framework.Assert.AreEqual("font.file some-font.ttf not.found", ex.Message);
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
