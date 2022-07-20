using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    /// <summary>
    /// This interface represents the wrapper for TimeStampRequest that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITimeStampRequest {
        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for wrapped TimeStampRequest object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>getNonce</c>
        /// method for wrapped TimeStampRequest object.
        /// </summary>
        /// <returns>nonce value.</returns>
        IBigInteger GetNonce();
    }
}
