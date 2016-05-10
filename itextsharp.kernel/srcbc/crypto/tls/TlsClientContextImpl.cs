using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class TlsClientContextImpl
		: TlsClientContext
	{
		private readonly SecureRandom secureRandom;
		private readonly SecurityParameters securityParameters;

		private object userObject = null;

		internal TlsClientContextImpl(SecureRandom secureRandom, SecurityParameters securityParameters)
		{
			this.secureRandom = secureRandom;
			this.securityParameters = securityParameters;
		}

		public virtual SecureRandom SecureRandom
		{
			get { return secureRandom; }
		}

		public virtual SecurityParameters SecurityParameters
		{
			get { return securityParameters; }
		}

		public virtual object UserObject
		{
			get { return userObject; }
			set { this.userObject = value; }
		}
	}
}
