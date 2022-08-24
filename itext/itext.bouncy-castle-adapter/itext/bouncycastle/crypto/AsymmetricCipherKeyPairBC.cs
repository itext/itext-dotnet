using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for AsymmetricCipherKeyPair.
    /// </summary>
    public class AsymmetricCipherKeyPairBC : IAsymmetricCipherKeyPair {
        private readonly AsymmetricCipherKeyPair keyPair;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="AsymmetricCipherKeyPair"/>.
        /// </summary>
        /// <param name="keyPair">
        /// <see cref="AsymmetricCipherKeyPair"/> to be wrapped
        /// </param>
        public AsymmetricCipherKeyPairBC(AsymmetricCipherKeyPair keyPair) {
            this.keyPair = keyPair;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped AsymmetricCipherKeyPair<IBlockResult>.
        /// </returns>
        public AsymmetricCipherKeyPair GetKeyPair() {
            return keyPair;
        }

        /// <summary><inheritDoc/></summary>
        public IPrivateKey GetPrivateKey() {
            return new PrivateKeyBC(keyPair.Private);
        }

        /// <summary><inheritDoc/></summary>
        public IPublicKey GetPublicKey() {
            return new PublicKeyBC(keyPair.Public);
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            AsymmetricCipherKeyPairBC that = (AsymmetricCipherKeyPairBC)o;
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