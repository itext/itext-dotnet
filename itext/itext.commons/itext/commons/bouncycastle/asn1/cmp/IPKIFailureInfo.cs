using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cmp {
    /// <summary>
    /// This interface represents the wrapper for PKIFailureInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IPKIFailureInfo : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>intValue</c>
        /// method for the wrapped PKIFailureInfo object.
        /// </summary>
        /// <returns>integer value.</returns>
        int IntValue();
    }
}
