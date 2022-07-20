using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1UTCTimeBC : ASN1PrimitiveBC, IASN1UTCTime {
        public ASN1UTCTimeBC(DerUtcTime asn1UTCTime)
            : base(asn1UTCTime) {
        }

        public virtual DerUtcTime GetASN1UTCTime() {
            return (DerUtcTime)GetEncodable();
        }
    }
}
