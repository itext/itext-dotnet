using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Utilities.IO;
using ICipher = iText.Commons.Bouncycastle.Crypto.ICipher;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for ICipher.
    /// </summary>
    public class CipherBCFips : ICipher {
        private readonly Org.BouncyCastle.Crypto.ICipher cipher;
        private MemoryOutputStream memoryStream = new MemoryOutputStream();

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Crypto.ICipher"/>.
        /// </summary>
        /// <param name="cipher">
        /// <see cref="Org.BouncyCastle.Crypto.ICipher"/> to be wrapped
        /// </param>
        public CipherBCFips(Org.BouncyCastle.Crypto.ICipher cipher) {
            this.cipher = cipher;
        }
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Crypto.ICipher"/>.
        /// </summary>
        /// <param name="forEncryption">boolean value</param>
        /// <param name="key">byte array</param>
        /// <param name="iv">init vector</param>
        public CipherBCFips(bool forEncryption, byte[] key, byte[] iv) {
            FipsAes.Key aesKey = new FipsAes.Key(key);
            IBlockCipherService provider = CryptoServicesRegistrar.CreateService(aesKey);
            IBlockCipherBuilder<IParameters<Algorithm>> cipherBuilder = forEncryption ?
                provider.CreateBlockEncryptorBuilder(FipsAes.Cbc.WithIV(iv)) : provider.CreateBlockDecryptorBuilder(FipsAes.Cbc.WithIV(iv));
            cipher = cipherBuilder.BuildPaddedCipher(memoryStream, new Pkcs7Padding());
        }
        
        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped ICipher<IBlockResult>.
        /// </returns>
        public virtual Org.BouncyCastle.Crypto.ICipher GetICipher() {
            return cipher;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Update(byte[] inp, int inpOff, int inpLen) {
            cipher.Stream.Write(inp, inpOff, inpLen);
            return Array.Empty<byte>();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] DoFinal() {
            cipher.Stream.Close();
            return memoryStream.ToArray();
        }
    }
}