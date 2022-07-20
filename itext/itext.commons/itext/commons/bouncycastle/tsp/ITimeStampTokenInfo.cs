using System;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Tsp {
    public interface ITimeStampTokenInfo {
        IAlgorithmIdentifier GetHashAlgorithm();

        ITSTInfo ToASN1Structure();

        DateTime GetGenTime();

        byte[] GetEncoded();
    }
}
