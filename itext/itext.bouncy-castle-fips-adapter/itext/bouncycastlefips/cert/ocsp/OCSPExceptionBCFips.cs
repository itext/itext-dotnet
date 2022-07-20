using System;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class OCSPExceptionBCFips : AbstractOCSPException {
        private readonly OcspException exception;

        public OCSPExceptionBCFips(OcspException exception) {
            this.exception = exception;
        }

        public virtual OcspException GetException() {
            return exception;
        }

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
