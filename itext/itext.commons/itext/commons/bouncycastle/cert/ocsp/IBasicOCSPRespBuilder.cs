using System;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for BasicOCSPRespBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IBasicOCSPRespBuilder {
        /// <summary>
        /// Calls actual
        /// <c>setResponseExtensions</c>
        /// method for the wrapped BasicOCSPRespBuilder object.
        /// </summary>
        /// <param name="extensions">response extensions wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IBasicOCSPRespBuilder"/>
        /// this wrapper object.
        /// </returns>
        IBasicOCSPRespBuilder SetResponseExtensions(IExtensions extensions);

        /// <summary>
        /// Calls actual
        /// <c>addResponse</c>
        /// method for the wrapped BasicOCSPRespBuilder object.
        /// </summary>
        /// <param name="certID">wrapped certificate ID details</param>
        /// <param name="certificateStatus">wrapped status of the certificate - wrapped null if okay</param>
        /// <param name="time">date this response was valid on</param>
        /// <param name="time1">date when next update should be requested</param>
        /// <param name="extensions">optional wrapped extensions</param>
        /// <returns>
        /// 
        /// <see cref="IBasicOCSPRespBuilder"/>
        /// this wrapper object.
        /// </returns>
        IBasicOCSPRespBuilder AddResponse(ICertificateID certID, ICertificateStatus certificateStatus, DateTime time
            , DateTime time1, IExtensions extensions);

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped BasicOCSPRespBuilder object.
        /// </summary>
        /// <param name="signer">ContentSigner wrapper</param>
        /// <param name="chain">list of wrapped X509CertificateHolder objects</param>
        /// <param name="time">produced at</param>
        /// <returns>
        /// 
        /// <see cref="IBasicOCSPResp"/>
        /// wrapper for built BasicOCSPResp object.
        /// </returns>
        IBasicOCSPResp Build(IContentSigner signer, IX509CertificateHolder[] chain, DateTime time);
    }
}
