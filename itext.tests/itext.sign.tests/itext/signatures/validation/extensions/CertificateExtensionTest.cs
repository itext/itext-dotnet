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
    }
}
