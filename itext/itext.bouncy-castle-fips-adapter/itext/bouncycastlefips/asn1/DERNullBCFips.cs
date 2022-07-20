using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class DERNullBCFips : ASN1PrimitiveBCFips, IDERNull {
        public static iText.Bouncycastlefips.Asn1.DERNullBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.DERNullBCFips
            ();

        private DERNullBCFips()
            : base(Org.BouncyCastle.Asn1.DerNull.Instance) {
        }

        public DERNullBCFips(DerNull derNull)
            : base(derNull) {
        }

        public virtual DerNull GetDERNull() {
            return (DerNull)GetPrimitive();
        }
    }
}
