/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
    public class BiMapTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SizeTest01() {
            BiMap<String, int> map = new BiMap<String, int>();
            NUnit.Framework.Assert.AreEqual(0, map.Size());
        }

        [NUnit.Framework.Test]
        public virtual void SizeTest02() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            NUnit.Framework.Assert.AreEqual(1, map.Size());
        }

        [NUnit.Framework.Test]
        public virtual void IsEmptyTest01() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            NUnit.Framework.Assert.IsFalse(map.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void PutTest() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            NUnit.Framework.Assert.AreEqual(1, (int)map.GetByKey("a"));
            NUnit.Framework.Assert.AreEqual("a", map.GetByValue(1));
        }

        [NUnit.Framework.Test]
        public virtual void PutOnExistingKey() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            map.Put("a", 2);
            NUnit.Framework.Assert.AreEqual(2, (int)map.GetByKey("a"));
            NUnit.Framework.Assert.AreEqual("a", map.GetByValue(2));
        }

        [NUnit.Framework.Test]
        public virtual void PutOnExistingValue() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            map.Put("b", 1);
            NUnit.Framework.Assert.AreEqual(1, (int)map.GetByKey("b"));
            NUnit.Framework.Assert.AreEqual("b", map.GetByValue(1));
        }

        [NUnit.Framework.Test]
        public virtual void PutOnExistingKeyAndValue() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            map.Put("a", 1);
            NUnit.Framework.Assert.AreEqual(1, (int)map.GetByKey("a"));
            NUnit.Framework.Assert.AreEqual("a", map.GetByValue(1));
        }

        [NUnit.Framework.Test]
        public virtual void PutMultipleValues() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            map.Put("b", 2);
            map.Put("c", 3);
            NUnit.Framework.Assert.AreEqual(1, (int)map.GetByKey("a"));
            NUnit.Framework.Assert.AreEqual("a", map.GetByValue(1));
            NUnit.Framework.Assert.AreEqual(2, (int)map.GetByKey("b"));
            NUnit.Framework.Assert.AreEqual("b", map.GetByValue(2));
            NUnit.Framework.Assert.AreEqual(3, (int)map.GetByKey("c"));
            NUnit.Framework.Assert.AreEqual("c", map.GetByValue(3));
            NUnit.Framework.Assert.AreEqual(3, map.Size());
        }

        [NUnit.Framework.Test]
        public virtual void ClearTest() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            map.Clear();
            NUnit.Framework.Assert.AreEqual(0, map.Size());
        }

        [NUnit.Framework.Test]
        public virtual void ContainsKeyTest() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            NUnit.Framework.Assert.IsTrue(map.ContainsKey("a"));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsValueTest() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            NUnit.Framework.Assert.IsTrue(map.ContainsValue(1));
        }

        [NUnit.Framework.Test]
        public virtual void GetByValue() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            NUnit.Framework.Assert.AreEqual(1, (int)map.GetByKey("a"));
        }

        [NUnit.Framework.Test]
        public virtual void GetByKey() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            NUnit.Framework.Assert.AreEqual("a", map.GetByValue(1));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveByKey() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            map.RemoveByKey("a");
            NUnit.Framework.Assert.AreEqual(0, map.Size());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveByValue() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.Put("a", 1);
            map.RemoveByValue(1);
            NUnit.Framework.Assert.AreEqual(0, map.Size());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveOnEmptyMap() {
            BiMap<String, int> map = new BiMap<String, int>();
            map.RemoveByKey("a");
            map.RemoveByValue(1);
            NUnit.Framework.Assert.AreEqual(0, map.Size());
        }
    }
}
