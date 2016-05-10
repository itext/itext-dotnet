using System;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes
{
    /**
    * Implements the Counter with Cipher Block Chaining mode (CCM) detailed in
    * NIST Special Publication 800-38C.
    * <p>
    * <b>Note</b>: this mode is a packet mode - it needs all the data up front.
    * </p>
    */
    public class CcmBlockCipher
        : IAeadBlockCipher
    {
        private static readonly int BlockSize = 16;

        private readonly IBlockCipher	cipher;
        private readonly byte[]			macBlock;
        private bool					forEncryption;
        private byte[]					nonce;
        private byte[]					initialAssociatedText;
        private int						macSize;
        private ICipherParameters		keyParam;
        private readonly MemoryStream   associatedText = new MemoryStream();
        private readonly MemoryStream   data = new MemoryStream();

        /**
        * Basic constructor.
        *
        * @param cipher the block cipher to be used.
        */
        public CcmBlockCipher(
            IBlockCipher cipher)
        {
            this.cipher = cipher;
            this.macBlock = new byte[BlockSize];

            if (cipher.GetBlockSize() != BlockSize)
                throw new ArgumentException("cipher required with a block size of " + BlockSize + ".");
        }

        /**
        * return the underlying block cipher that we are wrapping.
        *
        * @return the underlying block cipher that we are wrapping.
        */
        public virtual IBlockCipher GetUnderlyingCipher()
        {
            return cipher;
        }

        public virtual void Init(
            bool				forEncryption,
            ICipherParameters	parameters)
        {
            this.forEncryption = forEncryption;

            if (parameters is AeadParameters)
            {
                AeadParameters param = (AeadParameters) parameters;

                nonce = param.GetNonce();
                initialAssociatedText = param.GetAssociatedText();
                macSize = param.MacSize / 8;
                keyParam = param.Key;
            }
            else if (parameters is ParametersWithIV)
            {
                ParametersWithIV param = (ParametersWithIV) parameters;

                nonce = param.GetIV();
                initialAssociatedText = null;
                macSize = macBlock.Length / 2;
                keyParam = param.Parameters;
            }
            else
            {
                throw new ArgumentException("invalid parameters passed to CCM");
            }

            if (nonce == null || nonce.Length < 7 || nonce.Length > 13)
            {
                throw new ArgumentException("nonce must have length from 7 to 13 octets");
            }
        }

        public virtual string AlgorithmName
        {
            get { return cipher.AlgorithmName + "/CCM"; }
        }

        public virtual int GetBlockSize()
        {
            return cipher.GetBlockSize();
        }

        public virtual void ProcessAadByte(byte input)
        {
            associatedText.WriteByte(input);
        }

        public virtual void ProcessAadBytes(byte[] inBytes, int inOff, int len)
        {
            // TODO: Process AAD online
            associatedText.Write(inBytes, inOff, len);
        }

        public virtual int ProcessByte(
            byte	input,
            byte[]	outBytes,
            int		outOff)
        {
            data.WriteByte(input);

            return 0;
        }

        public virtual int ProcessBytes(
            byte[]	inBytes,
            int		inOff,
            int		inLen,
            byte[]	outBytes,
            int		outOff)
        {
            data.Write(inBytes, inOff, inLen);

            return 0;
        }

        public virtual int DoFinal(
            byte[]	outBytes,
            int		outOff)
        {
            byte[] enc = ProcessPacket(data.GetBuffer(), 0, (int)data.Position);

            Array.Copy(enc, 0, outBytes, outOff, enc.Length);

            Reset();

            return enc.Length;
        }

        public virtual void Reset()
        {
            cipher.Reset();
            associatedText.SetLength(0);
            data.SetLength(0);
        }

        /**
        * Returns a byte array containing the mac calculated as part of the
        * last encrypt or decrypt operation.
        *
        * @return the last mac calculated.
        */
        public virtual byte[] GetMac()
        {
            byte[] mac = new byte[macSize];

            Array.Copy(macBlock, 0, mac, 0, mac.Length);

            return mac;
        }

        public virtual int GetUpdateOutputSize(
            int len)
        {
            return 0;
        }

        public int GetOutputSize(
            int len)
        {
            int totalData = (int)data.Length + len;

            if (forEncryption)
            {
                return totalData + macSize;
            }

            return totalData < macSize ? 0 : totalData - macSize;
        }

        public byte[] ProcessPacket(
            byte[]	input,
            int		inOff,
            int		inLen)
        {
            // TODO: handle null keyParam (e.g. via RepeatedKeySpec)
            // Need to keep the CTR and CBC Mac parts around and reset
            if (keyParam == null)
                throw new InvalidOperationException("CCM cipher unitialized.");

            int n = nonce.Length;
            int q = 15 - n;
            if (q < 4)
            {
                int limitLen = 1 << (8 * q);
                if (inLen >= limitLen)
                    throw new InvalidOperationException("CCM packet too large for choice of q.");
            }

            byte[] iv = new byte[BlockSize];
            iv[0] = (byte)((q - 1) & 0x7);
            nonce.CopyTo(iv, 1);

            IBlockCipher ctrCipher = new SicBlockCipher(cipher);
            ctrCipher.Init(forEncryption, new ParametersWithIV(keyParam, iv));

            int index = inOff;
            int outOff = 0;
            byte[] output;

            if (forEncryption)
            {
                output = new byte[inLen + macSize];

                calculateMac(input, inOff, inLen, macBlock);

                ctrCipher.ProcessBlock(macBlock, 0, macBlock, 0);   // S0

                while (index < inLen - BlockSize)                   // S1...
                {
                    ctrCipher.ProcessBlock(input, index, output, outOff);
                    outOff += BlockSize;
                    index += BlockSize;
                }

                byte[] block = new byte[BlockSize];

                Array.Copy(input, index, block, 0, inLen - index);

                ctrCipher.ProcessBlock(block, 0, block, 0);

                Array.Copy(block, 0, output, outOff, inLen - index);

                outOff += inLen - index;

                Array.Copy(macBlock, 0, output, outOff, output.Length - outOff);
            }
            else
            {
                output = new byte[inLen - macSize];

                Array.Copy(input, inOff + inLen - macSize, macBlock, 0, macSize);

                ctrCipher.ProcessBlock(macBlock, 0, macBlock, 0);

                for (int i = macSize; i != macBlock.Length; i++)
                {
                    macBlock[i] = 0;
                }

                while (outOff < output.Length - BlockSize)
                {
                    ctrCipher.ProcessBlock(input, index, output, outOff);
                    outOff += BlockSize;
                    index += BlockSize;
                }

                byte[] block = new byte[BlockSize];

                Array.Copy(input, index, block, 0, output.Length - outOff);

                ctrCipher.ProcessBlock(block, 0, block, 0);

                Array.Copy(block, 0, output, outOff, output.Length - outOff);

                byte[] calculatedMacBlock = new byte[BlockSize];

                calculateMac(output, 0, output.Length, calculatedMacBlock);

                if (!Arrays.ConstantTimeAreEqual(macBlock, calculatedMacBlock))
                    throw new InvalidCipherTextException("mac check in CCM failed");
            }

            return output;
        }

        private int calculateMac(byte[] data, int dataOff, int dataLen, byte[] macBlock)
        {
            IMac cMac = new CbcBlockCipherMac(cipher, macSize * 8);

            cMac.Init(keyParam);

            //
            // build b0
            //
            byte[] b0 = new byte[16];

            if (HasAssociatedText())
            {
                b0[0] |= 0x40;
            }

            b0[0] |= (byte)((((cMac.GetMacSize() - 2) / 2) & 0x7) << 3);

            b0[0] |= (byte)(((15 - nonce.Length) - 1) & 0x7);

            Array.Copy(nonce, 0, b0, 1, nonce.Length);

            int q = dataLen;
            int count = 1;
            while (q > 0)
            {
                b0[b0.Length - count] = (byte)(q & 0xff);
                q >>= 8;
                count++;
            }

            cMac.BlockUpdate(b0, 0, b0.Length);

            //
            // process associated text
            //
            if (HasAssociatedText())
            {
                int extra;

                int textLength = GetAssociatedTextLength();
                if (textLength < ((1 << 16) - (1 << 8)))
                {
                    cMac.Update((byte)(textLength >> 8));
                    cMac.Update((byte)textLength);

                    extra = 2;
                }
                else // can't go any higher than 2^32
                {
                    cMac.Update((byte)0xff);
                    cMac.Update((byte)0xfe);
                    cMac.Update((byte)(textLength >> 24));
                    cMac.Update((byte)(textLength >> 16));
                    cMac.Update((byte)(textLength >> 8));
                    cMac.Update((byte)textLength);

                    extra = 6;
                }

                if (initialAssociatedText != null)
                {
                    cMac.BlockUpdate(initialAssociatedText, 0, initialAssociatedText.Length);
                }
                if (associatedText.Position > 0)
                {
                    cMac.BlockUpdate(associatedText.GetBuffer(), 0, (int)associatedText.Position);
                }

                extra = (extra + textLength) % 16;
                if (extra != 0)
                {
                    for (int i = extra; i < 16; ++i)
                    {
                        cMac.Update((byte)0x00);
                    }
                }
            }

            //
            // add the text
            //
            cMac.BlockUpdate(data, dataOff, dataLen);

            return cMac.DoFinal(macBlock, 0);
        }

        private int GetAssociatedTextLength()
        {
            return (int)associatedText.Length + ((initialAssociatedText == null) ? 0 : initialAssociatedText.Length);
        }

        private bool HasAssociatedText()
        {
            return GetAssociatedTextLength() > 0;
        }
    }
}
