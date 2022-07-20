using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class DERNullBC : ASN1PrimitiveBC, IDERNull {
        public static iText.Bouncycastle.Asn1.DERNullBC INSTANCE = new iText.Bouncycastle.Asn1.DERNullBC();

        private DERNullBC()
            : base(Org.BouncyCastle.Asn1.DerNull.Instance) {
        }

        public DERNullBC(DerNull derNull)
            : base(derNull) {
        }

        public virtual DerNull GetDERNull() {
            return (DerNull)GetPrimitive();
        }
    }
}
