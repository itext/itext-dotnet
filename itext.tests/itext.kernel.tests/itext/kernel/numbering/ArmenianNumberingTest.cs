using iText.Test;

namespace iText.Kernel.Numbering {
    public class ArmenianNumberingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NegativeToArmenianTest() {
            NUnit.Framework.Assert.AreEqual("", ArmenianNumbering.ToArmenian(-10));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroToArmenianTest() {
            NUnit.Framework.Assert.AreEqual("", ArmenianNumbering.ToArmenian(0));
        }

        [NUnit.Framework.Test]
        public virtual void ToArmenianTest() {
            NUnit.Framework.Assert.AreEqual("\u0554\u054B\u0542\u0539", ArmenianNumbering.ToArmenian(9999));
            NUnit.Framework.Assert.AreEqual("\u0552\u054A\u0540\u0534", ArmenianNumbering.ToArmenian(7874));
        }

        [NUnit.Framework.Test]
        public virtual void NumberGreaterThan9999toArmenianTest() {
            NUnit.Framework.Assert.AreEqual("\u0554\u0554\u0554\u0554\u0554\u0554\u0554\u0532", ArmenianNumbering.ToArmenian
                (63002));
        }
    }
}
