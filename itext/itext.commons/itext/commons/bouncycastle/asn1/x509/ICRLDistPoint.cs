using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for CRLDistPoint that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ICRLDistPoint : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getDistributionPoints</c>
        /// method for the wrapped CRLDistPoint object.
        /// </summary>
        /// <returns>
        /// array of the wrapped distribution points
        /// <see cref="IDistributionPoint"/>.
        /// </returns>
        IDistributionPoint[] GetDistributionPoints();
    }
}
