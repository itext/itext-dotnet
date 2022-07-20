using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class UnknownStatusBC : CertificateStatusBC, IUnknownStatus {
        public UnknownStatusBC(UnknownStatus certificateStatus)
            : base(certificateStatus) {
        }

        public virtual UnknownStatus GetUnknownStatus() {
            return (UnknownStatus)base.GetCertificateStatus();
        }
    }
}
