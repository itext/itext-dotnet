using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastlefips.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    public class TimeStampTokenBCFips : ITimeStampToken {
        private readonly TimeStampToken timeStampToken;

        public TimeStampTokenBCFips(TimeStampToken timeStampToken) {
            this.timeStampToken = timeStampToken;
        }

        public virtual TimeStampToken GetTimeStampToken() {
            return timeStampToken;
        }

        public virtual ITimeStampTokenInfo GetTimeStampInfo() {
            return new TimeStampTokenInfoBCFips(timeStampToken.TimeStampInfo);
        }

        public virtual byte[] GetEncoded() {
            return timeStampToken.GetEncoded();
        }

        public virtual void Validate(ISignerInformationVerifier verifier) {
            try {
                timeStampToken.Validate(((SignerInformationVerifierBCFips)verifier).GetVerifier());
            }
            catch (TSPException e) {
                throw new TSPExceptionBCFips(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TimeStampTokenBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampTokenBCFips)o;
            return Object.Equals(timeStampToken, that.timeStampToken);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampToken);
        }

        public override String ToString() {
            return timeStampToken.ToString();
        }
    }
}
