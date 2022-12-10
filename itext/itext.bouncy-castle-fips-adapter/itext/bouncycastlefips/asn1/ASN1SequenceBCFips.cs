using System;
using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
    /// </summary>
    public class ASN1SequenceBCFips : ASN1PrimitiveBCFips, IASN1Sequence {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>
        /// to be wrapped
        /// </param>
        public ASN1SequenceBCFips(Asn1Sequence sequence)
            : base(sequence) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </summary>
        /// <param name="obj">
        /// to get
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>
        /// instance to be wrapped
        /// </param>
        public ASN1SequenceBCFips(Object obj)
            : base(Asn1Sequence.GetInstance(obj)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </returns>
        public virtual Asn1Sequence GetASN1Sequence() {
            return (Asn1Sequence)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encodable GetObjectAt(int i) {
            return new ASN1EncodableBCFips(GetASN1Sequence()[i]);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEnumerator GetObjects() {
            return GetASN1Sequence().GetEnumerator();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Size() {
            return GetASN1Sequence().Count;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encodable[] ToArray() {
            Asn1Sequence encodables = GetASN1Sequence();
            ASN1EncodableBCFips[] encodablesBCFips = new ASN1EncodableBCFips[encodables.Count];
            for (int i = 0; i < encodables.Count; ++i) {
                encodablesBCFips[i] = new ASN1EncodableBCFips(encodables[i]);
            }
            return encodablesBCFips;
        }
    }
}
