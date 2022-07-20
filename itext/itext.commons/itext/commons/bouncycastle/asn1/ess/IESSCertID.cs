using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    /// <summary>
    /// This interface represents the wrapper for ESSCertID that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IESSCertID : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getCertHash</c>
        /// method for the wrapped ESSCertID object.
        /// </summary>
        /// <returns>certificate hash byte array.</returns>
        byte[] GetCertHash();
    }
}
