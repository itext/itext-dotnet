using System;
using System.Collections.Generic;
using iText.Test;

namespace iText.IO.Util {
    public class MapUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullMapsAreEqualTest() {
            NUnit.Framework.Assert.IsTrue(MapUtil.Equals(null, null));
        }

        [NUnit.Framework.Test]
        public virtual void NullMapIsNotEqualToEmptyMapTest() {
            NUnit.Framework.Assert.IsFalse(MapUtil.Equals(new Dictionary<String, String>(), null));
            NUnit.Framework.Assert.IsFalse(MapUtil.Equals(null, new Dictionary<String, String>()));
        }

        [NUnit.Framework.Test]
        public virtual void MapsOfDifferentTypesAreNotEqualTest() {
            NUnit.Framework.Assert.IsFalse(MapUtil.Equals(new Dictionary<String, String>(), new SortedDictionary<String
                , String>()));
        }

        [NUnit.Framework.Test]
        public virtual void MapsOfDifferentSizeAreNotEqualTest() {
            IDictionary<String, String> m1 = new Dictionary<String, String>();
            m1.Put("m1", "m1");
            IDictionary<String, String> m2 = new Dictionary<String, String>();
            m2.Put("m1", "m1");
            m2.Put("m2", "m2");
            NUnit.Framework.Assert.IsFalse(MapUtil.Equals(m1, m2));
        }

        [NUnit.Framework.Test]
        public virtual void NullValueInMapTest() {
            IDictionary<String, String> m1 = JavaCollectionsUtil.SingletonMap<String, String>("nullKey", null);
            IDictionary<String, String> m2 = JavaCollectionsUtil.SingletonMap("notNullKey", "notNull");
            NUnit.Framework.Assert.IsFalse(MapUtil.Equals(m1, m2));
        }

        [NUnit.Framework.Test]
        public virtual void MapsWithDifferentKeysAreNotEqualTest() {
            IDictionary<String, String> m1 = new Dictionary<String, String>();
            m1.Put("m1", "value");
            IDictionary<String, String> m2 = new Dictionary<String, String>();
            m2.Put("m2", "value");
            NUnit.Framework.Assert.IsFalse(MapUtil.Equals(m1, m2));
        }

        [NUnit.Framework.Test]
        public virtual void MapsWithDifferentValuesAreNotEqualTest() {
            IDictionary<String, String> m1 = new Dictionary<String, String>();
            m1.Put("key", "m1");
            IDictionary<String, String> m2 = new Dictionary<String, String>();
            m2.Put("key", "m2");
            NUnit.Framework.Assert.IsFalse(MapUtil.Equals(m1, m2));
        }

        [NUnit.Framework.Test]
        public virtual void EqualArraysTest() {
            IDictionary<String, String> m1 = new Dictionary<String, String>();
            m1.Put("key", "value");
            IDictionary<String, String> m2 = new Dictionary<String, String>();
            m2.Put("key", "value");
            NUnit.Framework.Assert.IsTrue(MapUtil.Equals(m1, m2));
        }

        [NUnit.Framework.Test]
        public virtual void PutIfNotNullTest() {
            IDictionary<String, String> m1 = new Dictionary<String, String>();
            MapUtil.PutIfNotNull(m1, "key", null);
            NUnit.Framework.Assert.IsTrue(m1.IsEmpty());
            MapUtil.PutIfNotNull(m1, "key", "value");
            NUnit.Framework.Assert.IsFalse(m1.IsEmpty());
            NUnit.Framework.Assert.AreEqual("value", m1.Get("key"));
        }

        [NUnit.Framework.Test]
        public virtual void NullMapsEqualEqualHashCodeTest() {
            NUnit.Framework.Assert.AreEqual(MapUtil.GetHashCode((IDictionary<String, String>)null), MapUtil.GetHashCode
                ((IDictionary<String, String>)null));
        }

        [NUnit.Framework.Test]
        public virtual void NullMapEmptyMapDiffHashCodeTest() {
            NUnit.Framework.Assert.AreEqual(MapUtil.GetHashCode((IDictionary<String, String>)null), MapUtil.GetHashCode
                (new Dictionary<String, String>()));
        }

        [NUnit.Framework.Test]
        public virtual void MapsOfDifferentTypesHashCodeTest() {
            NUnit.Framework.Assert.AreEqual(MapUtil.GetHashCode(new SortedDictionary<Object, Object>()), MapUtil.GetHashCode
                (new Dictionary<String, String>()));
        }

        [NUnit.Framework.Test]
        public virtual void EqualMapsHashCodeTest() {
            IDictionary<String, String> m1 = new Dictionary<String, String>();
            m1.Put("key", "value");
            IDictionary<String, String> m2 = new Dictionary<String, String>();
            m2.Put("key", "value");
            NUnit.Framework.Assert.AreEqual(MapUtil.GetHashCode(m1), MapUtil.GetHashCode(m2));
        }

        [NUnit.Framework.Test]
        public virtual void MapsMergeTest() {
            IDictionary<int, int?> destination = new Dictionary<int, int?>();
            destination.Put(1, 5);
            destination.Put(2, 5);
            destination.Put(4, 5);
            IDictionary<int, int?> source = new Dictionary<int, int?>();
            source.Put(1, 10);
            source.Put(2, 10);
            source.Put(3, 10);
            MapUtil.Merge(destination, JavaCollectionsUtil.UnmodifiableMap(source), (d, s) => d + s);
            IDictionary<int, int?> expectedMap = new Dictionary<int, int?>();
            expectedMap.Put(1, 15);
            expectedMap.Put(2, 15);
            expectedMap.Put(3, 10);
            expectedMap.Put(4, 5);
            NUnit.Framework.Assert.AreEqual(expectedMap, destination);
        }

        [NUnit.Framework.Test]
        public virtual void SameMapsMergeTest() {
            IDictionary<int, int?> map = new Dictionary<int, int?>();
            map.Put(1, 5);
            map.Put(2, 5);
            map.Put(4, 5);
            IDictionary<int, int?> expectedMap = new Dictionary<int, int?>(map);
            MapUtil.Merge(map, map, (d, s) => d + s);
            NUnit.Framework.Assert.AreEqual(expectedMap, map);
        }
    }
}
