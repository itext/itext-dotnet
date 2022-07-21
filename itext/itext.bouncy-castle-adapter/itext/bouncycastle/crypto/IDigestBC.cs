using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastle.Crypto {
    public class IDigestBC : IIDigest {
        private readonly IDigest iDigest;

        public IDigestBC(IDigest iDigest) {
            this.iDigest = iDigest;
        }

        public virtual IDigest GetIDigest() {
            return iDigest;
        }
        
        public void BlockUpdate(byte[] input, int offset, int len) {
            iDigest.BlockUpdate(input, offset, len);
        }

        public int GetDigestSize() {
            return iDigest.GetDigestSize();
        }

        public void DoFinal(byte[] output, int i) {
            iDigest.DoFinal(output, i);
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iDigest);
        }

        public override String ToString() {
            return iDigest.ToString();
        }

        public byte[] Digest(byte[] enc2) {
            iDigest.BlockUpdate(enc2, 0, enc2.Length);
            return Digest();
        }

        public byte[] Digest() {
            byte[] output = new byte[iDigest.GetDigestSize()];
            iDigest.DoFinal(output, 0);
            return output;
        }

        public void Update(byte[] buf, int off, int len) {
            iDigest.BlockUpdate(buf, off, len);
        }

        public void Update(byte[] buf) {
            Update(buf, 0, buf.Length);
        }
    }
}