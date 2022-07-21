using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Cert {
    public interface IX509Certificate {
        IASN1Encodable GetIssuerDN();
        IBigInteger GetSerialNumber();
        IPublicKey GetPublicKey();
        
        byte[] GetEncoded();
        byte[] GetTbsCertificate();
    }
}
