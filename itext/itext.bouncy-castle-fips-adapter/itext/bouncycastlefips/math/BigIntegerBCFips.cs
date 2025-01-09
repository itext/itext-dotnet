/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Math;

namespace iText.Bouncycastlefips.Math {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Math.BigInteger"/>.
    /// </summary>
    public class BigIntegerBCFips : IBigInteger {
        private static readonly BigIntegerBCFips INSTANCE = new BigIntegerBCFips(null);

        private readonly BigInteger bigInteger;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Math.BigInteger"/>.
        /// </summary>
        /// <param name="bigInteger">
        /// <see cref="Org.BouncyCastle.Math.BigInteger"/>
        /// to be wrapped
        /// </param>
        public BigIntegerBCFips(BigInteger bigInteger) {
            this.bigInteger = bigInteger;
        }
        
        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="BigIntegerBCFips"/>
        /// instance.
        /// </returns>
        public static BigIntegerBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Math.BigInteger"/>.
        /// </returns>
        public virtual BigInteger GetBigInteger() {
            return bigInteger;
        }
        
        /// <summary><inheritDoc/></summary>
        public int GetIntValue() {
            return bigInteger.IntValue;
        }

        /// <summary><inheritDoc/></summary>
        public string ToString(int radix) {
            return bigInteger.ToString(radix);
        }
        
        /// <summary><inheritDoc/></summary>
        public IBigInteger ValueOf(long value) {
            return new BigIntegerBCFips(BigInteger.ValueOf(value));
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger Remainder(IBigInteger n) {
            return new BigIntegerBCFips(bigInteger.Remainder(((BigIntegerBCFips) n).GetBigInteger()));
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(bigInteger);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return bigInteger.ToString();
        }
    }
}
