using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1Integer that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Integer : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>getValue</c>
        /// method for the wrapped ASN1Integer object.
        /// </summary>
        /// <returns>BigInteger value.</returns>
        IBigInteger GetValue();

        int GetIntValue();
    }
}
