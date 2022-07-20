using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    public class OCSPResponseStatusBC : ASN1EncodableBC, IOCSPResponseStatus {
        private static readonly iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC INSTANCE = new iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC
            (null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        public OCSPResponseStatusBC(OcspResponseStatus ocspResponseStatus)
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
