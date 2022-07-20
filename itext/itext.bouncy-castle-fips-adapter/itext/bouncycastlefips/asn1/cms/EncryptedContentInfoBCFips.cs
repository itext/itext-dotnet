using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class EncryptedContentInfoBCFips : ASN1EncodableBCFips, IEncryptedContentInfo {
        public EncryptedContentInfoBCFips(EncryptedContentInfo encryptedContentInfo)
            : base(encryptedContentInfo) {
        }

        public EncryptedContentInfoBCFips(IASN1ObjectIdentifier data, IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString
             octetString)
            : base(new EncryptedContentInfo(((ASN1ObjectIdentifierBCFips)data).GetASN1ObjectIdentifier(), ((AlgorithmIdentifierBCFips
                )algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBCFips)octetString).GetOctetString())
                ) {
        }

        public virtual EncryptedContentInfo GetEncryptedContentInfo() {
            return (EncryptedContentInfo)GetEncodable();
        }
    }
}
