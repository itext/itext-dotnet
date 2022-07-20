using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1SetBCFips : ASN1PrimitiveBCFips, IASN1Set {
        public ASN1SetBCFips(Asn1Set set)
            : base(set) {
        }

        public ASN1SetBCFips(Asn1TaggedObject taggedObject, bool b)
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
            return new ASN1EncodableBCFips(GetASN1Set()[index]);
        }

        public virtual IASN1Encodable[] ToArray() {
            Asn1Encodable[] encodables = GetASN1Set().ToArray();
            ASN1EncodableBCFips[] encodablesBCFips = new ASN1EncodableBCFips[encodables.Length];
            for (int i = 0; i < encodables.Length; ++i) {
                encodablesBCFips[i] = new ASN1EncodableBCFips(encodables[i]);
            }
            return encodablesBCFips;
        }
    }
}
