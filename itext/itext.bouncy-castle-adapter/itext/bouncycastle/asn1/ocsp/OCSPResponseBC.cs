using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    public class OCSPResponseBC : ASN1EncodableBC, IOCSPResponse {
        public OCSPResponseBC(OcspResponse ocspResponse)
            : base(ocspResponse) {
        }

        public OCSPResponseBC(IOCSPResponseStatus respStatus, IResponseBytes responseBytes)
            : base(new OcspResponse(((OCSPResponseStatusBC)respStatus).GetOcspResponseStatus(), ((ResponseBytesBC)responseBytes
                ).GetResponseBytes())) {
        }

        public virtual OcspResponse GetOcspResponse() {
            return (OcspResponse)GetEncodable();
        }
    }
}
