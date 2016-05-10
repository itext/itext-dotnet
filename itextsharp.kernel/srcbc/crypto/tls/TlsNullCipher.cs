using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// A NULL cipher suite, for use during handshake.
	/// </summary>
	public class TlsNullCipher
		: TlsCipher
	{
		public virtual byte[] EncodePlaintext(ContentType type, byte[] plaintext, int offset, int len)
		{
			return CopyData(plaintext, offset, len);
		}

        public virtual byte[] DecodeCiphertext(ContentType type, byte[] ciphertext, int offset, int len)
		{
			return CopyData(ciphertext, offset, len);
		}

		protected virtual byte[] CopyData(byte[] text, int offset, int len)
		{
			byte[] result = new byte[len];
			Array.Copy(text, offset, result, 0, len);
			return result;
		}
	}
}
