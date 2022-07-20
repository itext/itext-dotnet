using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface IGeneralNames : IASN1Encodable {
        IGeneralName[] GetNames();
    }
}
