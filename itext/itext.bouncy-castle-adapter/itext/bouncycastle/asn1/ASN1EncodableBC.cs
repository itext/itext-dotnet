using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>.
    /// </summary>
    public class ASN1EncodableBC : IASN1Encodable {
        private readonly Asn1Encodable encodable;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>.
        /// </summary>
        /// <param name="encodable">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>
        /// to be wrapped
        /// </param>
        public ASN1EncodableBC(Asn1Encodable encodable) {
            this.encodable = encodable;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>.
        /// </returns>
        public virtual Asn1Encodable GetEncodable() {
            return encodable;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Primitive ToASN1Primitive() {
            return new ASN1PrimitiveBC(encodable.ToAsn1Object());
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsNull() {
            return encodable == null;
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
            iText.Bouncycastle.Asn1.ASN1EncodableBC that = (iText.Bouncycastle.Asn1.ASN1EncodableBC)o;
            return Object.Equals(encodable, that.encodable);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(encodable);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return encodable.ToString();
        }
    }
}
