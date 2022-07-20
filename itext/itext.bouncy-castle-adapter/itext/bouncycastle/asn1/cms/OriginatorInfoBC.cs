using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    public class OriginatorInfoBC : ASN1EncodableBC, IOriginatorInfo {
        public OriginatorInfoBC(OriginatorInfo originatorInfo)
            : base(originatorInfo) {
        }

        public virtual OriginatorInfo GetOriginatorInfo() {
            return (OriginatorInfo)GetEncodable();
        }
    }
}
