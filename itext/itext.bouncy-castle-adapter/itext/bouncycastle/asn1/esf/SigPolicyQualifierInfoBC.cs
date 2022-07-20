using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastle.Asn1.Esf {
    public class SigPolicyQualifierInfoBC : ASN1EncodableBC, ISigPolicyQualifierInfo {
        public SigPolicyQualifierInfoBC(SigPolicyQualifierInfo qualifierInfo)
            : base(qualifierInfo) {
        }

        public SigPolicyQualifierInfoBC(IASN1ObjectIdentifier objectIdentifier, IDERIA5String @string)
            : this(new SigPolicyQualifierInfo(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(), ((
                DERIA5StringBC)@string).GetDerIA5String())) {
        }

        public virtual SigPolicyQualifierInfo GetSigPolicyQualifierInfo() {
            return (SigPolicyQualifierInfo)GetEncodable();
        }
    }
}
