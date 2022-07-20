using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for DistributionPoint that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IDistributionPoint : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getDistributionPoints</c>
        /// method for the wrapped DistributionPoint object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IDistributionPointName"/>
        /// wrapped distribution point.
        /// </returns>
        IDistributionPointName GetDistributionPoint();
    }
}
