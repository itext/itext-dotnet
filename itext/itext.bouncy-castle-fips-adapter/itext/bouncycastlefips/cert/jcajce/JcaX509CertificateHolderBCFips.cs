using Org.BouncyCastle.Cert.Jcajce;
using iText.Bouncycastlefips.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;

namespace iText.Bouncycastlefips.Cert.Jcajce {
    public class JcaX509CertificateHolderBCFips : X509CertificateHolderBCFips, IJcaX509CertificateHolder {
        public JcaX509CertificateHolderBCFips(JcaX509CertificateHolder certificateHolder)
            : base(certificateHolder) {
        }

        public virtual JcaX509CertificateHolder GetJcaCertificateHolder() {
            return (JcaX509CertificateHolder)GetCertificateHolder();
        }
    }
}
