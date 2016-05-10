using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DefaultTlsSignerCredentials
        : TlsSignerCredentials
    {
        protected TlsClientContext context;
        protected Certificate clientCert;
        protected AsymmetricKeyParameter clientPrivateKey;

        protected TlsSigner clientSigner;

        public DefaultTlsSignerCredentials(TlsClientContext context,
            Certificate clientCertificate, AsymmetricKeyParameter clientPrivateKey)
        {
            if (clientCertificate == null)
            {
                throw new ArgumentNullException("clientCertificate");
            }
            if (clientCertificate.certs.Length == 0)
            {
                throw new ArgumentException("cannot be empty", "clientCertificate");
            }
            if (clientPrivateKey == null)
            {
                throw new ArgumentNullException("clientPrivateKey");
            }
            if (!clientPrivateKey.IsPrivate)
            {
                throw new ArgumentException("must be private", "clientPrivateKey");
            }

            if (clientPrivateKey is RsaKeyParameters)
            {
                clientSigner = new TlsRsaSigner();
            }
            else if (clientPrivateKey is DsaPrivateKeyParameters)
            {
                clientSigner = new TlsDssSigner();
            }
            else if (clientPrivateKey is ECPrivateKeyParameters)
            {
                clientSigner = new TlsECDsaSigner();
            }
            else
            {
                throw new ArgumentException("type not supported: "
                    + clientPrivateKey.GetType().FullName, "clientPrivateKey");
            }

            this.context = context;
            this.clientCert = clientCertificate;
            this.clientPrivateKey = clientPrivateKey;
        }

        public virtual Certificate Certificate
        {
            get { return clientCert; }
        }

        public virtual byte[] GenerateCertificateSignature(byte[] md5andsha1)
        {
            try
            {
                return clientSigner.GenerateRawSignature(context.SecureRandom, clientPrivateKey, md5andsha1);
            }
            catch (CryptoException)
            {
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }
    }
}
