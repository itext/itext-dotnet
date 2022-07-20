using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class AttributeBCFips : ASN1EncodableBCFips, IAttribute {
        public AttributeBCFips(Org.BouncyCastle.Asn1.Cms.Attribute attribute)
            : base(attribute) {
        }

        public virtual Org.BouncyCastle.Asn1.Cms.Attribute GetAttribute() {
            return (Org.BouncyCastle.Asn1.Cms.Attribute)GetEncodable();
        }

        public virtual IASN1Set GetAttrValues() {
            return new ASN1SetBCFips(GetAttribute().AttrValues);
        }
    }
}
