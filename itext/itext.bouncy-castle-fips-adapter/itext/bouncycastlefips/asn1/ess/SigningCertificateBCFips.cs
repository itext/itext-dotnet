using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastlefips.Asn1.Ess {
    public class SigningCertificateBCFips : ASN1EncodableBCFips, ISigningCertificate {
        public SigningCertificateBCFips(SigningCertificate signingCertificate)
            : base(signingCertificate) {
        }

        public virtual SigningCertificate GetSigningCertificate() {
            return (SigningCertificate)GetEncodable();
        }

        public virtual IESSCertID[] GetCerts() {
            EssCertID[] certs = GetSigningCertificate().GetCerts();
            IESSCertID[] certsBCFips = new IESSCertID[certs.Length];
            for (int i = 0; i < certsBCFips.Length; i++) {
                certsBCFips[i] = new ESSCertIDBCFips(certs[i]);
            }
            return certsBCFips;
        }
    }
}
