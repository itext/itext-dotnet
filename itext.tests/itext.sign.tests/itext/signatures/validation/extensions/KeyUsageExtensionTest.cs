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
