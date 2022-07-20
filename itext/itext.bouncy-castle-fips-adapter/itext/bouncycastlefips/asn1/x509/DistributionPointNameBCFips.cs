using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>.
    /// </summary>
    public class DistributionPointNameBCFips : ASN1EncodableBCFips, IDistributionPointName {
        private static readonly iText.Bouncycastlefips.Asn1.X509.DistributionPointNameBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.DistributionPointNameBCFips
            (null);

        private const int FULL_NAME = DistributionPointName.FullName;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>.
        /// </summary>
        /// <param name="distributionPointName">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>
        /// to be wrapped
        /// </param>
        public DistributionPointNameBCFips(DistributionPointName distributionPointName)
            : base(distributionPointName) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="DistributionPointNameBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.X509.DistributionPointNameBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>.
        /// </returns>
        public virtual DistributionPointName GetDistributionPointName() {
            return (DistributionPointName)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetType() {
            return GetDistributionPointName().PointType;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encodable GetName() {
            return new ASN1EncodableBCFips(GetDistributionPointName().Name);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetFullName() {
            return FULL_NAME;
        }
    }
}
