using System;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface IBasicOCSPRespBuilder {
        IBasicOCSPRespBuilder SetResponseExtensions(IExtensions extensions);

        IBasicOCSPRespBuilder AddResponse(ICertificateID certID, ICertificateStatus certificateStatus, DateTime time
            , DateTime time1, IExtensions extensions);

        IBasicOCSPResp Build(IContentSigner signer, IX509CertificateHolder[] chain, DateTime time);
    }
}
