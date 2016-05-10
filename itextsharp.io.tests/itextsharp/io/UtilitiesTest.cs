using System.Linq;
using NUnit.Framework;
using com.itextpdf.io.util;
using io.itextsharp.io.util;

namespace com.itextpdf.io
{
	public class UtilitiesTest
	{
		[Test]
		public virtual void TestShortener()
		{
			byte[] src = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			byte[] dest = new byte[] { 1, 2, 3, 4, 5 };
			byte[] test = ArrayUtil.ShortenArray(src, 5);
			NUnit.Framework.Assert.AreEqual(dest, test);
		}

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
	}
}
