using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class CertificateRequest
	{
		private ClientCertificateType[] certificateTypes;
		private IList certificateAuthorities;

		public CertificateRequest(ClientCertificateType[] certificateTypes, IList certificateAuthorities)
		{
			this.certificateTypes = certificateTypes;
			this.certificateAuthorities = certificateAuthorities;
		}

		public ClientCertificateType[] CertificateTypes
		{
			get { return certificateTypes; }
		}

		/// <returns>A <see cref="IList"/> of X509Name</returns>
		public IList CertificateAuthorities
		{
			get { return certificateAuthorities; }
		}
	}
}