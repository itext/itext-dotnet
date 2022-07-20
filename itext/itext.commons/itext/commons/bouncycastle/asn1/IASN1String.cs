using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1String that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1String {
        /// <summary>
        /// Calls actual
        /// <c>getString</c>
        /// method for the wrapped ASN1String object.
        /// </summary>
        /// <returns>the resulting string.</returns>
        String GetString();
    }
}
