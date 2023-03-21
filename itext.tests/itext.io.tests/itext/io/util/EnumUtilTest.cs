using System.Collections.Generic;

namespace iText.IO.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class EnumUtilTest {
        [NUnit.Framework.Test]
        public virtual void TestEnumUtilSameAmount() {
            NUnit.Framework.Assert.AreEqual(3, EnumUtil.GetAllValuesOfEnum<TestEnum1>().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestEnumUtilSameValues() {
            IList<TestEnum1> list = EnumUtil.GetAllValuesOfEnum<TestEnum1>();
            NUnit.Framework.Assert.IsTrue(list.Contains(TestEnum1.A));
            NUnit.Framework.Assert.IsTrue(list.Contains(TestEnum1.B));
            NUnit.Framework.Assert.IsTrue(list.Contains(TestEnum1.C));
            NUnit.Framework.Assert.AreEqual(TestEnum1.A, list[0]);
            NUnit.Framework.Assert.AreEqual(TestEnum1.B, list[1]);
            NUnit.Framework.Assert.AreEqual(TestEnum1.C, list[2]);
        }
    }

    internal enum TestEnum1 {
        A,
        B,
        C
    }
}
