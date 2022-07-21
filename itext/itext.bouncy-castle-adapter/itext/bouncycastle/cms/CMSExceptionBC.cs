using System;
using Org.BouncyCastle.Cms;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cms {
    public class CMSExceptionBC : AbstractCMSException {
        private readonly CmsException exception;

        public CMSExceptionBC(CmsException exception) {
            this.exception = exception;
        }

        public virtual CmsException GetCmsException() {
            return exception;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cms.CMSExceptionBC that = (iText.Bouncycastle.Cms.CMSExceptionBC)o;
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
