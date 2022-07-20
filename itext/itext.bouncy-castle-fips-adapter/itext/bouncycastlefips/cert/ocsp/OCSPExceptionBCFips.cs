using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OcspException"/>.
    /// </summary>
    public class OCSPExceptionBCFips : AbstractOCSPException {
        private readonly OcspException exception;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspException"/>.
        /// </summary>
        /// <param name="exception">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.OcspException"/>
        /// to be wrapped
        /// </param>
        public OCSPExceptionBCFips(OcspException exception) {
            this.exception = exception;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.OcspException"/>.
        /// </returns>
        public virtual OcspException GetException() {
            return exception;
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPExceptionBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPExceptionBCFips
                )o;
            return Object.Equals(exception, that.exception);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(exception);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return exception.ToString();
        }

        /// <summary>
        /// Delegates
        /// <c>getMessage</c>
        /// method call to the wrapped exception.
        /// </summary>
        public override String Message {
            get {
                return exception.Message;
            }
        }
    }
}
