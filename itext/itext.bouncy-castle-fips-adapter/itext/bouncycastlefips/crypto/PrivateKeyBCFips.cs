using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto.Asymmetric;

namespace iText.Bouncycastlefips.Crypto {
    public class PrivateKeyBCFips: IPrivateKey {
        private readonly AsymmetricRsaPrivateKey privateKey;

        public PrivateKeyBCFips(AsymmetricRsaPrivateKey privateKey) {
            this.privateKey = privateKey;
        }

        public virtual AsymmetricRsaPrivateKey GetPrivateKey() {
            return privateKey;
        }
        
        public String GetAlgorithm() {
            return privateKey.Algorithm.Name;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(privateKey);
        }

        public override String ToString() {
            return privateKey.ToString();
        }
    }
}