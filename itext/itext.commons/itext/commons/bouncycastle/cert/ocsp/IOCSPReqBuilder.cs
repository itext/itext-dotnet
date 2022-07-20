using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface IOCSPReqBuilder {
        IOCSPReqBuilder SetRequestExtensions(IExtensions extensions);

        IOCSPReqBuilder AddRequest(ICertificateID certificateID);

        IOCSPReq Build();
    }
}
