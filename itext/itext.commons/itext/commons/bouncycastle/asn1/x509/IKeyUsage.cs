using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface IKeyUsage : IASN1Encodable {
        int GetDigitalSignature();

        int GetNonRepudiation();
    }
}
