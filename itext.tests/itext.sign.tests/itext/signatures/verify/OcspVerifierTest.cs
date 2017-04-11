using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Verify {
    public class OcspVerifierTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        [NUnit.Framework.Test]
        public virtual void ValidOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert);
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        [NUnit.Framework.Test]
        public virtual void InvalidRevokedOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert);
            builder.SetCertificateStatus(new RevokedStatus(DateTimeUtil.GetCurrentUtcTime().AddDays(-20), Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise
                ));
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        [NUnit.Framework.Test]
        public virtual void InvalidUnknownOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert);
            builder.SetCertificateStatus(new UnknownStatus());
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        [NUnit.Framework.Test]
        public virtual void InvalidOutdatedOcspTest01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert);
            DateTime thisUpdate = DateTimeUtil.GetCurrentTime().AddDays(-30);
            DateTime nextUpdate = DateTimeUtil.GetCurrentTime().AddDays(-15);
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        private bool VerifyTest(TestOcspResponseBuilder builder) {
            String caCertFileName = certsSrc + "rootRsa.p12";
            String checkCertFileName = certsSrc + "signCertRsa01.p12";
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            X509Certificate checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(checkCertFileName, password)[
                0];
            TestOcspClient ocspClient = new TestOcspClient(builder, caPrivateKey);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, caCert, null);
            Asn1Object var2 = Asn1Object.FromByteArray(basicOcspRespBytes);
            BasicOcspResp basicOCSPResp = new BasicOcspResp(BasicOcspResponse.GetInstance(var2));
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            return ocspVerifier.Verify(basicOCSPResp, checkCert, caCert, DateTimeUtil.GetCurrentUtcTime());
        }
    }
}
