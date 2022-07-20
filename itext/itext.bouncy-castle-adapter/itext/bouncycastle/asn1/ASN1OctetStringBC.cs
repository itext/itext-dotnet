using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1OctetStringBC : ASN1PrimitiveBC, IASN1OctetString {
        public ASN1OctetStringBC(Asn1OctetString @string)
            : base(@string) {
        }

        public ASN1OctetStringBC(IASN1TaggedObject taggedObject, bool b)
            : base(Asn1OctetString.GetInstance(((ASN1TaggedObjectBC)taggedObject).GetASN1TaggedObject(), b)) {
        }

        public virtual Asn1OctetString GetASN1OctetString() {
            return (Asn1OctetString)GetPrimitive();
        }

        public virtual byte[] GetOctets() {
            return GetASN1OctetString().GetOctets();
        }
    }
}
