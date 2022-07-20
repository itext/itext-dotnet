using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1IntegerBCFips : ASN1PrimitiveBCFips, IASN1Integer {
        public ASN1IntegerBCFips(DerInteger i)
            : base(i) {
        }

        public ASN1IntegerBCFips(int i)
            : base(new DerInteger(i)) {
        }

        public ASN1IntegerBCFips(BigInteger i)
            : base(new DerInteger(i)) {
        }

        public virtual DerInteger GetASN1Integer() {
            return (DerInteger)GetPrimitive();
        }

        public virtual BigInteger GetValue() {
            return GetASN1Integer().Value;
        }
    }
}
