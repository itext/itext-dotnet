using System;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Tsp {
    public class TSPExceptionBCFips : AbstractTSPException {
        private readonly TSPException tspException;

        public TSPExceptionBCFips(TSPException tspException) {
            this.tspException = tspException;
        }

        public virtual TSPException GetTSPException() {
            return tspException;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TSPExceptionBCFips that = (iText.Bouncycastlefips.Tsp.TSPExceptionBCFips)o;
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
