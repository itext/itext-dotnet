using Org.BouncyCastle.Cert.Jcajce;
using iText.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;

namespace iText.Bouncycastle.Cert.Jcajce {
    public class JcaX509CertificateHolderBC : X509CertificateHolderBC, IJcaX509CertificateHolder {
        public JcaX509CertificateHolderBC(JcaX509CertificateHolder certificateHolder)
            : base(certificateHolder) {
        }

        public virtual JcaX509CertificateHolder GetJcaCertificateHolder() {
            return (JcaX509CertificateHolder)GetCertificateHolder();
        }
    }
}
