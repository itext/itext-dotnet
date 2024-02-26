using System;

namespace iText.Commons.Bouncycastle.Security {
    /// <summary>
    /// This class represents the wrapper for CertificateNotYetValidException that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public class AbstractCertificateNotYetValidException : Exception
    {
        /// <summary>
        /// Base constructor for <see cref="CertificateNotYetValidException"/>.
        /// </summary>
        protected AbstractCertificateNotYetValidException() {
        }
        
        /// <summary>
        /// Creates new wrapper instance for <see cref="CertificateNotYetValidException"/>.
        /// The abstract class constructor gets executed from a derived class.
        /// </summary>
        /// <param name="message">Exception message</param>
        protected AbstractCertificateNotYetValidException(string message) : base(message) {
        }
    }
}