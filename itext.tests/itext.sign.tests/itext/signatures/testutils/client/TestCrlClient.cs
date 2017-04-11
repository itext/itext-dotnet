using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestCrlClient : ICrlClient {
        private readonly TestCrlBuilder crlBuilder;

        private readonly ICipherParameters caPrivateKey;

        /// <exception cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>
        public TestCrlClient(TestCrlBuilder crlBuilder, ICipherParameters caPrivateKey) {
            this.crlBuilder = crlBuilder;
            this.caPrivateKey = caPrivateKey;
        }

        /// <exception cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>
        public TestCrlClient(X509Certificate caCert, ICipherParameters caPrivateKey) {
            this.crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
            this.caPrivateKey = caPrivateKey;
        }

        public virtual ICollection<byte[]> GetEncoded(X509Certificate checkCert, String url) {
            ICollection<byte[]> crls = null;
            try {
                byte[] crl = crlBuilder.MakeCrl(caPrivateKey);
                crls = JavaCollectionsUtil.SingletonList(crl);
            }
            catch (Exception) {
            }
            return crls;
        }
    }
}
