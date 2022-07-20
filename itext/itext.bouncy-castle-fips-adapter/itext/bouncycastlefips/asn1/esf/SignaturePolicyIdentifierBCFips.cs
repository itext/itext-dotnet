using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    public class SignaturePolicyIdentifierBCFips : ASN1EncodableBCFips, ISignaturePolicyIdentifier {
        public SignaturePolicyIdentifierBCFips(SignaturePolicyIdentifier signaturePolicyIdentifier)
            : base(signaturePolicyIdentifier) {
        }

        public SignaturePolicyIdentifierBCFips(ISignaturePolicyId signaturePolicyId)
            : this(new SignaturePolicyIdentifier(((SignaturePolicyIdBCFips)signaturePolicyId).GetSignaturePolicyId())) {
        }

        public virtual SignaturePolicyIdentifier GetSignaturePolicyIdentifier() {
            return (SignaturePolicyIdentifier)GetEncodable();
        }
    }
}
