using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1ObjectIdentifier that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1ObjectIdentifier : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>getId</c>
        /// method for the wrapped ASN1ObjectIdentifier object.
        /// </summary>
        /// <returns>string ID.</returns>
        String GetId();
    }
}
