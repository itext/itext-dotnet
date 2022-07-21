using System;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Crypto;
using iText.Bouncycastlefips.Math;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    public class X509CertificateBCFips : IX509Certificate {
        private readonly X509Certificate certificate;

        public X509CertificateBCFips(X509Certificate certificate) {
            this.certificate = certificate;
        }

        public X509CertificateBCFips(byte[] bytes) {
            this.certificate = new X509Certificate(bytes);
        }

        public virtual X509Certificate GetCertificate() {
            return certificate;
        }

        public IASN1Encodable GetIssuerDN()
        {
            return new ASN1EncodableBCFips(certificate.IssuerDN);
        }

        public IBigInteger GetSerialNumber()
        {
            return new BigIntegerBCFips(certificate.SerialNumber);
        }

        public IPublicKey GetPublicKey()
        {
            return new PublicKeyBCFips(certificate.GetPublicKey());
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
            iText.Bouncycastlefips.Cert.X509CertificateBCFips that = (iText.Bouncycastlefips.Cert.X509CertificateBCFips
                )o;
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
