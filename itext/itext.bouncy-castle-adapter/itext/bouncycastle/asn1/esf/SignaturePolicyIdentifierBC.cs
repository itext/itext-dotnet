using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastle.Asn1.Esf {
    public class SignaturePolicyIdentifierBC : ASN1EncodableBC, ISignaturePolicyIdentifier {
        public SignaturePolicyIdentifierBC(SignaturePolicyIdentifier signaturePolicyIdentifier)
            : base(signaturePolicyIdentifier) {
        }

        public SignaturePolicyIdentifierBC(ISignaturePolicyId signaturePolicyId)
            : this(new SignaturePolicyIdentifier(((SignaturePolicyIdBC)signaturePolicyId).GetSignaturePolicyId())) {
        }

        public virtual SignaturePolicyIdentifier GetSignaturePolicyIdentifier() {
            return (SignaturePolicyIdentifier)GetEncodable();
        }
    }
}
