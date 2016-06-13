using iTextSharp.IO.Util;

namespace iTextSharp.IO {
    public class UtilitiesTest {
        [NUnit.Framework.Test]
        public virtual void TestShortener() {
            byte[] src = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] dest = new byte[] { 1, 2, 3, 4, 5 };
            byte[] test = ArrayUtil.ShortenArray(src, 5);
            NUnit.Framework.Assert.AreEqual(dest, test);
        }
    }
}
