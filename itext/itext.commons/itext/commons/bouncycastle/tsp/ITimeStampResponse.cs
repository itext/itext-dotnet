using System;
using iText.Commons.Bouncycastle.Asn1.Cmp;

namespace iText.Commons.Bouncycastle.Tsp {
    public interface ITimeStampResponse {
        void Validate(ITimeStampRequest request);

        IPKIFailureInfo GetFailInfo();

        ITimeStampToken GetTimeStampToken();

        String GetStatusString();

        byte[] GetEncoded();
    }
}
