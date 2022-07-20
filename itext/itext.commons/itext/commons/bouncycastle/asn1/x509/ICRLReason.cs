using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface ICRLReason : IASN1Encodable {
        int GetKeyCompromise();
    }
}
