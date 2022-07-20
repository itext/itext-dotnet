using System;
using Org.BouncyCastle.Math;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Tsp {
    public interface ITimeStampRequestGenerator {
        void SetCertReq(bool var1);

        void SetReqPolicy(String reqPolicy);

        ITimeStampRequest Generate(IASN1ObjectIdentifier objectIdentifier, byte[] imprint, BigInteger nonce);
    }
}
