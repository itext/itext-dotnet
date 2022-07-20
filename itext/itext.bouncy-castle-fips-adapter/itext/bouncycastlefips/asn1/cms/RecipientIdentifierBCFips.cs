using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
    /// </summary>
    public class RecipientIdentifierBCFips : ASN1EncodableBCFips, IRecipientIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
        /// </summary>
        /// <param name="recipientIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>
        /// to be wrapped
        /// </param>
        public RecipientIdentifierBCFips(RecipientIdentifier recipientIdentifier)
            : base(recipientIdentifier) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
        /// </summary>
        /// <param name="issuerAndSerialNumber">
        /// IssuerAndSerialNumber wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>
        /// </param>
        public RecipientIdentifierBCFips(IIssuerAndSerialNumber issuerAndSerialNumber)
            : base(new RecipientIdentifier(((IssuerAndSerialNumberBCFips)issuerAndSerialNumber).GetIssuerAndSerialNumber
                ())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.RecipientIdentifier"/>.
        /// </returns>
        public virtual RecipientIdentifier GetRecipientIdentifier() {
            return (RecipientIdentifier)GetEncodable();
        }
    }
}
