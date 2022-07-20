using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface IKeyPurposeId : IASN1Encodable {
        IKeyPurposeId GetIdKpOCSPSigning();
    }
}
