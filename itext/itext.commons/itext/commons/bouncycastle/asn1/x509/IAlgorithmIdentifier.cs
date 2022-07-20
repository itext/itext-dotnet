using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for AlgorithmIdentifier that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IAlgorithmIdentifier : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getAlgorithm</c>
        /// method for the wrapped AlgorithmIdentifier object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1ObjectIdentifier"/>
        /// wrapped algorithm ASN1ObjectIdentifier.
        /// </returns>
        IASN1ObjectIdentifier GetAlgorithm();
    }
}
