using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface ISubjectPublicKeyInfo : IASN1Encodable {
        IAlgorithmIdentifier GetAlgorithm();
    }
}
