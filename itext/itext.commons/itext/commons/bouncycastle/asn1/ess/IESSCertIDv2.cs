using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1.Ess {
    public interface IESSCertIDv2 : IASN1Encodable {
        IAlgorithmIdentifier GetHashAlgorithm();

        byte[] GetCertHash();
    }
}
