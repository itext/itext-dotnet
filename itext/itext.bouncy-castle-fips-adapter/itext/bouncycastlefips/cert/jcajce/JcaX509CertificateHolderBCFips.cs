using Org.BouncyCastle.Cert.Jcajce;
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;

namespace iText.Bouncycastlefips.Cert.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateHolder"/>.
    /// </summary>
    public class JcaX509CertificateHolderBCFips : X509CertificateHolderBCFips, IJcaX509CertificateHolder {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateHolder"/>.
        /// </summary>
        /// <param name="certificateHolder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateHolder"/>
        /// to be wrapped
        /// </param>
        public JcaX509CertificateHolderBCFips(JcaX509CertificateHolder certificateHolder)
            : base(certificateHolder) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Jcajce.JcaX509CertificateHolder"/>.
        /// </returns>
        public virtual JcaX509CertificateHolder GetJcaCertificateHolder() {
            return (JcaX509CertificateHolder)GetCertificateHolder();
        }
    }
}
