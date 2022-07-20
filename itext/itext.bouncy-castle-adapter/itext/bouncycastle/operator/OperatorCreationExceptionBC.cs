using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator {
    public class OperatorCreationExceptionBC : AbstractOperatorCreationException {
        private readonly OperatorCreationException exception;

        public OperatorCreationExceptionBC(OperatorCreationException exception) {
            this.exception = exception;
        }

        public virtual OperatorCreationException GetException() {
            return exception;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Operator.OperatorCreationExceptionBC that = (iText.Bouncycastle.Operator.OperatorCreationExceptionBC
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
