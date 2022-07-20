using System;
using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
    /// </summary>
    public class ASN1SequenceBC : ASN1PrimitiveBC, IASN1Sequence {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>
        /// to be wrapped
        /// </param>
        public ASN1SequenceBC(Asn1Sequence sequence)
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
        public ASN1SequenceBC(Object obj)
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
            return new ASN1EncodableBC(GetASN1Sequence()[i]);
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
            Asn1Encodable[] encodables = GetASN1Sequence().ToArray();
            ASN1EncodableBC[] encodablesBC = new ASN1EncodableBC[encodables.Length];
            for (int i = 0; i < encodables.Length; ++i) {
                encodablesBC[i] = new ASN1EncodableBC(encodables[i]);
            }
            return encodablesBC;
        }
    }
}
