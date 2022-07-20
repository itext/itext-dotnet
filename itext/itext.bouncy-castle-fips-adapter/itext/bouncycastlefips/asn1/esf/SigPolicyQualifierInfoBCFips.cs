using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;

namespace iText.Bouncycastlefips.Asn1.Esf {
    public class SigPolicyQualifierInfoBCFips : ASN1EncodableBCFips, ISigPolicyQualifierInfo {
        public SigPolicyQualifierInfoBCFips(SigPolicyQualifierInfo qualifierInfo)
            : base(qualifierInfo) {
        }

        public SigPolicyQualifierInfoBCFips(IASN1ObjectIdentifier objectIdentifier, IDERIA5String @string)
            : this(new SigPolicyQualifierInfo(((ASN1ObjectIdentifierBCFips)objectIdentifier).GetASN1ObjectIdentifier()
                , ((DERIA5StringBCFips)@string).GetDerIA5String())) {
        }

        public virtual SigPolicyQualifierInfo GetQualifierInfo() {
            return (SigPolicyQualifierInfo)GetEncodable();
        }
    }
}
