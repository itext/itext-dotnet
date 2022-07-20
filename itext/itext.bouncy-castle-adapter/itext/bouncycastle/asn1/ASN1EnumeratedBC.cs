using  Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1EnumeratedBC : ASN1PrimitiveBC, IASN1Enumerated {
        public ASN1EnumeratedBC(DerEnumerated asn1Enumerated)
            : base(asn1Enumerated) {
        }

        public ASN1EnumeratedBC(int i)
            : base(new DerEnumerated(i)) {
        }

        public virtual DerEnumerated GetASN1Enumerated() {
            return (DerEnumerated)GetEncodable();
        }
    }
}
