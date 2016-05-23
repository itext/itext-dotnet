using System;
using System.IO;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;

namespace iTextSharp.Signatures
{
	internal static class SignUtils
	{
		/// <exception cref="CertificateException"/>
		/// <exception cref="CrlException"/>
		internal static X509Crl ParseCrlFromStream(Stream input)
		{
			return new X509CrlParser().ReadCrl(input);
        }

        internal static byte[] GetExtensionValueByOid(X509Certificate certificate, String oid) {
            return certificate.GetExtensionValue(oid)?.GetOctets();
        }
    }
}
