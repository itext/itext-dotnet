using System;
using Org.BouncyCastle.Tsp;
using iText.Bouncycastlefips.Asn1.Cmp;
using iText.Commons.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>.
    /// </summary>
    public class TimeStampResponseBCFips : ITimeStampResponse {
        private readonly TimeStampResponse timeStampResponse;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>.
        /// </summary>
        /// <param name="timeStampResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>
        /// to be wrapped
        /// </param>
        public TimeStampResponseBCFips(TimeStampResponse timeStampResponse) {
            this.timeStampResponse = timeStampResponse;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampResponse"/>.
        /// </returns>
        public virtual TimeStampResponse GetTimeStampResponse() {
            return timeStampResponse;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Validate(ITimeStampRequest request) {
            try {
                timeStampResponse.Validate(((TimeStampRequestBCFips)request).GetTimeStampRequest());
            }
            catch (TspException e) {
                throw new TSPExceptionBCFips(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IPKIFailureInfo GetFailInfo() {
            return new PKIFailureInfoBCFips(timeStampResponse.GetFailInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampToken GetTimeStampToken() {
            return new TimeStampTokenBCFips(timeStampResponse.TimeStampToken);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetStatusString() {
            return timeStampResponse.GetStatusString();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampResponse.GetEncoded();
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
            iText.Bouncycastlefips.Tsp.TimeStampResponseBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampResponseBCFips
                )o;
            return Object.Equals(timeStampResponse, that.timeStampResponse);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampResponse);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampResponse.ToString();
        }
    }
}
