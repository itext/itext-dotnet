using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class RevokedStatusBCFips : CertificateStatusBCFips, IRevokedStatus {
        public RevokedStatusBCFips(RevokedStatus certificateStatus)
            : base(certificateStatus) {
        }

        public virtual RevokedStatus GetRevokedStatus() {
            return (RevokedStatus)base.GetCertificateStatus();
        }
    }
}
