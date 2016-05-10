using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// TLS 1.0 DH key exchange.
	/// </summary>
	internal class TlsDHKeyExchange
		: TlsKeyExchange
	{
		protected TlsClientContext context;
		protected KeyExchangeAlgorithm keyExchange;
		protected TlsSigner tlsSigner;

		protected AsymmetricKeyParameter serverPublicKey = null;
		protected DHPublicKeyParameters dhAgreeServerPublicKey = null;
		protected TlsAgreementCredentials agreementCredentials;
		protected DHPrivateKeyParameters dhAgreeClientPrivateKey = null;

		internal TlsDHKeyExchange(TlsClientContext context, KeyExchangeAlgorithm keyExchange)
		{
			switch (keyExchange)
			{
				case KeyExchangeAlgorithm.DH_RSA:
				case KeyExchangeAlgorithm.DH_DSS:
					this.tlsSigner = null;
					break;
				case KeyExchangeAlgorithm.DHE_RSA:
					this.tlsSigner = new TlsRsaSigner();
					break;
				case KeyExchangeAlgorithm.DHE_DSS:
					this.tlsSigner = new TlsDssSigner();
					break;
				default:
					throw new ArgumentException("unsupported key exchange algorithm", "keyExchange");
			}

			this.context = context;
			this.keyExchange = keyExchange;
		}

		public virtual void SkipServerCertificate()
		{
			throw new TlsFatalAlert(AlertDescription.unexpected_message);
		}

		public virtual void ProcessServerCertificate(Certificate serverCertificate)
		{
			X509CertificateStructure x509Cert = serverCertificate.certs[0];
			SubjectPublicKeyInfo keyInfo = x509Cert.SubjectPublicKeyInfo;

			try
			{
				this.serverPublicKey = PublicKeyFactory.CreateKey(keyInfo);
			}
			catch (Exception)
			{
				throw new TlsFatalAlert(AlertDescription.unsupported_certificate);
			}

			if (tlsSigner == null)
			{
				try
				{
					this.dhAgreeServerPublicKey = ValidateDHPublicKey((DHPublicKeyParameters)this.serverPublicKey);
				}
				catch (InvalidCastException)
				{
					throw new TlsFatalAlert(AlertDescription.certificate_unknown);
				}

				TlsUtilities.ValidateKeyUsage(x509Cert, KeyUsage.KeyAgreement);
			}
			else
			{
				if (!tlsSigner.IsValidPublicKey(this.serverPublicKey))
				{
					throw new TlsFatalAlert(AlertDescription.certificate_unknown);
				}

				TlsUtilities.ValidateKeyUsage(x509Cert, KeyUsage.DigitalSignature);
			}

			// TODO
			/*
			* Perform various checks per RFC2246 7.4.2: "Unless otherwise specified, the
			* signing algorithm for the certificate must be the same as the algorithm for the
			* certificate key."
			*/
		}

		public virtual void SkipServerKeyExchange()
		{
			// OK
		}

		public virtual void ProcessServerKeyExchange(Stream input)
		{
			throw new TlsFatalAlert(AlertDescription.unexpected_message);
		}

		public virtual void ValidateCertificateRequest(CertificateRequest certificateRequest)
		{
			ClientCertificateType[] types = certificateRequest.CertificateTypes;
			foreach (ClientCertificateType type in types)
			{
				switch (type)
				{
					case ClientCertificateType.rsa_sign:
					case ClientCertificateType.dss_sign:
					case ClientCertificateType.rsa_fixed_dh:
					case ClientCertificateType.dss_fixed_dh:
					case ClientCertificateType.ecdsa_sign:
						break;
					default:
						throw new TlsFatalAlert(AlertDescription.illegal_parameter);
				}
			}
		}

		public virtual void SkipClientCredentials()
		{
			this.agreementCredentials = null;
		}

		public virtual void ProcessClientCredentials(TlsCredentials clientCredentials)
		{
			if (clientCredentials is TlsAgreementCredentials)
			{
				// TODO Validate client cert has matching parameters (see 'areCompatibleParameters')?

				this.agreementCredentials = (TlsAgreementCredentials)clientCredentials;
			}
			else if (clientCredentials is TlsSignerCredentials)
			{
				// OK
			}
			else
			{
				throw new TlsFatalAlert(AlertDescription.internal_error);
			}
		}

		public virtual void GenerateClientKeyExchange(Stream output)
		{
			/*
			 * RFC 2246 7.4.7.2 If the client certificate already contains a suitable
			 * Diffie-Hellman key, then Yc is implicit and does not need to be sent again. In
			 * this case, the Client Key Exchange message will be sent, but will be empty.
			 */
			if (agreementCredentials == null)
			{
				GenerateEphemeralClientKeyExchange(dhAgreeServerPublicKey.Parameters, output);
			}
		}

        public virtual byte[] GeneratePremasterSecret()
		{
			if (agreementCredentials != null)
			{
				return agreementCredentials.GenerateAgreement(dhAgreeServerPublicKey);
			}

			return CalculateDHBasicAgreement(dhAgreeServerPublicKey, dhAgreeClientPrivateKey);
		}
		
		protected virtual bool AreCompatibleParameters(DHParameters a, DHParameters b)
		{
			return a.P.Equals(b.P) && a.G.Equals(b.G);
		}

		protected virtual byte[] CalculateDHBasicAgreement(DHPublicKeyParameters publicKey,
			DHPrivateKeyParameters privateKey)
		{
			return TlsDHUtilities.CalculateDHBasicAgreement(publicKey, privateKey);
		}

		protected virtual AsymmetricCipherKeyPair GenerateDHKeyPair(DHParameters dhParams)
		{
			return TlsDHUtilities.GenerateDHKeyPair(context.SecureRandom, dhParams);
		}

		protected virtual void GenerateEphemeralClientKeyExchange(DHParameters dhParams, Stream output)
		{
			this.dhAgreeClientPrivateKey = TlsDHUtilities.GenerateEphemeralClientKeyExchange(
				context.SecureRandom, dhParams, output);
		}

		protected virtual DHPublicKeyParameters ValidateDHPublicKey(DHPublicKeyParameters key)
		{
			return TlsDHUtilities.ValidateDHPublicKey(key);
		}
	}
}
