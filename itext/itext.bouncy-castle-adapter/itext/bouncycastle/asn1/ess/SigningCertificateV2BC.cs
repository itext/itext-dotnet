using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastle.Asn1.Ess {
    public class SigningCertificateV2BC : ASN1EncodableBC, ISigningCertificateV2 {
        public SigningCertificateV2BC(SigningCertificateV2 signingCertificateV2)
            : base(signingCertificateV2) {
        }

        public virtual SigningCertificateV2 GetSigningCertificateV2() {
            return (SigningCertificateV2)GetEncodable();
        }

        public virtual IESSCertIDv2[] GetCerts() {
            EssCertIDv2[] certs = GetSigningCertificateV2().GetCerts();
            IESSCertIDv2[] certsBC = new IESSCertIDv2[certs.Length];
            for (int i = 0; i < certsBC.Length; i++) {
                certsBC[i] = new ESSCertIDv2BC(certs[i]);
            }
            return certsBC;
        }
    }
}
