using System.Collections;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1Set that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Set : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>getObjects</c>
        /// method for the wrapped ASN1Set object.
        /// </summary>
        /// <returns>received objects.</returns>
        IEnumerator GetObjects();

        /// <summary>
        /// Calls actual
        /// <c>size</c>
        /// method for the wrapped ASN1Set object.
        /// </summary>
        /// <returns>set size.</returns>
        int Size();

        /// <summary>
        /// Calls actual
        /// <c>getObjectAt</c>
        /// method for the wrapped ASN1Set object.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>
        /// 
        /// <see cref="IASN1Encodable"/>
        /// wrapped ASN1Encodable object.
        /// </returns>
        IASN1Encodable GetObjectAt(int index);

        /// <summary>
        /// Calls actual
        /// <c>toArray</c>
        /// method for the wrapped ASN1Set object.
        /// </summary>
        /// <returns>array of wrapped ASN1Encodable objects.</returns>
        IASN1Encodable[] ToArray();
    }
}
