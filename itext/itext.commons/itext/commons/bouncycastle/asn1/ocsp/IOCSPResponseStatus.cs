using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPResponseStatus that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPResponseStatus : IASN1Encodable {
        /// <summary>
        /// Gets
        /// <c>SUCCESSFUL</c>
        /// constant for the wrapped OCSPResponseStatus.
        /// </summary>
        /// <returns>OCSPResponseStatus.SUCCESSFUL value.</returns>
        int GetSuccessful();
    }
}
