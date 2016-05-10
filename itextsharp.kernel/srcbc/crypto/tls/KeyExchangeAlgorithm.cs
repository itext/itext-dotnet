using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public enum KeyExchangeAlgorithm
	{
		/*
		 * Note that the values here are implementation-specific and arbitrary.
		 * It is recommended not to depend on the particular values (e.g. serialization).
		 */
		NULL,
		RSA,
		RSA_EXPORT,
		DHE_DSS,
		DHE_DSS_EXPORT,
		DHE_RSA,
		DHE_RSA_EXPORT,
		DH_DSS,
		DH_DSS_EXPORT,
		DH_RSA,
		DH_RSA_EXPORT,
		DH_anon,
		DH_anon_export,
		PSK,
		DHE_PSK,
		RSA_PSK,
		ECDH_ECDSA,
		ECDHE_ECDSA,
		ECDH_RSA,
		ECDHE_RSA,
		ECDH_anon,
		SRP,
		SRP_DSS,
		SRP_RSA,
	}
}
