using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    public class ResponseBytesBC : ASN1EncodableBC, IResponseBytes {
        public ResponseBytesBC(ResponseBytes encodable)
            : base(encodable) {
        }

        public ResponseBytesBC(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString derOctetString)
            : base(new ResponseBytes(((ASN1ObjectIdentifierBC)asn1ObjectIdentifier).GetASN1ObjectIdentifier(), ((DEROctetStringBC
                )derOctetString).GetDEROctetString())) {
        }

        public virtual ResponseBytes GetResponseBytes() {
            return (ResponseBytes)GetEncodable();
        }
    }
}
