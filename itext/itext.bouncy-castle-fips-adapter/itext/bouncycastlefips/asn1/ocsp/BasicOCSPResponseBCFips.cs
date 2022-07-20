using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class BasicOCSPResponseBCFips : ASN1EncodableBCFips, IBasicOCSPResponse {
        public BasicOCSPResponseBCFips(BasicOcspResponse basicOCSPResponse)
            : base(basicOCSPResponse) {
        }

        public virtual BasicOcspResponse GetBasicOCSPResponse() {
            return (BasicOcspResponse)GetEncodable();
        }
    }
}
