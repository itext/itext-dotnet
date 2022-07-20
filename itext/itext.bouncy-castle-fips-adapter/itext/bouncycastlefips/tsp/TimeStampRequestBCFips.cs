using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    public class TimeStampRequestBCFips : ITimeStampRequest {
        private readonly TimeStampRequest timeStampRequest;

        public TimeStampRequestBCFips(TimeStampRequest timeStampRequest) {
            this.timeStampRequest = timeStampRequest;
        }

        public virtual TimeStampRequest GetTimeStampRequest() {
            return timeStampRequest;
        }

        public virtual byte[] GetEncoded() {
            return timeStampRequest.GetEncoded();
        }

        public virtual BigInteger GetNonce() {
            return timeStampRequest.GetNonce();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TimeStampRequestBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampRequestBCFips
                )o;
            return Object.Equals(timeStampRequest, that.timeStampRequest);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampRequest);
        }

        public override String ToString() {
            return timeStampRequest.ToString();
        }
    }
}
