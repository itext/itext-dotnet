using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Operator.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaDigestCalculatorProviderBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaDigestCalculatorProviderBuilder {
        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaDigestCalculatorProviderBuilder object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Operator.IDigestCalculatorProvider"/>
        /// the wrapper for built DigestCalculatorProvider object.
        /// </returns>
        IDigestCalculatorProvider Build();
    }
}
