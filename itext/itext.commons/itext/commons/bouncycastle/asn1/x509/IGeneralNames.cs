using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for GeneralNames that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IGeneralNames : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getNames</c>
        /// method for the wrapped GeneralNames object.
        /// </summary>
        /// <returns>
        /// array of wrapped names
        /// <see cref="IGeneralName"/>.
        /// </returns>
        IGeneralName[] GetNames();
    }
}
