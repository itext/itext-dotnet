using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class EnvelopedDataBCFips : ASN1EncodableBCFips, IEnvelopedData {
        public EnvelopedDataBCFips(EnvelopedData envelopedData)
            : base(envelopedData) {
        }

        public EnvelopedDataBCFips(IOriginatorInfo originatorInfo, IASN1Set set, IEncryptedContentInfo encryptedContentInfo
            , IASN1Set set1)
            : base(new EnvelopedData(((OriginatorInfoBCFips)originatorInfo).GetOriginatorInfo(), ((ASN1SetBCFips)set).
                GetASN1Set(), ((EncryptedContentInfoBCFips)encryptedContentInfo).GetEncryptedContentInfo(), ((ASN1SetBCFips
                )set1).GetASN1Set())) {
        }

        public virtual EnvelopedData GetEnvelopedData() {
            return (EnvelopedData)GetEncodable();
        }
    }
}
