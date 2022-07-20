using Org.BouncyCastle.Asn1.Cmp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cmp;

namespace iText.Bouncycastle.Asn1.Cmp {
    public class PKIFailureInfoBC : ASN1PrimitiveBC, IPKIFailureInfo {
        public PKIFailureInfoBC(PkiFailureInfo pkiFailureInfo)
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
