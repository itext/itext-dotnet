using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastlefips.Asn1.Ess {
    public class SigningCertificateV2BCFips : ASN1EncodableBCFips, ISigningCertificateV2 {
        public SigningCertificateV2BCFips(SigningCertificateV2 signingCertificateV2)
            : base(signingCertificateV2) {
        }

        public virtual SigningCertificateV2 GetSigningCertificateV2() {
            return (SigningCertificateV2)GetEncodable();
        }

        public virtual IESSCertIDv2[] GetCerts() {
            EssCertIDv2[] certs = GetSigningCertificateV2().GetCerts();
            IESSCertIDv2[] certsBCFips = new IESSCertIDv2[certs.Length];
            for (int i = 0; i < certsBCFips.Length; i++) {
                certsBCFips[i] = new ESSCertIDv2BCFips(certs[i]);
            }
            return certsBCFips;
        }
    }
}
