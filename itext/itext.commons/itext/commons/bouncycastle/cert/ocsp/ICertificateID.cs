using Org.BouncyCastle.Math;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface ICertificateID {
        IASN1ObjectIdentifier GetHashAlgOID();

        IAlgorithmIdentifier GetHashSha1();

        bool MatchesIssuer(IX509CertificateHolder certificateHolder, IDigestCalculatorProvider provider);

        BigInteger GetSerialNumber();
    }
}
