using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TimeStampRequestBC : ITimeStampRequest {
        private readonly TimeStampRequest timeStampRequest;

        public TimeStampRequestBC(TimeStampRequest timeStampRequest) {
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
            iText.Bouncycastle.Tsp.TimeStampRequestBC that = (iText.Bouncycastle.Tsp.TimeStampRequestBC)o;
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
