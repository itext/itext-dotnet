using System;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastlefips.Security {
    /// <summary>Wrapper class for <see cref="Org.BouncyCastle.Security.GeneralSecurityException"/>.</summary>
    public class GeneralSecurityExceptionBCFips : AbstractGeneralSecurityException {
        private readonly GeneralSecurityException exception;

        /// <summary>
        /// Creates new wrapper for <see cref="Org.BouncyCastle.Security.GeneralSecurityException"/>.
        /// </summary>
        /// <param name="exception">
        /// <see cref="Org.BouncyCastle.Security.GeneralSecurityException"/> to be wrapped
        /// </param>
        public GeneralSecurityExceptionBCFips(GeneralSecurityException exception) {
            this.exception = exception;
        }

        /// <summary>
        /// Creates new wrapper for <see cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// using another exception.
        /// </summary>
        /// <param name="exceptionMessage">
        /// Another exception message to be used during instance creation
        /// </param>
        /// <param name="exception">
        /// Another exception to be used during instance creation
        /// </param>
        public GeneralSecurityExceptionBCFips(string exceptionMessage, Exception exception) : this(
            new GeneralSecurityException(exceptionMessage, exception)) {
        }

        /// <summary>Get actual org.bouncycastle object being wrapped.</summary>
        /// <returns>wrapped <see cref="Org.BouncyCastle.Security.GeneralSecurityException"/>.</returns>
        public GeneralSecurityException GetException() {
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
            GeneralSecurityExceptionBCFips that = (GeneralSecurityExceptionBCFips)o;
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
        public override String Message => exception.Message;
    }
}