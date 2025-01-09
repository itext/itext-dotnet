/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using iText.Test;

namespace iText.IO.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class EnumUtilTest : ExtendedITextTest {
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
