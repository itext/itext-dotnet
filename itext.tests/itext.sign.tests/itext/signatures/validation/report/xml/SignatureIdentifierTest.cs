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
using iText.Signatures.Cms;
using iText.Signatures.Testutils;

namespace iText.Signatures.Validation.Report.Xml {
    public class SignatureIdentifierTest : AbstractIdentifiableObjectTest {
        protected internal override void PerformTestHashForEqualInstances() {
            AbstractIdentifiableObject sut1 = GetIdentifiableObjectUnderTest();
            AbstractIdentifiableObject sut2 = GetIdentifiableObjectUnderTest();
            // CMS Containers have not equal hashes.
            NUnit.Framework.Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        protected internal override void PerformTestEqualsForEqualInstances() {
            AbstractIdentifiableObject sut1 = GetIdentifiableObjectUnderTest();
            AbstractIdentifiableObject sut2 = GetIdentifiableObjectUnderTest();
            // CMS Containers are not equal.
            NUnit.Framework.Assert.AreNotEqual(sut1, sut2);
        }

        protected internal override void PerformTestEqualsForDifferentInstances() {
            AbstractIdentifiableObject sut1 = GetIdentifiableObjectUnderTest();
            AbstractIdentifiableObject sut2 = new SignatureIdentifier(new ValidationObjects(), new CMSContainer(), "other test"
                , TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreNotEqual(sut1, sut2);
        }

        protected internal override void PerformTestHashForDifferentInstances() {
            AbstractIdentifiableObject sut1 = GetIdentifiableObjectUnderTest();
            AbstractIdentifiableObject sut2 = new SignatureIdentifier(new ValidationObjects(), new CMSContainer(), "other test"
                , TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

//\cond DO_NOT_DOCUMENT
        internal override AbstractIdentifiableObject GetIdentifiableObjectUnderTest() {
            return new SignatureIdentifier(new ValidationObjects(), new CMSContainer(), "test", TimeTestUtil.TEST_DATE_TIME
                );
        }
//\endcond
    }
}
