using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.KeyPurposeID"/>.
    /// </summary>
    public class KeyPurposeIdBC : ASN1EncodableBC, IKeyPurposeId {
        private static readonly iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC INSTANCE = new iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC
            (null);

        private static readonly iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC ID_KP_OCSP_SIGNING = new iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC
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
        public KeyPurposeIdBC(KeyPurposeID keyPurposeId)
            : base(keyPurposeId) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="KeyPurposeIdBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC GetInstance() {
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
