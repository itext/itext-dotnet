using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1Primitive that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Primitive : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped ASN1Primitive object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>getEncoded</c>
        /// method for the wrapped ASN1Primitive object.
        /// </summary>
        /// <param name="encoding">encoding value</param>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded(String encoding);
    }
}
