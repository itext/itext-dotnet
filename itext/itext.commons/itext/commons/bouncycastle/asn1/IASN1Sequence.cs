using System.Collections;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1Sequence that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Sequence : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>getObjectAt</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>
        /// 
        /// <see cref="IASN1Encodable"/>
        /// wrapped ASN1Encodable object.
        /// </returns>
        IASN1Encodable GetObjectAt(int i);

        /// <summary>
        /// Calls actual
        /// <c>getObjects</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <returns>received objects.</returns>
        IEnumerator GetObjects();

        /// <summary>
        /// Calls actual
        /// <c>size</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <returns>sequence size.</returns>
        int Size();

        /// <summary>
        /// Calls actual
        /// <c>toArray</c>
        /// method for the wrapped ASN1Sequence object.
        /// </summary>
        /// <returns>array of wrapped ASN1Encodable objects.</returns>
        IASN1Encodable[] ToArray();
    }
}
