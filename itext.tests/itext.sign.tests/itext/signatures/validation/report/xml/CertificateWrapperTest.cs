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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;

namespace iText.Signatures.Validation.Report.Xml {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateWrapperTest : AbstractCollectableObjectTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static IX509Certificate cert1;

        private static IX509Certificate cert2;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpFixture() {
            cert1 = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "root.pem")[0];
            cert2 = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertDsa01.pem")[0];
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualInstancesHaveUniqueIds() {
            CertificateWrapper sut1 = new CertificateWrapper(cert1);
            CertificateWrapper sut2 = new CertificateWrapper(cert1);
            NUnit.Framework.Assert.AreNotEqual(sut1.GetIdentifier().GetId(), sut2.GetIdentifier().GetId());
        }

        protected internal override void PerformTestHashForEqualInstances() {
            CertificateWrapper sut1 = new CertificateWrapper(cert1);
            CertificateWrapper sut2 = new CertificateWrapper(cert1);
            NUnit.Framework.Assert.AreEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        protected internal override void PerformTestEqualsForEqualInstances() {
            CertificateWrapper sut1 = new CertificateWrapper(cert1);
            CertificateWrapper sut2 = new CertificateWrapper(cert1);
            NUnit.Framework.Assert.AreEqual(sut1, sut2);
        }

        protected internal override void PerformTestEqualsForDifferentInstances() {
            CertificateWrapper sut1 = new CertificateWrapper(cert1);
            CertificateWrapper sut2 = new CertificateWrapper(cert2);
            NUnit.Framework.Assert.AreNotEqual(sut1, sut2);
        }

        protected internal override void PerformTestHashForDifferentInstances() {
            CertificateWrapper sut1 = new CertificateWrapper(cert1);
            CertificateWrapper sut2 = new CertificateWrapper(cert2);
            NUnit.Framework.Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetBase64ASN1Structure() {
            CertificateWrapper sut = new CertificateWrapper(cert1);
            IX509Certificate sutCert = FACTORY.CreateX509Certificate(Convert.FromBase64String(sut.GetBase64ASN1Structure
                ()));
            IX509Certificate origCert = FACTORY.CreateX509Certificate(cert1.GetEncoded());
            NUnit.Framework.Assert.AreEqual(origCert, sutCert);
        }

//\cond DO_NOT_DOCUMENT
        internal override AbstractCollectableObject GetCollectableObjectUnderTest() {
            return new CertificateWrapper(cert1);
        }
//\endcond
    }
}
