namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1OctetString that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1OctetString : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>getOctets</c>
        /// method for the wrapped ASN1OctetString object.
        /// </summary>
        /// <returns>octets byte array.</returns>
        byte[] GetOctets();

        /// <summary>
        /// Calls actual
        /// <c>GetDerEncoded</c>
        /// method for the wrapped ASN1OctetString object.
        /// </summary>
        /// <returns>DER-encoded byte array.</returns>
        byte[] GetDerEncoded();
    }
}
