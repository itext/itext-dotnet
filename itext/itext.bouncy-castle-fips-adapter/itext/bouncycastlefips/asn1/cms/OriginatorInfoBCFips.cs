using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class OriginatorInfoBCFips : ASN1EncodableBCFips, IOriginatorInfo {
        public OriginatorInfoBCFips(OriginatorInfo originatorInfo)
            : base(originatorInfo) {
        }

        public virtual OriginatorInfo GetOriginatorInfo() {
            return (OriginatorInfo)GetEncodable();
        }
    }
}
