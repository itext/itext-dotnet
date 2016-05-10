namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// RFC 2246 6.2.1
	/// </summary>
    public enum ContentType : byte
	{
		change_cipher_spec = 20,
		alert = 21,
		handshake = 22,
		application_data = 23,
	}
}
