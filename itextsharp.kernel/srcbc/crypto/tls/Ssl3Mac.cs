using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	/**
	 * HMAC implementation based on original internet draft for HMAC (RFC 2104)
	 * 
	 * The difference is that padding is concatentated versus XORed with the key
	 * 
	 * H(K + opad, H(K + ipad, text))
	 */
	public class Ssl3Mac
		: IMac
	{
		private const byte IPAD = 0x36;
		private const byte OPAD = 0x5C;

		internal static readonly byte[] MD5_IPAD = GenPad(IPAD, 48);
		internal static readonly byte[] MD5_OPAD = GenPad(OPAD, 48);
		internal static readonly byte[] SHA1_IPAD = GenPad(IPAD, 40);
		internal static readonly byte[] SHA1_OPAD = GenPad(OPAD, 40);

		private IDigest digest;

		private byte[] secret;
		private byte[] ipad, opad;

		/**
		 * Base constructor for one of the standard digest algorithms that the byteLength of
		 * the algorithm is know for. Behaviour is undefined for digests other than MD5 or SHA1.
		 * 
		 * @param digest the digest.
		 */
		public Ssl3Mac(IDigest digest)
		{
			this.digest = digest;

	        if (digest.GetDigestSize() == 20)
	        {
	            this.ipad = SHA1_IPAD;
	            this.opad = SHA1_OPAD;
	        }
	        else
	        {
	            this.ipad = MD5_IPAD;
	            this.opad = MD5_OPAD;
	        }
		}

		public virtual string AlgorithmName
		{
			get { return digest.AlgorithmName + "/SSL3MAC"; }
		}

		public virtual void Init(ICipherParameters parameters)
		{
			secret = Arrays.Clone(((KeyParameter)parameters).GetKey());

			Reset();
		}

		public virtual int GetMacSize()
		{
			return digest.GetDigestSize();
		}

		public virtual void Update(byte input)
		{
			digest.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int len)
		{
			digest.BlockUpdate(input, inOff, len);
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			byte[] tmp = new byte[digest.GetDigestSize()];
			digest.DoFinal(tmp, 0);

			digest.BlockUpdate(secret, 0, secret.Length);
			digest.BlockUpdate(opad, 0, opad.Length);
			digest.BlockUpdate(tmp, 0, tmp.Length);

			int len = digest.DoFinal(output, outOff);

			Reset();

			return len;
		}

		/**
		 * Reset the mac generator.
		 */
		public virtual void Reset()
		{
			digest.Reset();
			digest.BlockUpdate(secret, 0, secret.Length);
			digest.BlockUpdate(ipad, 0, ipad.Length);
		}

		private static byte[] GenPad(byte b, int count)
		{
			byte[] padding = new byte[count];
			Arrays.Fill(padding, b);
			return padding;
		}
	}
}
