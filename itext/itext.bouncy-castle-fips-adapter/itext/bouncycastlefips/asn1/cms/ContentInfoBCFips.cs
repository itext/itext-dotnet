using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class ContentInfoBCFips : ASN1EncodableBCFips, IContentInfo {
        public ContentInfoBCFips(ContentInfo contentInfo)
            : base(contentInfo) {
        }

        public ContentInfoBCFips(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable)
            : base(new ContentInfo(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier(), ((ASN1EncodableBCFips
                )encodable).GetEncodable())) {
        }

        public virtual ContentInfo GetContentInfo() {
            return (ContentInfo)GetEncodable();
        }
    }
}
