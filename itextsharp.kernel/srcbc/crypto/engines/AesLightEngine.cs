using System;
using System.Diagnostics;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* an implementation of the AES (Rijndael), from FIPS-197.
	* <p>
	* For further details see: <a href="http://csrc.nist.gov/encryption/aes/">http://csrc.nist.gov/encryption/aes/</a>.
	*
	* This implementation is based on optimizations from Dr. Brian Gladman's paper and C code at
	* <a href="http://fp.gladman.plus.com/cryptography_technology/rijndael/">http://fp.gladman.plus.com/cryptography_technology/rijndael/</a>
	*
	* There are three levels of tradeoff of speed vs memory
	* Because java has no preprocessor, they are written as three separate classes from which to choose
	*
	* The fastest uses 8Kbytes of static tables to precompute round calculations, 4 256 word tables for encryption
	* and 4 for decryption.
	*
	* The middle performance version uses only one 256 word table for each, for a total of 2Kbytes,
	* adding 12 rotate operations per round to compute the values contained in the other tables from
	* the contents of the first
	*
	* The slowest version uses no static tables at all and computes the values
	* in each round.
	* </p>
	* <p>
	* This file contains the slowest performance version with no static tables
	* for round precomputation, but it has the smallest foot print.
	* </p>
	*/
	public class AesLightEngine
		: IBlockCipher
	{
		// The S box
		private static readonly byte[] S =
		{
			99, 124, 119, 123, 242, 107, 111, 197,
			48,   1, 103,  43, 254, 215, 171, 118,
			202, 130, 201, 125, 250,  89,  71, 240,
			173, 212, 162, 175, 156, 164, 114, 192,
			183, 253, 147,  38,  54,  63, 247, 204,
			52, 165, 229, 241, 113, 216,  49,  21,
			4, 199,  35, 195,  24, 150,   5, 154,
			7,  18, 128, 226, 235,  39, 178, 117,
			9, 131,  44,  26,  27, 110,  90, 160,
			82,  59, 214, 179,  41, 227,  47, 132,
			83, 209,   0, 237,  32, 252, 177,  91,
			106, 203, 190,  57,  74,  76,  88, 207,
			208, 239, 170, 251,  67,  77,  51, 133,
			69, 249,   2, 127,  80,  60, 159, 168,
			81, 163,  64, 143, 146, 157,  56, 245,
			188, 182, 218,  33,  16, 255, 243, 210,
			205,  12,  19, 236,  95, 151,  68,  23,
			196, 167, 126,  61, 100,  93,  25, 115,
			96, 129,  79, 220,  34,  42, 144, 136,
			70, 238, 184,  20, 222,  94,  11, 219,
			224,  50,  58,  10,  73,   6,  36,  92,
			194, 211, 172,  98, 145, 149, 228, 121,
			231, 200,  55, 109, 141, 213,  78, 169,
			108,  86, 244, 234, 101, 122, 174,   8,
			186, 120,  37,  46,  28, 166, 180, 198,
			232, 221, 116,  31,  75, 189, 139, 138,
			112,  62, 181, 102,  72,   3, 246,  14,
			97,  53,  87, 185, 134, 193,  29, 158,
			225, 248, 152,  17, 105, 217, 142, 148,
			155,  30, 135, 233, 206,  85,  40, 223,
			140, 161, 137,  13, 191, 230,  66, 104,
			65, 153,  45,  15, 176,  84, 187,  22,
		};

		// The inverse S-box
		private static readonly byte[] Si =
		{
			82,   9, 106, 213,  48,  54, 165,  56,
			191,  64, 163, 158, 129, 243, 215, 251,
			124, 227,  57, 130, 155,  47, 255, 135,
			52, 142,  67,  68, 196, 222, 233, 203,
			84, 123, 148,  50, 166, 194,  35,  61,
			238,  76, 149,  11,  66, 250, 195,  78,
			8,  46, 161, 102,  40, 217,  36, 178,
			118,  91, 162,  73, 109, 139, 209,  37,
			114, 248, 246, 100, 134, 104, 152,  22,
			212, 164,  92, 204,  93, 101, 182, 146,
			108, 112,  72,  80, 253, 237, 185, 218,
			94,  21,  70,  87, 167, 141, 157, 132,
			144, 216, 171,   0, 140, 188, 211,  10,
			247, 228,  88,   5, 184, 179,  69,   6,
			208,  44,  30, 143, 202,  63,  15,   2,
			193, 175, 189,   3,   1,  19, 138, 107,
			58, 145,  17,  65,  79, 103, 220, 234,
			151, 242, 207, 206, 240, 180, 230, 115,
			150, 172, 116,  34, 231, 173,  53, 133,
			226, 249,  55, 232,  28, 117, 223, 110,
			71, 241,  26, 113,  29,  41, 197, 137,
			111, 183,  98,  14, 170,  24, 190,  27,
			252,  86,  62,  75, 198, 210, 121,  32,
			154, 219, 192, 254, 120, 205,  90, 244,
			31, 221, 168,  51, 136,   7, 199,  49,
			177,  18,  16,  89,  39, 128, 236,  95,
			96,  81, 127, 169,  25, 181,  74,  13,
			45, 229, 122, 159, 147, 201, 156, 239,
			160, 224,  59,  77, 174,  42, 245, 176,
			200, 235, 187,  60, 131,  83, 153,  97,
			23,  43,   4, 126, 186, 119, 214,  38,
			225, 105,  20,  99,  85,  33,  12, 125,
		};

		// vector used in calculating key schedule (powers of x in GF(256))
		private static readonly byte[] rcon =
		{
			0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a,
			0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91
		};

		private static uint Shift(uint r, int shift)
		{
			return (r >> shift) | (r << (32 - shift));
		}

		/* multiply four bytes in GF(2^8) by 'x' {02} in parallel */

		private const uint m1 = 0x80808080;
		private const uint m2 = 0x7f7f7f7f;
		private const uint m3 = 0x0000001b;

		private static uint FFmulX(uint x)
		{
			return ((x & m2) << 1) ^ (((x & m1) >> 7) * m3);
		}

        /*
		The following defines provide alternative definitions of FFmulX that might
		give improved performance if a fast 32-bit multiply is not available.

		private int FFmulX(int x) { int u = x & m1; u |= (u >> 1); return ((x & m2) << 1) ^ ((u >>> 3) | (u >>> 6)); }
		private static final int  m4 = 0x1b1b1b1b;
		private int FFmulX(int x) { int u = x & m1; return ((x & m2) << 1) ^ ((u - (u >>> 7)) & m4); }

		*/

		private static uint Mcol(uint x)
		{
			uint f2 = FFmulX(x);
			return f2 ^ Shift(x ^ f2, 8) ^ Shift(x, 16) ^ Shift(x, 24);
		}

		private static uint Inv_Mcol(uint x)
		{
			uint f2 = FFmulX(x);
			uint f4 = FFmulX(f2);
			uint f8 = FFmulX(f4);
			uint f9 = x ^ f8;

			return f2 ^ f4 ^ f8 ^ Shift(f2 ^ f9, 8) ^ Shift(f4 ^ f9, 16) ^ Shift(f9, 24);
		}

		private static uint SubWord(uint x)
		{
			return (uint)S[x&255]
				| (((uint)S[(x>>8)&255]) << 8)
				| (((uint)S[(x>>16)&255]) << 16)
				| (((uint)S[(x>>24)&255]) << 24);
		}

		/**
		* Calculate the necessary round keys
		* The number of calculations depends on key size and block size
		* AES specified a fixed block size of 128 bits and key sizes 128/192/256 bits
		* This code is written assuming those are the only possible values
		*/
		private uint[][] GenerateWorkingKey(
			byte[]	key,
			bool	forEncryption)
		{
			int KC = key.Length / 4;  // key length in words
			int t;

			if ((KC != 4) && (KC != 6) && (KC != 8))
				throw new ArgumentException("Key length not 128/192/256 bits.");

			ROUNDS = KC + 6;  // This is not always true for the generalized Rijndael that allows larger block sizes

            uint[][] W = new uint[ROUNDS + 1][]; // 4 words in a block
            for (int i = 0; i <= ROUNDS; ++i)
            {
                W[i] = new uint[4];
            }

			//
			// copy the key into the round key array
			//

			t = 0;
			for (int i = 0; i < key.Length; t++)
			{
				W[t >> 2][t & 3] = Pack.LE_To_UInt32(key, i);
				i+=4;
			}

			//
			// while not enough round key material calculated
			// calculate new values
			//
			int k = (ROUNDS + 1) << 2;
			for (int i = KC; (i < k); i++)
			{
				uint temp = W[(i-1)>>2][(i-1)&3];
				if ((i % KC) == 0) 
				{
					temp = SubWord(Shift(temp, 8)) ^ rcon[(i / KC)-1];
				} 
				else if ((KC > 6) && ((i % KC) == 4)) 
				{
					temp = SubWord(temp);
				}

				W[i>>2][i&3] = W[(i - KC)>>2][(i-KC)&3] ^ temp;
			}

			if (!forEncryption) 
			{
				for (int j = 1; j < ROUNDS; j++) 
				{
                    uint[] w = W[j];
                    for (int i = 0; i < 4; i++)
                    {
                        w[i] = Inv_Mcol(w[i]);
                    }
				}
			}

			return W;
		}

		private int ROUNDS;
		private uint[][] WorkingKey;
		private uint C0, C1, C2, C3;
		private bool forEncryption;

		private const int BLOCK_SIZE = 16;

		/**
		* default constructor - 128 bit block size.
		*/
		public AesLightEngine()
		{
		}

		/**
		* initialise an AES cipher.
		*
		* @param forEncryption whether or not we are for encryption.
		* @param parameters the parameters required to set up the cipher.
		* @exception ArgumentException if the parameters argument is
		* inappropriate.
		*/
		public void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
            KeyParameter keyParameter = parameters as KeyParameter;

            if (keyParameter == null)
                throw new ArgumentException("invalid parameter passed to AES init - " + parameters.GetType().Name);

            WorkingKey = GenerateWorkingKey(keyParameter.GetKey(), forEncryption);

            this.forEncryption = forEncryption;
        }

		public string AlgorithmName
		{
			get { return "AES"; }
		}

		public bool IsPartialBlockOkay
		{
			get { return false; }
		}

		public int GetBlockSize()
		{
			return BLOCK_SIZE;
		}

		public int ProcessBlock(
			byte[]	input,
			int		inOff,
			byte[]	output,
			int		outOff)
		{
			if (WorkingKey == null)
			{
				throw new InvalidOperationException("AES engine not initialised");
			}

			if ((inOff + (32 / 2)) > input.Length)
			{
				throw new DataLengthException("input buffer too short");
			}

			if ((outOff + (32 / 2)) > output.Length)
			{
				throw new DataLengthException("output buffer too short");
			}

			if (forEncryption)
			{
				UnPackBlock(input, inOff);
				EncryptBlock();
				PackBlock(output, outOff);
			}
			else
			{
				UnPackBlock(input, inOff);
				DecryptBlock();
				PackBlock(output, outOff);
			}

			return BLOCK_SIZE;
		}

		public void Reset()
		{
		}

		private void UnPackBlock(
			byte[]	bytes,
			int		off)
		{
			C0 = Pack.LE_To_UInt32(bytes, off);
			C1 = Pack.LE_To_UInt32(bytes, off + 4);
			C2 = Pack.LE_To_UInt32(bytes, off + 8);
			C3 = Pack.LE_To_UInt32(bytes, off + 12);
		}

		private void PackBlock(
			byte[]	bytes,
			int		off)
		{
			Pack.UInt32_To_LE(C0, bytes, off);
			Pack.UInt32_To_LE(C1, bytes, off + 4);
			Pack.UInt32_To_LE(C2, bytes, off + 8);
			Pack.UInt32_To_LE(C3, bytes, off + 12);
		}

		private void EncryptBlock()
		{
            uint[][] KW = WorkingKey;

            int r = 0;
            uint[] kw = KW[r];

            C0 ^= kw[0];
            C1 ^= kw[1];
            C2 ^= kw[2];
            C3 ^= kw[3];

            uint r0, r1, r2, r3;

            while (r < (ROUNDS - 2))
			{
                kw = KW[++r];
                r0 = Mcol((uint)S[C0 & 255] ^ (((uint)S[(C1 >> 8) & 255]) << 8) ^ (((uint)S[(C2 >> 16) & 255]) << 16) ^ (((uint)S[(C3 >> 24) & 255]) << 24)) ^ kw[0];
                r1 = Mcol((uint)S[C1 & 255] ^ (((uint)S[(C2 >> 8) & 255]) << 8) ^ (((uint)S[(C3 >> 16) & 255]) << 16) ^ (((uint)S[(C0 >> 24) & 255]) << 24)) ^ kw[1];
                r2 = Mcol((uint)S[C2 & 255] ^ (((uint)S[(C3 >> 8) & 255]) << 8) ^ (((uint)S[(C0 >> 16) & 255]) << 16) ^ (((uint)S[(C1 >> 24) & 255]) << 24)) ^ kw[2];
                r3 = Mcol((uint)S[C3 & 255] ^ (((uint)S[(C0 >> 8) & 255]) << 8) ^ (((uint)S[(C1 >> 16) & 255]) << 16) ^ (((uint)S[(C2 >> 24) & 255]) << 24)) ^ kw[3];
                kw = KW[++r];
                C0 = Mcol((uint)S[r0 & 255] ^ (((uint)S[(r1 >> 8) & 255]) << 8) ^ (((uint)S[(r2 >> 16) & 255]) << 16) ^ (((uint)S[(r3 >> 24) & 255]) << 24)) ^ kw[0];
                C1 = Mcol((uint)S[r1 & 255] ^ (((uint)S[(r2 >> 8) & 255]) << 8) ^ (((uint)S[(r3 >> 16) & 255]) << 16) ^ (((uint)S[(r0 >> 24) & 255]) << 24)) ^ kw[1];
                C2 = Mcol((uint)S[r2 & 255] ^ (((uint)S[(r3 >> 8) & 255]) << 8) ^ (((uint)S[(r0 >> 16) & 255]) << 16) ^ (((uint)S[(r1 >> 24) & 255]) << 24)) ^ kw[2];
                C3 = Mcol((uint)S[r3 & 255] ^ (((uint)S[(r0 >> 8) & 255]) << 8) ^ (((uint)S[(r1 >> 16) & 255]) << 16) ^ (((uint)S[(r2 >> 24) & 255]) << 24)) ^ kw[3];
			}

            kw = KW[++r];
            r0 = Mcol((uint)S[C0 & 255] ^ (((uint)S[(C1 >> 8) & 255]) << 8) ^ (((uint)S[(C2 >> 16) & 255]) << 16) ^ (((uint)S[(C3 >> 24) & 255]) << 24)) ^ kw[0];
            r1 = Mcol((uint)S[C1 & 255] ^ (((uint)S[(C2 >> 8) & 255]) << 8) ^ (((uint)S[(C3 >> 16) & 255]) << 16) ^ (((uint)S[(C0 >> 24) & 255]) << 24)) ^ kw[1];
            r2 = Mcol((uint)S[C2 & 255] ^ (((uint)S[(C3 >> 8) & 255]) << 8) ^ (((uint)S[(C0 >> 16) & 255]) << 16) ^ (((uint)S[(C1 >> 24) & 255]) << 24)) ^ kw[2];
            r3 = Mcol((uint)S[C3 & 255] ^ (((uint)S[(C0 >> 8) & 255]) << 8) ^ (((uint)S[(C1 >> 16) & 255]) << 16) ^ (((uint)S[(C2 >> 24) & 255]) << 24)) ^ kw[3];

			// the final round is a simple function of S

            kw = KW[++r];
            C0 = (uint)S[r0 & 255] ^ (((uint)S[(r1 >> 8) & 255]) << 8) ^ (((uint)S[(r2 >> 16) & 255]) << 16) ^ (((uint)S[(r3 >> 24) & 255]) << 24) ^ kw[0];
            C1 = (uint)S[r1 & 255] ^ (((uint)S[(r2 >> 8) & 255]) << 8) ^ (((uint)S[(r3 >> 16) & 255]) << 16) ^ (((uint)S[(r0 >> 24) & 255]) << 24) ^ kw[1];
            C2 = (uint)S[r2 & 255] ^ (((uint)S[(r3 >> 8) & 255]) << 8) ^ (((uint)S[(r0 >> 16) & 255]) << 16) ^ (((uint)S[(r1 >> 24) & 255]) << 24) ^ kw[2];
            C3 = (uint)S[r3 & 255] ^ (((uint)S[(r0 >> 8) & 255]) << 8) ^ (((uint)S[(r1 >> 16) & 255]) << 16) ^ (((uint)S[(r2 >> 24) & 255]) << 24) ^ kw[3];

            Debug.Assert(r == ROUNDS);
        }

		private void DecryptBlock()
		{
            uint[][] KW = WorkingKey;

            int r = ROUNDS;
            uint[] kw = KW[r];

            C0 ^= kw[0];
            C1 ^= kw[1];
            C2 ^= kw[2];
            C3 ^= kw[3];

            uint r0, r1, r2, r3;

            while (r > 2)
			{
                kw = KW[--r];
                r0 = Inv_Mcol((uint)Si[C0 & 255] ^ (((uint)Si[(C3 >> 8) & 255]) << 8) ^ (((uint)Si[(C2 >> 16) & 255]) << 16) ^ ((uint)Si[(C1 >> 24) & 255] << 24)) ^ kw[0];
                r1 = Inv_Mcol((uint)Si[C1 & 255] ^ (((uint)Si[(C0 >> 8) & 255]) << 8) ^ (((uint)Si[(C3 >> 16) & 255]) << 16) ^ ((uint)Si[(C2 >> 24) & 255] << 24)) ^ kw[1];
                r2 = Inv_Mcol((uint)Si[C2 & 255] ^ (((uint)Si[(C1 >> 8) & 255]) << 8) ^ (((uint)Si[(C0 >> 16) & 255]) << 16) ^ ((uint)Si[(C3 >> 24) & 255] << 24)) ^ kw[2];
                r3 = Inv_Mcol((uint)Si[C3 & 255] ^ (((uint)Si[(C2 >> 8) & 255]) << 8) ^ (((uint)Si[(C1 >> 16) & 255]) << 16) ^ ((uint)Si[(C0 >> 24) & 255] << 24)) ^ kw[3];
                kw = KW[--r];
                C0 = Inv_Mcol((uint)Si[r0 & 255] ^ (((uint)Si[(r3 >> 8) & 255]) << 8) ^ (((uint)Si[(r2 >> 16) & 255]) << 16) ^ ((uint)Si[(r1 >> 24) & 255] << 24)) ^ kw[0];
                C1 = Inv_Mcol((uint)Si[r1 & 255] ^ (((uint)Si[(r0 >> 8) & 255]) << 8) ^ (((uint)Si[(r3 >> 16) & 255]) << 16) ^ ((uint)Si[(r2 >> 24) & 255] << 24)) ^ kw[1];
                C2 = Inv_Mcol((uint)Si[r2 & 255] ^ (((uint)Si[(r1 >> 8) & 255]) << 8) ^ (((uint)Si[(r0 >> 16) & 255]) << 16) ^ ((uint)Si[(r3 >> 24) & 255] << 24)) ^ kw[2];
                C3 = Inv_Mcol((uint)Si[r3 & 255] ^ (((uint)Si[(r2 >> 8) & 255]) << 8) ^ (((uint)Si[(r1 >> 16) & 255]) << 16) ^ ((uint)Si[(r0 >> 24) & 255] << 24)) ^ kw[3];
			}

            kw = KW[--r];
            r0 = Inv_Mcol((uint)Si[C0 & 255] ^ (((uint)Si[(C3 >> 8) & 255]) << 8) ^ (((uint)Si[(C2 >> 16) & 255]) << 16) ^ ((uint)Si[(C1 >> 24) & 255] << 24)) ^ kw[0];
            r1 = Inv_Mcol((uint)Si[C1 & 255] ^ (((uint)Si[(C0 >> 8) & 255]) << 8) ^ (((uint)Si[(C3 >> 16) & 255]) << 16) ^ ((uint)Si[(C2 >> 24) & 255] << 24)) ^ kw[1];
            r2 = Inv_Mcol((uint)Si[C2 & 255] ^ (((uint)Si[(C1 >> 8) & 255]) << 8) ^ (((uint)Si[(C0 >> 16) & 255]) << 16) ^ ((uint)Si[(C3 >> 24) & 255] << 24)) ^ kw[2];
            r3 = Inv_Mcol((uint)Si[C3 & 255] ^ (((uint)Si[(C2 >> 8) & 255]) << 8) ^ (((uint)Si[(C1 >> 16) & 255]) << 16) ^ ((uint)Si[(C0 >> 24) & 255] << 24)) ^ kw[3];

			// the final round's table is a simple function of Si

            kw = KW[--r];
            C0 = (uint)Si[r0 & 255] ^ (((uint)Si[(r3 >> 8) & 255]) << 8) ^ (((uint)Si[(r2 >> 16) & 255]) << 16) ^ (((uint)Si[(r1 >> 24) & 255]) << 24) ^ kw[0];
            C1 = (uint)Si[r1 & 255] ^ (((uint)Si[(r0 >> 8) & 255]) << 8) ^ (((uint)Si[(r3 >> 16) & 255]) << 16) ^ (((uint)Si[(r2 >> 24) & 255]) << 24) ^ kw[1];
            C2 = (uint)Si[r2 & 255] ^ (((uint)Si[(r1 >> 8) & 255]) << 8) ^ (((uint)Si[(r0 >> 16) & 255]) << 16) ^ (((uint)Si[(r3 >> 24) & 255]) << 24) ^ kw[2];
            C3 = (uint)Si[r3 & 255] ^ (((uint)Si[(r2 >> 8) & 255]) << 8) ^ (((uint)Si[(r1 >> 16) & 255]) << 16) ^ (((uint)Si[(r0 >> 24) & 255]) << 24) ^ kw[3];

            Debug.Assert(r == 0);
        }
	}
}
