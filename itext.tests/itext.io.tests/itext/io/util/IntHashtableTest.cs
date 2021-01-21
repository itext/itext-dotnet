using iText.Test;

namespace iText.IO.Util {
    public class IntHashtableTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CloneTest() {
            IntHashtable hashtable = FillTable();
            IntHashtable clonedTable = (IntHashtable)hashtable.Clone();
            int[] keysArray = hashtable.GetKeys();
            int[] clonedKeysArray = clonedTable.GetKeys();
            NUnit.Framework.Assert.AreEqual(keysArray.Length, clonedKeysArray.Length);
            for (int i = 0; i < keysArray.Length; i++) {
                NUnit.Framework.Assert.AreEqual(keysArray[i], clonedKeysArray[i]);
                NUnit.Framework.Assert.AreEqual(hashtable.Get(keysArray[i]), clonedTable.Get(clonedKeysArray[i]));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CountIsEqualTest() {
            IntHashtable hashtable = FillTable();
            IntHashtable clonedTable = (IntHashtable)hashtable.Clone();
            NUnit.Framework.Assert.AreEqual(hashtable.count, clonedTable.count);
        }

        private static IntHashtable FillTable() {
            IntHashtable hashtable = new IntHashtable();
            hashtable.Put(1, 0);
            hashtable.Put(0, 1);
            hashtable.Put(-1, 2);
            hashtable.Put(2, -1);
            return hashtable;
        }
    }
}
