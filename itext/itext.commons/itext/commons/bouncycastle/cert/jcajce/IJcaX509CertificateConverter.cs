using Java.Security;
using Org.BouncyCastle.X509;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.Cert.Jcajce {
    public interface IJcaX509CertificateConverter {
        X509Certificate GetCertificate(IX509CertificateHolder certificateHolder);

        IJcaX509CertificateConverter SetProvider(Provider provider);
    }
}
