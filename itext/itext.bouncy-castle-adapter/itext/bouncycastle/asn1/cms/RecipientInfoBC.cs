using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    public class RecipientInfoBC : ASN1EncodableBC, IRecipientInfo {
        public RecipientInfoBC(RecipientInfo recipientInfo)
            : base(recipientInfo) {
        }

        public RecipientInfoBC(IKeyTransRecipientInfo keyTransRecipientInfo)
            : base(new RecipientInfo(((KeyTransRecipientInfoBC)keyTransRecipientInfo).GetKeyTransRecipientInfo())) {
        }

        public virtual RecipientInfo GetRecipientInfo() {
            return (RecipientInfo)GetEncodable();
        }
    }
}
