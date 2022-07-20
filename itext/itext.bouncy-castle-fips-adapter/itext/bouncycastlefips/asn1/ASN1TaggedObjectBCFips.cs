using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1TaggedObjectBCFips : ASN1PrimitiveBCFips, IASN1TaggedObject {
        public ASN1TaggedObjectBCFips(Asn1TaggedObject taggedObject)
            : base(taggedObject) {
        }

        public virtual Asn1TaggedObject GetTaggedObject() {
            return (Asn1TaggedObject)GetPrimitive();
        }

        public virtual IASN1Primitive GetObject() {
            return new ASN1PrimitiveBCFips(GetTaggedObject().GetObject());
        }

        public virtual int GetTagNo() {
            return GetTaggedObject().TagNo;
        }
    }
}
