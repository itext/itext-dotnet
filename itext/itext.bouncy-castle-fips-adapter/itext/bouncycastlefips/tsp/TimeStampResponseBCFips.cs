using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastlefips.Asn1.Cmp;
using iText.Commons.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    public class TimeStampResponseBCFips : ITimeStampResponse {
        private readonly TimeStampResponse timeStampResponse;

        public TimeStampResponseBCFips(TimeStampResponse timeStampResponse) {
            this.timeStampResponse = timeStampResponse;
        }

        public virtual TimeStampResponse GetTimeStampResponse() {
            return timeStampResponse;
        }

        public virtual void Validate(ITimeStampRequest request) {
            try {
                timeStampResponse.Validate(((TimeStampRequestBCFips)request).GetTimeStampRequest());
            }
            catch (TSPException e) {
                throw new TSPExceptionBCFips(e);
            }
        }

        public virtual IPKIFailureInfo GetFailInfo() {
            return new PKIFailureInfoBCFips(timeStampResponse.GetFailInfo());
        }

        public virtual ITimeStampToken GetTimeStampToken() {
            return new TimeStampTokenBCFips(timeStampResponse.TimeStampToken);
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
            iText.Bouncycastlefips.Tsp.TimeStampResponseBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampResponseBCFips
                )o;
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
