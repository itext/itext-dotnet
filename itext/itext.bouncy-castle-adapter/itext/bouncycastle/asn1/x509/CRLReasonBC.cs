using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class CRLReasonBC : ASN1EncodableBC, ICRLReason {
        private static readonly iText.Bouncycastle.Asn1.X509.CRLReasonBC INSTANCE = new iText.Bouncycastle.Asn1.X509.CRLReasonBC
            (null);

        private const int KEY_COMPROMISE = Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise;

        public CRLReasonBC(CrlReason reason)
            : base(reason) {
        }

        public static iText.Bouncycastle.Asn1.X509.CRLReasonBC GetInstance() {
            return INSTANCE;
        }

        public virtual CrlReason GetCRLReason() {
            return (CrlReason)GetEncodable();
        }

        public virtual int GetKeyCompromise() {
            return KEY_COMPROMISE;
        }
    }
}
