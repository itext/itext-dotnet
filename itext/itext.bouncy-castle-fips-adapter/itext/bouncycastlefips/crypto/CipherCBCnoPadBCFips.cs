using System;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.Utilities.IO;

namespace iText.Bouncycastlefips.Crypto {
    /// <summary>
    /// Wrapper class for IBlockCipher.
    /// </summary>
    public class CipherCBCnoPadBCFips : ICipherCBCnoPad {
        private readonly IBlockCipher blockCipher;
        private MemoryOutputStream memoryStream = new MemoryOutputStream();

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="IBlockCipher"/>.
        /// </summary>
        /// <param name="forEncryption">
        /// Defines whether this wrapper will be used for encryption or decryption.
        /// </param>
        /// <param name="key">
        /// Key bytes to be used during block cipher creation.
        /// </param>
        public CipherCBCnoPadBCFips(bool forEncryption, byte[] key) : this(forEncryption, key, new byte[16]) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="IBlockCipher"/>.
        /// </summary>
        /// <param name="forEncryption">
        /// Defines whether this wrapper will be used for encryption or decryption.
        /// </param>
        /// <param name="key">
        /// Key bytes to be used during block cipher creation.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector to be used during block cipher creation.
        /// </param>
        public CipherCBCnoPadBCFips(bool forEncryption, byte[] key, byte[] initVector) {
            FipsAes.Key aesKey = new FipsAes.Key(key);
            IBlockCipherService provider = CryptoServicesRegistrar.CreateService(aesKey);

            IBlockCipherBuilder<IParameters<Algorithm>> cipherBuilder = null;
            if (forEncryption) {
                cipherBuilder = provider.CreateBlockEncryptorBuilder(FipsAes.Cbc.WithIV(initVector));
            }
            else {
                cipherBuilder = provider.CreateBlockDecryptorBuilder(FipsAes.Cbc.WithIV(initVector));
            }

            blockCipher = cipherBuilder.BuildBlockCipher(memoryStream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen) {
            if (inpLen % blockCipher.BlockSize != 0) {
                throw new ArgumentException("Not multiple of block: " + inpLen);
            }
            try {
                blockCipher.Stream.Write(inp, inpOff, inpLen);
                blockCipher.Stream.Flush();
                return memoryStream.ToArray();
            }
            finally {
                memoryStream.SetLength(0);
            }
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            CipherCBCnoPadBCFips that = (CipherCBCnoPadBCFips)o;
            return Object.Equals(blockCipher, that.blockCipher);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(blockCipher);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return blockCipher.ToString();
        }
    }
}