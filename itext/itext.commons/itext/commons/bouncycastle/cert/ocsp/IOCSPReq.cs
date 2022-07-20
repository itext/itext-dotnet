using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface IOCSPReq {
        byte[] GetEncoded();

        IReq[] GetRequestList();

        IExtension GetExtension(IASN1ObjectIdentifier objectIdentifier);
    }
}
