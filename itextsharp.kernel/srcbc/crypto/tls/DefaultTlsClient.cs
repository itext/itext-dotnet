using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class DefaultTlsClient
		: TlsClient
	{
		protected TlsCipherFactory cipherFactory;

		protected TlsClientContext context;

        protected CompressionMethod selectedCompressionMethod;
        protected CipherSuite selectedCipherSuite;

		public DefaultTlsClient()
			: this(new DefaultTlsCipherFactory())
		{
		}

		public DefaultTlsClient(TlsCipherFactory cipherFactory)
		{
			this.cipherFactory = cipherFactory;
		}

		public virtual void Init(TlsClientContext context)
		{
			this.context = context;
		}

        public virtual CipherSuite[] GetCipherSuites()
		{
			return new CipherSuite[] {
				CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA,
				CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA,
				CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA,
				CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
				CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA,
                CipherSuite.TLS_RSA_WITH_RC4_128_SHA,
			};
		}

        public virtual CompressionMethod[] GetCompressionMethods()
        {
			/*
			 * To offer DEFLATE compression, override this method:
			 *     return new CompressionMethod[] { CompressionMethod.DEFLATE, CompressionMethod.NULL };
			 */

            return new CompressionMethod[] { CompressionMethod.NULL };
        }

        public virtual IDictionary GetClientExtensions()
		{
			return null;
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
				 * RFC 5746 3.4.
				 * If the extension is not present, the server does not support
				 * secure renegotiation; set secure_renegotiation flag to FALSE.
				 * In this case, some clients may want to terminate the handshake
				 * instead of continuing; see Section 4.1 for discussion.
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
				case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
					return CreateRsaKeyExchange();

				case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA:
					return CreateDHKeyExchange(KeyExchangeAlgorithm.DH_DSS);

				case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA:
					return CreateDHKeyExchange(KeyExchangeAlgorithm.DH_RSA);

				case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
					return CreateDheKeyExchange(KeyExchangeAlgorithm.DHE_DSS);

				case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
					return CreateDheKeyExchange(KeyExchangeAlgorithm.DHE_RSA);

                case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
                    return CreateECDHKeyExchange(KeyExchangeAlgorithm.ECDH_ECDSA);

                case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
                    return CreateECDheKeyExchange(KeyExchangeAlgorithm.ECDHE_ECDSA);

                case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
                    return CreateECDHKeyExchange(KeyExchangeAlgorithm.ECDH_RSA);

                case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
                    return CreateECDheKeyExchange(KeyExchangeAlgorithm.ECDHE_RSA);

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

				case CompressionMethod.DEFLATE:
					return new TlsDeflateCompression();

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
				case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
				case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.cls_3DES_EDE_CBC, DigestAlgorithm.SHA);

                case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
                case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
                    return cipherFactory.CreateCipher(context, EncryptionAlgorithm.RC4_128, DigestAlgorithm.SHA);

                case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
				case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
					return cipherFactory.CreateCipher(context, EncryptionAlgorithm.AES_128_CBC, DigestAlgorithm.SHA);

				case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
				case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
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

		protected virtual TlsKeyExchange CreateDHKeyExchange(KeyExchangeAlgorithm keyExchange)
		{
			return new TlsDHKeyExchange(context, keyExchange);
		}

        protected virtual TlsKeyExchange CreateDheKeyExchange(KeyExchangeAlgorithm keyExchange)
		{
			return new TlsDheKeyExchange(context, keyExchange);
		}

        protected virtual TlsKeyExchange CreateECDHKeyExchange(KeyExchangeAlgorithm keyExchange)
        {
            return new TlsECDHKeyExchange(context, keyExchange);
        }

        protected virtual TlsKeyExchange CreateECDheKeyExchange(KeyExchangeAlgorithm keyExchange)
        {
            return new TlsECDheKeyExchange(context, keyExchange);
        }

        protected virtual TlsKeyExchange CreateRsaKeyExchange()
		{
			return new TlsRsaKeyExchange(context);
		}
    }
}
