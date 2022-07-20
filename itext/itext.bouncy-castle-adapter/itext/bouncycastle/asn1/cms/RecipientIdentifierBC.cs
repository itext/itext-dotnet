using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    public class RecipientIdentifierBC : ASN1EncodableBC, IRecipientIdentifier {
        public RecipientIdentifierBC(RecipientIdentifier recipientIdentifier)
            : base(recipientIdentifier) {
        }

        public RecipientIdentifierBC(IIssuerAndSerialNumber issuerAndSerialNumber)
            : base(new RecipientIdentifier(((IssuerAndSerialNumberBC)issuerAndSerialNumber).GetIssuerAndSerialNumber()
                )) {
        }

        public virtual RecipientIdentifier GetRecipientIdentifier() {
            return (RecipientIdentifier)GetEncodable();
        }
    }
}
