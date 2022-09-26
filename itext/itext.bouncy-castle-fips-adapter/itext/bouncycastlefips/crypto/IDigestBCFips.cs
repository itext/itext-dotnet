using System;
using System.Security.Cryptography;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for System.Security.Cryptography.HashAlgorithm digest.
    /// </summary>
    public class IDigestBCFips : IIDigest {
        private readonly HashAlgorithm digest;
        private string algorithmName;

        /// <summary>
        /// Creates new wrapper instance for digest.
        /// </summary>
        /// <param name="digest">
        /// 
        /// System.Security.Cryptography.HashAlgorithm to be wrapped
        /// </param>
        public IDigestBCFips(HashAlgorithm digest) {
            this.digest = digest;
        }

        /// <summary>
        /// Creates new wrapper instance for digest.
        /// </summary>
        /// <param name="hashAlgorithm">
        /// 
        /// hash algorithm to create System.Security.Cryptography.HashAlgorithm
        /// </param>
        public IDigestBCFips(string hashAlgorithm) {
            Oid oid = new Oid(hashAlgorithm);
            digest = HashAlgorithm.Create(oid.FriendlyName);
            algorithmName = oid.FriendlyName;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped System.Security.Cryptography.HashAlgorithm.
        /// </returns>
        public virtual HashAlgorithm GetIDigest() {
            return digest;
        }
        
        /// <summary>Sets algorithm name.</summary>
        /// <param name = "algorithmName">algorithm name</param>
        public virtual void SetAlgorithmName(string algorithmName) {
            this.algorithmName = algorithmName;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest(byte[] enc) {
            return digest.ComputeHash(enc);
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest() {
            byte[] output = digest.Hash;
            digest.Clear();
            return output;
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            digest.ComputeHash(buf, off, len);
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf) {
            Update(buf, 0, buf.Length);
        }

        /// <summary><inheritDoc/></summary>
        public void Reset() {
            Digest();
        }

        /// <summary><inheritDoc/></summary>
        public string GetAlgorithmName() {
            return algorithmName;
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            IDigestBCFips that = (IDigestBCFips)o;
            return Object.Equals(digest, that.digest);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(digest);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return digest.ToString();
        }
    }
}