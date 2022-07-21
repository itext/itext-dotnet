using System.Numerics;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    public interface ICertID {
        IASN1ObjectIdentifier GetHashAlgOID();

        IAlgorithmIdentifier GetHashSha1();

        IASN1Integer GetSerialNumber();
    }
}
