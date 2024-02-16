using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Test;

namespace iText.Kernel.Utils.Checkers {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCheckUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckFontAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            NUnit.Framework.Assert.IsTrue(FontCheckUtil.DoesFontContainAllUsedGlyphs("123", font));
        }

        [NUnit.Framework.Test]
        public virtual void CheckFontNotAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            NUnit.Framework.Assert.IsFalse(FontCheckUtil.DoesFontContainAllUsedGlyphs("⫊", font));
        }
    }
}
