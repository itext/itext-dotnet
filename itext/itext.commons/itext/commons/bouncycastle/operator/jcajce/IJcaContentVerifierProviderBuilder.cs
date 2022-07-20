using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Operator.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaContentVerifierProviderBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaContentVerifierProviderBuilder {
        /// <summary>
        /// Calls actual
        /// <c>setProvider</c>
        /// method for the wrapped JcaContentVerifierProviderBuilder object.
        /// </summary>
        /// <param name="provider">provider name</param>
        /// <returns>
        /// 
        /// <see cref="IJcaContentVerifierProviderBuilder"/>
        /// this wrapper object.
        /// </returns>
        IJcaContentVerifierProviderBuilder SetProvider(String provider);

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaContentVerifierProviderBuilder object.
        /// </summary>
        /// <param name="publicKey">public key</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Operator.IContentVerifierProvider"/>
        /// the wrapper for built ContentVerifierProvider object.
        /// </returns>
        IContentVerifierProvider Build(IPublicKey publicKey);
    }
}
