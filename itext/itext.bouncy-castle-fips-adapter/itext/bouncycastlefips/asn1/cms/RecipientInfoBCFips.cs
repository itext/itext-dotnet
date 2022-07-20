using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>.
    /// </summary>
    public class RecipientInfoBCFips : ASN1EncodableBCFips, IRecipientInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>.
        /// </summary>
        /// <param name="recipientInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientInfo"/>
        /// to be wrapped
        /// </param>
        public RecipientInfoBCFips(RecipientInfo recipientInfo)
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
        public RecipientInfoBCFips(IKeyTransRecipientInfo keyTransRecipientInfo)
            : base(new RecipientInfo(((KeyTransRecipientInfoBCFips)keyTransRecipientInfo).GetKeyTransRecipientInfo())) {
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
