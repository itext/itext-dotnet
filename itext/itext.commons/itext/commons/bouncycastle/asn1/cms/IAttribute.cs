using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// This interface represents the wrapper for Attribute that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IAttribute : IASN1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getAttrValues</c>
        /// method for the wrapped Attribute object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1Set"/>
        /// wrapped attribute values.
        /// </returns>
        IASN1Set GetAttrValues();
    }
}
