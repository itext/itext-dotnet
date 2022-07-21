using System;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    public interface ISingleResponse {
        ICertID GetCertID();

        ICertStatus GetCertStatus();

        DateTime GetNextUpdate();

        DateTime GetThisUpdate();
    }
}
