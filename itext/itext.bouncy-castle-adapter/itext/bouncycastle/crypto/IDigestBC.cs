using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Crypto.IDigest"/>.
    /// </summary>
    public class IDigestBC : IIDigest {
        private readonly IDigest iDigest;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Crypto.IDigest"/>.
        /// </summary>
        /// <param name="iDigest">
        /// 
        /// <see cref="Org.BouncyCastle.Crypto.IDigest"/>
        /// to be wrapped
        /// </param>
        public IDigestBC(IDigest iDigest) {
            this.iDigest = iDigest;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped IDigest<IBlockResult>.
        /// </returns>
        public virtual IDigest GetIDigest() {
            return iDigest;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest(byte[] enc2) {
            iDigest.BlockUpdate(enc2, 0, enc2.Length);
            return Digest();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest() {
            byte[] output = new byte[iDigest.GetDigestSize()];
            iDigest.DoFinal(output, 0);
            return output;
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            iDigest.BlockUpdate(buf, off, len);
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf) {
            Update(buf, 0, buf.Length);
        }

        /// <summary><inheritDoc/></summary>
        public void Reset() {
            iDigest.Reset();
        }

        /// <summary><inheritDoc/></summary>
        public string GetAlgorithmName() {
            return iDigest.AlgorithmName;
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            IDigestBC that = (IDigestBC)o;
            return Object.Equals(iDigest, that.iDigest);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iDigest);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return iDigest.ToString();
        }
    }
}