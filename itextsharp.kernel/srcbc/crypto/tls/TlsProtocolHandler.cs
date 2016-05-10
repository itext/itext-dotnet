using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <remarks>An implementation of all high level protocols in TLS 1.0.</remarks>
    public class TlsProtocolHandler
    {
        /*
        * Our Connection states
        */
        private const short CS_CLIENT_HELLO_SEND = 1;
        private const short CS_SERVER_HELLO_RECEIVED = 2;
        private const short CS_SERVER_CERTIFICATE_RECEIVED = 3;
        private const short CS_SERVER_KEY_EXCHANGE_RECEIVED = 4;
        private const short CS_CERTIFICATE_REQUEST_RECEIVED = 5;
        private const short CS_SERVER_HELLO_DONE_RECEIVED = 6;
        private const short CS_CLIENT_KEY_EXCHANGE_SEND = 7;
        private const short CS_CERTIFICATE_VERIFY_SEND = 8;
        private const short CS_CLIENT_CHANGE_CIPHER_SPEC_SEND = 9;
        private const short CS_CLIENT_FINISHED_SEND = 10;
        private const short CS_SERVER_CHANGE_CIPHER_SPEC_RECEIVED = 11;
        private const short CS_DONE = 12;

        private static readonly byte[] emptybuf = new byte[0];

        private static readonly string TLS_ERROR_MESSAGE = "Internal TLS error, this could be an attack";

        /*
        * Queues for data from some protocols.
        */

        private ByteQueue applicationDataQueue = new ByteQueue();
        private ByteQueue alertQueue = new ByteQueue(2);
        private ByteQueue handshakeQueue = new ByteQueue();

        /*
        * The Record Stream we use
        */
        private RecordStream rs;
        private SecureRandom random;

        private TlsStream tlsStream = null;

        private bool closed = false;
        private bool failedWithError = false;
        private bool appDataReady = false;
        private IDictionary clientExtensions;

        private SecurityParameters securityParameters = null;

        private TlsClientContextImpl tlsClientContext = null;
        private TlsClient tlsClient = null;
        private CipherSuite[] offeredCipherSuites = null;
        private CompressionMethod[] offeredCompressionMethods = null;
        private TlsKeyExchange keyExchange = null;
        private TlsAuthentication authentication = null;
        private CertificateRequest certificateRequest = null;

        private short connection_state = 0;

        private static SecureRandom CreateSecureRandom()
        {
            /*
             * We use our threaded seed generator to generate a good random seed. If the user
             * has a better random seed, he should use the constructor with a SecureRandom.
             *
             * Hopefully, 20 bytes in fast mode are good enough.
             */
            byte[] seed = new ThreadedSeedGenerator().GenerateSeed(20, true);

            return new SecureRandom(seed);
        }

        public TlsProtocolHandler(
            Stream s)
            : this(s, s)
        {
        }

        public TlsProtocolHandler(
            Stream			s,
            SecureRandom	sr)
            : this(s, s, sr)
        {
        }

        /// <remarks>Both streams can be the same object</remarks>
        public TlsProtocolHandler(
            Stream	inStr,
            Stream	outStr)
            : this(inStr, outStr, CreateSecureRandom())
        {
        }

        /// <remarks>Both streams can be the same object</remarks>
        public TlsProtocolHandler(
            Stream			inStr,
            Stream			outStr,
            SecureRandom	sr)
        {
            this.rs = new RecordStream(this, inStr, outStr);
            this.random = sr;
        }

        internal void ProcessData(
            ContentType	protocol,
            byte[]		buf,
            int			offset,
            int			len)
        {
            /*
            * Have a look at the protocol type, and add it to the correct queue.
            */
            switch (protocol)
            {
                case ContentType.change_cipher_spec:
                    ProcessChangeCipherSpec(buf, offset, len);
                    break;
                case ContentType.alert:
                    alertQueue.AddData(buf, offset, len);
                    ProcessAlert();
                    break;
                case ContentType.handshake:
                    handshakeQueue.AddData(buf, offset, len);
                    ProcessHandshake();
                    break;
                case ContentType.application_data:
                    if (!appDataReady)
                    {
                        this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                    }
                    applicationDataQueue.AddData(buf, offset, len);
                    ProcessApplicationData();
                    break;
                default:
                    /*
                    * Uh, we don't know this protocol.
                    *
                    * RFC2246 defines on page 13, that we should ignore this.
                    */
                    break;
            }
        }

        private void ProcessHandshake()
        {
            bool read;
            do
            {
                read = false;

                /*
                * We need the first 4 bytes, they contain type and length of
                * the message.
                */
                if (handshakeQueue.Available >= 4)
                {
                    byte[] beginning = new byte[4];
                    handshakeQueue.Read(beginning, 0, 4, 0);
                    MemoryStream bis = new MemoryStream(beginning, false);
                    HandshakeType type = (HandshakeType)TlsUtilities.ReadUint8(bis);
                    int len = TlsUtilities.ReadUint24(bis);

                    /*
                    * Check if we have enough bytes in the buffer to read
                    * the full message.
                    */
                    if (handshakeQueue.Available >= (len + 4))
                    {
                        /*
                        * Read the message.
                        */
                        byte[] buf = handshakeQueue.RemoveData(len, 4);

                        /*
                         * RFC 2246 7.4.9. The value handshake_messages includes all
                         * handshake messages starting at client hello up to, but not
                         * including, this finished message. [..] Note: [Also,] Hello Request
                         * messages are omitted from handshake hashes.
                         */
                        switch (type)
                        {
                            case HandshakeType.hello_request:
                            case HandshakeType.finished:
                                break;
                            default:
                                rs.UpdateHandshakeData(beginning, 0, 4);
                                rs.UpdateHandshakeData(buf, 0, len);
                                break;
                        }

                        /*
                        * Now, parse the message.
                        */
                        ProcessHandshakeMessage(type, buf);
                        read = true;
                    }
                }
            }
            while (read);
        }

        private void ProcessHandshakeMessage(HandshakeType type, byte[] buf)
        {
            MemoryStream inStr = new MemoryStream(buf, false);

            /*
            * Check the type.
            */
            switch (type)
            {
                case HandshakeType.certificate:
                {
                    switch (connection_state)
                    {
                        case CS_SERVER_HELLO_RECEIVED:
                        {
                            // Parse the Certificate message and send to cipher suite

                            Certificate serverCertificate = Certificate.Parse(inStr);

                            AssertEmpty(inStr);

                            this.keyExchange.ProcessServerCertificate(serverCertificate);

                            this.authentication = tlsClient.GetAuthentication();
                            this.authentication.NotifyServerCertificate(serverCertificate);

                            break;
                        }
                        default:
                            this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                            break;
                    }

                    connection_state = CS_SERVER_CERTIFICATE_RECEIVED;
                    break;
                }
                case HandshakeType.finished:
                    switch (connection_state)
                    {
                        case CS_SERVER_CHANGE_CIPHER_SPEC_RECEIVED:
                            /*
                             * Read the checksum from the finished message, it has always 12 bytes.
                             */
                            byte[] serverVerifyData = new byte[12];
                            TlsUtilities.ReadFully(serverVerifyData, inStr);

                            AssertEmpty(inStr);

                            /*
                             * Calculate our own checksum.
                             */
                            byte[] expectedServerVerifyData = TlsUtilities.PRF(
                                securityParameters.masterSecret, "server finished",
                                rs.GetCurrentHash(), 12);

                            /*
                             * Compare both checksums.
                             */
                            if (!Arrays.ConstantTimeAreEqual(expectedServerVerifyData, serverVerifyData))
                            {
                                /*
                                 * Wrong checksum in the finished message.
                                 */
                                this.FailWithError(AlertLevel.fatal, AlertDescription.decrypt_error);
                            }

                            connection_state = CS_DONE;

                            /*
                            * We are now ready to receive application data.
                            */
                            this.appDataReady = true;
                            break;
                        default:
                            this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                            break;
                    }
                    break;
                case HandshakeType.server_hello:
                    switch (connection_state)
                    {
                        case CS_CLIENT_HELLO_SEND:
                            /*
                             * Read the server hello message
                             */
                            TlsUtilities.CheckVersion(inStr);

                            /*
                             * Read the server random
                             */
                            securityParameters.serverRandom = new byte[32];
                            TlsUtilities.ReadFully(securityParameters.serverRandom, inStr);

                            byte[] sessionID = TlsUtilities.ReadOpaque8(inStr);
                            if (sessionID.Length > 32)
                            {
                                this.FailWithError(AlertLevel.fatal, AlertDescription.illegal_parameter);
                            }

                            this.tlsClient.NotifySessionID(sessionID);

                            /*
                             * Find out which CipherSuite the server has chosen and check that
                             * it was one of the offered ones.
                             */
                            CipherSuite selectedCipherSuite = (CipherSuite)TlsUtilities.ReadUint16(inStr);
                            if (!ArrayContains(offeredCipherSuites, selectedCipherSuite)
                                || selectedCipherSuite == CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV)
                            {
                                this.FailWithError(AlertLevel.fatal, AlertDescription.illegal_parameter);
                            }

                            this.tlsClient.NotifySelectedCipherSuite(selectedCipherSuite);

                            /*
                             * Find out which CompressionMethod the server has chosen and check that
                             * it was one of the offered ones.
                             */
                            CompressionMethod selectedCompressionMethod = (CompressionMethod)TlsUtilities.ReadUint8(inStr);
                            if (!ArrayContains(offeredCompressionMethods, selectedCompressionMethod))
                            {
                                this.FailWithError(AlertLevel.fatal, AlertDescription.illegal_parameter);
                            }

                            this.tlsClient.NotifySelectedCompressionMethod(selectedCompressionMethod);

                            /*
                             * RFC3546 2.2 The extended server hello message format MAY be
                             * sent in place of the server hello message when the client has
                             * requested extended functionality via the extended client hello
                             * message specified in Section 2.1.
                             * ...
                             * Note that the extended server hello message is only sent in response
                             * to an extended client hello message.  This prevents the possibility
                             * that the extended server hello message could "break" existing TLS 1.0
                             * clients.
                             */

                            /*
                             * TODO RFC 3546 2.3
                             * If [...] the older session is resumed, then the server MUST ignore
                             * extensions appearing in the client hello, and send a server hello
                             * containing no extensions.
                             */

                            // ExtensionType -> byte[]
                            IDictionary serverExtensions = Platform.CreateHashtable();

                            if (inStr.Position < inStr.Length)
                            {
                                // Process extensions from extended server hello
                                byte[] extBytes = TlsUtilities.ReadOpaque16(inStr);

                                MemoryStream ext = new MemoryStream(extBytes, false);
                                while (ext.Position < ext.Length)
                                {
                                    ExtensionType extType = (ExtensionType)TlsUtilities.ReadUint16(ext);
                                    byte[] extValue = TlsUtilities.ReadOpaque16(ext);

                                    // Note: RFC 5746 makes a special case for EXT_RenegotiationInfo
                                    if (extType != ExtensionType.renegotiation_info
                                        && !clientExtensions.Contains(extType))
                                    {
                                        /*
                                         * RFC 3546 2.3
                                         * Note that for all extension types (including those defined in
                                         * future), the extension type MUST NOT appear in the extended server
                                         * hello unless the same extension type appeared in the corresponding
                                         * client hello.  Thus clients MUST abort the handshake if they receive
                                         * an extension type in the extended server hello that they did not
                                         * request in the associated (extended) client hello.
                                         */
                                        this.FailWithError(AlertLevel.fatal, AlertDescription.unsupported_extension);
                                    }

                                    if (serverExtensions.Contains(extType))
                                    {
                                        /*
                                         * RFC 3546 2.3
                                         * Also note that when multiple extensions of different types are
                                         * present in the extended client hello or the extended server hello,
                                         * the extensions may appear in any order. There MUST NOT be more than
                                         * one extension of the same type.
                                         */
                                        this.FailWithError(AlertLevel.fatal, AlertDescription.illegal_parameter);
                                    }

                                    serverExtensions.Add(extType, extValue);
                                }
                            }

                            AssertEmpty(inStr);

                            /*
                             * RFC 5746 3.4. When a ServerHello is received, the client MUST check if it
                             * includes the "renegotiation_info" extension:
                             */
                            {
                                bool secure_negotiation = serverExtensions.Contains(ExtensionType.renegotiation_info);

                                /*
                                 * If the extension is present, set the secure_renegotiation flag
                                 * to TRUE.  The client MUST then verify that the length of the
                                 * "renegotiated_connection" field is zero, and if it is not, MUST
                                 * abort the handshake (by sending a fatal handshake_failure
                                 * alert).
                                 */
                                if (secure_negotiation)
                                {
                                    byte[] renegExtValue = (byte[])serverExtensions[ExtensionType.renegotiation_info];

                                    if (!Arrays.ConstantTimeAreEqual(renegExtValue,
                                        CreateRenegotiationInfo(emptybuf)))
                                    {
                                        this.FailWithError(AlertLevel.fatal, AlertDescription.handshake_failure);
                                    }
                                }

                                tlsClient.NotifySecureRenegotiation(secure_negotiation);
                            }

                            if (clientExtensions != null)
                            {
                                tlsClient.ProcessServerExtensions(serverExtensions);
                            }

                            this.keyExchange = tlsClient.GetKeyExchange();

                            connection_state = CS_SERVER_HELLO_RECEIVED;
                            break;
                        default:
                            this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                            break;
                    }
                    break;
                case HandshakeType.server_hello_done:
                    switch (connection_state)
                    {
                        case CS_SERVER_HELLO_RECEIVED:
                        case CS_SERVER_CERTIFICATE_RECEIVED:
                        case CS_SERVER_KEY_EXCHANGE_RECEIVED:
                        case CS_CERTIFICATE_REQUEST_RECEIVED:

                            // NB: Original code used case label fall-through

                            if (connection_state == CS_SERVER_HELLO_RECEIVED)
                            {
                                // There was no server certificate message; check it's OK
                                this.keyExchange.SkipServerCertificate();
                                this.authentication = null;

                                // There was no server key exchange message; check it's OK
                                this.keyExchange.SkipServerKeyExchange();
                            }
                            else if (connection_state == CS_SERVER_CERTIFICATE_RECEIVED)
                            {
                                // There was no server key exchange message; check it's OK
                                this.keyExchange.SkipServerKeyExchange();
                            }

                            AssertEmpty(inStr);

                            connection_state = CS_SERVER_HELLO_DONE_RECEIVED;

                            TlsCredentials clientCreds = null;
                            if (certificateRequest == null)
                            {
                                this.keyExchange.SkipClientCredentials();
                            }
                            else
                            {
                                clientCreds = this.authentication.GetClientCredentials(certificateRequest);

                                Certificate clientCert;
                                if (clientCreds == null)
                                {
                                    this.keyExchange.SkipClientCredentials();
                                    clientCert = Certificate.EmptyChain;
                                }
                                else
                                {
                                    this.keyExchange.ProcessClientCredentials(clientCreds);
                                    clientCert = clientCreds.Certificate;
                                }

                                SendClientCertificate(clientCert);
                            }

                            /*
                             * Send the client key exchange message, depending on the key
                             * exchange we are using in our CipherSuite.
                             */
                            SendClientKeyExchange();

                            connection_state = CS_CLIENT_KEY_EXCHANGE_SEND;

                            if (clientCreds != null && clientCreds is TlsSignerCredentials)
                            {
                                TlsSignerCredentials signerCreds = (TlsSignerCredentials)clientCreds;
                                byte[] md5andsha1 = rs.GetCurrentHash();
                                byte[] clientCertificateSignature = signerCreds.GenerateCertificateSignature(
                                    md5andsha1);
                                SendCertificateVerify(clientCertificateSignature);

                                connection_state = CS_CERTIFICATE_VERIFY_SEND;
                            }
                    
                            /*
                            * Now, we send change cipher state
                            */
                            byte[] cmessage = new byte[1];
                            cmessage[0] = 1;
                            rs.WriteMessage(ContentType.change_cipher_spec, cmessage, 0, cmessage.Length);

                            connection_state = CS_CLIENT_CHANGE_CIPHER_SPEC_SEND;

                            /*
                             * Calculate the master_secret
                             */
                            byte[] pms = this.keyExchange.GeneratePremasterSecret();

                            securityParameters.masterSecret = TlsUtilities.PRF(pms, "master secret",
                                TlsUtilities.Concat(securityParameters.clientRandom, securityParameters.serverRandom),
                                48);

                            // TODO Is there a way to ensure the data is really overwritten?
                            /*
                             * RFC 2246 8.1. The pre_master_secret should be deleted from
                             * memory once the master_secret has been computed.
                             */
                            Array.Clear(pms, 0, pms.Length);

                            /*
                             * Initialize our cipher suite
                             */
                            rs.ClientCipherSpecDecided(tlsClient.GetCompression(), tlsClient.GetCipher());

                            /*
                             * Send our finished message.
                             */
                            byte[] clientVerifyData = TlsUtilities.PRF(securityParameters.masterSecret,
                                "client finished", rs.GetCurrentHash(), 12);

                            MemoryStream bos = new MemoryStream();
                            TlsUtilities.WriteUint8((byte)HandshakeType.finished, bos);
                            TlsUtilities.WriteOpaque24(clientVerifyData, bos);
                            byte[] message = bos.ToArray();

                            rs.WriteMessage(ContentType.handshake, message, 0, message.Length);

                            this.connection_state = CS_CLIENT_FINISHED_SEND;
                            break;
                        default:
                            this.FailWithError(AlertLevel.fatal, AlertDescription.handshake_failure);
                            break;
                    }
                    break;
                case HandshakeType.server_key_exchange:
                {
                    switch (connection_state)
                    {
                        case CS_SERVER_HELLO_RECEIVED:
                        case CS_SERVER_CERTIFICATE_RECEIVED:
                        {
                            // NB: Original code used case label fall-through
                            if (connection_state == CS_SERVER_HELLO_RECEIVED)
                            {
                                // There was no server certificate message; check it's OK
                                this.keyExchange.SkipServerCertificate();
                                this.authentication = null;
                            }

                            this.keyExchange.ProcessServerKeyExchange(inStr);

                            AssertEmpty(inStr);
                            break;
                        }
                        default:
                            this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                            break;
                    }

                    this.connection_state = CS_SERVER_KEY_EXCHANGE_RECEIVED;
                    break;
                }
                case HandshakeType.certificate_request:
                    switch (connection_state)
                    {
                        case CS_SERVER_CERTIFICATE_RECEIVED:
                        case CS_SERVER_KEY_EXCHANGE_RECEIVED:
                        {
                            // NB: Original code used case label fall-through
                            if (connection_state == CS_SERVER_CERTIFICATE_RECEIVED)
                            {
                                // There was no server key exchange message; check it's OK
                                this.keyExchange.SkipServerKeyExchange();
                            }

                            if (this.authentication == null)
                            {
                                /*
                                 * RFC 2246 7.4.4. It is a fatal handshake_failure alert
                                 * for an anonymous server to request client identification.
                                 */
                                this.FailWithError(AlertLevel.fatal, AlertDescription.handshake_failure);
                            }

                            int numTypes = TlsUtilities.ReadUint8(inStr);
                            ClientCertificateType[] certificateTypes = new ClientCertificateType[numTypes];
                            for (int i = 0; i < numTypes; ++i)
                            {
                                certificateTypes[i] = (ClientCertificateType)TlsUtilities.ReadUint8(inStr);
                            }

                            byte[] authorities = TlsUtilities.ReadOpaque16(inStr);

                            AssertEmpty(inStr);

                            IList authorityDNs = Platform.CreateArrayList();

                            MemoryStream bis = new MemoryStream(authorities, false);
                            while (bis.Position < bis.Length)
                            {
                                byte[] dnBytes = TlsUtilities.ReadOpaque16(bis);
                                // TODO Switch to X500Name when available
                                authorityDNs.Add(X509Name.GetInstance(Asn1Object.FromByteArray(dnBytes)));
                            }

                            this.certificateRequest = new CertificateRequest(certificateTypes,
                                authorityDNs);
                            this.keyExchange.ValidateCertificateRequest(this.certificateRequest);

                            break;
                        }
                        default:
                            this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                            break;
                    }

                    this.connection_state = CS_CERTIFICATE_REQUEST_RECEIVED;
                    break;
                case HandshakeType.hello_request:
                    /*
                     * RFC 2246 7.4.1.1 Hello request
                     * This message will be ignored by the client if the client is currently
                     * negotiating a session. This message may be ignored by the client if it
                     * does not wish to renegotiate a session, or the client may, if it wishes,
                     * respond with a no_renegotiation alert.
                     */
                    if (connection_state == CS_DONE)
                    {
                        // Renegotiation not supported yet
                        SendAlert(AlertLevel.warning, AlertDescription.no_renegotiation);
                    }
                    break;
                case HandshakeType.client_key_exchange:
                case HandshakeType.certificate_verify:
                case HandshakeType.client_hello:
                default:
                    // We do not support this!
                    this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                    break;
            }
        }

        private void ProcessApplicationData()
        {
            /*
            * There is nothing we need to do here.
            *
            * This function could be used for callbacks when application
            * data arrives in the future.
            */
        }

        private void ProcessAlert()
        {
            while (alertQueue.Available >= 2)
            {
                /*
                * An alert is always 2 bytes. Read the alert.
                */
                byte[] tmp = alertQueue.RemoveData(2, 0);
                byte level = tmp[0];
                byte description = tmp[1];
                if (level == (byte)AlertLevel.fatal)
                {
                    this.failedWithError = true;
                    this.closed = true;
                    /*
                    * Now try to Close the stream, ignore errors.
                    */
                    try
                    {
                        rs.Close();
                    }
                    catch (Exception)
                    {
                    }
                    throw new IOException(TLS_ERROR_MESSAGE);
                }
                else
                {
                    if (description == (byte)AlertDescription.close_notify)
                    {
                        HandleClose(false);
                    }

                    /*
                    * If it is just a warning, we continue.
                    */
                }
            }
        }

        /**
        * This method is called, when a change cipher spec message is received.
        *
        * @throws IOException If the message has an invalid content or the
        *                     handshake is not in the correct state.
        */
        private void ProcessChangeCipherSpec(byte[] buf, int off, int len)
        {
            for (int i = 0; i < len; ++i)
            {
                if (buf[off + i] != 1)
                {
                    this.FailWithError(AlertLevel.fatal, AlertDescription.decode_error);
                }

                /*
                 * Check if we are in the correct connection state.
                 */
                if (this.connection_state != CS_CLIENT_FINISHED_SEND)
                {
                    this.FailWithError(AlertLevel.fatal, AlertDescription.unexpected_message);
                }

                rs.ServerClientSpecReceived();

                this.connection_state = CS_SERVER_CHANGE_CIPHER_SPEC_RECEIVED;
            }
        }

        private void SendClientCertificate(Certificate clientCert)
        {
            MemoryStream bos = new MemoryStream();
            TlsUtilities.WriteUint8((byte)HandshakeType.certificate, bos);

            // Reserve space for length
            TlsUtilities.WriteUint24(0, bos);

            clientCert.Encode(bos);
            byte[] message = bos.ToArray();

            // Patch actual length back in
            TlsUtilities.WriteUint24(message.Length - 4, message, 1);

            rs.WriteMessage(ContentType.handshake, message, 0, message.Length);
        }

        private void SendClientKeyExchange()
        {
            MemoryStream bos = new MemoryStream();
            TlsUtilities.WriteUint8((byte)HandshakeType.client_key_exchange, bos);

            // Reserve space for length
            TlsUtilities.WriteUint24(0, bos);

            this.keyExchange.GenerateClientKeyExchange(bos);
            byte[] message = bos.ToArray();

            // Patch actual length back in
            TlsUtilities.WriteUint24(message.Length - 4, message, 1);

            rs.WriteMessage(ContentType.handshake, message, 0, message.Length);
        }

        private void SendCertificateVerify(byte[] data)
        {
            /*
             * Send signature of handshake messages so far to prove we are the owner of
             * the cert See RFC 2246 sections 4.7, 7.4.3 and 7.4.8
             */
            MemoryStream bos = new MemoryStream();
            TlsUtilities.WriteUint8((byte)HandshakeType.certificate_verify, bos);
            TlsUtilities.WriteUint24(data.Length + 2, bos);
            TlsUtilities.WriteOpaque16(data, bos);
            byte[] message = bos.ToArray();

            rs.WriteMessage(ContentType.handshake, message, 0, message.Length);
        }

        /// <summary>Connects to the remote system.</summary>
        /// <param name="verifyer">Will be used when a certificate is received to verify
        /// that this certificate is accepted by the client.</param>
        /// <exception cref="IOException">If handshake was not successful</exception>
        [Obsolete("Use version taking TlsClient")]
        public virtual void Connect(
            ICertificateVerifyer verifyer)
        {
            this.Connect(new LegacyTlsClient(verifyer));
        }

        public virtual void Connect(TlsClient tlsClient)
        {
            if (tlsClient == null)
                throw new ArgumentNullException("tlsClient");
            if (this.tlsClient != null)
                throw new InvalidOperationException("Connect can only be called once");

            /*
             * Send Client hello
             *
             * First, generate some random data.
             */
            this.securityParameters = new SecurityParameters();
            this.securityParameters.clientRandom = new byte[32];
            random.NextBytes(securityParameters.clientRandom, 4, 28);
            TlsUtilities.WriteGmtUnixTime(securityParameters.clientRandom, 0);

            this.tlsClientContext = new TlsClientContextImpl(random, securityParameters);
            this.tlsClient = tlsClient;
            this.tlsClient.Init(tlsClientContext);

            MemoryStream outStr = new MemoryStream();
            TlsUtilities.WriteVersion(outStr);
            outStr.Write(securityParameters.clientRandom, 0, 32);

            /*
            * Length of Session id
            */
            TlsUtilities.WriteUint8(0, outStr);

            this.offeredCipherSuites = this.tlsClient.GetCipherSuites();

            // ExtensionType -> byte[]
            this.clientExtensions = this.tlsClient.GetClientExtensions();

            // Cipher Suites (and SCSV)
            {
                /*
                 * RFC 5746 3.4.
                 * The client MUST include either an empty "renegotiation_info"
                 * extension, or the TLS_EMPTY_RENEGOTIATION_INFO_SCSV signaling
                 * cipher suite value in the ClientHello.  Including both is NOT
                 * RECOMMENDED.
                 */
                bool noRenegExt = clientExtensions == null
                    || !clientExtensions.Contains(ExtensionType.renegotiation_info);

                int count = offeredCipherSuites.Length;
                if (noRenegExt)
                {
                    // Note: 1 extra slot for TLS_EMPTY_RENEGOTIATION_INFO_SCSV
                    ++count;
                }

                TlsUtilities.WriteUint16(2 * count, outStr);

                for (int i = 0; i < offeredCipherSuites.Length; ++i)
                {
                    TlsUtilities.WriteUint16((int)offeredCipherSuites[i], outStr);
                }

                if (noRenegExt)
                {
                    TlsUtilities.WriteUint16((int)CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV, outStr);
                }
            }

            /*
             * Compression methods, just the null method.
             */
            this.offeredCompressionMethods = tlsClient.GetCompressionMethods();

            {
                TlsUtilities.WriteUint8((byte)offeredCompressionMethods.Length, outStr);
                for (int i = 0; i < offeredCompressionMethods.Length; ++i)
                {
                    TlsUtilities.WriteUint8((byte)offeredCompressionMethods[i], outStr);
                }
            }

            // Extensions
            if (clientExtensions != null)
            {
                MemoryStream ext = new MemoryStream();

                foreach (ExtensionType extType in clientExtensions.Keys)
                {
                    WriteExtension(ext, extType, (byte[])clientExtensions[extType]);
                }

                TlsUtilities.WriteOpaque16(ext.ToArray(), outStr);
            }

            MemoryStream bos = new MemoryStream();
            TlsUtilities.WriteUint8((byte)HandshakeType.client_hello, bos);
            TlsUtilities.WriteUint24((int)outStr.Length, bos);
            byte[] outBytes = outStr.ToArray();
            bos.Write(outBytes, 0, outBytes.Length);
            byte[] message = bos.ToArray();
            SafeWriteMessage(ContentType.handshake, message, 0, message.Length);
            connection_state = CS_CLIENT_HELLO_SEND;

            /*
            * We will now read data, until we have completed the handshake.
            */
            while (connection_state != CS_DONE)
            {
                SafeReadData();
            }

            this.tlsStream = new TlsStream(this);
        }

        /**
        * Read data from the network. The method will return immediately, if there is
        * still some data left in the buffer, or block until some application
        * data has been read from the network.
        *
        * @param buf    The buffer where the data will be copied to.
        * @param offset The position where the data will be placed in the buffer.
        * @param len    The maximum number of bytes to read.
        * @return The number of bytes read.
        * @throws IOException If something goes wrong during reading data.
        */
        internal int ReadApplicationData(byte[] buf, int offset, int len)
        {
            while (applicationDataQueue.Available == 0)
            {
                if (this.closed)
                {
                    /*
                    * We need to read some data.
                    */
                    if (this.failedWithError)
                    {
                        /*
                        * Something went terribly wrong, we should throw an IOException
                        */
                        throw new IOException(TLS_ERROR_MESSAGE);
                    }

                    /*
                    * Connection has been closed, there is no more data to read.
                    */
                    return 0;
                }

                SafeReadData();
            }
            len = System.Math.Min(len, applicationDataQueue.Available);
            applicationDataQueue.RemoveData(buf, offset, len, 0);
            return len;
        }

        private void SafeReadData()
        {
            try
            {
                rs.ReadData();
            }
            catch (TlsFatalAlert e)
            {
                if (!this.closed)
                {
                    this.FailWithError(AlertLevel.fatal, e.AlertDescription);
                }
                throw e;
            }
            catch (IOException e)
            {
                if (!this.closed)
                {
                    this.FailWithError(AlertLevel.fatal, AlertDescription.internal_error);
                }
                throw e;
            }
            catch (Exception e)
            {
                if (!this.closed)
                {
                    this.FailWithError(AlertLevel.fatal, AlertDescription.internal_error);
                }
                throw e;
            }
        }

        private void SafeWriteMessage(ContentType type, byte[] buf, int offset, int len)
        {
            try
            {
                rs.WriteMessage(type, buf, offset, len);
            }
            catch (TlsFatalAlert e)
            {
                if (!this.closed)
                {
                    this.FailWithError(AlertLevel.fatal, e.AlertDescription);
                }
                throw e;
            }
            catch (IOException e)
            {
                if (!closed)
                {
                    this.FailWithError(AlertLevel.fatal, AlertDescription.internal_error);
                }
                throw e;
            }
            catch (Exception e)
            {
                if (!closed)
                {
                    this.FailWithError(AlertLevel.fatal, AlertDescription.internal_error);
                }
                throw e;
            }
        }

        /**
        * Send some application data to the remote system.
        * <p/>
        * The method will handle fragmentation internally.
        *
        * @param buf    The buffer with the data.
        * @param offset The position in the buffer where the data is placed.
        * @param len    The length of the data.
        * @throws IOException If something goes wrong during sending.
        */
        internal void WriteData(byte[] buf, int offset, int len)
        {
            if (this.closed)
            {
                if (this.failedWithError)
                    throw new IOException(TLS_ERROR_MESSAGE);

                throw new IOException("Sorry, connection has been closed, you cannot write more data");
            }

            while (len > 0)
            {
                /*
                 * Protect against known IV attack!
                 *
                 * DO NOT REMOVE THIS LINE, EXCEPT YOU KNOW EXACTLY WHAT
                 * YOU ARE DOING HERE.
                 */
                SafeWriteMessage(ContentType.application_data, emptybuf, 0, 0);

                /*
                * We are only allowed to write fragments up to 2^14 bytes.
                */
                int toWrite = System.Math.Min(len, 1 << 14);

                SafeWriteMessage(ContentType.application_data, buf, offset, toWrite);

                offset += toWrite;
                len -= toWrite;
            }
        }

        /// <summary>A Stream which can be used to send data.</summary>
        [Obsolete("Use 'Stream' property instead")]
        public virtual Stream OutputStream
        {
            get { return this.tlsStream; }
        }

        /// <summary>A Stream which can be used to read data.</summary>
        [Obsolete("Use 'Stream' property instead")]
        public virtual Stream InputStream
        {
            get { return this.tlsStream; }
        }

        /// <summary>The secure bidirectional stream for this connection</summary>
        public virtual Stream Stream
        {
            get { return this.tlsStream; }
        }

        /**
        * Terminate this connection with an alert.
        * <p/>
        * Can be used for normal closure too.
        *
        * @param alertLevel       The level of the alert, an be AlertLevel.fatal or AL_warning.
        * @param alertDescription The exact alert message.
        * @throws IOException If alert was fatal.
        */
        private void FailWithError(AlertLevel alertLevel, AlertDescription	alertDescription)
        {
            /*
            * Check if the connection is still open.
            */
            if (!closed)
            {
                /*
                * Prepare the message
                */
                this.closed = true;

                if (alertLevel == AlertLevel.fatal)
                {
                    /*
                    * This is a fatal message.
                    */
                    this.failedWithError = true;
                }
                SendAlert(alertLevel, alertDescription);
                rs.Close();
                if (alertLevel == AlertLevel.fatal)
                {
                    throw new IOException(TLS_ERROR_MESSAGE);
                }
            }
            else
            {
                throw new IOException(TLS_ERROR_MESSAGE);
            }
        }

        internal void SendAlert(AlertLevel alertLevel, AlertDescription alertDescription)
        {
            byte[] error = new byte[2];
            error[0] = (byte)alertLevel;
            error[1] = (byte)alertDescription;

            rs.WriteMessage(ContentType.alert, error, 0, 2);
        }

        /// <summary>Closes this connection</summary>
        /// <exception cref="IOException">If something goes wrong during closing.</exception>
        public virtual void Close()
        {
            HandleClose(true);
        }

        protected virtual void HandleClose(bool user_canceled)
        {
            if (!closed)
            {
                if (user_canceled && !appDataReady)
                {
                    SendAlert(AlertLevel.warning, AlertDescription.user_canceled);
                }
                this.FailWithError(AlertLevel.warning, AlertDescription.close_notify);
            }
        }

        /**
        * Make sure the Stream is now empty. Fail otherwise.
        *
        * @param is The Stream to check.
        * @throws IOException If is is not empty.
        */
        internal void AssertEmpty(
            MemoryStream inStr)
        {
            if (inStr.Position < inStr.Length)
            {
                throw new TlsFatalAlert(AlertDescription.decode_error);
            }
        }

        internal void Flush()
        {
            rs.Flush();
        }

        internal bool IsClosed
        {
            get { return closed; }
        }

        private static bool ArrayContains(CipherSuite[] a, CipherSuite n)
        {
            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] == n)
                    return true;
            }
            return false;
        }

        private static bool ArrayContains(CompressionMethod[] a, CompressionMethod n)
        {
            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] == n)
                    return true;
            }
            return false;
        }

        private static byte[] CreateRenegotiationInfo(byte[] renegotiated_connection)
        {
            MemoryStream buf = new MemoryStream();
            TlsUtilities.WriteOpaque8(renegotiated_connection, buf);
            return buf.ToArray();
        }

        private static void WriteExtension(Stream output, ExtensionType extType, byte[] extValue)
        {
            TlsUtilities.WriteUint16((int)extType, output);
            TlsUtilities.WriteOpaque16(extValue, output);
        }
    }
}
