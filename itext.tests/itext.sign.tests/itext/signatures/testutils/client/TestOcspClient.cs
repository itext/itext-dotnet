using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestOcspClient : IOcspClient {
        private readonly TestOcspResponseBuilder builder;

        private readonly ICipherParameters caPrivateKey;

        public TestOcspClient(TestOcspResponseBuilder builder, ICipherParameters caPrivateKey) {
            this.builder = builder;
            this.caPrivateKey = caPrivateKey;
        }

        /// <exception cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>
        public TestOcspClient(X509Certificate caCert, ICipherParameters caPrivateKey) {
            this.builder = new TestOcspResponseBuilder(caCert);
            this.caPrivateKey = caPrivateKey;
        }

        public virtual byte[] GetEncoded(X509Certificate checkCert, X509Certificate issuerCert, String url) {
            byte[] bytes = null;
            try {
                CertificateID id = SignTestPortUtil.GenerateCertificateId(issuerCert, checkCert.SerialNumber, Org.BouncyCastle.Ocsp.CertificateID.HashSha1
                    );
                bytes = builder.MakeOcspResponse(SignTestPortUtil.GenerateOcspRequestWithNonce(id).GetEncoded(), caPrivateKey
                    );
            }
            catch (Exception) {
            }
            return bytes;
        }
    }
}
