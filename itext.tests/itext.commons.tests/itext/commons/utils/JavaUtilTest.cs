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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace iText.Commons.Utils {
    public class JavaUtilTest {
        [Test]
        public virtual void LinkedHashSetTest() {
            LinkedHashSet<int> set1 = new LinkedHashSet<int>();
            set1.Add(5);
            set1.Add(2);
            set1.Add(3);
            set1.Add(5);

            Assert.AreEqual(5, set1.First());
            Assert.AreEqual(3, set1.Last());
            Assert.AreEqual(3, set1.Count);

            LinkedHashSet<int> set2 = new LinkedHashSet<int>();
            set2.Add(2);
            set2.Add(5);

            Assert.True(set1.IsSupersetOf(set2));
            Assert.True(set1.IsProperSupersetOf(set2));
            Assert.True(set2.IsSubsetOf(set1));
            Assert.True(set2.IsProperSubsetOf(set1));

            Assert.False(set2.IsSupersetOf(set1));

            LinkedHashSet<int> set3 = new LinkedHashSet<int>();
            set3.Add(5);
            set3.Add(2);

            Assert.True(set3.SetEquals(set2));
        }

        [Test]
        public virtual void JavaCollectionsUtilTest() {
            IList<int> emptyList = JavaCollectionsUtil.EmptyList<int>();
            Assert.IsEmpty(emptyList);
            Assert.Throws<NotSupportedException>(() => emptyList.Add(10));

            IDictionary<int, int> emptyMap = JavaCollectionsUtil.EmptyMap<int, int>();
            Assert.IsEmpty(emptyMap);
            Assert.Throws<NotSupportedException>(() => { emptyMap[5] = 10; });

            IEnumerator<int> emptyIterator = JavaCollectionsUtil.EmptyIterator<int>();
            Assert.False(emptyIterator.MoveNext());

            IList<int> unmodifiableList =
                JavaCollectionsUtil.UnmodifiableList<int>(new int[] {10, 20, 30, 20}.ToList());
            Assert.Throws<NotSupportedException>(() => unmodifiableList.Insert(0, 20));
            Assert.Throws<NotSupportedException>(() => { unmodifiableList[2] = 50; });
            int test = unmodifiableList[3];
            Assert.Throws<NotSupportedException>(() => JavaCollectionsUtil.Sort(unmodifiableList));

            IDictionary<int, int> unodifiableMap = JavaCollectionsUtil.UnmodifiableMap(new Dictionary<int, int>() {
                {1, 20},
                {2, 40},
                {70, 80},
            });
            test = unodifiableMap[2];
            Assert.Throws<KeyNotFoundException>(() => {
                int temp = unodifiableMap[3];
            });
            Assert.Throws<NotSupportedException>(() => { unodifiableMap[11] = 11; });

            IList<int> singletonList = JavaCollectionsUtil.SingletonList(4);
            Assert.AreEqual(4, singletonList[0]);
            Assert.Throws<NotSupportedException>(() => singletonList.Add(9));

            List<int> x = new int[] {60, 50, 20}.ToList();
            JavaCollectionsUtil.Sort(x);
            Assert.AreEqual(20, x[0]);
            Assert.AreEqual(60, x[2]);

            x = new int[] {-1, 0, 1}.ToList();
            JavaCollectionsUtil.Reverse(x);
            Assert.AreEqual(1, x[0]);
            Assert.AreEqual(0, x[1]);
        }
        
        [Test]
        public virtual void JavaUtilFillBytesTest() {
            byte[] bytes = new byte[5];
            byte fillVal = 1;
            JavaUtil.Fill(bytes, fillVal);
            foreach (byte b in bytes) {
                Assert.AreEqual(fillVal, b);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillCharsTest() {
            char[] chars = new char[5];
            char fillVal = 'a';
            JavaUtil.Fill(chars, fillVal);
            foreach (char c in chars) {
                Assert.AreEqual(fillVal, c);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillBoolTest() {
            bool[] bools = new bool[5];
            bool fillVal = true;
            JavaUtil.Fill(bools, fillVal);
            foreach (bool b in bools) {
                Assert.AreEqual(fillVal, b);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillShortTest() {
            short[] shorts = new short[5];
            short fillVal = 1;
            JavaUtil.Fill(shorts, fillVal);
            foreach (short b in shorts) {
                Assert.AreEqual(fillVal, b);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillIntTest() {
            int[] ints = new int[5];
            int fillVal = 1;
            JavaUtil.Fill(ints, fillVal);
            foreach (int b in ints) {
                Assert.AreEqual(fillVal, b);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillLongTest() {
            long[] longs = new long[5];
            long fillVal = 1;
            JavaUtil.Fill(longs, fillVal);
            foreach (long b in longs) {
                Assert.AreEqual(fillVal, b);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillFloatTest() {
            float[] floats = new float[5];
            float fillVal = 1;
            JavaUtil.Fill(floats, fillVal);
            foreach (float b in floats) {
                Assert.AreEqual(fillVal, b);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillDoubleTest() {
            double[] doubles = new double[5];
            double fillVal = 1;
            JavaUtil.Fill(doubles, fillVal);
            foreach (double d in doubles) {
                Assert.AreEqual(fillVal, d);
            }
        }
        
        [Test]
        public virtual void JavaUtilFillObjectTest() {
            string[] strings = new string[5];
            string fillVal = "hello";
            JavaUtil.Fill(strings, fillVal);
            foreach (string s in strings) {
                Assert.AreEqual(fillVal, s);
            }
        }
    }
}
