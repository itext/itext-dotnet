using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for OCSPObjectIdentifiers that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IOCSPObjectIdentifiers {
        /// <summary>
        /// Gets
        /// <c>id_pkix_ocsp_basic</c>
        /// constant for the wrapped OCSPObjectIdentifiers.
        /// </summary>
        /// <returns>OCSPObjectIdentifiers.id_pkix_ocsp_basic wrapper.</returns>
        IASN1ObjectIdentifier GetIdPkixOcspBasic();

        /// <summary>
        /// Gets
        /// <c>id_pkix_ocsp_nonce</c>
        /// constant for the wrapped OCSPObjectIdentifiers.
        /// </summary>
        /// <returns>OCSPObjectIdentifiers.id_pkix_ocsp_nonce wrapper.</returns>
        IASN1ObjectIdentifier GetIdPkixOcspNonce();

        /// <summary>
        /// Gets
        /// <c>id_pkix_ocsp_nocheck</c>
        /// constant for the wrapped OCSPObjectIdentifiers.
        /// </summary>
        /// <returns>OCSPObjectIdentifiers.id_pkix_ocsp_nocheck wrapper.</returns>
        IASN1ObjectIdentifier GetIdPkixOcspNoCheck();
    }
}
