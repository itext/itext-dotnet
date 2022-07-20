using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for SubjectPublicKeyInfo that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ISubjectPublicKeyInfo : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getAlgorithm</c>
        /// method for the wrapped SubjectPublicKeyInfo object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IAlgorithmIdentifier"/>
        /// wrapped AlgorithmIdentifier.
        /// </returns>
        IAlgorithmIdentifier GetAlgorithm();
    }
}
