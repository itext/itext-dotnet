using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class KeyPurposeIdBC : ASN1EncodableBC, IKeyPurposeId {
        private static readonly iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC INSTANCE = new iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC
            (null);

        private static readonly iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC ID_KP_OCSP_SIGNING = new iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC
            (KeyPurposeID.IdKPOcspSigning);

        public KeyPurposeIdBC(KeyPurposeID KeyPurposeId)
            : base(KeyPurposeId) {
        }

        public static iText.Bouncycastle.Asn1.X509.KeyPurposeIdBC GetInstance() {
            return INSTANCE;
        }

        public virtual KeyPurposeID GetKeyPurposeId() {
            return (KeyPurposeID)GetEncodable();
        }

        public virtual IKeyPurposeId GetIdKpOCSPSigning() {
            return ID_KP_OCSP_SIGNING;
        }
    }
}
