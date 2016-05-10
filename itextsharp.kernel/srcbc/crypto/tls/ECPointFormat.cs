namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// RFC 4492 5.1.2
	/// </summary>
    public enum ECPointFormat : byte
	{
		uncompressed = 0,
		ansiX962_compressed_prime = 1,
		ansiX962_compressed_char2 = 2,

		/*
		 * reserved (248..255)
		 */
	}
}
