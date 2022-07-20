using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    public class SignaturePolicyIdBCFips : ASN1EncodableBCFips, ISignaturePolicyId {
        public SignaturePolicyIdBCFips(SignaturePolicyId signaturePolicyId)
            : base(signaturePolicyId) {
        }

        public SignaturePolicyIdBCFips(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue, 
            ISigPolicyQualifiers policyQualifiers)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBCFips
                )algAndValue).GetOtherHashAlgAndValue(), ((SigPolicyQualifiersBCFips)policyQualifiers).GetSigPolityQualifiers
                ())) {
        }

        public SignaturePolicyIdBCFips(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue)
            : this(new SignaturePolicyId(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier(), ((OtherHashAlgAndValueBCFips
                )algAndValue).GetOtherHashAlgAndValue())) {
        }

        public virtual SignaturePolicyId GetSignaturePolicyId() {
            return (SignaturePolicyId)GetEncodable();
        }
    }
}
