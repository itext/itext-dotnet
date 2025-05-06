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
using System;
using iText.Test;

namespace iText.Commons.Datastructures {
    [NUnit.Framework.Category("UnitTest")]
    public class Tuple2Test : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestTuple2_StringInt() {
            Tuple2<String, int> tuple = new Tuple2<String, int>("test", 1);
            NUnit.Framework.Assert.AreEqual("test", tuple.GetFirst());
            NUnit.Framework.Assert.AreEqual(Convert.ToInt32(1), tuple.GetSecond());
        }

        [NUnit.Framework.Test]
        public virtual void TestTuple2_ToString() {
            Tuple2<String, int> tuple = new Tuple2<String, int>("test", 1);
            NUnit.Framework.Assert.AreEqual("Tuple2{first=test, second=1}", tuple.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestTuple2_TestWithNullFirstValue() {
            Tuple2<String, int> tuple = new Tuple2<String, int>(null, 1);
            NUnit.Framework.Assert.IsNull(tuple.GetFirst());
            NUnit.Framework.Assert.AreEqual(Convert.ToInt32(1), tuple.GetSecond());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsTest() {
            Tuple2<String, int> tuple1 = new Tuple2<String, int>("test", 1);
            Tuple2<String, int> tuple2 = new Tuple2<String, int>("test", 1);
            NUnit.Framework.Assert.AreEqual(tuple1, tuple2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameTest() {
            Tuple2<String, int> tuple = new Tuple2<String, int>("test", 1);
            NUnit.Framework.Assert.AreEqual(tuple, tuple);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsNullTest() {
            Tuple2<String, int> tuple = new Tuple2<String, int>("test", 1);
            NUnit.Framework.Assert.AreNotEqual(tuple, null);
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsTest() {
            Tuple2<String, int> tuple1 = new Tuple2<String, int>("test", 1);
            Tuple2<String, int> tuple2 = new Tuple2<String, int>("test", 2);
            Tuple2<String, int> tuple3 = new Tuple2<String, int>("test2", 2);
            NUnit.Framework.Assert.AreNotEqual(tuple1, tuple2);
            NUnit.Framework.Assert.AreNotEqual(tuple2, tuple3);
            NUnit.Framework.Assert.AreNotEqual(tuple1, tuple3);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWithCustomTest() {
            Tuple2<String, int> tuple1 = new Tuple2<String, int>("test", 1);
            Tuple2<String, int> tuple2 = new Tuple2Test.CustomTuple2<String, int>("test", 1);
            Tuple2<String, int> tuple3 = new Tuple2Test.CustomTuple2<String, int>("test", 1);
            NUnit.Framework.Assert.AreNotEqual(tuple1, tuple2);
            NUnit.Framework.Assert.AreEqual(tuple2, tuple3);
        }

        private class CustomTuple2<T1, T2> : Tuple2<T1, T2> {
            public CustomTuple2(T1 test, T2 i)
                : base(test, i) {
            }
        }
    }
}
