namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1Encodable that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>toASN1Primitive</c>
        /// method for the wrapped ASN1Encodable object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IASN1Primitive"/>
        /// wrapped ASN1Primitive object.
        /// </returns>
        IASN1Primitive ToASN1Primitive();

        /// <summary>Checks if wrapped object is null.</summary>
        /// <returns>
        /// true if
        /// <see langword="null"/>
        /// is wrapped, false otherwise.
        /// </returns>
        bool IsNull();
    }
}
