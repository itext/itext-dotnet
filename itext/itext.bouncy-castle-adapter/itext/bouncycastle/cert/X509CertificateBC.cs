using System;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Cert {
    public class X509CertificateBC : IX509Certificate {
        private readonly X509Certificate certificate;

        public X509CertificateBC(X509Certificate certificate) {
            this.certificate = certificate;
        }

        public virtual X509Certificate GetCertificate() {
            return certificate;
        }
        
        public IASN1Encodable GetIssuerDN()
        {
            return new ASN1EncodableBC(certificate.IssuerDN);
        }

        public IBigInteger GetSerialNumber()
        {
            return new BigIntegerBC(certificate.SerialNumber);
        }

        public IPublicKey GetPublicKey()
        {
            return new PublicKeyBC(certificate.GetPublicKey());
        }

        public byte[] GetEncoded()
        {
            return certificate.GetEncoded();
        }

        public byte[] GetTbsCertificate()
        {
            return certificate.GetTbsCertificate();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.X509CertificateBC that = (iText.Bouncycastle.Cert.X509CertificateBC)o;
            return Object.Equals(certificate, that.certificate);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificate);
        }

        public override String ToString() {
            return certificate.ToString();
        }
    }
}
