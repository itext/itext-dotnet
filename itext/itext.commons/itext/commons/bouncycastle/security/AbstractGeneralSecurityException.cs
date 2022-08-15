using System;

namespace iText.Commons.Bouncycastle.Security {
    /// <summary>
    /// This class represents the wrapper for GeneralSecurityException that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public abstract class AbstractGeneralSecurityException : Exception {
        /// <summary>
        /// Base constructor for <see cref="Org.BouncyCastle.Security.GeneralSecurityException"/>.
        /// </summary>
        protected AbstractGeneralSecurityException() {
        }
        
        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Security.GeneralSecurityException"/>.
        /// The abstract class constructor gets executed from a derived class.
        /// </summary>
        /// <param name="format">Exception message</param>
        protected AbstractGeneralSecurityException(string format) : base(format) {
        }
    }
}