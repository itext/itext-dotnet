using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    /// <summary>
    /// This interface represents the wrapper for SigningCertificateV2 that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ISigningCertificateV2 : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getCerts</c>
        /// method for the wrapped SigningCertificateV2 object.
        /// </summary>
        /// <returns>
        /// array of wrapped certificates
        /// <see cref="IESSCertIDv2"/>.
        /// </returns>
        IESSCertIDv2[] GetCerts();
    }
}
