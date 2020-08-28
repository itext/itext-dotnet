using iText.Test;

namespace iText.Kernel.Numbering {
    public class GeorgianNumberingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NegativeToGeorgianTest() {
            NUnit.Framework.Assert.AreEqual("", GeorgianNumbering.ToGeorgian(-10));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroToGeorgianTest() {
            NUnit.Framework.Assert.AreEqual("", GeorgianNumbering.ToGeorgian(0));
        }

        [NUnit.Framework.Test]
        public virtual void ToGeorgianTest() {
            NUnit.Framework.Assert.AreEqual("\u10F5", GeorgianNumbering.ToGeorgian(10000));
            NUnit.Framework.Assert.AreEqual("\u10F4\u10E8\u10F2\u10D6", GeorgianNumbering.ToGeorgian(7967));
        }

        [NUnit.Framework.Test]
        public virtual void NumberGreaterThan10000toGeorgianTest() {
            NUnit.Framework.Assert.AreEqual("\u10F5\u10F5\u10F5\u10F5\u10F5\u10F5\u10D2", GeorgianNumbering.ToGeorgian
                (60003));
        }
    }
}
