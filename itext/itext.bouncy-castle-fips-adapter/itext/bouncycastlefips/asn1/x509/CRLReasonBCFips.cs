using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class CRLReasonBCFips : ASN1EncodableBCFips, ICRLReason {
        private static readonly iText.Bouncycastlefips.Asn1.X509.CRLReasonBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.CRLReasonBCFips
            (null);

        private const int KEY_COMPROMISE = Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise;

        public CRLReasonBCFips(CRLReason reason)
            : base(reason) {
        }

        public static iText.Bouncycastlefips.Asn1.X509.CRLReasonBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual CRLReason GetCRLReason() {
            return (CRLReason)GetEncodable();
        }

        public virtual int GetKeyCompromise() {
            return KEY_COMPROMISE;
        }
    }
}
