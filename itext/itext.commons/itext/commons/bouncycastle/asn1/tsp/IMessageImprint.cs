using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1.Tsp {
    /// <summary>
    /// This interface represents the wrapper for MessageImprint that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IMessageImprint : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getHashedMessage</c>
        /// method for the wrapped MessageImprint object.
        /// </summary>
        /// <returns>hashed message byte array.</returns>
        byte[] GetHashedMessage();

        /// <summary>
        /// Calls actual
        /// <c>getHashAlgorithm</c>
        /// method for the wrapped MessageImprint object.
        /// </summary>
        /// <returns>algorithm identifier wrapper.</returns>
        IAlgorithmIdentifier GetHashAlgorithm();
    }
}
