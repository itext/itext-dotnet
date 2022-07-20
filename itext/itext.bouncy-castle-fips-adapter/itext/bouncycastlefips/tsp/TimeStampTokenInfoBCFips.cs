using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastlefips.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>.
    /// </summary>
    public class TimeStampTokenInfoBCFips : ITimeStampTokenInfo {
        private readonly TimeStampTokenInfo timeStampTokenInfo;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>.
        /// </summary>
        /// <param name="timeStampTokenInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>
        /// to be wrapped
        /// </param>
        public TimeStampTokenInfoBCFips(TimeStampTokenInfo timeStampTokenInfo) {
            this.timeStampTokenInfo = timeStampTokenInfo;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampTokenInfo"/>.
        /// </returns>
        public virtual TimeStampTokenInfo GetTimeStampTokenInfo() {
            return timeStampTokenInfo;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBCFips(timeStampTokenInfo.HashAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITSTInfo ToASN1Structure() {
            return new TSTInfoBCFips(timeStampTokenInfo.TstInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetGenTime() {
            return timeStampTokenInfo.GenTime;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampTokenInfo.GetEncoded();
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampTokenInfo);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampTokenInfo.ToString();
        }
    }
}
