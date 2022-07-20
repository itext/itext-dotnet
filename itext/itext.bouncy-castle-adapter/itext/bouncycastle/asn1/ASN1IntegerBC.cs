using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1IntegerBC : ASN1PrimitiveBC, IASN1Integer {
        public ASN1IntegerBC(DerInteger i)
            : base(i) {
        }

        public ASN1IntegerBC(int i)
            : base(new DerInteger(i)) {
        }

        public ASN1IntegerBC(BigInteger i)
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
