using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    /// <summary>
    /// This interface represents the wrapper for ESSCertIDv2 that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IESSCertIDv2 : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getHashAlgorithm</c>
        /// method for the wrapped ESSCertIDv2 object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAlgorithmIdentifier"/>
        /// hash algorithm wrapper.
        /// </returns>
        IAlgorithmIdentifier GetHashAlgorithm();

        /// <summary>
        /// Calls actual
        /// <c>getCertHash</c>
        /// method for the wrapped ESSCertIDv2 object.
        /// </summary>
        /// <returns>certificate hash byte array.</returns>
        byte[] GetCertHash();
    }
}
