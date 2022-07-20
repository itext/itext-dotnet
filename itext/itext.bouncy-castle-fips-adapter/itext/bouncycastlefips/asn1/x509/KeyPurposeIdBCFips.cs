using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.KeyPurposeID"/>.
    /// </summary>
    public class KeyPurposeIdBCFips : ASN1EncodableBCFips, IKeyPurposeId {
        private static readonly iText.Bouncycastlefips.Asn1.X509.KeyPurposeIdBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.KeyPurposeIdBCFips
            (null);

        private static readonly iText.Bouncycastlefips.Asn1.X509.KeyPurposeIdBCFips ID_KP_OCSP_SIGNING = new iText.Bouncycastlefips.Asn1.X509.KeyPurposeIdBCFips
            (KeyPurposeID.IdKPOcspSigning);

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.KeyPurposeID"/>.
        /// </summary>
        /// <param name="keyPurposeId">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.KeyPurposeID"/>
        /// to be wrapped
        /// </param>
        public KeyPurposeIdBCFips(KeyPurposeID keyPurposeId)
            : base(keyPurposeId) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="KeyPurposeIdBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.X509.KeyPurposeIdBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.KeyPurposeID"/>.
        /// </returns>
        public virtual KeyPurposeID GetKeyPurposeId() {
            return (KeyPurposeID)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyPurposeId GetIdKpOCSPSigning() {
            return ID_KP_OCSP_SIGNING;
        }
    }
}
