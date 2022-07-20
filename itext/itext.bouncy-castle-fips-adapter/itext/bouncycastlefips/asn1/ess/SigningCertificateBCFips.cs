using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastlefips.Asn1.Ess {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificate"/>.
    /// </summary>
    public class SigningCertificateBCFips : ASN1EncodableBCFips, ISigningCertificate {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificate"/>.
        /// </summary>
        /// <param name="signingCertificate">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificate"/>
        /// to be wrapped
        /// </param>
        public SigningCertificateBCFips(SigningCertificate signingCertificate)
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
            IESSCertID[] certsBCFips = new IESSCertID[certs.Length];
            for (int i = 0; i < certsBCFips.Length; i++) {
                certsBCFips[i] = new ESSCertIDBCFips(certs[i]);
            }
            return certsBCFips;
        }
    }
}
