using iText.IO.Font;

namespace iText.IO {
    public class AdobeGlyphListTest {
        [NUnit.Framework.Test]
        public virtual void TestGlyphListCount() {
            NUnit.Framework.Assert.AreEqual(4200, AdobeGlyphList.GetNameToUnicodeLength());
            NUnit.Framework.Assert.AreEqual(3680, AdobeGlyphList.GetUnicodeToNameLength());
        }
    }
}
