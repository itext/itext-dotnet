using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1TaggedObjectBC : ASN1PrimitiveBC, IASN1TaggedObject {
        public ASN1TaggedObjectBC(Asn1TaggedObject taggedObject)
            : base(taggedObject) {
        }

        public virtual Asn1TaggedObject GetASN1TaggedObject() {
            return (Asn1TaggedObject)GetPrimitive();
        }

        public virtual IASN1Primitive GetObject() {
            return new ASN1PrimitiveBC(GetASN1TaggedObject().GetObject());
        }

        public virtual int GetTagNo() {
            return GetASN1TaggedObject().TagNo;
        }
    }
}
