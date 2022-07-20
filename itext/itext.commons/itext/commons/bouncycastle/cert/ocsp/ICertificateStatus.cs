namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for CertificateStatus that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ICertificateStatus {
        /// <summary>
        /// Gets
        /// <c>GOOD</c>
        /// constant for the wrapped CertificateStatus.
        /// </summary>
        /// <returns>CertificateStatus.GOOD wrapper.</returns>
        ICertificateStatus GetGood();
    }
}
