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
using iText.Test;

namespace iText.Signatures.Validation.Report.Xml {
    public abstract class AbstractIdentifiableObjectTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestIdentifiersAreUnique() {
            AbstractIdentifiableObject sut1 = new AbstractIdentifiableObjectTest.TestIdentifiableObject("A");
            AbstractIdentifiableObject sut2 = new AbstractIdentifiableObjectTest.TestIdentifiableObject("A");
            NUnit.Framework.Assert.AreNotEqual(sut1.GetIdentifier().GetId(), sut2.GetIdentifier().GetId());
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualsForEqualIdentity() {
            AbstractIdentifiableObject sut1 = GetIdentifiableObjectUnderTest();
            AbstractIdentifiableObject sut2 = sut1;
            // Equals is being tested here.
            NUnit.Framework.Assert.IsTrue(sut1.Equals(sut2));
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualsForNull() {
            AbstractIdentifiableObject sut = GetIdentifiableObjectUnderTest();
            // Equals is being tested here.
            NUnit.Framework.Assert.IsFalse(sut.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualsForSomeObject() {
            AbstractIdentifiableObject sut = GetIdentifiableObjectUnderTest();
            // Equals is being tested here.
            NUnit.Framework.Assert.IsFalse(sut.Equals("Test"));
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualsForEqualInstances() {
            PerformTestEqualsForEqualInstances();
        }

        [NUnit.Framework.Test]
        public virtual void TestHashForEqualInstances() {
            PerformTestHashForEqualInstances();
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualsForDifferentInstances() {
            PerformTestEqualsForDifferentInstances();
        }

        [NUnit.Framework.Test]
        public virtual void TestHashForDifferentInstances() {
            PerformTestHashForDifferentInstances();
        }

        protected internal abstract void PerformTestHashForEqualInstances();

        protected internal abstract void PerformTestEqualsForEqualInstances();

        protected internal abstract void PerformTestEqualsForDifferentInstances();

        protected internal abstract void PerformTestHashForDifferentInstances();

//\cond DO_NOT_DOCUMENT
        internal abstract AbstractIdentifiableObject GetIdentifiableObjectUnderTest();
//\endcond

        private class TestIdentifiableObject : AbstractIdentifiableObject {
            protected internal TestIdentifiableObject(String prefix)
                : base(prefix) {
            }

            public override bool Equals(Object o) {
                return false;
            }

            public override int GetHashCode() {
                return 0;
            }
        }
    }
}
