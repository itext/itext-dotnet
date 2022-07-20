using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.OriginatorInfo"/>.
    /// </summary>
    public class OriginatorInfoBC : ASN1EncodableBC, IOriginatorInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.OriginatorInfo"/>.
        /// </summary>
        /// <param name="originatorInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.OriginatorInfo"/>
        /// to be wrapped
        /// </param>
        public OriginatorInfoBC(OriginatorInfo originatorInfo)
            : base(originatorInfo) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.OriginatorInfo"/>.
        /// </returns>
        public virtual OriginatorInfo GetOriginatorInfo() {
            return (OriginatorInfo)GetEncodable();
        }
    }
}
