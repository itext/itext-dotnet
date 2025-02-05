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
namespace iText.Signatures.Validation.Report.Xml {
    public abstract class AbstractCollectableObjectTest : AbstractIdentifiableObjectTest {
        private AbstractCollectableObjectTest.MockCollectableObjectVisitor mockVisitor;

        [NUnit.Framework.SetUp]
        public virtual void SetUpParent() {
            mockVisitor = new AbstractCollectableObjectTest.MockCollectableObjectVisitor();
        }

        [NUnit.Framework.Test]
        public virtual void TestVisitorUsage() {
            AbstractCollectableObject sut = GetCollectableObjectUnderTest();
            sut.Accept(mockVisitor);
            NUnit.Framework.Assert.AreEqual(1, mockVisitor.calls);
        }

//\cond DO_NOT_DOCUMENT
        internal override AbstractIdentifiableObject GetIdentifiableObjectUnderTest() {
            return GetCollectableObjectUnderTest();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract AbstractCollectableObject GetCollectableObjectUnderTest();
//\endcond

        private class MockCollectableObjectVisitor : CollectableObjectVisitor {
            public int calls;

            public virtual void Visit(CertificateWrapper certificateWrapper) {
                calls++;
            }

            public virtual void Visit(POEValidationReport poeValidationReport) {
                calls++;
            }
        }
    }
}
