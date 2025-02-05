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
using iText.Commons.Datastructures;
using iText.Test;

namespace iText.Commons.Datastructures.Portable {
    [NUnit.Framework.Category("UnitTest")]
    public class SimpleArrayListTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Add01() {
            SimpleArrayList<int> list = new SimpleArrayList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            NUnit.Framework.Assert.AreEqual(3, list.Size());
            NUnit.Framework.Assert.AreEqual(1, list.Get(0));
            NUnit.Framework.Assert.AreEqual(2, list.Get(1));
            NUnit.Framework.Assert.AreEqual(3, list.Get(2));
        }

        [NUnit.Framework.Test]
        public virtual void Add02() {
            SimpleArrayList<int> list = new SimpleArrayList<int>();
            list.Add(1);
            list.Add(3);
            list.Add(2, 2);
            NUnit.Framework.Assert.AreEqual(3, list.Size());
        }

        [NUnit.Framework.Test]
        public virtual void Set01() {
            SimpleArrayList<int> list = new SimpleArrayList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            NUnit.Framework.Assert.AreEqual(2, list.Set(1, 4));
            NUnit.Framework.Assert.AreEqual(4, list.Get(1));
        }

        [NUnit.Framework.Test]
        public virtual void IndexOf01() {
            SimpleArrayList<int> list = new SimpleArrayList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            NUnit.Framework.Assert.AreEqual(0, list.IndexOf(1));
            NUnit.Framework.Assert.AreEqual(1, list.IndexOf(2));
            NUnit.Framework.Assert.AreEqual(2, list.IndexOf(3));
        }

        [NUnit.Framework.Test]
        public virtual void Remove() {
            SimpleArrayList<int> list = new SimpleArrayList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(1);
            NUnit.Framework.Assert.AreEqual(2, list.Size());
            NUnit.Framework.Assert.AreEqual(1, list.Get(0));
            NUnit.Framework.Assert.AreEqual(3, list.Get(1));
        }

        [NUnit.Framework.Test]
        public virtual void Size() {
            SimpleArrayList<int> list = new SimpleArrayList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            NUnit.Framework.Assert.AreEqual(3, list.Size());
        }

        [NUnit.Framework.Test]
        public virtual void InitializeWithCapacity() {
            SimpleArrayList<int> list = new SimpleArrayList<int>(20);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            NUnit.Framework.Assert.AreEqual(3, list.Size());
        }

        [NUnit.Framework.Test]
        public virtual void IsEmpty() {
            SimpleArrayList<int> list = new SimpleArrayList<int>();
            NUnit.Framework.Assert.IsTrue(list.IsEmpty());
            list.Add(1);
            NUnit.Framework.Assert.IsFalse(list.IsEmpty());
        }
    }
}
