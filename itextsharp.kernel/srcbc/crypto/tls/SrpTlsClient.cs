using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class SrpTlsClient
		: TlsClient
	{
		protected TlsCipherFactory cipherFactory;
		protected byte[] identity;
		protected byte[] password;

		protected TlsClientContext context;

        protected CompressionMethod selectedCompressionMethod;
        protected CipherSuite selectedCipherSuite;

		public SrpTlsClient(byte[] identity, byte[] password)
			: this(new DefaultTlsCipherFactory(), identity, password)
		{
		}

		public SrpTlsClient(TlsCipherFactory cipherFactory, byte[] identity, byte[] password)
		{
			this.cipherFactory = cipherFactory;
			this.identity = Arrays.Clone(identity);
			this.password = Arrays.Clone(password);
		}

		public virtual void Init(TlsClientContext context)
		{
			this.context = context;
		}

		public virtual CipherSuite[] GetCipherSuites()
		{
			return new CipherSuite[] {
				CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA,
			};
		}

		public virtual IDictionary GetClientExtensions()
		{
			IDictionary clientExtensions = Platform.CreateHashtable();

			MemoryStream srpData = new MemoryStream();
			TlsUtilities.WriteOpaque8(this.identity, srpData);
			clientExtensions[ExtensionType.srp] = srpData.ToArray();

			return clientExtensions;
		}

		public virtual CompressionMethod[] GetCompressionMethods()
		{
			return new CompressionMethod[] { CompressionMethod.NULL };
		}

		public virtual void NotifySessionID(byte[] sessionID)
		{
			// Currently ignored 
		}

		public virtual void NotifySelectedCipherSuite(CipherSuite selectedCipherSuite)
		{
			this.selectedCipherSuite = selectedCipherSuite;
		}

		public virtual void NotifySelectedCompressionMethod(CompressionMethod selectedCompressionMethod)
		{
            this.selectedCompressionMethod = selectedCompressionMethod;
        }

		public virtual void NotifySecureRenegotiation(bool secureRenegotiation)
		{
			if (!secureRenegotiation)
			{
				/*
				 * RFC 5746 3.4. If the extension is not present, the server does not support
				 * secure renegotiation; set secure_renegotiation flag to FALSE. In this case,
				 * some clients may want to terminate the handshake instead of continuing; see
				 * Section 4.1 for discussion.
				 */
//				throw new TlsFatalAlert(AlertDescription.handshake_failure);
			}
		}

		public virtual void ProcessServerExtensions(IDictionary serverExtensions)
		{
			// There is no server response for the SRP extension
		}

		public virtual TlsKeyExchange GetKeyExchange()
		{
			switch (selectedCipherSuite)
			{
				case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
					return CreateSrpKeyExchange(KeyExchangeAlgorithm.SRP);

				case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
					return CreateSrpKeyExchange(KeyExchangeAlgorithm.SRP_RSA);

				case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
					return CreateSrpKeyExchange(KeyExchangeAlgorithm.SRP_DSS);

				default:
					/*
					 * Note: internal error here; the TlsProtocolHandler verifies that the
					 * server-selected cipher suite was in the list of client-offered cipher
					 * suites, so if we now can't produce an implementation, we shouldn't have
					 * offered it!
					 */
					throw new TlsFatalAlert(AlertDescription.internal_error);
			}
		}
		
		public abstract TlsAuthentication GetAuthentication();

		public virtual TlsCompression GetCompression()
		{
			switch (selectedCompressionMethod)
			{
				case CompressionMethod.NULL:
					return new TlsNullCompression();

				default:
					/*
					 * Note: internal error here; the TlsProtocolHandler verifies that the
					 * server-selected compression method was in the list of client-offered compression
					 * methods, so if we now can't produce an implementation, we shouldn't have
					 * offered it!
					 */
					throw new TlsFatalAlert(AlertDescription.internal_error);
			}
		}

		public virtual TlsCipher GetCipher()
		{
			switch (selectedCipherSuite)
			{
				case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.cls_3DES_EDE_CBC, DigestAlgorithm.SHA);

				case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.AES_128_CBC, DigestAlgorithm.SHA);

				case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.AES_256_CBC, DigestAlgorithm.SHA);

				default:
					/*
					 * Note: internal error here; the TlsProtocolHandler verifies that the
					 * server-selected cipher suite was in the list of client-offered cipher
					 * suites, so if we now can't produce an implementation, we shouldn't have
					 * offered it!
					 */
					throw new TlsFatalAlert(AlertDescription.internal_error);
			}
		}

		protected virtual TlsKeyExchange CreateSrpKeyExchange(KeyExchangeAlgorithm keyExchange)
		{
			return new TlsSrpKeyExchange(context, keyExchange, identity, password);
		}
	}
}
