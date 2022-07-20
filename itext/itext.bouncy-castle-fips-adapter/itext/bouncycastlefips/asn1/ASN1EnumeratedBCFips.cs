using  Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1EnumeratedBCFips : ASN1PrimitiveBCFips, IASN1Enumerated {
        public ASN1EnumeratedBCFips(DerEnumerated asn1Enumerated)
            : base(asn1Enumerated) {
        }

        public ASN1EnumeratedBCFips(int i)
            : base(new DerEnumerated(i)) {
        }

        public virtual DerEnumerated GetASN1Enumerated() {
            return (DerEnumerated)GetEncodable();
        }
    }
}
