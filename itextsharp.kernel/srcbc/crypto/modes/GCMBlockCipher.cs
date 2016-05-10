using System;

using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes
{
	/// <summary>
	/// Implements the Galois/Counter mode (GCM) detailed in
	/// NIST Special Publication 800-38D.
	/// </summary>
	public class GcmBlockCipher
		: IAeadBlockCipher
	{
		private const int BlockSize = 16;

        private readonly IBlockCipher	cipher;
		private readonly IGcmMultiplier	multiplier;
        private IGcmExponentiator exp;

        // These fields are set by Init and not modified by processing
		private bool        forEncryption;
		private int         macSize;
		private byte[]      nonce;
		private byte[]      initialAssociatedText;
        private byte[]      H;
		private byte[]      J0;

        // These fields are modified during processing
		private byte[]		bufBlock;
		private byte[]		macBlock;
        private byte[]      S, S_at, S_atPre;
		private byte[]      counter;
		private int         bufOff;
		private ulong		totalLength;
        private byte[]      atBlock;
        private int         atBlockPos;
        private ulong       atLength;
        private ulong       atLengthPre;

        public GcmBlockCipher(
			IBlockCipher c)
			: this(c, null)
		{
	    }

	    public GcmBlockCipher(
			IBlockCipher	c,
			IGcmMultiplier	m)
	    {
			if (c.GetBlockSize() != BlockSize)
				throw new ArgumentException("cipher required with a block size of " + BlockSize + ".");

	        if (m == null)
	        {
	            // TODO Consider a static property specifying default multiplier
	            m = new Tables8kGcmMultiplier();
	        }

			this.cipher = c;
			this.multiplier = m;
		}

		public virtual string AlgorithmName
		{
			get { return cipher.AlgorithmName + "/GCM"; }
		}

		public virtual int GetBlockSize()
		{
			return BlockSize;
		}

		public virtual void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			this.forEncryption = forEncryption;
			this.macBlock = null;

            KeyParameter keyParam;

            if (parameters is AeadParameters)
			{
				AeadParameters param = (AeadParameters)parameters;

				nonce = param.GetNonce();
				initialAssociatedText = param.GetAssociatedText();

				int macSizeBits = param.MacSize;
	            if (macSizeBits < 96 || macSizeBits > 128 || macSizeBits % 8 != 0)
	            {
	                throw new ArgumentException("Invalid value for MAC size: " + macSizeBits);
	            }

	            macSize = macSizeBits / 8; 
				keyParam = param.Key;
			}
			else if (parameters is ParametersWithIV)
			{
				ParametersWithIV param = (ParametersWithIV)parameters;

				nonce = param.GetIV();
                initialAssociatedText = null;
	            macSize = 16; 
				keyParam = (KeyParameter)param.Parameters;
			}
			else
			{
				throw new ArgumentException("invalid parameters passed to GCM");
			}

			int bufLength = forEncryption ? BlockSize : (BlockSize + macSize);
			this.bufBlock = new byte[bufLength];

			if (nonce == null || nonce.Length < 1)
			{
				throw new ArgumentException("IV must be at least 1 byte");
			}

            // TODO This should be configurable by Init parameters
            // (but must be 16 if nonce length not 12) (BlockSize?)
//			this.tagLength = 16;

            // Cipher always used in forward mode
            // if keyParam is null we're reusing the last key.
            if (keyParam != null)
            {
                cipher.Init(true, keyParam);

			    this.H = new byte[BlockSize];
			    cipher.ProcessBlock(H, 0, H, 0);

                // if keyParam is null we're reusing the last key and the multiplier doesn't need re-init
                multiplier.Init(H);
                exp = null;
            }

            this.J0 = new byte[BlockSize];

            if (nonce.Length == 12)
			{
				Array.Copy(nonce, 0, J0, 0, nonce.Length);
				this.J0[BlockSize - 1] = 0x01;
			}
			else
			{
                gHASH(J0, nonce, nonce.Length);
				byte[] X = new byte[BlockSize];
                Pack.UInt64_To_BE((ulong)nonce.Length * 8UL, X, 8);
                gHASHBlock(J0, X);
			}

            this.S = new byte[BlockSize];
            this.S_at = new byte[BlockSize];
            this.S_atPre = new byte[BlockSize];
            this.atBlock = new byte[BlockSize];
            this.atBlockPos = 0;
            this.atLength = 0;
            this.atLengthPre = 0;
            this.counter = Arrays.Clone(J0);
            this.bufOff = 0;
            this.totalLength = 0;

            if (initialAssociatedText != null)
            {
                ProcessAadBytes(initialAssociatedText, 0, initialAssociatedText.Length);
            }
		}

		public virtual byte[] GetMac()
		{
			return Arrays.Clone(macBlock);
		}

		public virtual int GetOutputSize(
			int len)
		{
            int totalData = len + bufOff;

            if (forEncryption)
            {
                return totalData + macSize;
            }

            return totalData < macSize ? 0 : totalData - macSize;
		}

        public virtual int GetUpdateOutputSize(
			int len)
		{
            int totalData = len + bufOff;
            if (!forEncryption)
            {
                if (totalData < macSize)
                {
                    return 0;
                }
                totalData -= macSize;
            }
            return totalData - totalData % BlockSize;
		}

        public virtual void ProcessAadByte(byte input)
        {
            atBlock[atBlockPos] = input;
            if (++atBlockPos == BlockSize)
            {
                // Hash each block as it fills
                gHASHBlock(S_at, atBlock);
                atBlockPos = 0;
                atLength += BlockSize;
            }
        }

        public virtual void ProcessAadBytes(byte[] inBytes, int inOff, int len)
        {
            for (int i = 0; i < len; ++i)
            {
                atBlock[atBlockPos] = inBytes[inOff + i];
                if (++atBlockPos == BlockSize)
                {
                    // Hash each block as it fills
                    gHASHBlock(S_at, atBlock);
                    atBlockPos = 0;
                    atLength += BlockSize;
                }
            }
        }

        private void InitCipher()
        {
            if (atLength > 0)
            {
                Array.Copy(S_at, 0, S_atPre, 0, BlockSize);
                atLengthPre = atLength;
            }

            // Finish hash for partial AAD block
            if (atBlockPos > 0)
            {
                gHASHPartial(S_atPre, atBlock, 0, atBlockPos);
                atLengthPre += (uint)atBlockPos;
            }

            if (atLengthPre > 0)
            {
                Array.Copy(S_atPre, 0, S, 0, BlockSize);
            }
        }

        public virtual int ProcessByte(
			byte	input,
			byte[]	output,
			int		outOff)
		{
            bufBlock[bufOff] = input;
            if (++bufOff == bufBlock.Length)
            {
                OutputBlock(output, outOff);
                return BlockSize;
            }
            return 0;
		}

        public virtual int ProcessBytes(
			byte[]	input,
			int		inOff,
			int		len,
			byte[]	output,
			int		outOff)
		{
            int resultLen = 0;

            for (int i = 0; i < len; ++i)
            {
                bufBlock[bufOff] = input[inOff + i];
                if (++bufOff == bufBlock.Length)
                {
                    OutputBlock(output, outOff + resultLen);
                    resultLen += BlockSize;
                }
            }

            return resultLen;
		}

        private void OutputBlock(byte[] output, int offset)
        {
            if (totalLength == 0)
            {
                InitCipher();
            }
            gCTRBlock(bufBlock, output, offset);
            if (forEncryption)
            {
                bufOff = 0;
            }
            else
            {
                Array.Copy(bufBlock, BlockSize, bufBlock, 0, macSize);
                bufOff = macSize;
            }
        }

		public int DoFinal(byte[] output, int outOff)
		{
            if (totalLength == 0)
            {
                InitCipher();
            }

            int extra = bufOff;
            if (!forEncryption)
            {
                if (extra < macSize)
                    throw new InvalidCipherTextException("data too short");

                extra -= macSize;
            }

            if (extra > 0)
            {
                gCTRPartial(bufBlock, 0, extra, output, outOff);
            }

            atLength += (uint)atBlockPos;

            if (atLength > atLengthPre)
            {
                /*
                 *  Some AAD was sent after the cipher started. We determine the difference b/w the hash value
                 *  we actually used when the cipher started (S_atPre) and the final hash value calculated (S_at).
                 *  Then we carry this difference forward by multiplying by H^c, where c is the number of (full or
                 *  partial) cipher-text blocks produced, and adjust the current hash.
                 */

                // Finish hash for partial AAD block
                if (atBlockPos > 0)
                {
                    gHASHPartial(S_at, atBlock, 0, atBlockPos);
                }

                // Find the difference between the AAD hashes
                if (atLengthPre > 0)
                {
                    GcmUtilities.Xor(S_at, S_atPre);
                }

                // Number of cipher-text blocks produced
                long c = (long)(((totalLength * 8) + 127) >> 7);

                // Calculate the adjustment factor
                byte[] H_c = new byte[16];
                if (exp == null)
                {
                    exp = new Tables1kGcmExponentiator();
                    exp.Init(H);
                }
                exp.ExponentiateX(c, H_c);

                // Carry the difference forward
                GcmUtilities.Multiply(S_at, H_c);

                // Adjust the current hash
                GcmUtilities.Xor(S, S_at);
            }

            // Final gHASH
			byte[] X = new byte[BlockSize];
            Pack.UInt64_To_BE(atLength * 8UL, X, 0);
            Pack.UInt64_To_BE(totalLength * 8UL, X, 8);

            gHASHBlock(S, X);

			// TODO Fix this if tagLength becomes configurable
			// T = MSBt(GCTRk(J0,S))
			byte[] tag = new byte[BlockSize];
			cipher.ProcessBlock(J0, 0, tag, 0);
			GcmUtilities.Xor(tag, S);

			int resultLen = extra;

			// We place into macBlock our calculated value for T
			this.macBlock = new byte[macSize];
			Array.Copy(tag, 0, macBlock, 0, macSize);

			if (forEncryption)
			{
				// Append T to the message
				Array.Copy(macBlock, 0, output, outOff + bufOff, macSize);
				resultLen += macSize;
			}
			else
			{
				// Retrieve the T value from the message and compare to calculated one
				byte[] msgMac = new byte[macSize];
				Array.Copy(bufBlock, extra, msgMac, 0, macSize);
				if (!Arrays.ConstantTimeAreEqual(this.macBlock, msgMac))
					throw new InvalidCipherTextException("mac check in GCM failed");
			}

			Reset(false);

			return resultLen;
		}

		public virtual void Reset()
		{
			Reset(true);
		}

		private void Reset(
			bool clearMac)
		{
            cipher.Reset();

            S = new byte[BlockSize];
            S_at = new byte[BlockSize];
            S_atPre = new byte[BlockSize];
            atBlock = new byte[BlockSize];
            atBlockPos = 0;
            atLength = 0;
            atLengthPre = 0;
            counter = Arrays.Clone(J0);
            bufOff = 0;
            totalLength = 0;

            if (bufBlock != null)
            {
                Arrays.Fill(bufBlock, 0);
            }

            if (clearMac)
            {
                macBlock = null;
            }

            if (initialAssociatedText != null)
            {
                ProcessAadBytes(initialAssociatedText, 0, initialAssociatedText.Length);
            }
		}

        private void gCTRBlock(byte[] block, byte[] output, int outOff)
        {
            byte[] tmp = GetNextCounterBlock();

            GcmUtilities.Xor(tmp, block);
            Array.Copy(tmp, 0, output, outOff, BlockSize);

            gHASHBlock(S, forEncryption ? tmp : block);

            totalLength += BlockSize;
        }

        private void gCTRPartial(byte[] buf, int off, int len, byte[] output, int outOff)
        {
            byte[] tmp = GetNextCounterBlock();

            GcmUtilities.Xor(tmp, buf, off, len);
            Array.Copy(tmp, 0, output, outOff, len);

            gHASHPartial(S, forEncryption ? tmp : buf, 0, len);

            totalLength += (uint)len;
        }

        private void gHASH(byte[] Y, byte[] b, int len)
        {
            for (int pos = 0; pos < len; pos += BlockSize)
            {
                int num = System.Math.Min(len - pos, BlockSize);
                gHASHPartial(Y, b, pos, num);
            }
        }

        private void gHASHBlock(byte[] Y, byte[] b)
        {
            GcmUtilities.Xor(Y, b);
            multiplier.MultiplyH(Y);
        }

        private void gHASHPartial(byte[] Y, byte[] b, int off, int len)
        {
            GcmUtilities.Xor(Y, b, off, len);
            multiplier.MultiplyH(Y);
        }

        private byte[] GetNextCounterBlock()
        {
            for (int i = 15; i >= 12; --i)
            {
                if (++counter[i] != 0) break;
            }

            byte[] tmp = new byte[BlockSize];
            // TODO Sure would be nice if ciphers could operate on int[]
            cipher.ProcessBlock(counter, 0, tmp, 0);
            return tmp;
        }
	}
}
