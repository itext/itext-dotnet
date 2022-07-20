using System;
using Org.BouncyCastle.Asn1;
using iText.Bouncycastlefips.Asn1.Cms;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1EncodableVectorBCFips : IASN1EncodableVector {
        private readonly Asn1EncodableVector encodableVector;

        public ASN1EncodableVectorBCFips() {
            encodableVector = new Asn1EncodableVector();
        }

        public ASN1EncodableVectorBCFips(Asn1EncodableVector encodableVector) {
            this.encodableVector = encodableVector;
        }

        public virtual Asn1EncodableVector GetEncodableVector() {
            return encodableVector;
        }

        public virtual void Add(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            encodableVector.Add(primitiveBCFips.GetPrimitive());
        }

        public virtual void Add(IAttribute attribute) {
            AttributeBCFips attributeBCFips = (AttributeBCFips)attribute;
            encodableVector.Add(attributeBCFips.GetAttribute());
        }

        public virtual void Add(IAlgorithmIdentifier element) {
            AlgorithmIdentifierBCFips elementBCFips = (AlgorithmIdentifierBCFips)element;
            encodableVector.Add(elementBCFips.GetAlgorithmIdentifier());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Asn1.ASN1EncodableVectorBCFips that = (iText.Bouncycastlefips.Asn1.ASN1EncodableVectorBCFips
                )o;
            return Object.Equals(encodableVector, that.encodableVector);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(encodableVector);
        }

        public override String ToString() {
            return encodableVector.ToString();
        }
    }
}
