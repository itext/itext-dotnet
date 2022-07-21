using System;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    public class TSPExceptionBC : AbstractTSPException {
        private readonly TspException tspException;

        public TSPExceptionBC(TspException tspException) {
            this.tspException = tspException;
        }

        public virtual TspException GetTSPException() {
            return tspException;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Tsp.TSPExceptionBC that = (iText.Bouncycastle.Tsp.TSPExceptionBC)o;
            return Object.Equals(tspException, that.tspException);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(tspException);
        }

        public override String ToString() {
            return tspException.ToString();
        }

        public override String Message {
            get {
                return tspException.Message;
            }
        }
    }
}
