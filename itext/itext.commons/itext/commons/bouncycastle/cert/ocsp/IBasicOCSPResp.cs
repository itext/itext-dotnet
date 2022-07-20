using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for BasicOCSPResp that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IBasicOCSPResp {
        /// <summary>
        /// Calls actual
        /// <c>getResponses</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>wrapped SingleResp list.</returns>
        ISingleResp[] GetResponses();

        /// <summary>
        /// Calls actual
        /// <c>isSignatureValid</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <param name="provider">ContentVerifierProvider wrapper</param>
        /// <returns>boolean value.</returns>
        bool IsSignatureValid(IContentVerifierProvider provider);

        /// <summary>
        /// Calls actual
        /// <c>getCerts</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>wrapped certificates list.</returns>
        IX509CertificateHolder[] GetCerts();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>getProducedAt</c>
        /// method for the wrapped BasicOCSPResp object.
        /// </summary>
        /// <returns>produced at date.</returns>
        DateTime GetProducedAt();
    }
}
