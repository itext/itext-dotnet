using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.UnknownStatus"/>.
    /// </summary>
    public class UnknownStatusBC : CertificateStatusBC, IUnknownStatus {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.UnknownStatus"/>.
        /// </summary>
        /// <param name="certificateStatus">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.UnknownStatus"/>
        /// to be wrapped
        /// </param>
        public UnknownStatusBC(UnknownStatus certificateStatus)
            : base(certificateStatus) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.UnknownStatus"/>.
        /// </returns>
        public virtual UnknownStatus GetUnknownStatus() {
            return (UnknownStatus)base.GetCertificateStatus();
        }
    }
}
