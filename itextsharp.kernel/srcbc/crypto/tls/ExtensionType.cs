namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// RFC 4366 2.3
	/// </summary>
	public enum ExtensionType : int
	{
		server_name = 0,
		max_fragment_length = 1,
		client_certificate_url = 2,
		trusted_ca_keys = 3,
		truncated_hmac = 4,
		status_request = 5,

		/*
		 * RFC 4492
		 */
		elliptic_curves = 10,
		ec_point_formats = 11,

		/*
		 * RFC 5054 2.8.1
		 */
		srp = 12,

		/*
		 * RFC 5746 6
		 */
		renegotiation_info = 0xff01,
	}
}
