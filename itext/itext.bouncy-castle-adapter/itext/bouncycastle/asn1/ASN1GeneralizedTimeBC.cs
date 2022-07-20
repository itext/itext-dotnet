using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1GeneralizedTimeBC : ASN1PrimitiveBC, IASN1GeneralizedTime {
        public ASN1GeneralizedTimeBC(DerGeneralizedTime asn1GeneralizedTime)
            : base(asn1GeneralizedTime) {
        }

        public virtual DerGeneralizedTime GetASN1GeneralizedTime() {
            return (DerGeneralizedTime)GetEncodable();
        }
    }
}
