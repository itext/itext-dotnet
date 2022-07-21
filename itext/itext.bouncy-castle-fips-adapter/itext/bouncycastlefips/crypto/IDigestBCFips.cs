using System;
using System.IO;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastlefips.Crypto {
    public class IDigestBCFips : IIDigest {
        private readonly IStreamCalculator<IBlockResult> iDigest;

        public IDigestBCFips(IStreamCalculator<IBlockResult> iDigest) {
            this.iDigest = iDigest;
        }

        public virtual IStreamCalculator<IBlockResult> GetIDigest() {
            return iDigest;
        }

        public byte[] Digest(byte[] enc2) {
            using (Stream digestStream = iDigest.Stream) {
                digestStream.Write(enc2, 0, enc2.Length);
            }
            return Digest();
        }

        public byte[] Digest() {
            return iDigest.GetResult().Collect();
        }

        public void Update(byte[] buf, int off, int len) {
            using (Stream digStream = iDigest.Stream) {
                digStream.Write(buf, off, len);
            }
        }

        public void Update(byte[] buf) {
            Update(buf, 0, buf.Length);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            IDigestBCFips that = (IDigestBCFips)o;
            return Object.Equals(iDigest, that.iDigest);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iDigest);
        }

        public override String ToString() {
            return iDigest.ToString();
        }
    }
}