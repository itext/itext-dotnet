using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// A temporary class to wrap old CertificateVerifyer stuff for new TlsAuthentication.
	/// </summary>
	[Obsolete]
	public class LegacyTlsAuthentication
		: TlsAuthentication
	{
		protected ICertificateVerifyer verifyer;

		public LegacyTlsAuthentication(ICertificateVerifyer verifyer)
		{
			this.verifyer = verifyer;
		}

		public virtual void NotifyServerCertificate(Certificate serverCertificate)
		{
			if (!this.verifyer.IsValid(serverCertificate.GetCerts()))
				throw new TlsFatalAlert(AlertDescription.user_canceled);
		}

		public virtual TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
		{
			return null;
		}
	}
}
