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
using iText.Commons.Utils;
using iText.Signatures.Cms;
using iText.Signatures.Testutils;

namespace iText.Signatures.Validation.Report.Xml {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignatureValidationReportTest : AbstractIdentifiableObjectTest {
        private static CMSContainer signature1;

        private static CMSContainer signature2;

        private readonly ValidationObjects validationObjects = new ValidationObjects();

        [NUnit.Framework.OneTimeSetUp]
        public static void SetupFixture() {
            signature1 = new CMSContainer(Convert.FromBase64String(XmlReportTestHelper.SIGNATURE1_BASE64));
            signature2 = new CMSContainer(Convert.FromBase64String(XmlReportTestHelper.SIGNATURE2_BASE64));
        }

        [NUnit.Framework.Test]
        public virtual void TestCreation() {
            SignatureValidationReport sut = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.IsNotNull(sut);
        }

        [NUnit.Framework.Test]
        public virtual void TestSetSignatureValidationStatus() {
            SignatureValidationReport sut = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            SignatureValidationStatus status = new SignatureValidationStatus();
            sut.SetSignatureValidationStatus(status);
            NUnit.Framework.Assert.AreEqual(status, sut.GetSignatureValidationStatus());
        }

        protected internal override void PerformTestHashForEqualInstances() {
            SignatureValidationReport sut1 = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            SignatureValidationReport sut2 = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        protected internal override void PerformTestEqualsForEqualInstances() {
            SignatureValidationReport sut1 = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            SignatureValidationReport sut2 = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(sut1, sut2);
        }

        protected internal override void PerformTestEqualsForDifferentInstances() {
            SignatureValidationReport sut1 = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            SignatureValidationReport sut2 = new SignatureValidationReport(validationObjects, signature2, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreNotEqual(sut1, sut2);
        }

        protected internal override void PerformTestHashForDifferentInstances() {
            SignatureValidationReport sut1 = new SignatureValidationReport(validationObjects, signature1, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            SignatureValidationReport sut2 = new SignatureValidationReport(validationObjects, signature2, "signatureName"
                , TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

//\cond DO_NOT_DOCUMENT
        internal override AbstractIdentifiableObject GetIdentifiableObjectUnderTest() {
            return new SignatureValidationReport(validationObjects, signature1, "signatureName", TimeTestUtil.TEST_DATE_TIME
                );
        }
//\endcond
    }
}
