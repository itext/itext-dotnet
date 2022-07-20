using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    public interface IOCSPResponseStatus : IASN1Encodable {
        int GetSuccessful();
    }
}
