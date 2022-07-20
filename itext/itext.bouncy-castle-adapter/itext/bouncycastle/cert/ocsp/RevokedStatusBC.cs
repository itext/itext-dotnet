using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class RevokedStatusBC : CertificateStatusBC, IRevokedStatus {
        public RevokedStatusBC(RevokedStatus certificateStatus)
            : base(certificateStatus) {
        }

        public virtual RevokedStatus GetRevokedStatus() {
            return (RevokedStatus)base.GetCertificateStatus();
        }
    }
}
