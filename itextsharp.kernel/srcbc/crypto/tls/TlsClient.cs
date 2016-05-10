using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface TlsClient
	{
		/// <summary>
		/// Called at the start of a new TLS session, before any other methods.
		/// </summary>
		/// <param name="context">
		/// A <see cref="TlsProtocolHandler"/>
		/// </param>
		void Init(TlsClientContext context);

		/// <summary>
		/// Get the list of cipher suites that this client supports.
		/// </summary>
		/// <returns>
        /// An array of <see cref="CipherSuite"/>, each specifying a supported cipher suite.
		/// </returns>
		CipherSuite[] GetCipherSuites();

        /// <summary>
        /// Get the list of compression methods that this client supports.
        /// </summary>
        /// <returns>
        /// An array of <see cref="CompressionMethod"/>, each specifying a supported compression method.
        /// </returns>
        CompressionMethod[] GetCompressionMethods();

		/// <summary>
		/// Get the (optional) table of client extensions to be included in (extended) client hello.
		/// </summary>
		/// <returns>
        /// A <see cref="IDictionary"/> (<see cref="ExtensionType"/> -> byte[]). May be null.
		/// </returns>
		/// <exception cref="IOException"></exception>
		IDictionary GetClientExtensions();

		/// <summary>
		/// Reports the session ID once it has been determined.
		/// </summary>
		/// <param name="sessionID">
		/// A <see cref="System.Byte"/>
		/// </param>
		void NotifySessionID(byte[] sessionID);

		/// <summary>
		/// Report the cipher suite that was selected by the server.
		/// </summary>
		/// <remarks>
		/// The protocol handler validates this value against the offered cipher suites
		/// <seealso cref="GetCipherSuites"/>
		/// </remarks>
		/// <param name="selectedCipherSuite">
		/// A <see cref="CipherSuite"/>
		/// </param>
		void NotifySelectedCipherSuite(CipherSuite selectedCipherSuite);

        /// <summary>
        /// Report the compression method that was selected by the server.
        /// </summary>
        /// <remarks>
        /// The protocol handler validates this value against the offered compression methods
        /// <seealso cref="GetCompressionMethods"/>
        /// </remarks>
        /// <param name="selectedCompressionMethod">
        /// A <see cref="CompressionMethod"/>
        /// </param>
        void NotifySelectedCompressionMethod(CompressionMethod selectedCompressionMethod);

		/// <summary>
		/// Report whether the server supports secure renegotiation
		/// </summary>
		/// <remarks>
		/// The protocol handler automatically processes the relevant extensions
		/// </remarks>
		/// <param name="secureRenegotiation">
		/// A <see cref="System.Boolean"/>, true if the server supports secure renegotiation
		/// </param>
		/// <exception cref="IOException"></exception>
		void NotifySecureRenegotiation(bool secureRenegotiation);

		/// <summary>
		/// Report the extensions from an extended server hello.
		/// </summary>
		/// <remarks>
		/// Will only be called if we returned a non-null result from <see cref="GetClientExtensions"/>.
		/// </remarks>
		/// <param name="serverExtensions">
        /// A <see cref="IDictionary"/>  (<see cref="ExtensionType"/> -> byte[])
		/// </param>
		void ProcessServerExtensions(IDictionary serverExtensions);

		/// <summary>
		/// Return an implementation of <see cref="TlsKeyExchange"/> to negotiate the key exchange
		/// part of the protocol.
		/// </summary>
		/// <returns>
		/// A <see cref="TlsKeyExchange"/>
		/// </returns>
		/// <exception cref="IOException"/>
		TlsKeyExchange GetKeyExchange();

		/// <summary>
		/// Return an implementation of <see cref="TlsAuthentication"/> to handle authentication
		/// part of the protocol.
		/// </summary>
		/// <exception cref="IOException"/>
		TlsAuthentication GetAuthentication();

		/// <summary>
		/// Return an implementation of <see cref="TlsCompression"/> to handle record compression.
		/// </summary>
		/// <exception cref="IOException"/>
		TlsCompression GetCompression();

		/// <summary>
		/// Return an implementation of <see cref="TlsCipher"/> to use for encryption/decryption.
		/// </summary>
		/// <returns>
		/// A <see cref="TlsCipher"/>
		/// </returns>
		/// <exception cref="IOException"/>
		TlsCipher GetCipher();
	}
}
