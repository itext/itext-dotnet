using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace iText.Bouncycastle.Crypto {
    public class PrivateKeyBC: IPrivateKey {
        private readonly ICipherParameters privateKey;

        public PrivateKeyBC(ICipherParameters privateKey) {
            this.privateKey = privateKey;
        }

        public virtual ICipherParameters GetPrivateKey() {
            return privateKey;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(privateKey);
        }

        public override String ToString() {
            return privateKey.ToString();
        }
    }
}