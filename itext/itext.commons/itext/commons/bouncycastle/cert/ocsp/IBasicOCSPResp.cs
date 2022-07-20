using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface IBasicOCSPResp {
        ISingleResp[] GetResponses();

        bool IsSignatureValid(IContentVerifierProvider provider);

        IX509CertificateHolder[] GetCerts();

        byte[] GetEncoded();

        DateTime GetProducedAt();
    }
}
