using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class KeyTransRecipientInfoBCFips : ASN1EncodableBCFips, IKeyTransRecipientInfo {
        public KeyTransRecipientInfoBCFips(KeyTransRecipientInfo keyTransRecipientInfo)
            : base(keyTransRecipientInfo) {
        }

        public KeyTransRecipientInfoBCFips(IRecipientIdentifier recipientIdentifier, IAlgorithmIdentifier algorithmIdentifier
            , IASN1OctetString octetString)
            : base(new KeyTransRecipientInfo(((RecipientIdentifierBCFips)recipientIdentifier).GetRecipientIdentifier()
                , ((AlgorithmIdentifierBCFips)algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBCFips)octetString
                ).GetOctetString())) {
        }

        public virtual KeyTransRecipientInfo GetKeyTransRecipientInfo() {
            return (KeyTransRecipientInfo)GetEncodable();
        }
    }
}
