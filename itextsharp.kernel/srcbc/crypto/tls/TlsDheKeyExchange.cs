using System;
using System.IO;

using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class TlsDheKeyExchange
		: TlsDHKeyExchange
	{
		internal TlsDheKeyExchange(TlsClientContext context, KeyExchangeAlgorithm keyExchange)
			: base(context, keyExchange)
		{
		}

		public override void SkipServerKeyExchange()
		{
			throw new TlsFatalAlert(AlertDescription.unexpected_message);
		}

		public override void ProcessServerKeyExchange(Stream input)
		{
			SecurityParameters securityParameters = context.SecurityParameters;

			ISigner signer = InitSigner(tlsSigner, securityParameters);
			Stream sigIn = new SignerStream(input, signer, null);

			byte[] pBytes = TlsUtilities.ReadOpaque16(sigIn);
			byte[] gBytes = TlsUtilities.ReadOpaque16(sigIn);
			byte[] YsBytes = TlsUtilities.ReadOpaque16(sigIn);

			byte[] sigByte = TlsUtilities.ReadOpaque16(input);
			if (!signer.VerifySignature(sigByte))
			{
                throw new TlsFatalAlert(AlertDescription.decrypt_error);
			}

			BigInteger p = new BigInteger(1, pBytes);
			BigInteger g = new BigInteger(1, gBytes);
			BigInteger Ys = new BigInteger(1, YsBytes);

			this.dhAgreeServerPublicKey = ValidateDHPublicKey(
				new DHPublicKeyParameters(Ys, new DHParameters(p, g)));
		}

		protected virtual ISigner InitSigner(TlsSigner tlsSigner, SecurityParameters securityParameters)
		{
			ISigner signer = tlsSigner.CreateVerifyer(this.serverPublicKey);
			signer.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
			signer.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
			return signer;
		}
	}
}
