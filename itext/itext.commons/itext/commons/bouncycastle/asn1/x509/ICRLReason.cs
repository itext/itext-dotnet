using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for CRLReason that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ICRLReason : IASN1Encodable {
        /// <summary>
        /// Gets
        /// <c>keyCompromise</c>
        /// constant for the wrapped CRLReason.
        /// </summary>
        /// <returns>CRLReason.keyCompromise value.</returns>
        int GetKeyCompromise();
    }
}
