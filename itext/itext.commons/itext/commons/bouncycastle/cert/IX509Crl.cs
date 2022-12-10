using System;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Crypto;

namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509Crl that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509Crl {
        /// <summary>
        /// Calls actual
        /// <c>IsRevoked</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <param name="cert">x509 certificate wrapper</param>
        /// <returns>boolean value.</returns>
        bool IsRevoked(IX509Certificate cert);

        /// <summary>
        /// Calls actual
        /// <c>GetIssuerDN</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <returns>X500Name wrapper.</returns>
        IX500Name GetIssuerDN();
        
        /// <summary>
        /// Calls actual
        /// <c>GetNextUpdate</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <returns>DateTime value of the next update.</returns>
        DateTime GetNextUpdate();
        
        /// <summary>
        /// Calls actual
        /// <c>Verify</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <param name="publicKey">public key to verify</param>
        void Verify(IPublicKey publicKey);

        /// <summary>
        /// Calls actual
        /// <c>GetEncoded</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <returns>encoded array</returns>
        byte[] GetEncoded();
    }
}