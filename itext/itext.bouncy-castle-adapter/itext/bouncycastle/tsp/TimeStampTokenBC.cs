using System;
using iText.Bouncycastle.Asn1.Tsp;
using iText.Bouncycastle.Cert;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampToken"/>.
    /// </summary>
    public class TimeStampTokenBC : ITimeStampToken {
        private readonly TimeStampToken timeStampToken;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampToken"/>.
        /// </summary>
        /// <param name="timeStampToken">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampToken"/>
        /// to be wrapped
        /// </param>
        public TimeStampTokenBC(TimeStampToken timeStampToken) {
            this.timeStampToken = timeStampToken;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TimeStampToken"/>.
        /// </returns>
        public virtual TimeStampToken GetTimeStampToken() {
            return timeStampToken;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITSTInfo GetTimeStampInfo() {
            return new TSTInfoBC(timeStampToken.TimeStampInfo.TstInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampToken.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public void Validate(IX509Certificate cert) {
            try {
                timeStampToken.Validate(((X509CertificateBC) cert).GetCertificate());
            } catch (TspException e) {
                throw new TSPExceptionBC(e);
            }
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
            iText.Bouncycastle.Tsp.TimeStampTokenBC that = (iText.Bouncycastle.Tsp.TimeStampTokenBC)o;
            return Object.Equals(timeStampToken, that.timeStampToken);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampToken);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampToken.ToString();
        }
    }
}
