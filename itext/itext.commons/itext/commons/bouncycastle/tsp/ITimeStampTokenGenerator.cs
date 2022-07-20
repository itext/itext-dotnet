using System;
using Org.BouncyCastle.Math;
using iText.Commons.Bouncycastle.Cert.Jcajce;

namespace iText.Commons.Bouncycastle.Tsp {
    public interface ITimeStampTokenGenerator {
        void SetAccuracySeconds(int i);

        void AddCertificates(IJcaCertStore jcaCertStore);

        ITimeStampToken Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date);
    }
}
