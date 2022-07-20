using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Crypto.ICipherParameters"/>.
    /// </summary>
    public class PrivateKeyBC: IPrivateKey {
        private readonly ICipherParameters privateKey;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Crypto.ICipherParameters"/>.
        /// </summary>
        /// <param name="privateKey">
        /// <see cref="Org.BouncyCastle.Crypto.ICipherParameters"/>
        /// to be wrapped
        /// </param>
        public PrivateKeyBC(ICipherParameters privateKey) {
            this.privateKey = privateKey;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Crypto.ICipherParameters"/>.
        /// </returns>
        public virtual ICipherParameters GetPrivateKey() {
            return privateKey;
        }

        /// <summary><inheritDoc/></summary>
        public string GetAlgorithm() {
            if (privateKey is RsaKeyParameters) {
                return "RSA";
            }
            if (privateKey is DsaKeyParameters) {
                return "DSA";
            }
            if (privateKey is ECKeyParameters && ((ECKeyParameters)privateKey).AlgorithmName == "EC") {
                return "ECDSA";
            }
            return null;
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            PrivateKeyBC that = (PrivateKeyBC)o;
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