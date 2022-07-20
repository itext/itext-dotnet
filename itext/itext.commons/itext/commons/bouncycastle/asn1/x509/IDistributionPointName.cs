using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for DistributionPointName that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IDistributionPointName : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getType</c>
        /// method for the wrapped DistributionPointName object.
        /// </summary>
        /// <returns>type value.</returns>
        int GetType();

        /// <summary>
        /// Calls actual
        /// <c>getName</c>
        /// method for the wrapped DistributionPointName object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1Encodable"/>
        /// ASN1Encodable wrapper.
        /// </returns>
        IASN1Encodable GetName();

        /// <summary>
        /// Gets
        /// <c>FULL_NAME</c>
        /// constant for the wrapped DistributionPointName.
        /// </summary>
        /// <returns>DistributionPointName.FULL_NAME value.</returns>
        int GetFullName();
    }
}
