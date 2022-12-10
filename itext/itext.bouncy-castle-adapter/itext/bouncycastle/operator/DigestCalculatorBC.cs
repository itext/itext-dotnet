/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using Org.BouncyCastle.Crypto;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Operator {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Crypto.IDigestFactory"/>.
    /// </summary>
    public class DigestCalculatorBC : IDigestCalculator {
        private readonly IDigestFactory digestCalculator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Crypto.IDigestFactory"/>.
        /// </summary>
        /// <param name="digestCalculator">
        /// 
        /// <see cref="Org.BouncyCastle.Crypto.IDigestFactory"/>
        /// to be wrapped
        /// </param>
        public DigestCalculatorBC(IDigestFactory digestCalculator) {
            this.digestCalculator = digestCalculator;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Crypto.IDigestFactory"/>.
        /// </returns>
        public virtual IDigestFactory GetDigestCalculator() {
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
