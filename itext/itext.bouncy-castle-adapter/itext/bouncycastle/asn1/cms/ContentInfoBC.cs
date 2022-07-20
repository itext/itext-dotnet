using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    public class ContentInfoBC : ASN1EncodableBC, IContentInfo {
        public ContentInfoBC(ContentInfo contentInfo)
            : base(contentInfo) {
        }

        public ContentInfoBC(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable)
            : base(new ContentInfo(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), ((ASN1EncodableBC
                )encodable).GetEncodable())) {
        }

        public virtual ContentInfo GetContentInfo() {
            return (ContentInfo)GetEncodable();
        }
    }
}
