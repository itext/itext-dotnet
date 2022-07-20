using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.RevokedStatus"/>.
    /// </summary>
    public class RevokedStatusBC : CertificateStatusBC, IRevokedStatus {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.RevokedStatus"/>.
        /// </summary>
        /// <param name="certificateStatus">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.RevokedStatus"/>
        /// to be wrapped
        /// </param>
        public RevokedStatusBC(RevokedStatus certificateStatus)
            : base(certificateStatus) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.RevokedStatus"/>.
        /// </returns>
        public virtual RevokedStatus GetRevokedStatus() {
            return (RevokedStatus)base.GetCertificateStatus();
        }
    }
}
