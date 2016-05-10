using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class PskTlsClient
		:TlsClient
	{
		protected TlsCipherFactory cipherFactory;
		protected TlsPskIdentity pskIdentity;

        protected TlsClientContext context;

        protected CompressionMethod selectedCompressionMethod;
		protected CipherSuite selectedCipherSuite;

        public PskTlsClient(TlsPskIdentity pskIdentity)
			: this(new DefaultTlsCipherFactory(), pskIdentity)
		{
		}

        public PskTlsClient(TlsCipherFactory cipherFactory, TlsPskIdentity pskIdentity)
		{
			this.cipherFactory = cipherFactory;
			this.pskIdentity = pskIdentity;
		}

        public virtual void Init(TlsClientContext context)
		{
			this.context = context;
		}

        public virtual CipherSuite[] GetCipherSuites()
		{
			return new CipherSuite[] {
				CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA,
				CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA,
				CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA,
				CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA,
				CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA,
				CipherSuite.TLS_PSK_WITH_RC4_128_SHA,
			};
		}

        public virtual IDictionary GetClientExtensions()
		{
			return null;
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
		}

        public virtual TlsKeyExchange GetKeyExchange()
		{
			switch (selectedCipherSuite)
			{
				case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_PSK_WITH_RC4_128_SHA:
					return CreatePskKeyExchange(KeyExchangeAlgorithm.PSK);

                case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA:
                    return CreatePskKeyExchange(KeyExchangeAlgorithm.RSA_PSK);

                case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
                    return CreatePskKeyExchange(KeyExchangeAlgorithm.DHE_PSK);

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
				case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.cls_3DES_EDE_CBC,
						DigestAlgorithm.SHA);

                case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.AES_128_CBC,
						DigestAlgorithm.SHA);

                case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.AES_256_CBC,
						DigestAlgorithm.SHA);

                case CipherSuite.TLS_PSK_WITH_RC4_128_SHA:
                case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA:
                case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
                    return cipherFactory.CreateCipher(context, EncryptionAlgorithm.RC4_128,
                        DigestAlgorithm.SHA);

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

        protected virtual TlsKeyExchange CreatePskKeyExchange(KeyExchangeAlgorithm keyExchange)
		{
			return new TlsPskKeyExchange(context, keyExchange, pskIdentity);
		}
	}
}
