using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class DistributionPointBCFips : ASN1EncodableBCFips, IDistributionPoint {
        public DistributionPointBCFips(DistributionPoint distributionPoint)
            : base(distributionPoint) {
        }

        public virtual DistributionPoint GetPoint() {
            return (DistributionPoint)GetEncodable();
        }

        public virtual IDistributionPointName GetDistributionPoint() {
            return new DistributionPointNameBCFips(GetPoint().DistributionPointName);
        }
    }
}
