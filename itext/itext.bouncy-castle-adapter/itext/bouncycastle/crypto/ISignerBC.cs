using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastle.Crypto {
    public class ISignerBC: IISigner {
        private readonly ISigner iSigner;

        public ISignerBC(ISigner iSigner) {
            this.iSigner = iSigner;
        }

        public virtual ISigner GetISigner() {
            return iSigner;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            ISignerBC that = (ISignerBC)o;
            return Object.Equals(iSigner, that.iSigner);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iSigner);
        }

        public override String ToString() {
            return iSigner.ToString();
        }

        public void InitVerify(IPublicKey publicKey, string hashAlgorithm, string encrAlgorithm) {
            iSigner.Init(false, ((PublicKeyBC) publicKey).GetPublicKey());
        }

        public void Update(byte[] buf, int off, int len) {
            iSigner.BlockUpdate(buf, off, len);
        }
        
        public void Update(byte[] digest) { 
            Update(digest, 0, digest.Length);
        }

        public bool VerifySignature(byte[] digest)
        {
            return iSigner.VerifySignature(digest);
        }
    }
}