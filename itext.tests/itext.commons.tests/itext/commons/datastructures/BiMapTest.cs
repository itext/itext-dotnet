using System;

namespace iText.Commons.Datastructures {
    [NUnit.Framework.Category("UnitTest")]
    public class BiMapTest {
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
