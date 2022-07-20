using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    public class SigPolicyQualifiersBCFips : ASN1EncodableBCFips, ISigPolicyQualifiers {
        public SigPolicyQualifiersBCFips(SigPolicyQualifiers policyQualifiers)
            : base(policyQualifiers) {
        }

        public SigPolicyQualifiersBCFips(params SigPolicyQualifierInfo[] qualifierInfo)
            : base(new SigPolicyQualifiers(qualifierInfo)) {
        }

        public virtual SigPolicyQualifiers GetSigPolityQualifiers() {
            return (SigPolicyQualifiers)GetEncodable();
        }
    }
}
