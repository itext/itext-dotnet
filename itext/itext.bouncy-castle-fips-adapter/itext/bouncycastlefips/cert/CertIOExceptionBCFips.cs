using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    public class CertIOExceptionBCFips : AbstractCertIOException {
        private readonly CertIOException exception;

        public CertIOExceptionBCFips(CertIOException exception) {
            this.exception = exception;
        }

        public virtual CertIOException GetException() {
            return exception;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.CertIOExceptionBCFips that = (iText.Bouncycastlefips.Cert.CertIOExceptionBCFips
                )o;
            return Object.Equals(exception, that.exception);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(exception);
        }

        public override String ToString() {
            return exception.ToString();
        }

        public override String Message {
            get {
                return exception.Message;
            }
        }
    }
}
