using System;
using System.Security.Cryptography;

namespace iText.Bouncycastleconnector {
    /// <summary>
    /// A utility class for exception-free initialization of bouncy-castle-dependent
    /// secure random number generator.
    /// </summary>
    /// <remarks>
    /// A utility class for exception-free initialization of bouncy-castle-dependent
    /// secure random number generator. It is meant to be used specifically for
    /// instances initialized as a static field value, because exceptions thrown
    /// during class initialization causes troubles.
    /// </remarks>
    public class BouncyCastleSecureRandomHolder {
        private volatile RNGCryptoServiceProvider RNG = null;

        private Object Lock = new Object();

        /// <summary>
        /// Creates a new
        /// <see cref="BouncyCastleSecureRandomHolder"/>
        /// instance.
        /// </summary>
        public BouncyCastleSecureRandomHolder() {
        }

        // empty constructor
        /// <summary>Gets a secure random number generator instance.</summary>
        /// <remarks>
        /// Gets a secure random number generator instance.
        /// <para />
        /// If bouncy-castle dependency is missing it will throw an exception.
        /// </remarks>
        /// <returns>
        /// the lazily initialized
        /// <see cref="System.Security.Cryptography.RNGCryptoServiceProvider"/>
        /// </returns>
        public virtual RNGCryptoServiceProvider GetSecureRandom() {
            if (RNG == null) {
                lock (Lock) {
                    if (RNG == null) {
                        // can throw an exception if BC is missing
                        RNG = BouncyCastleFactoryCreator.GetFactory().GetSecureRandom();
                    }
                }
            }
            return RNG;
        }
    }
}
