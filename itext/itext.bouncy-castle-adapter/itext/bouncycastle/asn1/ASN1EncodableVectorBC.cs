using System;
using Org.BouncyCastle.Asn1;
using iText.Bouncycastle.Asn1.Cms;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1EncodableVectorBC : IASN1EncodableVector {
        private readonly Asn1EncodableVector encodableVector;

        public ASN1EncodableVectorBC() {
            encodableVector = new Asn1EncodableVector();
        }

        public ASN1EncodableVectorBC(Asn1EncodableVector encodableVector) {
            this.encodableVector = encodableVector;
        }

        public virtual Asn1EncodableVector GetEncodableVector() {
            return encodableVector;
        }

        public virtual void Add(IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            encodableVector.Add(primitiveBC.GetPrimitive());
        }

        public virtual void Add(IAttribute attribute) {
            AttributeBC attributeBC = (AttributeBC)attribute;
            encodableVector.Add(attributeBC.GetAttribute());
        }

        public virtual void Add(IAlgorithmIdentifier element) {
            AlgorithmIdentifierBC elementBc = (AlgorithmIdentifierBC)element;
            encodableVector.Add(elementBc.GetAlgorithmIdentifier());
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.ASN1EncodableVectorBC that = (iText.Bouncycastle.Asn1.ASN1EncodableVectorBC)o;
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
