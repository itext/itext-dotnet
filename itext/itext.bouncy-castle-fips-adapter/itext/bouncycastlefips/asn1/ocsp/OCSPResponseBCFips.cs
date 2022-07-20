using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class OCSPResponseBCFips : ASN1EncodableBCFips, IOCSPResponse {
        public OCSPResponseBCFips(OcspResponse ocspResponse)
            : base(ocspResponse) {
        }

        public OCSPResponseBCFips(IOCSPResponseStatus respStatus, IResponseBytes responseBytes)
            : base(new OcspResponse(((OCSPResponseStatusBCFips)respStatus).GetOcspResponseStatus(), ((ResponseBytesBCFips
                )responseBytes).GetResponseBytes())) {
        }

        public virtual OcspResponse GetOcspResponse() {
            return (OcspResponse)GetEncodable();
        }
    }
}
