using System;
using Org.BouncyCastle.Math;

namespace iText.Commons.Bouncycastle.Tsp {
    public interface ITimeStampResponseGenerator {
        ITimeStampResponse Generate(ITimeStampRequest request, BigInteger bigInteger, DateTime date);
    }
}
