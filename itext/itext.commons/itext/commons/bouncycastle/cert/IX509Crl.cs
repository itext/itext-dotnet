namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509Crl that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509Crl {
        /// <summary>
        /// Calls actual
        /// <c>IsRevoked</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <param name="cert">x509 certificate wrapper</param>
        /// <returns>boolean value.</returns>
        bool IsRevoked(IX509Certificate cert);
    }
}