using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for CertificateID that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ICertificateID {
        /// <summary>
        /// Calls actual
        /// <c>getHashAlgOID</c>
        /// method for the wrapped CertificateID object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1ObjectIdentifier"/>
        /// hash algorithm OID wrapper.
        /// </returns>
        IASN1ObjectIdentifier GetHashAlgOID();

        /// <summary>
        /// Gets
        /// <c>getHashSha1</c>
        /// constant for the wrapped CertificateID.
        /// </summary>
        /// <returns>CertificateID.HASH_SHA1 value.</returns>
        string GetHashSha1();

        /// <summary>
        /// Calls actual
        /// <c>matchesIssuer</c>
        /// method for the wrapped CertificateID object.
        /// </summary>
        /// <param name="certificate">X509Certificate wrapper</param>
        /// <returns>boolean value.</returns>
        bool MatchesIssuer(IX509Certificate certificate);

        /// <summary>
        /// Calls actual
        /// <c>getSerialNumber</c>
        /// method for the wrapped CertificateID object.
        /// </summary>
        /// <returns>serial number value.</returns>
        IBigInteger GetSerialNumber();
    }
}
