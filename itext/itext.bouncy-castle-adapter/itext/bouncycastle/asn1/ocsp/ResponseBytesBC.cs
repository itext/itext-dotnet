using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
    /// </summary>
    public class ResponseBytesBC : ASN1EncodableBC, IResponseBytes {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </summary>
        /// <param name="responseBytes">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>
        /// to be wrapped
        /// </param>
        public ResponseBytesBC(ResponseBytes responseBytes)
            : base(responseBytes) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </summary>
        /// <param name="asn1ObjectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="derOctetString">DEROctetString wrapper</param>
        public ResponseBytesBC(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString derOctetString)
            : base(new ResponseBytes(((ASN1ObjectIdentifierBC)asn1ObjectIdentifier).GetASN1ObjectIdentifier(), ((DEROctetStringBC
                )derOctetString).GetDEROctetString())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </returns>
        public virtual ResponseBytes GetResponseBytes() {
            return (ResponseBytes)GetEncodable();
        }
    }
}
