using System;

namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface IOCSPResp {
        byte[] GetEncoded();

        int GetStatus();

        Object GetResponseObject();

        int GetSuccessful();
    }
}
