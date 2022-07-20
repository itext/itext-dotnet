using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TimeStampRequestGeneratorBC : ITimeStampRequestGenerator {
        private readonly TimeStampRequestGenerator requestGenerator;

        public TimeStampRequestGeneratorBC(TimeStampRequestGenerator requestGenerator) {
            this.requestGenerator = requestGenerator;
        }

        public virtual TimeStampRequestGenerator GetRequestGenerator() {
            return requestGenerator;
        }

        public virtual void SetCertReq(bool var1) {
            requestGenerator.SetCertReq(var1);
        }

        public virtual void SetReqPolicy(String reqPolicy) {
            requestGenerator.SetReqPolicy(reqPolicy);
        }

        public virtual ITimeStampRequest Generate(IASN1ObjectIdentifier objectIdentifier, byte[] imprint, BigInteger
             nonce) {
            return new TimeStampRequestBC(requestGenerator.Generate(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier
                (), imprint, nonce));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Tsp.TimeStampRequestGeneratorBC that = (iText.Bouncycastle.Tsp.TimeStampRequestGeneratorBC
                )o;
            return Object.Equals(requestGenerator, that.requestGenerator);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(requestGenerator);
        }

        public override String ToString() {
            return requestGenerator.ToString();
        }
    }
}
