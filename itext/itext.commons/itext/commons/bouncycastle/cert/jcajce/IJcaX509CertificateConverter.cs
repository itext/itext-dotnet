using Java.Security;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.Cert.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaX509CertificateConverter that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaX509CertificateConverter {
        /// <summary>
        /// Calls actual
        /// <c>getCertificate</c>
        /// method for the wrapped JcaX509CertificateConverter object.
        /// </summary>
        /// <param name="certificateHolder">X509CertificateHolder wrapper</param>
        /// <returns>received X509Certificate.</returns>
        IX509Certificate GetCertificate(IX509CertificateHolder certificateHolder);

        /// <summary>
        /// Calls actual
        /// <c>setProvider</c>
        /// method for the wrapped JcaX509CertificateConverter object.
        /// </summary>
        /// <param name="provider">provider to set</param>
        /// <returns>
        /// 
        /// <see cref="IJcaX509CertificateConverter"/>
        /// this wrapped object.
        /// </returns>
        IJcaX509CertificateConverter SetProvider(Provider provider);
    }
}
