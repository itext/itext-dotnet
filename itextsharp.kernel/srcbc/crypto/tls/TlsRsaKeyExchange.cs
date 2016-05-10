using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <summary>
	/// TLS 1.0 RSA key exchange.
	/// </summary>
	internal class TlsRsaKeyExchange
		: TlsKeyExchange
	{
		protected TlsClientContext context;

		protected AsymmetricKeyParameter serverPublicKey = null;

        protected RsaKeyParameters rsaServerPublicKey = null;

        protected byte[] premasterSecret;

		internal TlsRsaKeyExchange(TlsClientContext context)
		{
			this.context = context;
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
//			catch (RuntimeException)
			catch (Exception)
			{
				throw new TlsFatalAlert(AlertDescription.unsupported_certificate);
			}

			// Sanity check the PublicKeyFactory
			if (this.serverPublicKey.IsPrivate)
			{
				throw new TlsFatalAlert(AlertDescription.internal_error);
			}

			this.rsaServerPublicKey = ValidateRsaPublicKey((RsaKeyParameters)this.serverPublicKey);

			TlsUtilities.ValidateKeyUsage(x509Cert, KeyUsage.KeyEncipherment);

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
					case ClientCertificateType.ecdsa_sign:
						break;
					default:
						throw new TlsFatalAlert(AlertDescription.illegal_parameter);
				}
			}
		}

		public virtual void SkipClientCredentials()
		{
			// OK
		}

		public virtual void ProcessClientCredentials(TlsCredentials clientCredentials)
		{
			if (!(clientCredentials is TlsSignerCredentials))
			{
				throw new TlsFatalAlert(AlertDescription.internal_error);
			}
		}
		
        public virtual void GenerateClientKeyExchange(Stream output)
		{
			this.premasterSecret = TlsRsaUtilities.GenerateEncryptedPreMasterSecret(
				context.SecureRandom, this.rsaServerPublicKey, output);
		}

		public virtual byte[] GeneratePremasterSecret()
		{
			byte[] tmp = this.premasterSecret;
			this.premasterSecret = null;
			return tmp;
		}

    	// Would be needed to process RSA_EXPORT server key exchange
//	    protected virtual void ProcessRsaServerKeyExchange(Stream input, ISigner signer)
//	    {
//	        Stream sigIn = input;
//	        if (signer != null)
//	        {
//	            sigIn = new SignerStream(input, signer, null);
//	        }
//
//	        byte[] modulusBytes = TlsUtilities.ReadOpaque16(sigIn);
//	        byte[] exponentBytes = TlsUtilities.ReadOpaque16(sigIn);
//
//	        if (signer != null)
//	        {
//	            byte[] sigByte = TlsUtilities.ReadOpaque16(input);
//
//	            if (!signer.VerifySignature(sigByte))
//	            {
//	                handler.FailWithError(AlertLevel.fatal, AlertDescription.decrypt_error);
//	            }
//	        }
//
//	        BigInteger modulus = new BigInteger(1, modulusBytes);
//	        BigInteger exponent = new BigInteger(1, exponentBytes);
//
//	        this.rsaServerPublicKey = ValidateRSAPublicKey(new RsaKeyParameters(false, modulus, exponent));
//	    }

        protected virtual RsaKeyParameters ValidateRsaPublicKey(RsaKeyParameters key)
		{
			// TODO What is the minimum bit length required?
//			key.Modulus.BitLength;

			if (!key.Exponent.IsProbablePrime(2))
			{
				throw new TlsFatalAlert(AlertDescription.illegal_parameter);
			}

			return key;
		}
	}
}
