using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1EncodableVector that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1EncodableVector {
        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object.
        /// </summary>
        /// <param name="primitive">ASN1Primitive wrapper.</param>
        void Add(IASN1Primitive primitive);

        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object.
        /// </summary>
        /// <param name="attribute">Attribute wrapper.</param>
        void Add(IAttribute attribute);

        /// <summary>
        /// Calls actual
        /// <c>add</c>
        /// method for the wrapped ASN1EncodableVector object.
        /// </summary>
        /// <param name="element">AlgorithmIdentifier wrapper.</param>
        void Add(IAlgorithmIdentifier element);
    }
}
