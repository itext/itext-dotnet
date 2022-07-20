using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface IDistributionPointName : IASN1Encodable {
        int GetType();

        IASN1Encodable GetName();

        int GetFullName();
    }
}
