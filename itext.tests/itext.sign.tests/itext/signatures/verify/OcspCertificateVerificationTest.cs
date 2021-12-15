using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.Signatures;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Signutils;

namespace iText.Signatures.Verify {
    public class OcspCertificateVerificationTest : ExtendedITextTest {
        // Such messageTemplate is equal to any log message. This is required for porting reasons.
        private const String ANY_LOG_MESSAGE = "{0}";

        private static readonly String ocspCertsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/OcspCertificateVerificationTest/";

        private static readonly String rootOcspCert = ocspCertsSrc + "ocspRootRsa.p12";

        private static readonly String signOcspCert = ocspCertsSrc + "ocspSignRsa.p12";

        private static readonly String notOcspAndOcspCert = ocspCertsSrc + "notOcspAndOcspCertificates.p12";

        private static readonly char[] password = "testpass".ToCharArray();

        private const String ocspServiceUrl = "http://localhost:9000/demo/ocsp/ocsp-service";

        private static X509Certificate checkCert;

        private static X509Certificate rootCert;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(signOcspCert, password)[0];
            rootCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(rootOcspCert, password)[0];
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithRootOcspCertificateTest() {
            BasicOcspResp response = GetOcspResponse();
            NUnit.Framework.Assert.IsTrue(CertificateVerification.VerifyOcspCertificates(response, Pkcs12FileHelper.InitStore
                (rootOcspCert, password)));
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithSignOcspCertificateTest() {
            BasicOcspResp response = GetOcspResponse();
            NUnit.Framework.Assert.IsFalse(CertificateVerification.VerifyOcspCertificates(response, Pkcs12FileHelper.InitStore
                (signOcspCert, password)));
        }

        [NUnit.Framework.Test]
        public virtual void KeyStoreWithNotOcspAndOcspCertificatesTest() {
            BasicOcspResp response = GetOcspResponse();
            NUnit.Framework.Assert.IsTrue(CertificateVerification.VerifyOcspCertificates(response, Pkcs12FileHelper.InitStore
                (notOcspAndOcspCert, password)));
        }

        [NUnit.Framework.Test]
        [LogMessage(ANY_LOG_MESSAGE)]
        public virtual void KeyStoreWithNotOcspCertificateTest() {
            NUnit.Framework.Assert.IsFalse(CertificateVerification.VerifyOcspCertificates(null, Pkcs12FileHelper.InitStore
                (signOcspCert, password)));
        }

        private static BasicOcspResp GetOcspResponse() {
            TestOcspClient testClient = new TestOcspClient();
            ICipherParameters key = Pkcs12FileHelper.ReadFirstKey(rootOcspCert, password, password);
            testClient.AddBuilderForCertIssuer(rootCert, key);
            byte[] ocspResponseBytes = testClient.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            Asn1Object var2 = Asn1Object.FromByteArray(ocspResponseBytes);
            return new BasicOcspResp(BasicOcspResponse.GetInstance(var2));
        }
    }
}
