using System;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for SingleResp that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ISingleResp {
        /// <summary>
        /// Calls actual
        /// <c>getCertID</c>
        /// method for the wrapped SingleResp object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ICertificateID"/>
        /// the wrapper for the received CertificateID.
        /// </returns>
        ICertificateID GetCertID();

        /// <summary>
        /// Calls actual
        /// <c>getCertStatus</c>
        /// method for the wrapped SingleResp object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ICertificateStatus"/>
        /// the wrapper for the received CertificateStatus.
        /// </returns>
        ICertificateStatus GetCertStatus();

        /// <summary>
        /// Calls actual
        /// <c>getNextUpdate</c>
        /// method for the wrapped SingleResp object.
        /// </summary>
        /// <returns>date of next update.</returns>
        DateTime GetNextUpdate();

        /// <summary>
        /// Calls actual
        /// <c>getThisUpdate</c>
        /// method for the wrapped SingleResp object.
        /// </summary>
        /// <returns>date of this update.</returns>
        DateTime GetThisUpdate();
    }
}
