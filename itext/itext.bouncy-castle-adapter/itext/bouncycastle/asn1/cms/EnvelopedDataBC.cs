using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    public class EnvelopedDataBC : ASN1EncodableBC, IEnvelopedData {
        public EnvelopedDataBC(EnvelopedData envelopedData)
            : base(envelopedData) {
        }

        public EnvelopedDataBC(IOriginatorInfo originatorInfo, IASN1Set set, IEncryptedContentInfo encryptedContentInfo
            , IASN1Set set1)
            : base(new EnvelopedData(((OriginatorInfoBC)originatorInfo).GetOriginatorInfo(), ((ASN1SetBC)set).GetASN1Set
                (), ((EncryptedContentInfoBC)encryptedContentInfo).GetEncryptedContentInfo(), ((ASN1SetBC)set1).GetASN1Set
                ())) {
        }

        public virtual EnvelopedData GetEnvelopedData() {
            return (EnvelopedData)GetEncodable();
        }
    }
}
