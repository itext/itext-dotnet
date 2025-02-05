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
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for
    /// <see cref="IAsymmetricPrivateKey"/>.
    /// </summary>
    public class PrivateKeyBCFips: IPrivateKey {
        private readonly IAsymmetricPrivateKey privateKey;

        /// <summary>
        /// Creates new wrapper instance for <see cref="IAsymmetricPrivateKey"/>.
        /// </summary>
        /// <param name="privateKey">
        /// <see cref="IAsymmetricPrivateKey"/>
        /// to be wrapped
        /// </param>
        public PrivateKeyBCFips(IAsymmetricPrivateKey privateKey) {
            this.privateKey = privateKey;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="IAsymmetricPrivateKey"/>.
        /// </returns>
        public virtual IAsymmetricPrivateKey GetPrivateKey() {
            return privateKey;
        }
        
        /// <summary><inheritDoc/></summary>
        public String GetAlgorithm() {
            string name = privateKey.Algorithm.Name;
            return "EC".Equals(name) ? "ECDSA" : name;
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            PrivateKeyBCFips that = (PrivateKeyBCFips)o;
            return Object.Equals(privateKey, that.privateKey);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(privateKey);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return privateKey.ToString();
        }
    }
}
