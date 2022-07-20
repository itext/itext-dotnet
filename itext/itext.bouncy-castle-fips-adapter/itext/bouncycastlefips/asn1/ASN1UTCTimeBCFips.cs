using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1UTCTimeBCFips : ASN1PrimitiveBCFips, IASN1UTCTime {
        public ASN1UTCTimeBCFips(DerUtcTime asn1UTCTime)
            : base(asn1UTCTime) {
        }

        public virtual DerUtcTime GetASN1UTCTime() {
            return (DerUtcTime)GetEncodable();
        }
    }
}
