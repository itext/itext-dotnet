using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    public interface ISigningCertificateV2 : IASN1Encodable {
        IESSCertIDv2[] GetCerts();
    }
}
