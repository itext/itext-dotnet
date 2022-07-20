using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto.Asymmetric;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Crypto.Asymmetric.AsymmetricRsaPrivateKey"/>.
    /// </summary>
    public class PrivateKeyBCFips: IPrivateKey {
        private readonly AsymmetricRsaPrivateKey privateKey;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Crypto.Asymmetric.AsymmetricRsaPrivateKey"/>.
        /// </summary>
        /// <param name="privateKey">
        /// <see cref="Org.BouncyCastle.Crypto.Asymmetric.AsymmetricRsaPrivateKey"/>
        /// to be wrapped
        /// </param>
        public PrivateKeyBCFips(AsymmetricRsaPrivateKey privateKey) {
            this.privateKey = privateKey;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Crypto.Asymmetric.AsymmetricRsaPrivateKey"/>.
        /// </returns>
        public virtual AsymmetricRsaPrivateKey GetPrivateKey() {
            return privateKey;
        }
        
        /// <summary><inheritDoc/></summary>
        public String GetAlgorithm() {
            return privateKey.Algorithm.Name;
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