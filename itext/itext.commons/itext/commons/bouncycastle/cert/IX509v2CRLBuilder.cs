using System;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509v2CRLBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509v2CRLBuilder {
        /// <summary>
        /// Calls actual
        /// <c>addCRLEntry</c>
        /// method for the wrapped X509v2CRLBuilder object.
        /// </summary>
        /// <param name="bigInteger">serial number of revoked certificate</param>
        /// <param name="date">date of certificate revocation</param>
        /// <param name="i">the reason code, as indicated in CRLReason, i.e CRLReason.keyCompromise, or 0 if not to be used
        ///     </param>
        /// <returns>
        /// 
        /// <see cref="IX509v2CRLBuilder"/>
        /// the current wrapper object.
        /// </returns>
        IX509v2CRLBuilder AddCRLEntry(IBigInteger bigInteger, DateTime date, int i);

        /// <summary>
        /// Calls actual
        /// <c>setNextUpdate</c>
        /// method for the wrapped X509v2CRLBuilder object.
        /// </summary>
        /// <param name="nextUpdate">date of next CRL update</param>
        /// <returns>
        /// 
        /// <see cref="IX509v2CRLBuilder"/>
        /// the current wrapper object.
        /// </returns>
        IX509v2CRLBuilder SetNextUpdate(DateTime nextUpdate);

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped X509v2CRLBuilder object.
        /// </summary>
        /// <param name="signer">ContentSigner wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IX509CRLHolder"/>
        /// the wrapper for built X509CRLHolder object.
        /// </returns>
        IX509CRLHolder Build(IContentSigner signer);
    }
}
