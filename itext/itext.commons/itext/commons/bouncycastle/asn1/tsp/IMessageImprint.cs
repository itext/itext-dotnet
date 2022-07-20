using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Tsp {
    public interface IMessageImprint : IASN1Encodable {
        byte[] GetHashedMessage();
    }
}
