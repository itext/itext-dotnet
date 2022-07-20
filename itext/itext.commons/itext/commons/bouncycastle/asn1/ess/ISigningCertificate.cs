using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    public interface ISigningCertificate : IASN1Encodable {
        IESSCertID[] GetCerts();
    }
}
