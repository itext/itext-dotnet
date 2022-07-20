using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
    /// </summary>
    public class OCSPResponseBC : ASN1EncodableBC, IOCSPResponse {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </summary>
        /// <param name="ocspResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>
        /// to be wrapped
        /// </param>
        public OCSPResponseBC(OcspResponse ocspResponse)
            : base(ocspResponse) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </summary>
        /// <param name="respStatus">OCSPResponseStatus wrapper</param>
        /// <param name="responseBytes">ResponseBytes wrapper</param>
        public OCSPResponseBC(IOCSPResponseStatus respStatus, IResponseBytes responseBytes)
            : base(new OcspResponse(((OCSPResponseStatusBC)respStatus).GetOcspResponseStatus(), ((ResponseBytesBC)responseBytes
                ).GetResponseBytes())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </returns>
        public virtual OcspResponse GetOcspResponse() {
            return (OcspResponse)GetEncodable();
        }
    }
}
