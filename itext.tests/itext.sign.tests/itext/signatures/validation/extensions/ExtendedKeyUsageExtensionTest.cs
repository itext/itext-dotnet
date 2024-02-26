using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation.Extensions {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class ExtendedKeyUsageExtensionTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/extensions/ExtendedKeyUsageExtensionTest/";

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageNotSetExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageNoSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaCollectionsUtil.EmptyList<String>(
                ));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageNotSetNotExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageNoSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList(ExtendedKeyUsageExtension
                .TIME_STAMPING));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageTimestampingExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageTimeStampingCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList(ExtendedKeyUsageExtension
                .TIME_STAMPING));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageTimestampingNotExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageTimeStampingCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList(ExtendedKeyUsageExtension
                .OCSP_SIGNING));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageOcspSigningExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageOcspSigningCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList(ExtendedKeyUsageExtension
                .OCSP_SIGNING));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageOcspSigningNotExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageOcspSigningCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList(ExtendedKeyUsageExtension
                .CODE_SIGNING));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageAnyUsageTest1() {
            String certName = certsSrc + "extendedKeyUsageAnyUsageCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaCollectionsUtil.SingletonList(ExtendedKeyUsageExtension
                .CODE_SIGNING));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageAnyUsageTest2() {
            String certName = certsSrc + "extendedKeyUsageAnyUsageCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaUtil.ArraysAsList(ExtendedKeyUsageExtension
                .CODE_SIGNING, ExtendedKeyUsageExtension.OCSP_SIGNING));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageSeveralValues1PartiallyExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageSeveralValues1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaUtil.ArraysAsList(ExtendedKeyUsageExtension
                .TIME_STAMPING, ExtendedKeyUsageExtension.OCSP_SIGNING));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageSeveralValues1PartiallyNotExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageSeveralValues1Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaUtil.ArraysAsList(ExtendedKeyUsageExtension
                .TIME_STAMPING, ExtendedKeyUsageExtension.ANY_EXTENDED_KEY_USAGE_OID));
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageSeveralValues2PartiallyExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageSeveralValues2Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaUtil.ArraysAsList(ExtendedKeyUsageExtension
                .OCSP_SIGNING, ExtendedKeyUsageExtension.CLIENT_AUTH));
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedKeyUsageSeveralValues2PartiallyNotExpectedTest() {
            String certName = certsSrc + "extendedKeyUsageSeveralValues2Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            ExtendedKeyUsageExtension extension = new ExtendedKeyUsageExtension(JavaUtil.ArraysAsList(ExtendedKeyUsageExtension
                .CODE_SIGNING, ExtendedKeyUsageExtension.CLIENT_AUTH));
            // Certificate contains any_extended_key_usage OID, that's why results is always true.
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }
    }
}
