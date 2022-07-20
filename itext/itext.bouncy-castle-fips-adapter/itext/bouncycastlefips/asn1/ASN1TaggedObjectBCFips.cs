using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
    /// </summary>
    public class ASN1TaggedObjectBCFips : ASN1PrimitiveBCFips, IASN1TaggedObject {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
        /// </summary>
        /// <param name="taggedObject">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>
        /// to be wrapped
        /// </param>
        public ASN1TaggedObjectBCFips(Asn1TaggedObject taggedObject)
            : base(taggedObject) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
        /// </returns>
        public virtual Asn1TaggedObject GetTaggedObject() {
            return (Asn1TaggedObject)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Primitive GetObject() {
            return new ASN1PrimitiveBCFips(GetTaggedObject().GetObject());
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetTagNo() {
            return GetTaggedObject().TagNo;
        }
    }
}
