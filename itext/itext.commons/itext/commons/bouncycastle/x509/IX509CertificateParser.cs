using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.X509 {
    /// <summary>
    /// This interface represents the wrapper for X509CertificateParser that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509CertificateParser {
        /// <summary>
        /// Calls actual
        /// <c>ReadAllCerts</c>
        /// method for the wrapped X509CertificateParser object.
        /// </summary>
        /// <param name="contentsKey">Bytes from which certificated will be read</param>
        /// <returns>All read certificated</returns>
        List<IX509Certificate> ReadAllCerts(byte[] contentsKey);
    }
}