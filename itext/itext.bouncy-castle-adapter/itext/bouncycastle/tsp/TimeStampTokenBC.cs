using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TimeStampTokenBC : ITimeStampToken {
        private readonly TimeStampToken timeStampToken;

        public TimeStampTokenBC(TimeStampToken timeStampToken) {
            this.timeStampToken = timeStampToken;
        }

        public virtual TimeStampToken GetTimeStampToken() {
            return timeStampToken;
        }

        public virtual ITimeStampTokenInfo GetTimeStampInfo() {
            return new TimeStampTokenInfoBC(timeStampToken.TimeStampInfo);
        }

        public virtual byte[] GetEncoded() {
            return timeStampToken.GetEncoded();
        }

        public virtual void Validate(ISignerInformationVerifier verifier) {
            try {
                timeStampToken.Validate(((SignerInformationVerifierBC)verifier).GetVerifier());
            }
            catch (TspException e) {
                throw new TSPExceptionBC(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Tsp.TimeStampTokenBC that = (iText.Bouncycastle.Tsp.TimeStampTokenBC)o;
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
