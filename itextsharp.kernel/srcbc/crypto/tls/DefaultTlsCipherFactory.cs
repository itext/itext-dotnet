using System;
using System.IO;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsCipherFactory
		: TlsCipherFactory
	{
		public virtual TlsCipher CreateCipher(TlsClientContext context,
			EncryptionAlgorithm encryptionAlgorithm, DigestAlgorithm digestAlgorithm)
		{
			switch (encryptionAlgorithm)
			{
				case EncryptionAlgorithm.cls_3DES_EDE_CBC:
					return CreateDesEdeCipher(context, 24, digestAlgorithm);
				case EncryptionAlgorithm.AES_128_CBC:
					return CreateAesCipher(context, 16, digestAlgorithm);
				case EncryptionAlgorithm.AES_256_CBC:
					return CreateAesCipher(context, 32, digestAlgorithm);
                case EncryptionAlgorithm.RC4_128:
                    return CreateRC4Cipher(context, 16, digestAlgorithm);
				default:
					throw new TlsFatalAlert(AlertDescription.internal_error);
			}
		}

        /// <exception cref="IOException"></exception>
        protected virtual TlsCipher CreateRC4Cipher(TlsClientContext context, int cipherKeySize, DigestAlgorithm digestAlgorithm)
        {
            return new TlsStreamCipher(context, CreateRC4StreamCipher(), CreateRC4StreamCipher(), CreateDigest(digestAlgorithm), CreateDigest(digestAlgorithm), cipherKeySize);
        }

		/// <exception cref="IOException"></exception>
		protected virtual TlsCipher CreateAesCipher(TlsClientContext context, int cipherKeySize,
			DigestAlgorithm digestAlgorithm)
		{
			return new TlsBlockCipher(context, CreateAesBlockCipher(), CreateAesBlockCipher(),
				CreateDigest(digestAlgorithm), CreateDigest(digestAlgorithm), cipherKeySize);
		}

		/// <exception cref="IOException"></exception>
		protected virtual TlsCipher CreateDesEdeCipher(TlsClientContext context, int cipherKeySize,
			DigestAlgorithm digestAlgorithm)
		{
			return new TlsBlockCipher(context, CreateDesEdeBlockCipher(), CreateDesEdeBlockCipher(),
				CreateDigest(digestAlgorithm), CreateDigest(digestAlgorithm), cipherKeySize);
		}

        protected virtual IStreamCipher CreateRC4StreamCipher()
        {
            return new RC4Engine();
        }

		protected virtual IBlockCipher CreateAesBlockCipher()
		{
			return new CbcBlockCipher(new AesFastEngine());
		}

		protected virtual IBlockCipher CreateDesEdeBlockCipher()
		{
			return new CbcBlockCipher(new DesEdeEngine());
		}

		/// <exception cref="IOException"></exception>
		protected virtual IDigest CreateDigest(DigestAlgorithm digestAlgorithm)
		{
			switch (digestAlgorithm)
			{
				case DigestAlgorithm.MD5:
					return new MD5Digest();
				case DigestAlgorithm.SHA:
					return new Sha1Digest();
				case DigestAlgorithm.SHA256:
					return new Sha256Digest();
				case DigestAlgorithm.SHA384:
					return new Sha384Digest();
				default:
					throw new TlsFatalAlert(AlertDescription.internal_error);
			}
		}
	}
}
