using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class OCSPResponseStatusBCFips : ASN1EncodableBCFips, IOCSPResponseStatus {
        private static readonly iText.Bouncycastlefips.Asn1.Ocsp.OCSPResponseStatusBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.Ocsp.OCSPResponseStatusBCFips
            (null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        public OCSPResponseStatusBCFips(OcspResponseStatus ocspResponseStatus)
            : base(ocspResponseStatus) {
        }

        public static IOCSPResponseStatus GetInstance() {
            return INSTANCE;
        }

        public virtual OcspResponseStatus GetOcspResponseStatus() {
            return (OcspResponseStatus)GetEncodable();
        }

        public virtual int GetSuccessful() {
            return SUCCESSFUL;
        }
    }
}
