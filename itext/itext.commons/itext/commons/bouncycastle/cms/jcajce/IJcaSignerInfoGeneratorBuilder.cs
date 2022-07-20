using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cms.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaSignerInfoGeneratorBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaSignerInfoGeneratorBuilder {
        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaSignerInfoGeneratorBuilder object.
        /// </summary>
        /// <param name="signer">ContentSigner wrapper</param>
        /// <param name="cert">X509Certificate</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cms.ISignerInfoGenerator"/>
        /// the wrapper for built SignerInfoGenerator object.
        /// </returns>
        ISignerInfoGenerator Build(IContentSigner signer, IX509Certificate cert);
    }
}
