using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Operator.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaContentSignerBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaContentSignerBuilder {
        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaContentSignerBuilder object.
        /// </summary>
        /// <param name="pk">private key</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Operator.IContentSigner"/>
        /// the wrapper for built ContentSigner object.
        /// </returns>
        IContentSigner Build(IPrivateKey pk);

        /// <summary>
        /// Calls actual
        /// <c>setProvider</c>
        /// method for the wrapped JcaContentSignerBuilder object.
        /// </summary>
        /// <param name="providerName">provider name</param>
        /// <returns>
        /// 
        /// <see cref="IJcaContentSignerBuilder"/>
        /// this wrapper object.
        /// </returns>
        IJcaContentSignerBuilder SetProvider(String providerName);
    }
}
