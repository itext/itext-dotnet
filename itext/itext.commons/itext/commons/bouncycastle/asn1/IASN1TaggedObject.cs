namespace iText.Commons.Bouncycastle.Asn1 {
    /// <summary>
    /// This interface represents the wrapper for ASN1TaggedObject that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IASN1TaggedObject : IASN1Primitive {
        /// <summary>
        /// Calls actual
        /// <c>getObject</c>
        /// method for the wrapped ASN1TaggedObject object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IASN1Primitive"/>
        /// wrapped ASN1Primitive object.
        /// </returns>
        IASN1Primitive GetObject();

        /// <summary>
        /// Calls actual
        /// <c>getTagNo</c>
        /// method for the wrapped ASN1TaggedObject object.
        /// </summary>
        /// <returns>tagNo value.</returns>
        int GetTagNo();
    }
}
