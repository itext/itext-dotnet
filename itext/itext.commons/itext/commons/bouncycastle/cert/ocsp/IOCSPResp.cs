using System;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPResp that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPResp {
        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped OCSPResp object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>getStatus</c>
        /// method for the wrapped OCSPResp object.
        /// </summary>
        /// <returns>status value.</returns>
        int GetStatus();

        /// <summary>
        /// Calls actual
        /// <c>getResponseObject</c>
        /// method for the wrapped OCSPResp object.
        /// </summary>
        /// <returns>response object.</returns>
        Object GetResponseObject();

        /// <summary>
        /// Gets
        /// <c>SUCCESSFUL</c>
        /// constant for the wrapped OCSPResp.
        /// </summary>
        /// <returns>OCSPResp.SUCCESSFUL value.</returns>
        int GetSuccessful();
    }
}
