using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class DERSetBCFips : ASN1SetBCFips, IDERSet {
        public DERSetBCFips(DerSet derSet)
            : base(derSet) {
        }

        public DERSetBCFips(Asn1EncodableVector vector)
            : base(new DerSet(vector)) {
        }

        public DERSetBCFips(Asn1Encodable encodable)
            : base(new DerSet(encodable)) {
        }

        public virtual DerSet GetDERSet() {
            return (DerSet)GetEncodable();
        }
    }
}
