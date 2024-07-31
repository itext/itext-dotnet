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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation.Extensions {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateExtensionTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/extensions/CertificateExtensionTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        [NUnit.Framework.Test]
        public virtual void KeyUsageNotSetExpectedTest() {
            String certName = certsSrc + "keyUsageNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.KEY_USAGE, null);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageNotSetNotExpectedTest() {
            String certName = certsSrc + "keyUsageNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (98).ToASN1Primitive());
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageWrongOIDTest() {
            String certName = certsSrc + "keyUsageSeveralKeys1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.BASIC_CONSTRAINTS, FACTORY.CreateKeyUsage
                (98).ToASN1Primitive());
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageExpectedValueTest() {
            String certName = certsSrc + "keyUsageSeveralKeys1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (98).ToASN1Primitive());
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsagePartiallyExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (66).ToASN1Primitive());
            // CertificateExtension#existsInCertificate only returns true in case of complete match, therefore false.
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsagePartiallyNotExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsTest() {
            CertificateExtension extension1 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            CertificateExtension extension2 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            NUnit.Framework.Assert.AreEqual(extension1, extension2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsOtherTypeTest() {
            CertificateExtension extension1 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            NUnit.Framework.Assert.AreNotEqual("extension1", extension1);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsOtherExtensionTest() {
            CertificateExtension extension1 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            CertificateExtension extension2 = new CertificateExtension(OID.X509Extensions.EXTENDED_KEY_USAGE, FACTORY.
                CreateKeyUsage(32802).ToASN1Primitive());
            NUnit.Framework.Assert.AreNotEqual(extension1, extension2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsOtherValueTest() {
            CertificateExtension extension1 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            CertificateExtension extension2 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32800).ToASN1Primitive());
            NUnit.Framework.Assert.AreNotEqual(extension1, extension2);
        }

        [NUnit.Framework.Test]
        public virtual void SameHashCode() {
            CertificateExtension extension1 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            CertificateExtension extension2 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            NUnit.Framework.Assert.AreEqual(extension1.GetHashCode(), extension2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashOtherValueTest() {
            CertificateExtension extension1 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            CertificateExtension extension2 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32800).ToASN1Primitive());
            NUnit.Framework.Assert.AreNotEqual(extension1.GetHashCode(), extension2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void HashOtherExtensionTest() {
            CertificateExtension extension1 = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            CertificateExtension extension2 = new CertificateExtension(OID.X509Extensions.EXTENDED_KEY_USAGE, FACTORY.
                CreateKeyUsage(32802).ToASN1Primitive());
            NUnit.Framework.Assert.AreNotEqual(extension1.GetHashCode(), extension2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void GetExtensionValueTest() {
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            NUnit.Framework.Assert.AreEqual(FACTORY.CreateKeyUsage(32802).ToASN1Primitive(), extension.GetExtensionValue
                ());
        }

        [NUnit.Framework.Test]
        public virtual void GetExtensionOidTest() {
            CertificateExtension extension = new CertificateExtension(OID.X509Extensions.KEY_USAGE, FACTORY.CreateKeyUsage
                (32802).ToASN1Primitive());
            NUnit.Framework.Assert.AreEqual(OID.X509Extensions.KEY_USAGE, extension.GetExtensionOid());
        }
    }
}
