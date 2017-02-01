using System.Collections.Generic;

namespace iText.Layout.Renderer {
    public class TextFilteringReversedRangesTest {
        [NUnit.Framework.Test]
        public virtual void Test01() {
            List<int> removedIds = new List<int>();
            removedIds.Add(0);
            int[] range = new int[] { 0, 1 };
            TextRenderer.UpdateRangeBasedOnRemovedCharacters(removedIds, range);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 0 }, range);
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            List<int> removedIds = new List<int>();
            removedIds.Add(10);
            int[] range = new int[] { 0, 5 };
            TextRenderer.UpdateRangeBasedOnRemovedCharacters(removedIds, range);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 5 }, range);
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            List<int> removedIds = new List<int>();
            removedIds.Add(0);
            removedIds.Add(3);
            removedIds.Add(10);
            int[] range = new int[] { 0, 5 };
            TextRenderer.UpdateRangeBasedOnRemovedCharacters(removedIds, range);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 3 }, range);
        }
    }
}
