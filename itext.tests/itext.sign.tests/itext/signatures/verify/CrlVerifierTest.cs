using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Verify {
    public class CrlVerifierTest : ExtendedITextTest {
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
        public virtual void ValidCrl01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
            NUnit.Framework.Assert.IsTrue(VerifyTest(crlBuilder));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        [NUnit.Framework.Test]
        public virtual void InvalidRevokedCrl01() {
            NUnit.Framework.Assert.That(() =>  {
                X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                    )[0];
                TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
                String checkCertFileName = certsSrc + "signCertRsa01.p12";
                X509Certificate checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(checkCertFileName, password)[
                    0];
                crlBuilder.AddCrlEntry(checkCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-40), Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise
                    );
                VerifyTest(crlBuilder);
            }
            , NUnit.Framework.Throws.TypeOf<VerificationException>());
;
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        [NUnit.Framework.Test]
        public virtual void InvalidOutdatedCrl01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-2));
            crlBuilder.SetNextUpdate(DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
            NUnit.Framework.Assert.IsFalse(VerifyTest(crlBuilder));
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        private bool VerifyTest(TestCrlBuilder crlBuilder) {
            String caCertFileName = certsSrc + "rootRsa.p12";
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            String checkCertFileName = certsSrc + "signCertRsa01.p12";
            X509Certificate checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(checkCertFileName, password)[
                0];
            TestCrlClient crlClient = new TestCrlClient(crlBuilder, caPrivateKey);
            ICollection<byte[]> crlBytesCollection = crlClient.GetEncoded(checkCert, null);
            bool verify = false;
            foreach (byte[] crlBytes in crlBytesCollection) {
                X509Crl crl = (X509Crl)SignTestPortUtil.ParseCrlFromStream(new MemoryStream(crlBytes));
                CRLVerifier verifier = new CRLVerifier(null, null);
                verify = verifier.Verify(crl, checkCert, caCert, DateTimeUtil.GetCurrentUtcTime());
                break;
            }
            return verify;
        }
    }
}
