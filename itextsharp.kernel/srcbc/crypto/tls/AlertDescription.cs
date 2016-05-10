namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// RFC 2246 7.2
	/// </summary>
	public enum AlertDescription : byte
	{
		close_notify = 0,
		unexpected_message = 10,
		bad_record_mac = 20,
		decryption_failed = 21,
		record_overflow = 22,
		decompression_failure = 30,
		handshake_failure = 40,
		/* 41 is not defined, for historical reasons */
		bad_certificate = 42,
		unsupported_certificate = 43,
		certificate_revoked = 44,
		certificate_expired = 45,
		certificate_unknown = 46,
		illegal_parameter = 47,
		unknown_ca = 48,
		access_denied = 49,
		decode_error = 50,
		decrypt_error = 51,
		export_restriction = 60,
		protocol_version = 70,
		insufficient_security = 71,
		internal_error = 80,
		user_canceled = 90,
		no_renegotiation = 100,

		/*
		 *  RFC 3546
		 */
		unsupported_extension = 110,
		certificate_unobtainable = 111,
		unrecognized_name = 112,
		bad_certificate_status_response = 113,
		bad_certificate_hash_value = 114,

		/*
		 *  RFC 4279
		 */
		unknown_psk_identity = 115,
	}
}
