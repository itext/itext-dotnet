using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface IAlgorithmIdentifier : IASN1Encodable {
        IASN1ObjectIdentifier GetAlgorithm();
    }
}
