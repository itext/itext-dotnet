using System;

namespace iText.Commons.Bouncycastle.Security {
    /// <summary>
    /// This class represents the wrapper for CertificateExpiredException that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public class AbstractCertificateExpiredException : Exception {
        /// <summary>
        /// Base constructor for <see cref="CertificateExpiredException"/>.
        /// </summary>
        protected AbstractCertificateExpiredException() {
        }
        
        /// <summary>
        /// Creates new wrapper instance for <see cref="CertificateExpiredException"/>.
        /// The abstract class constructor gets executed from a derived class.
        /// </summary>
        /// <param name="message">Exception message</param>
        protected AbstractCertificateExpiredException(string message) : base(message) {
        }
    }
}