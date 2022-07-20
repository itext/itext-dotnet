using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>.
    /// </summary>
    public class RecipientInfoBC : ASN1EncodableBC, IRecipientInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>.
        /// </summary>
        /// <param name="recipientInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>
        /// to be wrapped
        /// </param>
        public RecipientInfoBC(RecipientInfo recipientInfo)
            : base(recipientInfo) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>.
        /// </summary>
        /// <param name="keyTransRecipientInfo">
        /// KeyTransRecipientInfo to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>
        /// </param>
        public RecipientInfoBC(IKeyTransRecipientInfo keyTransRecipientInfo)
            : base(new RecipientInfo(((KeyTransRecipientInfoBC)keyTransRecipientInfo).GetKeyTransRecipientInfo())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>.
        /// </returns>
        public virtual RecipientInfo GetRecipientInfo() {
            return (RecipientInfo)GetEncodable();
        }
    }
}
