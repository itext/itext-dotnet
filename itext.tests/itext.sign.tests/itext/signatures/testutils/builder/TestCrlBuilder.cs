using System;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using Org.BouncyCastle.Crypto.Operators;

namespace iText.Signatures.Testutils.Builder {
    public class TestCrlBuilder {
        private const String SIGN_ALG = "SHA256withRSA";

        private X509V2CrlGenerator crlBuilder;

        private DateTime nextUpdate = DateTimeUtil.GetCurrentUtcTime().AddDays(30);

        /// <exception cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>
        public TestCrlBuilder(X509Certificate caCert, DateTime thisUpdate) {
            X509Name issuerDN = caCert.IssuerDN;
            crlBuilder = new X509V2CrlGenerator();
            crlBuilder.SetIssuerDN(issuerDN);
            crlBuilder.SetThisUpdate(thisUpdate);
        }

        public virtual void SetNextUpdate(DateTime nextUpdate) {
            this.nextUpdate = nextUpdate;
        }

        /// <summary>See CRLReason</summary>
        public virtual void AddCrlEntry(X509Certificate certificate, DateTime revocationDate, int reason) {
            crlBuilder.AddCrlEntry(certificate.SerialNumber, revocationDate, reason);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        public virtual byte[] MakeCrl(ICipherParameters caPrivateKey) {
            crlBuilder.SetNextUpdate(nextUpdate);
            X509Crl crl = crlBuilder.Generate(new Asn1SignatureFactory(SIGN_ALG, (AsymmetricKeyParameter) caPrivateKey));
            crlBuilder = null;
            return crl.GetEncoded();
        }
    }
}
