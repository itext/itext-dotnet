using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastle.Crypto {
    public class PublicKeyBC: IPublicKey {
        private readonly AsymmetricKeyParameter publicKey;

        public PublicKeyBC(AsymmetricKeyParameter publicKey) {
            this.publicKey = publicKey;
        }

        public virtual AsymmetricKeyParameter GetPublicKey() {
            return publicKey;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(publicKey);
        }

        public override String ToString() {
            return publicKey.ToString();
        }
    }
}