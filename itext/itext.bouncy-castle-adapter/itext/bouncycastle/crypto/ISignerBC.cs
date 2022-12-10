using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for <see cref="Org.BouncyCastle.Crypto.ISigner"/>.
    /// </summary>
    public class ISignerBC : IISigner {
        private ISigner iSigner;
        
        private string lastHashAlgorithm;
        private string lastEncryptionAlgorithm;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Crypto.ISigner"/>.
        /// </summary>
        /// <param name="iSigner">
        /// 
        /// <see cref="Org.BouncyCastle.Crypto.ISigner"/> to be wrapped
        /// </param>
        public ISignerBC(ISigner iSigner) {
            this.iSigner = iSigner;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Crypto.ISigner"/>.
        /// </returns>
        public virtual ISigner GetISigner() {
            return iSigner;
        }

        /// <summary><inheritDoc/></summary>
        public void InitVerify(IPublicKey publicKey) {
            InitVerify(publicKey, lastHashAlgorithm, lastEncryptionAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public void InitSign(IPrivateKey key) {
            InitSign(key, lastHashAlgorithm, lastEncryptionAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            iSigner.BlockUpdate(buf, off, len);
        }
        
        /// <summary><inheritDoc/></summary>
        public void Update(byte[] digest) { 
            Update(digest, 0, digest.Length);
        }

        /// <summary><inheritDoc/></summary>
        public bool VerifySignature(byte[] digest) {
            return iSigner.VerifySignature(digest);
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GenerateSignature() {
            return iSigner.GenerateSignature();
        }

        /// <summary><inheritDoc/></summary>
        public void UpdateVerifier(byte[] digest) {
            Update(digest);
        }

        /// <summary><inheritDoc/></summary>
        public void SetDigestAlgorithm(string algorithm) {
            lastHashAlgorithm = algorithm.Split(new string[] { "with" }, StringSplitOptions.None)[0];
            lastEncryptionAlgorithm = algorithm.Split(new string[] { "with" }, StringSplitOptions.None)[1];
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
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

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iSigner);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return iSigner.ToString();
        }

        private void InitVerify(IPublicKey publicKey, string hashAlgorithm, string encrAlgorithm) {
            iSigner = SignerUtilities.GetSigner(hashAlgorithm + "with" + encrAlgorithm);
            iSigner.Init(false, ((PublicKeyBC) publicKey).GetPublicKey());
        }

        private void InitSign(IPrivateKey key, string hashAlgorithm, string encrAlgorithm) {
            iSigner = SignerUtilities.GetSigner(hashAlgorithm + "with" + encrAlgorithm);
            iSigner.Init(true, ((PrivateKeyBC) key).GetPrivateKey());
        }
    }
}