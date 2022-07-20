using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    public class OperatorCreationExceptionBCFips : AbstractOperatorCreationException {
        private readonly OperatorCreationException exception;

        public OperatorCreationExceptionBCFips(OperatorCreationException exception) {
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
            iText.Bouncycastlefips.Operator.OperatorCreationExceptionBCFips that = (iText.Bouncycastlefips.Operator.OperatorCreationExceptionBCFips
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
