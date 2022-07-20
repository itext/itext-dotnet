using Org.BouncyCastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    public interface ITimeStampRequest {
        byte[] GetEncoded();

        BigInteger GetNonce();
    }
}
