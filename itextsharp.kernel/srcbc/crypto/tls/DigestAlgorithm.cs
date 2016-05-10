using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public enum DigestAlgorithm
	{
		/*
		 * Note that the values here are implementation-specific and arbitrary.
		 * It is recommended not to depend on the particular values (e.g. serialization).
		 */
		NULL,
		MD5,
		SHA,

		/*
		 * RFC 5289
		 */
		SHA256,
		SHA384,
	}
}
