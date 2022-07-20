using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPoint"/>.
    /// </summary>
    public class DistributionPointBCFips : ASN1EncodableBCFips, IDistributionPoint {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPoint"/>.
        /// </summary>
        /// <param name="distributionPoint">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPoint"/>
        /// to be wrapped
        /// </param>
        public DistributionPointBCFips(DistributionPoint distributionPoint)
            : base(distributionPoint) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPoint"/>.
        /// </returns>
        public virtual DistributionPoint GetPoint() {
            return (DistributionPoint)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDistributionPointName GetDistributionPoint() {
            return new DistributionPointNameBCFips(GetPoint().DistributionPointName);
        }
    }
}
