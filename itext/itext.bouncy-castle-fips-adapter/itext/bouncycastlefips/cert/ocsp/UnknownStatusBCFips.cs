using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class UnknownStatusBCFips : CertificateStatusBCFips, IUnknownStatus {
        public UnknownStatusBCFips(UnknownStatus certificateStatus)
            : base(certificateStatus) {
        }

        public virtual UnknownStatus GetUnknownStatus() {
            return (UnknownStatus)base.GetCertificateStatus();
        }
    }
}
