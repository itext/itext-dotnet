using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPReqBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPReqBuilder {
        /// <summary>
        /// Calls actual
        /// <c>setRequestExtensions</c>
        /// method for the wrapped OCSPReqBuilder object.
        /// </summary>
        /// <param name="extensions">wrapper for extensions to set</param>
        /// <returns>
        /// 
        /// <see cref="IOCSPReqBuilder"/>
        /// this wrapper object.
        /// </returns>
        IOCSPReqBuilder SetRequestExtensions(IExtensions extensions);

        /// <summary>
        /// Calls actual
        /// <c>addRequest</c>
        /// method for the wrapped OCSPReqBuilder object.
        /// </summary>
        /// <param name="certificateID">CertificateID wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IOCSPReqBuilder"/>
        /// this wrapper object.
        /// </returns>
        IOCSPReqBuilder AddRequest(ICertificateID certificateID);

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped OCSPReqBuilder object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IOCSPReq"/>
        /// wrapper for built OCSPReq object.
        /// </returns>
        IOCSPReq Build();
    }
}
