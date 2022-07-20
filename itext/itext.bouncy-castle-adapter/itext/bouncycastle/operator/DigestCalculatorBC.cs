using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Operator.DigestCalculator"/>.
    /// </summary>
    public class DigestCalculatorBC : IDigestCalculator {
        private readonly DigestCalculator digestCalculator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Operator.DigestCalculator"/>.
        /// </summary>
        /// <param name="digestCalculator">
        /// 
        /// <see cref="Org.BouncyCastle.Operator.DigestCalculator"/>
        /// to be wrapped
        /// </param>
        public DigestCalculatorBC(DigestCalculator digestCalculator) {
            this.digestCalculator = digestCalculator;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Operator.DigestCalculator"/>.
        /// </returns>
        public virtual DigestCalculator GetDigestCalculator() {
            return digestCalculator;
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
            iText.Bouncycastle.Operator.DigestCalculatorBC that = (iText.Bouncycastle.Operator.DigestCalculatorBC)o;
            return Object.Equals(digestCalculator, that.digestCalculator);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(digestCalculator);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return digestCalculator.ToString();
        }
    }
}
