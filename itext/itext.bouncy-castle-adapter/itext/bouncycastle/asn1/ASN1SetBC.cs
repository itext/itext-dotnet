using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1SetBC : ASN1PrimitiveBC, IASN1Set {
        public ASN1SetBC(Asn1Set set)
            : base(set) {
        }

        public ASN1SetBC(Asn1TaggedObject taggedObject, bool b)
            : base(Asn1Set.GetInstance(taggedObject, b)) {
        }

        public virtual Asn1Set GetASN1Set() {
            return (Asn1Set)GetPrimitive();
        }

        public virtual IEnumerator GetObjects() {
            return GetASN1Set().GetEnumerator();
        }

        public virtual int Size() {
            return GetASN1Set().Count;
        }

        public virtual IASN1Encodable GetObjectAt(int index) {
            return new ASN1EncodableBC(GetASN1Set()[index]);
        }

        public virtual IASN1Encodable[] ToArray() {
            Asn1Encodable[] encodables = GetASN1Set().ToArray();
            ASN1EncodableBC[] encodablesBC = new ASN1EncodableBC[encodables.Length];
            for (int i = 0; i < encodables.Length; ++i) {
                encodablesBC[i] = new ASN1EncodableBC(encodables[i]);
            }
            return encodablesBC;
        }
    }
}
