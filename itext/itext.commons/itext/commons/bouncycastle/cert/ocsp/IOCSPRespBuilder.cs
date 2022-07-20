namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPRespBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPRespBuilder {
        /// <summary>
        /// Gets
        /// <c>SUCCESSFUL</c>
        /// constant for the wrapped OCSPRespBuilder.
        /// </summary>
        /// <returns>OCSPRespBuilder.SUCCESSFUL value.</returns>
        int GetSuccessful();

        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped OCSPRespBuilder object.
        /// </summary>
        /// <param name="i">status</param>
        /// <param name="basicOCSPResp">BasicOCSPResp wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IOCSPResp"/>
        /// the wrapper for built OCSPResp object.
        /// </returns>
        IOCSPResp Build(int i, IBasicOCSPResp basicOCSPResp);
    }
}
