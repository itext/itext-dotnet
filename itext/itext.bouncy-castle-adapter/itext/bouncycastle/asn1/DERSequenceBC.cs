using  Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class DERSequenceBC : ASN1SequenceBC, IDERSequence {
        public DERSequenceBC(DerSequence derSequence)
            : base(derSequence) {
        }

        public DERSequenceBC(Asn1EncodableVector vector)
            : base(new DerSequence(vector)) {
        }

        public DERSequenceBC(Asn1Encodable encodable)
            : base(new DerSequence(encodable)) {
        }

        public virtual DerSequence GetDERSequence() {
            return (DerSequence)GetEncodable();
        }
    }
}
