using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
    /// </summary>
    public class ResponseBytesBCFips : ASN1EncodableBCFips, IResponseBytes {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </summary>
        /// <param name="responseBytes">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>
        /// to be wrapped
        /// </param>
        public ResponseBytesBCFips(ResponseBytes responseBytes)
            : base(responseBytes) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </summary>
        /// <param name="asn1ObjectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="derOctetString">DEROctetString wrapper</param>
        public ResponseBytesBCFips(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString derOctetString)
            : base(new ResponseBytes(((ASN1ObjectIdentifierBCFips)asn1ObjectIdentifier).GetASN1ObjectIdentifier(), ((DEROctetStringBCFips
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
