using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class CRLDistPointBC : ASN1EncodableBC, ICRLDistPoint {
        public CRLDistPointBC(CrlDistPoint crlDistPoint)
            : base(crlDistPoint) {
        }

        public virtual CrlDistPoint GetCrlDistPoint() {
            return (CrlDistPoint)GetEncodable();
        }

        public virtual IDistributionPoint[] GetDistributionPoints() {
            DistributionPoint[] distributionPoints = GetCrlDistPoint().GetDistributionPoints();
            IDistributionPoint[] distributionPointsBC = new DistributionPointBC[distributionPoints.Length];
            for (int i = 0; i < distributionPoints.Length; ++i) {
                distributionPointsBC[i] = new DistributionPointBC(distributionPoints[i]);
            }
            return distributionPointsBC;
        }
    }
}
