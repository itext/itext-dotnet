namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509CRLHolder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509CRLHolder {
        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped X509CRLHolder object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();
    }
}
