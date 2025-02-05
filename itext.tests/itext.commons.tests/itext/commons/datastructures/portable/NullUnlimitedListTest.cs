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
using iText.Commons.Datastructures;
using iText.Test;

namespace iText.Commons.Datastructures.Portable {
    [NUnit.Framework.Category("UnitTest")]
    public class NullUnlimitedListTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullUnlimitedListAddTest() {
            NullUnlimitedList<String> list = new NullUnlimitedList<String>();
            list.Add("hey");
            list.Add("bye");
            NUnit.Framework.Assert.AreEqual(2, list.Size());
            list.Add(-1, "hello");
            list.Add(3, "goodbye");
            NUnit.Framework.Assert.AreEqual(2, list.Size());
        }

        [NUnit.Framework.Test]
        public virtual void NullUnlimitedListIndexOfTest() {
            NullUnlimitedList<String> list = new NullUnlimitedList<String>();
            list.Add("hey");
            list.Add(null);
            list.Add("bye");
            list.Add(null);
            NUnit.Framework.Assert.AreEqual(4, list.Size());
            NUnit.Framework.Assert.AreEqual(1, list.IndexOf(null));
        }

        [NUnit.Framework.Test]
        public virtual void NullUnlimitedListRemoveTest() {
            NullUnlimitedList<String> list = new NullUnlimitedList<String>();
            list.Add("hey");
            list.Add("bye");
            NUnit.Framework.Assert.AreEqual(2, list.Size());
            list.Remove(-1);
            list.Remove(2);
            NUnit.Framework.Assert.AreEqual(2, list.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestIsEmpty() {
            NullUnlimitedList<String> list = new NullUnlimitedList<String>();
            NUnit.Framework.Assert.IsTrue(list.IsEmpty());
            list.Add("hey");
            NUnit.Framework.Assert.IsFalse(list.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour01() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add(null));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(1, list.IndexOf(null)));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour02() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add(null));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(4, list.Size()));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour03() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add(null));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add(null));
            actionList.Add((list) => list.Set(1, "4"));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(list.Get(1), "4"));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour04() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add(null));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add(null));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(1, list.IndexOf(null)));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour05() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(-1, list.IndexOf(null)));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour06() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("2"));
            actionList.Add((list) => list.Add("3"));
            actionList.Add((list) => list.Add("4"));
            actionList.Add((list) => list.Add("5"));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(4, list.IndexOf("5")));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour07() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("2"));
            actionList.Add((list) => list.Add("3"));
            actionList.Add((list) => list.Add("4"));
            actionList.Add((list) => list.Add("5"));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(-1, list.IndexOf("6")));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour08() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("2"));
            actionList.Add((list) => list.Add("3"));
            actionList.Add((list) => list.Add("4"));
            actionList.Add((list) => list.Add(2, "5"));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(5, list.Size()));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour09() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("2"));
            actionList.Add((list) => list.Add("3"));
            actionList.Add((list) => list.Add("4"));
            actionList.Add((list) => list.Set(2, null));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(4, list.Size()));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour10() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => list.Add("2"));
            actionList.Add((list) => list.Add("3"));
            actionList.Add((list) => list.Add("4"));
            actionList.Add((list) => list.Remove(2));
            actionList.Add((list) => NUnit.Framework.Assert.AreEqual(3, list.Size()));
            ExecuteActions(actionList);
        }

        [NUnit.Framework.Test]
        public virtual void TestSameBehaviour11() {
            IList<Action<ISimpleList<String>>> actionList = new List<Action<ISimpleList<String>>>();
            actionList.Add((list) => NUnit.Framework.Assert.IsTrue(list.IsEmpty()));
            actionList.Add((list) => list.Add("1"));
            actionList.Add((list) => NUnit.Framework.Assert.IsFalse(list.IsEmpty()));
            actionList.Add((list) => list.Add("2"));
            actionList.Add((list) => list.Add("3"));
            actionList.Add((list) => list.Add("4"));
            actionList.Add((list) => NUnit.Framework.Assert.IsFalse(list.IsEmpty()));
            ExecuteActions(actionList);
        }

        public virtual void ExecuteActions(IList<Action<ISimpleList<String>>> actionList) {
            NullUnlimitedList<String> list = new NullUnlimitedList<String>();
            SimpleArrayList<String> list2 = new SimpleArrayList<String>();
            foreach (Action<ISimpleList<String>> action in actionList) {
                action(list);
                action(list2);
            }
        }
    }
}
