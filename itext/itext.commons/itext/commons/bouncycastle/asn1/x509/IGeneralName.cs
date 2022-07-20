using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface IGeneralName : IASN1Encodable {
        int GetTagNo();

        int GetUniformResourceIdentifier();
    }
}
