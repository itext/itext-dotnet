using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Tsp {
    public interface ITSTInfo : IASN1Encodable {
        IMessageImprint GetMessageImprint();
    }
}
