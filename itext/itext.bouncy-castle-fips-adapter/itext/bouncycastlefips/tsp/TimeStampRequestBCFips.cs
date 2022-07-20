using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
    /// </summary>
    public class TimeStampRequestBCFips : ITimeStampRequest {
        private readonly TimeStampRequest timeStampRequest;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
        /// </summary>
        /// <param name="timeStampRequest">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>
        /// to be wrapped
        /// </param>
        public TimeStampRequestBCFips(TimeStampRequest timeStampRequest) {
            this.timeStampRequest = timeStampRequest;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampRequest"/>.
        /// </returns>
        public virtual TimeStampRequest GetTimeStampRequest() {
            return timeStampRequest;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampRequest.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual BigInteger GetNonce() {
            return timeStampRequest.GetNonce();
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
            iText.Bouncycastlefips.Tsp.TimeStampRequestBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampRequestBCFips
                )o;
            return Object.Equals(timeStampRequest, that.timeStampRequest);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampRequest);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampRequest.ToString();
        }
    }
}
