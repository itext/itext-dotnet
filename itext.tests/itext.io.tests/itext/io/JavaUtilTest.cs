using System;
using System.Collections.Generic;
using System.Linq;
using iText.IO.Util;
using NUnit.Framework;

namespace iText.IO
{
    public class JavaUtilTest
    {
        [Test]
        public virtual void LinkedHashSetTest()
        {
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
        public virtual void JavaCollectionsUtilTest()
        {
            IList<int> emptyList = JavaCollectionsUtil.EmptyList<int>();
            Assert.IsEmpty(emptyList);
            Assert.Throws<NotSupportedException>(() => emptyList.Add(10));

            IDictionary<int, int> emptyMap = JavaCollectionsUtil.EmptyMap<int, int>();
            Assert.IsEmpty(emptyMap);
            Assert.Throws<NotSupportedException>(() => { emptyMap[5] = 10; });

            IEnumerator<int> emptyIterator = JavaCollectionsUtil.EmptyIterator<int>();
            Assert.False(emptyIterator.MoveNext());

            IList<int> unmodifiableList = JavaCollectionsUtil.UnmodifiableList<int>(new int[] { 10, 20, 30, 20 }.ToList());
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
            Assert.Throws<KeyNotFoundException>(() => { int temp = unodifiableMap[3]; });
            Assert.Throws<NotSupportedException>(() => { unodifiableMap[11] = 11; });

            IList<int> singletonList = JavaCollectionsUtil.SingletonList(4);
            Assert.AreEqual(4, singletonList[0]);
            Assert.Throws<NotSupportedException>(() => singletonList.Add(9));

            List<int> x = new int[] { 60, 50, 20 }.ToList();
            JavaCollectionsUtil.Sort(x);
            Assert.AreEqual(20, x[0]);
            Assert.AreEqual(60, x[2]);

            x = new int[] { -1, 0, 1 }.ToList();
            JavaCollectionsUtil.Reverse(x);
            Assert.AreEqual(1, x[0]);
            Assert.AreEqual(0, x[1]);
        }
    }
}
