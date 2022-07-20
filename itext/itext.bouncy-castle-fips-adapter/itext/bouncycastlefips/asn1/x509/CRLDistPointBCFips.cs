using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class CRLDistPointBCFips : ASN1EncodableBCFips, ICRLDistPoint {
        public CRLDistPointBCFips(CrlDistPoint crlDistPoint)
            : base(crlDistPoint) {
        }

        public virtual CrlDistPoint GetCrlDistPoint() {
            return (CrlDistPoint)GetEncodable();
        }

        public virtual IDistributionPoint[] GetDistributionPoints() {
            DistributionPoint[] distributionPoints = GetCrlDistPoint().GetDistributionPoints();
            IDistributionPoint[] distributionPointsBC = new DistributionPointBCFips[distributionPoints.Length];
            for (int i = 0; i < distributionPoints.Length; ++i) {
                distributionPointsBC[i] = new DistributionPointBCFips(distributionPoints[i]);
            }
            return distributionPointsBC;
        }
    }
}
