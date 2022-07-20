using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Operator {
    /// <summary>
    /// This interface represents the wrapper for DigestCalculatorProvider that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IDigestCalculatorProvider {
        /// <summary>
        /// Calls actual
        /// <c>get</c>
        /// method for the wrapped DigestCalculatorProvider object.
        /// </summary>
        /// <param name="algorithmIdentifier">AlgorithmIdentifier wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IDigestCalculator"/>
        /// the wrapper for received DigestCalculator object.
        /// </returns>
        IDigestCalculator Get(IAlgorithmIdentifier algorithmIdentifier);
    }
}
