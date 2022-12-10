using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
    /// </summary>
    public class ASN1OctetStringBCFips : ASN1PrimitiveBCFips, IASN1OctetString {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
        /// </summary>
        /// <param name="string">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>
        /// to be wrapped
        /// </param>
        public ASN1OctetStringBCFips(Asn1OctetString @string)
            : base(@string) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
        /// </summary>
        /// <param name="taggedObject">
        /// ASN1TaggedObject wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>
        /// </param>
        /// <param name="b">
        /// boolean to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>
        /// </param>
        public ASN1OctetStringBCFips(IASN1TaggedObject taggedObject, bool b)
            : base(Asn1OctetString.GetInstance(((ASN1TaggedObjectBCFips)taggedObject).GetTaggedObject(), b)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
        /// </returns>
        public virtual Asn1OctetString GetOctetString() {
            return (Asn1OctetString)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetOctets() {
            return GetOctetString().GetOctets();
        }
        
        /// <summary><inheritDoc/></summary>
        public byte[] GetDerEncoded() {
            return !IsNull() ? GetOctetString().GetDerEncoded() : null;
        }
    }
}
