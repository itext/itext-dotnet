using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using Org.BouncyCastle.Asn1;

namespace iText.Bouncycastlefips.Asn1.Esf {
    public class SigPolicyQualifiersBCFips : ASN1EncodableBCFips, ISigPolicyQualifiers {
        public SigPolicyQualifiersBCFips(params SigPolicyQualifierInfo[] sigPolicyQualifiers)
            : base(new DerSequence(sigPolicyQualifiers)) {
        }

        public virtual SigPolicyQualifierInfo[] GetSigPolityQualifiers()
        {
            Asn1Sequence sigPolicyQualifiers = (Asn1Sequence)GetEncodable();
            SigPolicyQualifierInfo[] policyQualifiers = new SigPolicyQualifierInfo[sigPolicyQualifiers.Count];
            for (int index = 0; index < sigPolicyQualifiers.Count; ++index)
                policyQualifiers[index] = SigPolicyQualifierInfo.GetInstance((object) sigPolicyQualifiers[index]);
            return policyQualifiers;
        }
    }
}
