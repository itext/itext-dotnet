using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// A generic interface for key exchange implementations in TLS 1.0.
	/// </summary>
	public interface TlsKeyExchange
	{
		/// <exception cref="IOException"/>
		void SkipServerCertificate();

		/// <exception cref="IOException"/>
		void ProcessServerCertificate(Certificate serverCertificate);

		/// <exception cref="IOException"/>
		void SkipServerKeyExchange();

		/// <exception cref="IOException"/>
		void ProcessServerKeyExchange(Stream input);

		/// <exception cref="IOException"/>
		void ValidateCertificateRequest(CertificateRequest certificateRequest);

		/// <exception cref="IOException"/>
		void SkipClientCredentials();

		/// <exception cref="IOException"/>
		void ProcessClientCredentials(TlsCredentials clientCredentials);
		
		/// <exception cref="IOException"/>
		void GenerateClientKeyExchange(Stream output);

		/// <exception cref="IOException"/>
		byte[] GeneratePremasterSecret();
	}
}
