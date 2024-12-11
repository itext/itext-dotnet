/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Datastructures {
    [NUnit.Framework.Category("UnitTest")]
    public class ConcurrentHashSetTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SizeTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            set.Add("2");
            NUnit.Framework.Assert.AreEqual(2, set.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ContainsKeyTrueTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            set.Add("2");
            set.Add("3");
            NUnit.Framework.Assert.IsTrue(set.Contains("1"));
            NUnit.Framework.Assert.IsTrue(set.Contains("2"));
            NUnit.Framework.Assert.IsTrue(set.Contains("3"));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsKeyFalseTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            NUnit.Framework.Assert.IsFalse(set.Contains("5"));
        }

        [NUnit.Framework.Test]
        public virtual void ClearTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            set.Clear();
            NUnit.Framework.Assert.IsFalse(set.Contains("1"));
        }

        [NUnit.Framework.Test]
        public virtual void AddTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            set.Add("1");
            NUnit.Framework.Assert.AreEqual(1, set.Count);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            set.Add("2");
            set.Remove("1");
            NUnit.Framework.Assert.IsFalse(set.Contains("1"));
        }

        [NUnit.Framework.Test]
        public virtual void ForEachTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            ICollection<String> anotherSet = new HashSet<String>();
            set.Add("1");
            set.Add("2");
            set.Add("3");
            set.ForEach((str) => {
                anotherSet.Add(str);
            }
            );
            NUnit.Framework.Assert.AreEqual(3, anotherSet.Count);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            HashSet<String> anotherSet = new HashSet<String>();
            NUnit.Framework.Assert.IsFalse(set.Equals(anotherSet));
        }

        [NUnit.Framework.Test]
        public virtual void AddAllTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            HashSet<String> anotherSet = new HashSet<String>();
            anotherSet.Add("1");
            anotherSet.Add("2");
            set.AddAll(anotherSet);
            NUnit.Framework.Assert.AreEqual(2, set.Count);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAllTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            HashSet<String> anotherSet = new HashSet<String>();
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => set.RemoveAll(anotherSet));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RetainAllTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            HashSet<String> anotherSet = new HashSet<String>();
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => set.RetainAll(anotherSet));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ContainsAllTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            HashSet<String> anotherSet = new HashSet<String>();
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => set.ContainsAll(anotherSet
                ));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.UNSUPPORTED_OPERATION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeTest() {
            ConcurrentHashSet<String> set = new ConcurrentHashSet<String>();
            set.Add("1");
            HashSet<String> anotherSet = new HashSet<String>();
            anotherSet.Add("2");
            NUnit.Framework.Assert.AreNotEqual(set.GetHashCode(), JavaUtil.SetHashCode(anotherSet));
        }
    }
}
