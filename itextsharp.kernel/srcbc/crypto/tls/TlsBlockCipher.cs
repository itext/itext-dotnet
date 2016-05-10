using System;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// A generic TLS 1.0 block cipher. This can be used for AES or 3DES for example.
	/// </summary>
	public class TlsBlockCipher
        : TlsCipher
	{
		protected TlsClientContext context;
        protected byte[] randomData;

        protected IBlockCipher encryptCipher;
        protected IBlockCipher decryptCipher;

        protected TlsMac wMac;
        protected TlsMac rMac;

        public virtual TlsMac WriteMac
		{
            get { return wMac; }
		}

		public virtual TlsMac ReadMac
		{
            get { return rMac; }
		}

		public TlsBlockCipher(TlsClientContext context, IBlockCipher encryptCipher,
			IBlockCipher decryptCipher, IDigest writeDigest, IDigest readDigest, int cipherKeySize)
		{
			this.context = context;

            this.randomData = new byte[256];
            context.SecureRandom.NextBytes(randomData);

            this.encryptCipher = encryptCipher;
			this.decryptCipher = decryptCipher;

			int prfSize = (2 * cipherKeySize) + writeDigest.GetDigestSize()
				+ readDigest.GetDigestSize() + encryptCipher.GetBlockSize()
				+ decryptCipher.GetBlockSize();

			SecurityParameters securityParameters = context.SecurityParameters;

			byte[] keyBlock = TlsUtilities.PRF(securityParameters.masterSecret, "key expansion",
				TlsUtilities.Concat(securityParameters.serverRandom, securityParameters.clientRandom),
				prfSize);

			int offset = 0;

			// Init MACs
			wMac = CreateTlsMac(writeDigest, keyBlock, ref offset);
            rMac = CreateTlsMac(readDigest, keyBlock, ref offset);

			// Build keys
			KeyParameter encryptKey = CreateKeyParameter(keyBlock, ref offset, cipherKeySize);
			KeyParameter decryptKey = CreateKeyParameter(keyBlock, ref offset, cipherKeySize);

			// Add IVs
			ParametersWithIV encryptParams = CreateParametersWithIV(encryptKey,
				keyBlock, ref offset, encryptCipher.GetBlockSize());
			ParametersWithIV decryptParams = CreateParametersWithIV(decryptKey,
				keyBlock, ref offset, decryptCipher.GetBlockSize());

			if (offset != prfSize)
				throw new TlsFatalAlert(AlertDescription.internal_error);

			// Init Ciphers
			encryptCipher.Init(true, encryptParams);
			decryptCipher.Init(false, decryptParams);
		}

        protected virtual TlsMac CreateTlsMac(IDigest digest, byte[] buf, ref int off)
		{
			int len = digest.GetDigestSize();
			TlsMac mac = new TlsMac(digest, buf, off, len);
			off += len;
			return mac;
		}

        protected virtual KeyParameter CreateKeyParameter(byte[] buf, ref int off, int len)
		{
			KeyParameter key = new KeyParameter(buf, off, len);
			off += len;
			return key;
		}

        protected virtual ParametersWithIV CreateParametersWithIV(KeyParameter key,
			byte[] buf, ref int off, int len)
		{
			ParametersWithIV ivParams = new ParametersWithIV(key, buf, off, len);
			off += len;
			return ivParams;
		}

		public virtual byte[] EncodePlaintext(ContentType type, byte[] plaintext, int offset, int len)
		{
			int blocksize = encryptCipher.GetBlockSize();
            int padding_length = blocksize - 1 - ((len + wMac.Size) % blocksize);

            //bool isTls = context.ServerVersion.FullVersion >= ProtocolVersion.TLSv10.FullVersion;
            bool isTls = true;

            if (isTls)
            {
                // Add a random number of extra blocks worth of padding
                int maxExtraPadBlocks = (255 - padding_length) / blocksize;
                int actualExtraPadBlocks = ChooseExtraPadBlocks(context.SecureRandom, maxExtraPadBlocks);
                padding_length += actualExtraPadBlocks * blocksize;
            }

            int totalsize = len + wMac.Size + padding_length + 1;
			byte[] outbuf = new byte[totalsize];
			Array.Copy(plaintext, offset, outbuf, 0, len);
            byte[] mac = wMac.CalculateMac(type, plaintext, offset, len);
			Array.Copy(mac, 0, outbuf, len, mac.Length);
			int paddoffset = len + mac.Length;
            for (int i = 0; i <= padding_length; i++)
			{
                outbuf[i + paddoffset] = (byte)padding_length;
			}
			for (int i = 0; i < totalsize; i += blocksize)
			{
				encryptCipher.ProcessBlock(outbuf, i, outbuf, i);
			}
			return outbuf;
		}

        public virtual byte[] DecodeCiphertext(ContentType type, byte[] ciphertext, int offset, int len)
		{
            int blockSize = decryptCipher.GetBlockSize();
            int macSize = rMac.Size;

            /*
             *  TODO[TLS 1.1] Explicit IV implies minLen = blockSize + max(blockSize, macSize + 1),
             *  and will need further changes to offset and plen variables below.
             */

            int minLen = System.Math.Max(blockSize, macSize + 1);
            if (len < minLen)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            if (len % blockSize != 0)
                throw new TlsFatalAlert(AlertDescription.decryption_failed);

            for (int i = 0; i < len; i += blockSize)
            {
                decryptCipher.ProcessBlock(ciphertext, offset + i, ciphertext, offset + i);
            }

            int plen = len;

            // If there's anything wrong with the padding, this will return zero
            int totalPad = CheckPaddingConstantTime(ciphertext, offset, plen, blockSize, macSize);

            int macInputLen = plen - totalPad - macSize;

            byte[] decryptedMac = Arrays.Copy(ciphertext, offset + macInputLen, macSize);
            byte[] calculatedMac = rMac.CalculateMacConstantTime(type, ciphertext, offset, macInputLen, plen - macSize, randomData);

            bool badMac = !Arrays.ConstantTimeAreEqual(calculatedMac, decryptedMac);

            if (badMac || totalPad == 0)
                throw new TlsFatalAlert(AlertDescription.bad_record_mac);

            return Arrays.Copy(ciphertext, offset, macInputLen);
		}

        protected virtual int CheckPaddingConstantTime(byte[] buf, int off, int len, int blockSize, int macSize)
        {
            int end = off + len;
            byte lastByte = buf[end - 1];
            int padlen = lastByte & 0xff;
            int totalPad = padlen + 1;

            int dummyIndex = 0;
            byte padDiff = 0;

            //bool isTls = context.ServerVersion.FullVersion >= ProtocolVersion.TLSv10.FullVersion;
            bool isTls = true;

            if ((!isTls && totalPad > blockSize) || (macSize + totalPad > len))
            {
                totalPad = 0;
            }
            else
            {
                int padPos = end - totalPad;
                do
                {
                    padDiff |= (byte)(buf[padPos++] ^ lastByte);
                }
                while (padPos < end);

                dummyIndex = totalPad;

                if (padDiff != 0)
                {
                    totalPad = 0;
                }
            }

            // Run some extra dummy checks so the number of checks is always constant
            {
                byte[] dummyPad = randomData;
                while (dummyIndex < 256)
                {
                    padDiff |= (byte)(dummyPad[dummyIndex++] ^ lastByte);
                }
                // Ensure the above loop is not eliminated
                dummyPad[0] ^= padDiff;
            }

            return totalPad;
        }

        protected virtual int ChooseExtraPadBlocks(SecureRandom r, int max)
		{
//			return r.NextInt(max + 1);

			uint x = (uint)r.NextInt();
			int n = LowestBitSet(x);
			return System.Math.Min(n, max);
		}

        private int LowestBitSet(uint x)
		{
			if (x == 0)
			{
				return 32;
			}

			int n = 0;
			while ((x & 1) == 0)
			{
				++n;
				x >>= 1;
			}
			return n;
		}
	}
}
