using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.ContentInfo"/>.
    /// </summary>
    public class ContentInfoBC : ASN1EncodableBC, IContentInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.ContentInfo"/>.
        /// </summary>
        /// <param name="contentInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.ContentInfo"/>
        /// to be wrapped
        /// </param>
        public ContentInfoBC(ContentInfo contentInfo)
            : base(contentInfo) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.ContentInfo"/>.
        /// </summary>
        /// <param name="objectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="encodable">ASN1Encodable wrapper</param>
        public ContentInfoBC(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable)
            : base(new ContentInfo(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), ((ASN1EncodableBC
                )encodable).GetEncodable())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.ContentInfo"/>.
        /// </returns>
        public virtual ContentInfo GetContentInfo() {
            return (ContentInfo)GetEncodable();
        }
    }
}
