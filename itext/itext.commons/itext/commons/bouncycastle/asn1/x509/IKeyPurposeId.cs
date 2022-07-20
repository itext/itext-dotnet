using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for KeyPurposeId that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IKeyPurposeId : IASN1Encodable {
        /// <summary>
        /// Gets
        /// <c>id_kp_OCSPSigning</c>
        /// constant for the wrapped KeyPurposeId.
        /// </summary>
        /// <returns>KeyPurposeId.id_kp_OCSPSigning value.</returns>
        IKeyPurposeId GetIdKpOCSPSigning();
    }
}
