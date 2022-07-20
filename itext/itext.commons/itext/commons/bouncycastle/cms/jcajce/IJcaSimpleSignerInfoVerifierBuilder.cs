using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;

namespace iText.Commons.Bouncycastle.Cms.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaSimpleSignerInfoVerifierBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaSimpleSignerInfoVerifierBuilder {
        /// <summary>
        /// Calls actual
        /// <c>setProvider</c>
        /// method for the wrapped JcaSimpleSignerInfoVerifierBuilder object.
        /// </summary>
        /// <param name="provider">provider name</param>
        /// <returns>
        /// 
        /// <see cref="IJcaSimpleSignerInfoVerifierBuilder"/>
        /// this wrapper object.
        /// </returns>
        IJcaSimpleSignerInfoVerifierBuilder SetProvider(String provider);

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaSimpleSignerInfoVerifierBuilder object.
        /// </summary>
        /// <param name="certificate">X509Certificate</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cms.ISignerInformationVerifier"/>
        /// the wrapper for built SignerInformationVerifier object.
        /// </returns>
        ISignerInformationVerifier Build(IX509Certificate certificate);
    }
}
