using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastle.Asn1.Ess {
    public class SigningCertificateBC : ASN1EncodableBC, ISigningCertificate {
        public SigningCertificateBC(SigningCertificate signingCertificate)
            : base(signingCertificate) {
        }

        public virtual SigningCertificate GetSigningCertificate() {
            return (SigningCertificate)GetEncodable();
        }

        public virtual IESSCertID[] GetCerts() {
            EssCertID[] certs = GetSigningCertificate().GetCerts();
            IESSCertID[] certsBC = new IESSCertID[certs.Length];
            for (int i = 0; i < certsBC.Length; i++) {
                certsBC[i] = new ESSCertIDBC(certs[i]);
            }
            return certsBC;
        }
    }
}
