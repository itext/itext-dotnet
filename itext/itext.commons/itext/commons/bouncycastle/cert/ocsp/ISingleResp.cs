using System;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface ISingleResp {
        ICertificateID GetCertID();

        ICertificateStatus GetCertStatus();

        DateTime GetNextUpdate();

        DateTime GetThisUpdate();
    }
}
