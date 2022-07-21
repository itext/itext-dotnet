using iText.Bouncycastle.Math;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1IntegerBC : ASN1PrimitiveBC, IASN1Integer {
        public ASN1IntegerBC(DerInteger i)
            : base(i) {
        }

        public ASN1IntegerBC(int i)
            : base(new DerInteger(i)) {
        }

        public ASN1IntegerBC(IBigInteger i)
            : base(new DerInteger(((BigIntegerBC) i).GetBigInteger())) {
        }

        public virtual DerInteger GetASN1Integer() {
            return (DerInteger)GetPrimitive();
        }

        public virtual IBigInteger GetValue() {
            return new BigIntegerBC(GetASN1Integer().Value);
        }
    }
}
