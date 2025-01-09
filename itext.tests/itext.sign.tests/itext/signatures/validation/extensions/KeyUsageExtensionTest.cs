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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation.Extensions {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class KeyUsageExtensionTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/extensions/KeyUsageExtensionTest/";

        [NUnit.Framework.Test]
        public virtual void KeyUsageNotSetExpectedTest() {
            String certName = certsSrc + "keyUsageNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(0);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageNotSetNotExpectedTest() {
            String certName = certsSrc + "keyUsageNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(8);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageNotSetButConfiguredToReturnFalseTest() {
            String certName = certsSrc + "keyUsageNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(8, true);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageKeyCertSignExpectedTest() {
            String certName = certsSrc + "keyUsageKeyCertSignCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(KeyUsage.KEY_CERT_SIGN);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageKeyCertSignPartiallyExpectedTest() {
            String certName = certsSrc + "keyUsageKeyCertSignCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(JavaUtil.ArraysAsList(KeyUsage.KEY_CERT_SIGN, KeyUsage
                .CRL_SIGN));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageKeyCertSignNotExpectedTest() {
            String certName = certsSrc + "keyUsageKeyCertSignCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(KeyUsage.CRL_SIGN);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageDigitalSignatureTest() {
            String certName = certsSrc + "keyUsageDigitalSignatureCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(KeyUsage.DIGITAL_SIGNATURE);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageDecipherOnlyExpectedTest() {
            String certName = certsSrc + "keyUsageDecipherOnlyCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(KeyUsage.DECIPHER_ONLY);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageDecipherOnlyNotExpectedTest() {
            String certName = certsSrc + "keyUsageDecipherOnlyCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(KeyUsage.ENCIPHER_ONLY);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageSeveralKeys1PartiallyExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(JavaUtil.ArraysAsList(KeyUsage.CRL_SIGN, KeyUsage.NON_REPUDIATION
                ));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageSeveralKeys1ExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(JavaUtil.ArraysAsList(KeyUsage.CRL_SIGN, KeyUsage.NON_REPUDIATION
                , KeyUsage.KEY_ENCIPHERMENT));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageSeveralKeys1PartiallyNotExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(JavaUtil.ArraysAsList(KeyUsage.CRL_SIGN, KeyUsage.DECIPHER_ONLY
                , KeyUsage.KEY_ENCIPHERMENT));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageSeveralKeys2PartiallyExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys2Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(JavaUtil.ArraysAsList(KeyUsage.DECIPHER_ONLY, KeyUsage
                .DIGITAL_SIGNATURE));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageSeveralKeys2ExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys2Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(JavaUtil.ArraysAsList(KeyUsage.DECIPHER_ONLY, KeyUsage
                .DIGITAL_SIGNATURE, KeyUsage.KEY_AGREEMENT));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void KeyUsageSeveralKeys2PartiallyNotExpectedTest() {
            String certName = certsSrc + "keyUsageSeveralKeys2Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            KeyUsageExtension extension = new KeyUsageExtension(JavaUtil.ArraysAsList(KeyUsage.CRL_SIGN, KeyUsage.DECIPHER_ONLY
                , KeyUsage.DIGITAL_SIGNATURE));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }
    }
}
