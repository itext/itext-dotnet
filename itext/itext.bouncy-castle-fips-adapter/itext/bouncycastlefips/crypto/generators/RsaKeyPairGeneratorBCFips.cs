using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.Math;

namespace iText.Bouncycastlefips.Crypto.Generators {
    /// <summary>
    /// Wrapper class for RSA KeyPairGenerator.
    /// </summary>
    public class RsaKeyPairGeneratorBCFips : IRsaKeyPairGenerator {
        private readonly FipsRsa.KeyPairGenerator generator;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="FipsRsa.KeyPairGenerator"/>.
        /// </summary>
        public RsaKeyPairGeneratorBCFips() {
            this.generator = CryptoServicesRegistrar.CreateGenerator(
                new FipsRsa.KeyGenerationParameters(BigInteger.ValueOf(0x10001), 2048), 
                new BouncyCastleFipsFactory().GetSecureRandom());
        }
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="FipsRsa.KeyPairGenerator"/>.
        /// </summary>
        /// <param name="generator">
        /// <see cref="FipsRsa.KeyPairGenerator"/> to be wrapped
        /// </param>
        public RsaKeyPairGeneratorBCFips(FipsRsa.KeyPairGenerator generator) {
            this.generator = generator;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped FipsRsa.KeyPairGenerator<IBlockResult>.
        /// </returns>
        public FipsRsa.KeyPairGenerator GetGenerator() {
            return generator;
        }

        /// <summary><inheritDoc/></summary>
        public IAsymmetricCipherKeyPair GenerateKeyPair() {
            return new AsymmetricCipherKeyPairBCFips(generator.GenerateKeyPair());
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            RsaKeyPairGeneratorBCFips that = (RsaKeyPairGeneratorBCFips)o;
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