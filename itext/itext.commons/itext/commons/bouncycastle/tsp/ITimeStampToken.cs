using iText.Commons.Bouncycastle.Cms;

namespace iText.Commons.Bouncycastle.Tsp {
    public interface ITimeStampToken {
        ITimeStampTokenInfo GetTimeStampInfo();

        void Validate(ISignerInformationVerifier verifier);

        byte[] GetEncoded();
    }
}
