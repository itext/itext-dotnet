using  Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class DERSequenceBCFips : ASN1SequenceBCFips, IDERSequence {
        public DERSequenceBCFips(DerSequence derSequence)
            : base(derSequence) {
        }

        public DERSequenceBCFips(Asn1EncodableVector vector)
            : base(new DerSequence(vector)) {
        }

        public DERSequenceBCFips(Asn1Encodable encodable)
            : base(new DerSequence(encodable)) {
        }

        public virtual DerSequence GetDERSequence() {
            return (DerSequence)GetEncodable();
        }
    }
}
