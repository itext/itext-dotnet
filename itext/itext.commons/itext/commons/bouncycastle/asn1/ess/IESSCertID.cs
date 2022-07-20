using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    public interface IESSCertID : IASN1Encodable {
        byte[] GetCertHash();
    }
}
