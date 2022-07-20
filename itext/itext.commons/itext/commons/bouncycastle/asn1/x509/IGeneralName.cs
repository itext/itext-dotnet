using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for GeneralName that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IGeneralName : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getTagNo</c>
        /// method for the wrapped GeneralName object.
        /// </summary>
        /// <returns>tagNo value.</returns>
        int GetTagNo();

        /// <summary>
        /// Gets
        /// <c>uniformResourceIdentifier</c>
        /// constant for the wrapped GeneralName.
        /// </summary>
        /// <returns>GeneralName.uniformResourceIdentifier value.</returns>
        int GetUniformResourceIdentifier();
    }
}
