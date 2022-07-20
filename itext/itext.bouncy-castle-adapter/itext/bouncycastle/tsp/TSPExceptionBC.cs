using System;
using Org.BouncyCastle.Tsp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TspException"/>.
    /// </summary>
    public class TSPExceptionBC : AbstractTSPException {
        private readonly TspException tspException;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Tsp.TspException"/>.
        /// </summary>
        /// <param name="tspException">
        /// 
        /// <see cref="Org.BouncyCastle.Tsp.TspException"/>
        /// to be wrapped
        /// </param>
        public TSPExceptionBC(TspException tspException) {
            this.tspException = tspException;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Tsp.TspException"/>.
        /// </returns>
        public virtual TspException GetTSPException() {
            return tspException;
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
            iText.Bouncycastle.Tsp.TSPExceptionBC that = (iText.Bouncycastle.Tsp.TSPExceptionBC)o;
            return Object.Equals(tspException, that.tspException);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(tspException);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return tspException.ToString();
        }

        /// <summary>
        /// Delegates
        /// <c>getMessage</c>
        /// method call to the wrapped exception.
        /// </summary>
        public override String Message {
            get {
                return tspException.Message;
            }
        }
    }
}
