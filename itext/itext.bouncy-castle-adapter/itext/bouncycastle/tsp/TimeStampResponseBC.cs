using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TimeStampResponseBC : ITimeStampResponse {
        private readonly TimeStampResponse timeStampResponse;

        public TimeStampResponseBC(TimeStampResponse timeStampRequest) {
            this.timeStampResponse = timeStampRequest;
        }

        public virtual TimeStampResponse GetTimeStampResponse() {
            return timeStampResponse;
        }

        public virtual void Validate(ITimeStampRequest request) {
            try {
                timeStampResponse.Validate(((TimeStampRequestBC)request).GetTimeStampRequest());
            }
            catch (TSPException e) {
                throw new TSPExceptionBC(e);
            }
        }

        public virtual IPKIFailureInfo GetFailInfo() {
            return new PKIFailureInfoBC(timeStampResponse.GetFailInfo());
        }

        public virtual ITimeStampToken GetTimeStampToken() {
            return new TimeStampTokenBC(timeStampResponse.TimeStampToken);
        }

        public virtual String GetStatusString() {
            return timeStampResponse.GetStatusString();
        }

        public virtual byte[] GetEncoded() {
            return timeStampResponse.GetEncoded();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Tsp.TimeStampResponseBC that = (iText.Bouncycastle.Tsp.TimeStampResponseBC)o;
            return Object.Equals(timeStampResponse, that.timeStampResponse);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampResponse);
        }

        public override String ToString() {
            return timeStampResponse.ToString();
        }
    }
}
