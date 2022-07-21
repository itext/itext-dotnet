using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastlefips.Crypto {
    public class PublicKeyBCFips: IPublicKey {
        private readonly IAsymmetricPublicKey publicKey;

        public PublicKeyBCFips(IAsymmetricPublicKey publicKey) {
            this.publicKey = publicKey;
        }

        public virtual IAsymmetricPublicKey GetPublicKey() {
            return publicKey;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            PublicKeyBCFips that = (PublicKeyBCFips)o;
            return Object.Equals(publicKey, that.publicKey);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(publicKey);
        }

        public override String ToString() {
            return publicKey.ToString();
        }
    }
}