using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
    /// </summary>
    public class ASN1TaggedObjectBC : ASN1PrimitiveBC, IASN1TaggedObject {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
        /// </summary>
        /// <param name="taggedObject">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>
        /// to be wrapped
        /// </param>
        public ASN1TaggedObjectBC(Asn1TaggedObject taggedObject)
            : base(taggedObject) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
        /// </returns>
        public virtual Asn1TaggedObject GetASN1TaggedObject() {
            return (Asn1TaggedObject)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Primitive GetObject() {
            return new ASN1PrimitiveBC(GetASN1TaggedObject().GetObject());
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetTagNo() {
            return GetASN1TaggedObject().TagNo;
        }
    }
}
