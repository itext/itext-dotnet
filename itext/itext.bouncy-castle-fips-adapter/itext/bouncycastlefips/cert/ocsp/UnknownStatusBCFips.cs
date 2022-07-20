using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.UnknownStatus"/>.
    /// </summary>
    public class UnknownStatusBCFips : CertificateStatusBCFips, IUnknownStatus {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.UnknownStatus"/>.
        /// </summary>
        /// <param name="certificateStatus">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.UnknownStatus"/>
        /// to be wrapped
        /// </param>
        public UnknownStatusBCFips(UnknownStatus certificateStatus)
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
