using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastle.Asn1.Esf {
    public class SigPolicyQualifiersBC : ASN1EncodableBC, ISigPolicyQualifiers {
        public SigPolicyQualifiersBC(SigPolicyQualifiers policyQualifiers)
            : base(policyQualifiers) {
        }

        public SigPolicyQualifiersBC(params SigPolicyQualifierInfo[] qualifierInfo)
            : base(new SigPolicyQualifiers(qualifierInfo)) {
        }

        public virtual SigPolicyQualifiers GetSigPolityQualifiers() {
            return (SigPolicyQualifiers)GetEncodable();
        }
    }
}
