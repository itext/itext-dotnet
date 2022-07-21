using Org.BouncyCastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class CRLReasonBCFips : ASN1EncodableBCFips, ICRLReason {
        private static readonly CRLReasonBCFips INSTANCE = new CRLReasonBCFips(null);

        private const int KEY_COMPROMISE = CrlReason.KeyCompromise;

        public CRLReasonBCFips(CrlReason reason)
            : base(reason) {
        }

        public static CRLReasonBCFips GetInstance() {
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
