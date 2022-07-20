using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ess;

namespace iText.Bouncycastlefips.Asn1.Ess {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>.
    /// </summary>
    public class SigningCertificateV2BCFips : ASN1EncodableBCFips, ISigningCertificateV2 {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>.
        /// </summary>
        /// <param name="signingCertificateV2">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>
        /// to be wrapped
        /// </param>
        public SigningCertificateV2BCFips(SigningCertificateV2 signingCertificateV2)
            : base(signingCertificateV2) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ess.SigningCertificateV2"/>.
        /// </returns>
        public virtual SigningCertificateV2 GetSigningCertificateV2() {
            return (SigningCertificateV2)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
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
