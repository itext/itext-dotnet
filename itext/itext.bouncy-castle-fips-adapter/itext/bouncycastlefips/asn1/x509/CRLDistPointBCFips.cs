using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>.
    /// </summary>
    public class CRLDistPointBCFips : ASN1EncodableBCFips, ICRLDistPoint {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>.
        /// </summary>
        /// <param name="crlDistPoint">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>
        /// to be wrapped
        /// </param>
        public CRLDistPointBCFips(CrlDistPoint crlDistPoint)
            : base(crlDistPoint) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>.
        /// </returns>
        public virtual CrlDistPoint GetCrlDistPoint() {
            return (CrlDistPoint)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
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
