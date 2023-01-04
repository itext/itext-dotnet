/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
