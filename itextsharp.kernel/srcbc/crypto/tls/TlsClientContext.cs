using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface TlsClientContext
	{
		SecureRandom SecureRandom { get; }

		SecurityParameters SecurityParameters { get; }

		object UserObject { get; set; }
	}
}
