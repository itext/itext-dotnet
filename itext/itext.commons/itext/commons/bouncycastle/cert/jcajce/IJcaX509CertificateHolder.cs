using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.Cert.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaX509CertificateHolder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaX509CertificateHolder : IX509CertificateHolder {
    }
}
