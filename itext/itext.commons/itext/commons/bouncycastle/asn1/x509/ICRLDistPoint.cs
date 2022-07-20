using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface ICRLDistPoint : IASN1Encodable {
        IDistributionPoint[] GetDistributionPoints();
    }
}
