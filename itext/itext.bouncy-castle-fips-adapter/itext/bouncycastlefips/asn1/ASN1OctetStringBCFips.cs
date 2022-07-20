using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1OctetStringBCFips : ASN1PrimitiveBCFips, IASN1OctetString {
        public ASN1OctetStringBCFips(Asn1OctetString @string)
            : base(@string) {
        }

        public ASN1OctetStringBCFips(IASN1TaggedObject taggedObject, bool b)
            : base(Asn1OctetString.GetInstance(((ASN1TaggedObjectBCFips)taggedObject).GetTaggedObject(), b)) {
        }

        public virtual Asn1OctetString GetOctetString() {
            return (Asn1OctetString)GetPrimitive();
        }

        public virtual byte[] GetOctets() {
            return GetOctetString().GetOctets();
        }
    }
}
