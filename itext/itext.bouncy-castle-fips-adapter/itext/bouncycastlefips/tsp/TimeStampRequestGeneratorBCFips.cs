using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    public class TimeStampRequestGeneratorBCFips : ITimeStampRequestGenerator {
        private readonly TimeStampRequestGenerator requestGenerator;

        public TimeStampRequestGeneratorBCFips(TimeStampRequestGenerator requestGenerator) {
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
            return new TimeStampRequestBCFips(requestGenerator.Generate(((ASN1ObjectIdentifierBCFips)objectIdentifier)
                .GetASN1ObjectIdentifier(), imprint, nonce));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TimeStampRequestGeneratorBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampRequestGeneratorBCFips
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
