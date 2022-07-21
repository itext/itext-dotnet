using System;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using Org.BouncyCastle.Security.Certificates;

namespace iText.Bouncycastle.Cert {
    public class CertIOExceptionBC : AbstractCertIOException {
        private readonly CertificateEncodingException exception;

        public CertIOExceptionBC(CertificateEncodingException exception) {
            this.exception = exception;
        }

        public virtual CertificateEncodingException GetException() {
            return exception;
        }

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
