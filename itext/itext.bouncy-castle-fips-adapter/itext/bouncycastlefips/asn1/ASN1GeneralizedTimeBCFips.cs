using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1GeneralizedTimeBCFips : ASN1PrimitiveBCFips, IASN1GeneralizedTime {
        public ASN1GeneralizedTimeBCFips(DerGeneralizedTime asn1GeneralizedTime)
            : base(asn1GeneralizedTime) {
        }

        public virtual DerGeneralizedTime GetASN1GeneralizedTime() {
            return (DerGeneralizedTime)GetEncodable();
        }
    }
}
