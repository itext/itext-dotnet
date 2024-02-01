/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using Org.BouncyCastle.Crypto.Asymmetric;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for RSA AsymmetricKeyPair.
    /// </summary>
    public class AsymmetricCipherKeyPairBCFips : IAsymmetricCipherKeyPair {
        private readonly AsymmetricKeyPair<AsymmetricRsaPublicKey, AsymmetricRsaPrivateKey> keyPair;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="AsymmetricKeyPair<AsymmetricRsaPublicKey, AsymmetricRsaPrivateKey>"/>.
        /// </summary>
        /// <param name="keyPair">
        /// <see cref="AsymmetricKeyPair<AsymmetricRsaPublicKey, AsymmetricRsaPrivateKey>"/> to be wrapped
        /// </param>
        public AsymmetricCipherKeyPairBCFips(AsymmetricKeyPair<AsymmetricRsaPublicKey, AsymmetricRsaPrivateKey> keyPair) {
            this.keyPair = keyPair;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped AsymmetricKeyPair<AsymmetricRsaPublicKey, AsymmetricRsaPrivateKey><IBlockResult>.
        /// </returns>
        public AsymmetricKeyPair<AsymmetricRsaPublicKey, AsymmetricRsaPrivateKey> GetKeyPair() {
            return keyPair;
        }

        /// <summary><inheritDoc/></summary>
        public IPrivateKey GetPrivateKey() {
            return new PrivateKeyBCFips(keyPair.PrivateKey);
        }

        /// <summary><inheritDoc/></summary>
        public IPublicKey GetPublicKey() {
            return new PublicKeyBCFips(keyPair.PublicKey);
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            AsymmetricCipherKeyPairBCFips that = (AsymmetricCipherKeyPairBCFips)o;
            return Object.Equals(keyPair, that.keyPair);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(keyPair);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return keyPair.ToString();
        }
    }
}
