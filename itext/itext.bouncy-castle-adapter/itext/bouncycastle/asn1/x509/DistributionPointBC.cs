using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class DistributionPointBC : ASN1EncodableBC, IDistributionPoint {
        public DistributionPointBC(DistributionPoint distributionPoint)
            : base(distributionPoint) {
        }

        public virtual DistributionPoint GetPoint() {
            return (DistributionPoint)GetEncodable();
        }

        public virtual IDistributionPointName GetDistributionPoint() {
            return new DistributionPointNameBC(GetPoint().DistributionPointName);
        }
    }
}
