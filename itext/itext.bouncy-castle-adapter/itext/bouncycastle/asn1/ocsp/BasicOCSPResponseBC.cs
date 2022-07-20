using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    public class BasicOCSPResponseBC : ASN1EncodableBC, IBasicOCSPResponse {
        public BasicOCSPResponseBC(BasicOcspResponse basicOCSPResponse)
            : base(basicOCSPResponse) {
        }

        public virtual BasicOcspResponse GetBasicOCSPResponse() {
            return (BasicOcspResponse)GetEncodable();
        }
    }
}
