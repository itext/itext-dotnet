using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastle.Asn1.Esf {
    public class SignaturePolicyIdBC : ASN1EncodableBC, ISignaturePolicyId {
        public SignaturePolicyIdBC(SignaturePolicyId signaturePolicyId)
            : base(signaturePolicyId) {
        }

        public SignaturePolicyIdBC(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue, ISigPolicyQualifiers
             policyQualifiers)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBC
                )algAndValue).GetOtherHashAlgAndValue(), ((SigPolicyQualifiersBC)policyQualifiers).GetSigPolityQualifiers
                ())) {
        }

        public SignaturePolicyIdBC(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBC
                )algAndValue).GetOtherHashAlgAndValue())) {
        }

        public virtual SignaturePolicyId GetSignaturePolicyId() {
            return (SignaturePolicyId)GetEncodable();
        }
    }
}
