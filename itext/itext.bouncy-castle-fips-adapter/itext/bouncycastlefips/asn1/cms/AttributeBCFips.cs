using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.Attribute"/>.
    /// </summary>
    public class AttributeBCFips : ASN1EncodableBCFips, IAttribute {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.Attribute"/>.
        /// </summary>
        /// <param name="attribute">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.Attribute"/>
        /// to be wrapped
        /// </param>
        public AttributeBCFips(Org.BouncyCastle.Asn1.Cms.Attribute attribute)
            : base(attribute) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.Attribute"/>.
        /// </returns>
        public virtual Org.BouncyCastle.Asn1.Cms.Attribute GetAttribute() {
            return (Org.BouncyCastle.Asn1.Cms.Attribute)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Set GetAttrValues() {
            return new ASN1SetBCFips(GetAttribute().AttrValues);
        }
    }
}
