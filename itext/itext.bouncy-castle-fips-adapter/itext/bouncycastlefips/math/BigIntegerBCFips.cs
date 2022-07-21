using System;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Math;

namespace iText.Bouncycastlefips.Math {
    public class BigIntegerBCFips : IBigInteger {
        private readonly BigInteger bigInteger;

        public BigIntegerBCFips(BigInteger bigInteger) {
            this.bigInteger = bigInteger;
        }

        public virtual BigInteger GetBigInteger() {
            return bigInteger;
        }
        
        public int GetIntValue()
        {
            return bigInteger.IntValue;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            BigIntegerBCFips that = (BigIntegerBCFips)o;
            return Object.Equals(bigInteger, that.bigInteger);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(bigInteger);
        }

        public override String ToString() {
            return bigInteger.ToString();
        }
    }
}