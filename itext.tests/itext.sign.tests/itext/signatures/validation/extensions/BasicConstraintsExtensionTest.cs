using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation.Extensions {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class BasicConstraintsExtensionTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/extensions/BasicConstraintsExtensionTest/";

        [NUnit.Framework.Test]
        public virtual void BasicConstraintNotSetExpectedTest() {
            String certName = certsSrc + "basicConstraintsNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(-2);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintNotSetNotExpectedTest() {
            String certName = certsSrc + "basicConstraintsNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintMaxLengthExpectedTest() {
            String certName = certsSrc + "basicConstraintsMaxCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(true);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintMaxLengthNotExpectedTest() {
            String certName = certsSrc + "basicConstraintsMaxCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(false);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintLength10Test() {
            String certName = certsSrc + "basicConstraints10Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintLength5ExpectedTest() {
            String certName = certsSrc + "basicConstraints5Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(2);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintLength5NotExpectedTest() {
            String certName = certsSrc + "basicConstraints5Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintFalseExpectedTest() {
            String certName = certsSrc + "basicConstraintsFalseCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(false);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintFalseNotExpectedTest() {
            String certName = certsSrc + "basicConstraintsFalseCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }
    }
}
