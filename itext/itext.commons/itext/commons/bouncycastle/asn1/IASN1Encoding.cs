using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1Encoding that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Encoding {
        /// <summary>
        /// Gets
        /// <c>DER</c>
        /// constant for the wrapped ASN1Encoding.
        /// </summary>
        /// <returns>ASN1Encoding.DER value.</returns>
        String GetDer();

        /// <summary>
        /// Gets
        /// <c>BER</c>
        /// constant for the wrapped ASN1Encoding.
        /// </summary>
        /// <returns>ASN1Encoding.BER value.</returns>
        String GetBer();
    }
}
