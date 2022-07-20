using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    /// <summary>
    /// This interface represents the wrapper for SigningCertificate that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ISigningCertificate : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getCerts</c>
        /// method for the wrapped SigningCertificate object.
        /// </summary>
        /// <returns>
        /// array of wrapped certificates
        /// <see cref="IESSCertID"/>.
        /// </returns>
        IESSCertID[] GetCerts();
    }
}
