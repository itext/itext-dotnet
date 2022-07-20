using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for KeyUsage that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IKeyUsage : IASN1Encodable {
        /// <summary>
        /// Gets
        /// <c>digitalSignature</c>
        /// constant for the wrapped KeyUsage.
        /// </summary>
        /// <returns>KeyUsage.digitalSignature value.</returns>
        int GetDigitalSignature();

        /// <summary>
        /// Gets
        /// <c>nonRepudiation</c>
        /// constant for the wrapped KeyUsage.
        /// </summary>
        /// <returns>KeyUsage.nonRepudiation value.</returns>
        int GetNonRepudiation();
    }
}
