using System;
using Org.BouncyCastle.Asn1;
using iText.Bouncycastle.Asn1.Cms;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>.
    /// </summary>
    public class ASN1EncodableVectorBC : IASN1EncodableVector {
        private readonly Asn1EncodableVector encodableVector;

        /// <summary>
        /// Creates new wrapper instance for new
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>
        /// object.
        /// </summary>
        public ASN1EncodableVectorBC() {
            encodableVector = new Asn1EncodableVector();
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>.
        /// </summary>
        /// <param name="encodableVector">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>
        /// to be wrapped
        /// </param>
        public ASN1EncodableVectorBC(Asn1EncodableVector encodableVector) {
            this.encodableVector = encodableVector;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>.
        /// </returns>
        public virtual Asn1EncodableVector GetEncodableVector() {
            return encodableVector;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            encodableVector.Add(primitiveBC.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(IAttribute attribute) {
            AttributeBC attributeBC = (AttributeBC)attribute;
            encodableVector.Add(attributeBC.GetAttribute());
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(IAlgorithmIdentifier element) {
            AlgorithmIdentifierBC elementBc = (AlgorithmIdentifierBC)element;
            encodableVector.Add(elementBc.GetAlgorithmIdentifier());
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(encodableVector);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return encodableVector.ToString();
        }
    }
}
