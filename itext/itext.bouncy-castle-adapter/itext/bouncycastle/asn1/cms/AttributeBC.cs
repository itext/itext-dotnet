using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastle.Asn1.Cms {
    public class AttributeBC : ASN1EncodableBC, IAttribute {
        public AttributeBC(Org.BouncyCastle.Asn1.Cms.Attribute attribute)
            : base(attribute) {
        }

        public virtual Org.BouncyCastle.Asn1.Cms.Attribute GetAttribute() {
            return (Org.BouncyCastle.Asn1.Cms.Attribute)GetEncodable();
        }

        public virtual IASN1Set GetAttrValues() {
            return new ASN1SetBC(GetAttribute().AttrValues);
        }
    }
}
