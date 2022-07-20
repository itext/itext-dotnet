namespace iText.Commons.Bouncycastle.Cert.Ocsp {
    public interface IOCSPRespBuilder {
        int GetSuccessful();

        IOCSPResp Build(int i, IBasicOCSPResp basicOCSPResp);
    }
}
