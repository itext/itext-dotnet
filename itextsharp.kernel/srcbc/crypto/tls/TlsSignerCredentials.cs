using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface TlsSignerCredentials : TlsCredentials
	{
		/// <exception cref="IOException"></exception>
		byte[] GenerateCertificateSignature(byte[] md5andsha1);
	}
}
