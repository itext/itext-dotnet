using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Crypto.AsymmetricKeyParameter"/>.
    /// </summary>
    public class PublicKeyBC: IPublicKey {
        private readonly AsymmetricKeyParameter publicKey;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Crypto.AsymmetricKeyParameter"/>.
        /// </summary>
        /// <param name="publicKey">
        /// <see cref="Org.BouncyCastle.Crypto.AsymmetricKeyParameter"/>
        /// to be wrapped
        /// </param>
        public PublicKeyBC(AsymmetricKeyParameter publicKey) {
            this.publicKey = publicKey;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Crypto.AsymmetricKeyParameter"/>.
        /// </returns>
        public virtual AsymmetricKeyParameter GetPublicKey() {
            return publicKey;
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            PublicKeyBC that = (PublicKeyBC)o;
            return Object.Equals(publicKey, that.publicKey);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(publicKey);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return publicKey.ToString();
        }
    }
}