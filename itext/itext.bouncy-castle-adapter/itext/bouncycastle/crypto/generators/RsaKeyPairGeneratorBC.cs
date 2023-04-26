/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastle.Crypto.Generators {
    /// <summary>
    /// Wrapper class for RsaKeyPairGenerator.
    /// </summary>
    public class RsaKeyPairGeneratorBC : IRsaKeyPairGenerator {
        private readonly RsaKeyPairGenerator generator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="RsaKeyPairGeneratorr"/>.
        /// </summary>
        public RsaKeyPairGeneratorBC() {
            generator = new RsaKeyPairGenerator();
            generator.Init(new KeyGenerationParameters(new SecureRandom(new VmpcRandomGenerator()), 2048));
        }
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="RsaKeyPairGenerator"/>.
        /// </summary>
        /// <param name="generator">
        /// <see cref="RsaKeyPairGenerator"/> to be wrapped
        /// </param>
        public RsaKeyPairGeneratorBC(RsaKeyPairGenerator generator) {
            this.generator = generator;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped RsaKeyPairGenerator<IBlockResult>.
        /// </returns>
        public RsaKeyPairGenerator GetGenerator() {
            return generator;
        }

        /// <summary><inheritDoc/></summary>
        public IAsymmetricCipherKeyPair GenerateKeyPair() {
            return new AsymmetricCipherKeyPairBC(generator.GenerateKeyPair());
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            RsaKeyPairGeneratorBC that = (RsaKeyPairGeneratorBC)o;
            return Object.Equals(generator, that.generator);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(generator);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return generator.ToString();
        }
    }
}
