using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPReq that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPReq {
        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped OCSPReq object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>GetRequestList</c>
        /// method for the wrapped OCSPReq object.
        /// </summary>
        /// <returns>the array of the wrapped Req objects.</returns>
        IReq[] GetRequestList();

        /// Calls actual
        /// <c>GetExtension</c>
        /// method for the <c>RequestExtensions</c>
        /// field of the wrapped OCSPReq object.
        /// </summary>
        /// <returns>Extension wrapper.</returns>
        IExtension GetExtension(IASN1ObjectIdentifier objectIdentifier);
    }
}
