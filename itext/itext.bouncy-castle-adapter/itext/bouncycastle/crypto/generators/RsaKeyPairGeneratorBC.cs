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