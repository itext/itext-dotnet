using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class DistributionPointNameBC : ASN1EncodableBC, IDistributionPointName {
        private static readonly iText.Bouncycastle.Asn1.X509.DistributionPointNameBC INSTANCE = new iText.Bouncycastle.Asn1.X509.DistributionPointNameBC
            (null);

        private const int FULL_NAME = DistributionPointName.FullName;

        public DistributionPointNameBC(DistributionPointName distributionPointName)
            : base(distributionPointName) {
        }

        public static iText.Bouncycastle.Asn1.X509.DistributionPointNameBC GetInstance() {
            return INSTANCE;
        }

        public virtual DistributionPointName GetDistributionPointName() {
            return (DistributionPointName)GetEncodable();
        }

        public virtual int GetType() {
            return GetDistributionPointName().PointType;
        }

        public virtual IASN1Encodable GetName() {
            return new ASN1EncodableBC(GetDistributionPointName().Name);
        }

        public virtual int GetFullName() {
            return FULL_NAME;
        }
    }
}
