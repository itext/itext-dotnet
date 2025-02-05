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
using iText.Test;

namespace iText.Commons.Datastructures {
    [NUnit.Framework.Category("UnitTest")]
    public class ConcurrentWeakMapTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SizeTest() {
            ConcurrentWeakMap<string, string> map = new ConcurrentWeakMap<string, string>();
            map.Put("5", "6");
            map.Put("3", "0");
            map.Put("6", "2");
            NUnit.Framework.Assert.AreEqual("6", map["5"]);
            NUnit.Framework.Assert.AreEqual("0", map["3"]);
            NUnit.Framework.Assert.AreEqual("2", map["6"]);
        }

        [NUnit.Framework.Test]
        public virtual void ContainsKeyTrueTest() {
            ConcurrentWeakMap<string, string> map = new ConcurrentWeakMap<string, string>();
            map.Put("5", "6");
            NUnit.Framework.Assert.IsTrue(map.ContainsKey("5"));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsKeyFalseTest() {
            ConcurrentWeakMap<string, string> map = new ConcurrentWeakMap<string, string>();
            map.Put("5", "6");
            NUnit.Framework.Assert.IsFalse(map.ContainsKey("6"));
        }

        [NUnit.Framework.Test]
        public virtual void GetTest() {
            ConcurrentWeakMap<string, string> map = new ConcurrentWeakMap<string, string>();
            map.Put("5", "6");
            NUnit.Framework.Assert.AreEqual("6", map.Get("5"));
        }

        [NUnit.Framework.Test]
        public virtual void PutTest() {
            ConcurrentWeakMap<string, string> map = new ConcurrentWeakMap<string, string>();
            map.Put("5", "6");
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () =>
            {
                map.Put("5", "10");
            });
        }

        [NUnit.Framework.Test]
        public virtual void RemoveTest() {
            ConcurrentWeakMap<string, string> map = new ConcurrentWeakMap<string, string>();
            map.Put("5", "6");
            NUnit.Framework.Assert.AreEqual("6", map.JRemove("5"));
        }

        [NUnit.Framework.Test]
        public virtual void PutAllTest() {
            ConcurrentWeakMap<string, string> map = new ConcurrentWeakMap<string, string>();
            map.Put("5", "6");
            IDictionary<string, string> anotherMap = new Dictionary<string,string>();
            anotherMap.Put("5", "10");
            anotherMap.Put("4", "3");
            anotherMap.Put("3", "7");
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () =>
            {
                map.AddAll(anotherMap);
            });
        }
    }
}
