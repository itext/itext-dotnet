using System;
using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1SequenceBCFips : ASN1PrimitiveBCFips, IASN1Sequence {
        public ASN1SequenceBCFips(Asn1Sequence sequence)
            : base(sequence) {
        }

        public ASN1SequenceBCFips(Object obj)
            : base(Asn1Sequence.GetInstance(obj)) {
        }

        public virtual Asn1Sequence GetASN1Sequence() {
            return (Asn1Sequence)GetPrimitive();
        }

        public virtual IASN1Encodable GetObjectAt(int i) {
            return new ASN1EncodableBCFips(GetASN1Sequence()[i]);
        }

        public virtual IEnumerator GetObjects() {
            return GetASN1Sequence().GetEnumerator();
        }

        public virtual int Size() {
            return GetASN1Sequence().Count;
        }

        public virtual IASN1Encodable[] ToArray() {
            Asn1Sequence sequence = GetASN1Sequence();
            ASN1EncodableBCFips[] encodablesBCFips = new ASN1EncodableBCFips[sequence.Count];
            for (int i = 0; i < sequence.Count; ++i) {
                encodablesBCFips[i] = new ASN1EncodableBCFips(sequence[i]);
            }
            return encodablesBCFips;
        }
    }
}
