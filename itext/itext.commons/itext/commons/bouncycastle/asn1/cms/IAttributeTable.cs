using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// This interface represents the wrapper for AttributeTable that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IAttributeTable {
        /// <summary>
        /// Calls actual
        /// <c>get</c>
        /// method for the wrapped AttributeTable object.
        /// </summary>
        /// <param name="oid">ASN1ObjectIdentifier wrapper</param>
        /// <returns>
        /// 
        /// <see cref="IAttribute"/>
        /// wrapper for the received Attribute object.
        /// </returns>
        IAttribute Get(IASN1ObjectIdentifier oid);
    }
}
