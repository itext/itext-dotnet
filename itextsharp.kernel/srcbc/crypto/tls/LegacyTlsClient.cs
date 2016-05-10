using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// A temporary class to use LegacyTlsAuthentication 
	/// </summary>
	[Obsolete]
	public class LegacyTlsClient
		: DefaultTlsClient
	{
		[Obsolete]
		protected ICertificateVerifyer verifyer;

		[Obsolete]
		public LegacyTlsClient(ICertificateVerifyer verifyer)
		{
			this.verifyer = verifyer;
		}

		public override TlsAuthentication GetAuthentication()
		{
			return new LegacyTlsAuthentication(verifyer);
		}
	}
}