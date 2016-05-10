using System;
using System.IO;

using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class TlsRsaUtilities
	{
		public static byte[] GenerateEncryptedPreMasterSecret(SecureRandom random,
			RsaKeyParameters rsaServerPublicKey, Stream output)
		{
			/*
			 * Choose a PremasterSecret and send it encrypted to the server
			 */
			byte[] premasterSecret = new byte[48];
			random.NextBytes(premasterSecret);
			TlsUtilities.WriteVersion(premasterSecret, 0);

			Pkcs1Encoding encoding = new Pkcs1Encoding(new RsaBlindedEngine());
			encoding.Init(true, new ParametersWithRandom(rsaServerPublicKey, random));

			try
			{
				byte[] keData = encoding.ProcessBlock(premasterSecret, 0, premasterSecret.Length);
                TlsUtilities.WriteOpaque16(keData, output);
			}
			catch (InvalidCipherTextException)
			{
				/*
				* This should never happen, only during decryption.
				*/
				throw new TlsFatalAlert(AlertDescription.internal_error);
			}

			return premasterSecret;
		}
	}
}
