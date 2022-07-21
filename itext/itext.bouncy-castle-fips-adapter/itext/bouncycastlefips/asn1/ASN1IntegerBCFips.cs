using iText.Bouncycastlefips.Math;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1IntegerBCFips : ASN1PrimitiveBCFips, IASN1Integer {
        public ASN1IntegerBCFips(DerInteger i)
            : base(i) {
        }

        public ASN1IntegerBCFips(int i)
            : base(new DerInteger(i)) {
        }

        public ASN1IntegerBCFips(IBigInteger i)
            : base(new DerInteger(((BigIntegerBCFips) i).GetBigInteger())) {
        }

        public virtual DerInteger GetASN1Integer() {
            return (DerInteger)GetPrimitive();
        }

        public virtual IBigInteger GetValue() {
            return new BigIntegerBCFips(GetASN1Integer().Value);
        }
    }
}
