using System;
using Org.BouncyCastle.Security.Certificates;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>.
    /// </summary>
    public class CertIOExceptionBC : AbstractCertIOException {
        private readonly CertificateEncodingException exception;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>.
        /// </summary>
        /// <param name="exception">
        /// 
        /// <see cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>
        /// to be wrapped
        /// </param>
        public CertIOExceptionBC(CertificateEncodingException exception) {
            this.exception = exception;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>.
        /// </returns>
        public virtual CertificateEncodingException GetException() {
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
            iText.Bouncycastle.Cert.CertIOExceptionBC that = (iText.Bouncycastle.Cert.CertIOExceptionBC)o;
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
