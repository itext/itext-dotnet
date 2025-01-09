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
using iText.Test;

namespace iText.Kernel.Utils.Objectpathitems {
    [NUnit.Framework.Category("UnitTest")]
    public class ArrayPathItemTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeTest() {
            int index = 1;
            ArrayPathItem arrayPathItem1 = new ArrayPathItem(index);
            ArrayPathItem arrayPathItem2 = new ArrayPathItem(index);
            bool result = arrayPathItem1.Equals(arrayPathItem2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(arrayPathItem1.GetHashCode(), arrayPathItem2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsAndHashCodeTest() {
            ArrayPathItem arrayPathItem1 = new ArrayPathItem(1);
            ArrayPathItem arrayPathItem2 = new ArrayPathItem(2);
            bool result = arrayPathItem1.Equals(arrayPathItem2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(arrayPathItem1.GetHashCode(), arrayPathItem2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void GetIndexTest() {
            int index = 1;
            ArrayPathItem arrayPathItem = new ArrayPathItem(index);
            NUnit.Framework.Assert.AreEqual(index, arrayPathItem.GetIndex());
        }
    }
}
