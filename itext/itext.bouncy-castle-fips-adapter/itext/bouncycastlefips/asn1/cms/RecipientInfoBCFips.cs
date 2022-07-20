using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class RecipientInfoBCFips : ASN1EncodableBCFips, IRecipientInfo {
        public RecipientInfoBCFips(RecipientInfo recipientInfo)
            : base(recipientInfo) {
        }

        public RecipientInfoBCFips(IKeyTransRecipientInfo keyTransRecipientInfo)
            : base(new RecipientInfo(((KeyTransRecipientInfoBCFips)keyTransRecipientInfo).GetKeyTransRecipientInfo())) {
        }

        public virtual RecipientInfo GetRecipientInfo() {
            return (RecipientInfo)GetEncodable();
        }
    }
}
