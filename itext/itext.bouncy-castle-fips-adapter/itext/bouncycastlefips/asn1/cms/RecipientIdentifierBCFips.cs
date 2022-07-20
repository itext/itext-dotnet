using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class RecipientIdentifierBCFips : ASN1EncodableBCFips, IRecipientIdentifier {
        public RecipientIdentifierBCFips(RecipientIdentifier recipientIdentifier)
            : base(recipientIdentifier) {
        }

        public RecipientIdentifierBCFips(IIssuerAndSerialNumber issuerAndSerialNumber)
            : base(new RecipientIdentifier(((IssuerAndSerialNumberBCFips)issuerAndSerialNumber).GetIssuerAndSerialNumber
                ())) {
        }

        public virtual RecipientIdentifier GetRecipientIdentifier() {
            return (RecipientIdentifier)GetEncodable();
        }
    }
}
