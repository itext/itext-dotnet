using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TimeStampTokenInfoBC : ITimeStampTokenInfo {
        private readonly TimeStampTokenInfo timeStampTokenInfo;

        public TimeStampTokenInfoBC(TimeStampTokenInfo timeStampTokenInfo) {
            this.timeStampTokenInfo = timeStampTokenInfo;
        }

        public virtual TimeStampTokenInfo GetTimeStampTokenInfo() {
            return timeStampTokenInfo;
        }

        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBC(timeStampTokenInfo.HashAlgorithm);
        }

        public virtual ITSTInfo ToASN1Structure() {
            return new TSTInfoBC(timeStampTokenInfo.TstInfo);
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
            iText.Bouncycastle.Tsp.TimeStampTokenInfoBC that = (iText.Bouncycastle.Tsp.TimeStampTokenInfoBC)o;
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
