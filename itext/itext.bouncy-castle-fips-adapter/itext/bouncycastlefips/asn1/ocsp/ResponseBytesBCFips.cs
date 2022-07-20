using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class ResponseBytesBCFips : ASN1EncodableBCFips, IResponseBytes {
        public ResponseBytesBCFips(ResponseBytes encodable)
            : base(encodable) {
        }

        public ResponseBytesBCFips(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString derOctetString)
            : base(new ResponseBytes(((ASN1ObjectIdentifierBCFips)asn1ObjectIdentifier).GetASN1ObjectIdentifier(), ((DEROctetStringBCFips
                )derOctetString).GetDEROctetString())) {
        }

        public virtual ResponseBytes GetResponseBytes() {
            return (ResponseBytes)GetEncodable();
        }
    }
}
