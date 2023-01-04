/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Test;

namespace iText.IO.Util {
    [NUnit.Framework.Category("UnitTest")]
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
