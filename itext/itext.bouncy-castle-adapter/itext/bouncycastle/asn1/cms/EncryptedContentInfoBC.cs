using Org.BouncyCastle.Asn1.Cms;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Cms {
    public class EncryptedContentInfoBC : ASN1EncodableBC, IEncryptedContentInfo {
        public EncryptedContentInfoBC(EncryptedContentInfo encryptedContentInfo)
            : base(encryptedContentInfo) {
        }

        public EncryptedContentInfoBC(IASN1ObjectIdentifier data, IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString
             octetString)
            : base(new EncryptedContentInfo(((ASN1ObjectIdentifierBC)data).GetASN1ObjectIdentifier(), ((AlgorithmIdentifierBC
                )algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBC)octetString).GetASN1OctetString())
                ) {
        }

        public virtual EncryptedContentInfo GetEncryptedContentInfo() {
            return (EncryptedContentInfo)GetEncodable();
        }
    }
}
