using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for Extension that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IExtension : IASN1Encodable {
        /// <summary>
        /// Gets
        /// <c>cRLDistributionPoints</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.cRLDistributionPoints wrapper.</returns>
        IASN1ObjectIdentifier GetCRlDistributionPoints();

        /// <summary>
        /// Gets
        /// <c>authorityInfoAccess</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.authorityInfoAccess wrapper.</returns>
        IASN1ObjectIdentifier GetAuthorityInfoAccess();

        /// <summary>
        /// Gets
        /// <c>basicConstraints</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.basicConstraints wrapper.</returns>
        IASN1ObjectIdentifier GetBasicConstraints();

        /// <summary>
        /// Gets
        /// <c>keyUsage</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.keyUsage wrapper.</returns>
        IASN1ObjectIdentifier GetKeyUsage();

        /// <summary>
        /// Gets
        /// <c>extendedKeyUsage</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.extendedKeyUsage wrapper.</returns>
        IASN1ObjectIdentifier GetExtendedKeyUsage();

        /// <summary>
        /// Gets
        /// <c>authorityKeyIdentifier</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.authorityKeyIdentifier wrapper.</returns>
        IASN1ObjectIdentifier GetAuthorityKeyIdentifier();

        /// <summary>
        /// Gets
        /// <c>subjectKeyIdentifier</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.subjectKeyIdentifier wrapper.</returns>
        IASN1ObjectIdentifier GetSubjectKeyIdentifier();
    }
}
