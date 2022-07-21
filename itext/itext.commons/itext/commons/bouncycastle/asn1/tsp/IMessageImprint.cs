using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1.Tsp {
    public interface IMessageImprint : IASN1Encodable {
        byte[] GetHashedMessage();

        IAlgorithmIdentifier GetHashAlgorithm();
    }
}
