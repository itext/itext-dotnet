using Org.BouncyCastle.Asn1.Cmp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cmp;

namespace iText.Bouncycastlefips.Asn1.Cmp {
    public class PKIFailureInfoBCFips : ASN1PrimitiveBCFips, IPKIFailureInfo {
        public PKIFailureInfoBCFips(PkiFailureInfo pkiFailureInfo)
            : base(pkiFailureInfo) {
        }

        public virtual PkiFailureInfo GetPkiFailureInfo() {
            return (PkiFailureInfo)GetEncodable();
        }

        public virtual int IntValue() {
            return GetPkiFailureInfo().IntValue;
        }
    }
}
