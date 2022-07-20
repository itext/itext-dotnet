using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    public interface ITBSCertificate : IASN1Encodable {
        ISubjectPublicKeyInfo GetSubjectPublicKeyInfo();

        IX500Name GetIssuer();

        IASN1Integer GetSerialNumber();
    }
}
