using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastle.Asn1.Ess {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificate"/>.
    /// </summary>
    public class SigningCertificateBC : ASN1EncodableBC, ISigningCertificate {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificate"/>.
        /// </summary>
        /// <param name="signingCertificate">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificate"/>
        /// to be wrapped
        /// </param>
        public SigningCertificateBC(SigningCertificate signingCertificate)
            : base(signingCertificate) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificate"/>.
        /// </returns>
        public virtual SigningCertificate GetSigningCertificate() {
            return (SigningCertificate)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
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
