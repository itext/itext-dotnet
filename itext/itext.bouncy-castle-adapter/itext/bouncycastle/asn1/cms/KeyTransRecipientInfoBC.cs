using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Cms {
    public class KeyTransRecipientInfoBC : ASN1EncodableBC, IKeyTransRecipientInfo {
        public KeyTransRecipientInfoBC(KeyTransRecipientInfo keyTransRecipientInfo)
            : base(keyTransRecipientInfo) {
        }

        public KeyTransRecipientInfoBC(IRecipientIdentifier recipientIdentifier, IAlgorithmIdentifier algorithmIdentifier
            , IASN1OctetString octetString)
            : base(new KeyTransRecipientInfo(((RecipientIdentifierBC)recipientIdentifier).GetRecipientIdentifier(), ((
                AlgorithmIdentifierBC)algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBC)octetString).
                GetASN1OctetString())) {
        }

        public virtual KeyTransRecipientInfo GetKeyTransRecipientInfo() {
            return (KeyTransRecipientInfo)GetEncodable();
        }
    }
}
