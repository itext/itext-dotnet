using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class DERSetBC : ASN1SetBC, IDERSet {
        public DERSetBC(DerSet derSet)
            : base(derSet) {
        }

        public DERSetBC(Asn1EncodableVector vector)
            : base(new DerSet(vector)) {
        }

        public DERSetBC(Asn1Encodable encodable)
            : base(new DerSet(encodable)) {
        }

        public virtual DerSet GetDERSet() {
            return (DerSet)GetEncodable();
        }
    }
}
