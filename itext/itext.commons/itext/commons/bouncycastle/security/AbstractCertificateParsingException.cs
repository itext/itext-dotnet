using System;

namespace iText.Commons.Bouncycastle.Security
{
    /// <summary>
    /// This class represents the wrapper for CertificateParsingException that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public abstract class AbstractCertificateParsingException : Exception {
        /// <summary>
        /// Base constructor for <see cref="CertificateParsingException"/>.
        /// </summary>
        protected AbstractCertificateParsingException() {
        }
        
        /// <summary>
        /// Creates new wrapper instance for <see cref="CertificateParsingException"/>.
        /// The abstract class constructor gets executed from a derived class.
        /// </summary>
        /// <param name="message">Exception message</param>
        protected AbstractCertificateParsingException(string message) : base(message) {
        }

    }
}