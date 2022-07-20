using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastlefips.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    public class TimeStampTokenInfoBCFips : ITimeStampTokenInfo {
        private readonly TimeStampTokenInfo timeStampTokenInfo;

        public TimeStampTokenInfoBCFips(TimeStampTokenInfo timeStampTokenInfo) {
            this.timeStampTokenInfo = timeStampTokenInfo;
        }

        public virtual TimeStampTokenInfo GetTimeStampTokenInfo() {
            return timeStampTokenInfo;
        }

        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBCFips(timeStampTokenInfo.HashAlgorithm);
        }

        public virtual ITSTInfo ToASN1Structure() {
            return new TSTInfoBCFips(timeStampTokenInfo.TstInfo);
        }

        public virtual DateTime GetGenTime() {
            return timeStampTokenInfo.GenTime;
        }

        public virtual byte[] GetEncoded() {
            return timeStampTokenInfo.GetEncoded();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TimeStampTokenInfoBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampTokenInfoBCFips
                )o;
            return Object.Equals(timeStampTokenInfo, that.timeStampTokenInfo);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampTokenInfo);
        }

        public override String ToString() {
            return timeStampTokenInfo.ToString();
        }
    }
}
