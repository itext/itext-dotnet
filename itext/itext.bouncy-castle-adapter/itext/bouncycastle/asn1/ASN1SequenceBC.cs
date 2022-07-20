using System;
using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1SequenceBC : ASN1PrimitiveBC, IASN1Sequence {
        public ASN1SequenceBC(Asn1Sequence sequence)
            : base(sequence) {
        }

        public ASN1SequenceBC(Object obj)
            : base(Asn1Sequence.GetInstance(obj)) {
        }

        public virtual Asn1Sequence GetASN1Sequence() {
            return (Asn1Sequence)GetPrimitive();
        }

        public virtual IASN1Encodable GetObjectAt(int i) {
            return new ASN1EncodableBC(GetASN1Sequence()[i]);
        }

        public virtual IEnumerator GetObjects() {
            return GetASN1Sequence().GetEnumerator();
        }

        public virtual int Size() {
            return GetASN1Sequence().Count;
        }

        public virtual IASN1Encodable[] ToArray() {
            Asn1Encodable[] encodables = GetASN1Sequence().ToArray();
            ASN1EncodableBC[] encodablesBC = new ASN1EncodableBC[encodables.Length];
            for (int i = 0; i < encodables.Length; ++i) {
                encodablesBC[i] = new ASN1EncodableBC(encodables[i]);
            }
            return encodablesBC;
        }
    }
}
