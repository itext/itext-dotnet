using System;
using Org.BouncyCastle.Operator;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Operator {
    public class DigestCalculatorBCFips : IDigestCalculator {
        private readonly DigestCalculator digestCalculator;

        public DigestCalculatorBCFips(DigestCalculator digestCalculator) {
            this.digestCalculator = digestCalculator;
        }

        public virtual DigestCalculator GetDigestCalculator() {
            return digestCalculator;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Operator.DigestCalculatorBCFips that = (iText.Bouncycastlefips.Operator.DigestCalculatorBCFips
                )o;
            return Object.Equals(digestCalculator, that.digestCalculator);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(digestCalculator);
        }

        public override String ToString() {
            return digestCalculator.ToString();
        }
    }
}
