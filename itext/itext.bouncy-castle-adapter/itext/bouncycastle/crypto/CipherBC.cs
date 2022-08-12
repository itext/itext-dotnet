using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using ICipher = iText.Commons.Bouncycastle.Crypto.ICipher;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for PaddedBufferedBlockCipher.
    /// </summary>
    public class CipherBC : ICipher {
        private readonly PaddedBufferedBlockCipher cipher;
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="PaddedBufferedBlockCipher"/>.
        /// </summary>
        /// <param name="cipher">
        /// <see cref="PaddedBufferedBlockCipher"/> to be wrapped
        /// </param>
        public CipherBC(PaddedBufferedBlockCipher cipher) {
            this.cipher = cipher;
        }
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="PaddedBufferedBlockCipher"/>.
        /// </summary>
        /// <param name="forEncryption">boolean value</param>
        /// <param name="key">byte array</param>
        /// <param name="iv">init vector</param>
        public CipherBC(bool forEncryption, byte[] key, byte[] iv) {
            IBlockCipher aes = new AesFastEngine();
            IBlockCipher cbc = new CbcBlockCipher(aes);
            cipher = new PaddedBufferedBlockCipher(cbc);
            KeyParameter kp = new KeyParameter(key);
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            cipher.Init(forEncryption, piv);
        }
        
        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped PaddedBufferedBlockCipher<IBlockResult>.
        /// </returns>
        public virtual PaddedBufferedBlockCipher GetICipher() {
            return cipher;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Update(byte[] inp, int inpOff, int inpLen) {
            int neededLen = cipher.GetUpdateOutputSize(inpLen);
            byte[] outp;
            if (neededLen > 0) {
                outp = new byte[neededLen];
            }
            else {
                outp = new byte[0];
            }
            cipher.ProcessBytes(inp, inpOff, inpLen, outp, 0);
            return outp;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] DoFinal() {
            int neededLen = cipher.GetOutputSize(0);
            byte[] outp = new byte[neededLen];
            int n;
            try {
                n = cipher.DoFinal(outp, 0);
            } catch (Exception) {
                return outp;
            }
            if (n != outp.Length) {
                byte[] outp2 = new byte[n];
                Array.Copy(outp, 0, outp2, 0, n);
                return outp2;
            }
            return outp;
        }
    }
}