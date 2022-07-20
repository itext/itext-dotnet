using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class DistributionPointNameBCFips : ASN1EncodableBCFips, IDistributionPointName {
        private static readonly iText.Bouncycastlefips.Asn1.X509.DistributionPointNameBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.DistributionPointNameBCFips
            (null);

        private const int FULL_NAME = DistributionPointName.FullName;

        public DistributionPointNameBCFips(DistributionPointName distributionPointName)
            : base(distributionPointName) {
        }

        public static iText.Bouncycastlefips.Asn1.X509.DistributionPointNameBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual DistributionPointName GetDistributionPointName() {
            return (DistributionPointName)GetEncodable();
        }

        public virtual int GetType() {
            return GetDistributionPointName().PointType;
        }

        public virtual IASN1Encodable GetName() {
            return new ASN1EncodableBCFips(GetDistributionPointName().Name);
        }

        public virtual int GetFullName() {
            return FULL_NAME;
        }
    }
}
