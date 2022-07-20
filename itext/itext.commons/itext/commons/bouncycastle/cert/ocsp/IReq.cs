namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for Req that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IReq {
        /// <summary>
        /// Calls actual
        /// <c>getCertID</c>
        /// method for the wrapped Req object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ICertificateID"/>
        /// the wrapper for the received CertificateID.
        /// </returns>
        ICertificateID GetCertID();
    }
}
