using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
    * ECDH key exchange (see RFC 4492)
    */
    internal class TlsECDHKeyExchange
        : TlsKeyExchange
    {
        protected TlsClientContext context;
        protected KeyExchangeAlgorithm keyExchange;
        protected TlsSigner tlsSigner;

        protected AsymmetricKeyParameter serverPublicKey;
        protected ECPublicKeyParameters ecAgreeServerPublicKey;
        protected TlsAgreementCredentials agreementCredentials;
        protected ECPrivateKeyParameters ecAgreeClientPrivateKey = null;

        internal TlsECDHKeyExchange(TlsClientContext context, KeyExchangeAlgorithm keyExchange)
        {
            switch (keyExchange)
            {
                case KeyExchangeAlgorithm.ECDHE_RSA:
                    this.tlsSigner = new TlsRsaSigner();
                    break;
                case KeyExchangeAlgorithm.ECDHE_ECDSA:
                    this.tlsSigner = new TlsECDsaSigner();
                    break;
                case KeyExchangeAlgorithm.ECDH_RSA:
                case KeyExchangeAlgorithm.ECDH_ECDSA:
                    this.tlsSigner = null;
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
                    this.ecAgreeServerPublicKey = ValidateECPublicKey((ECPublicKeyParameters)this.serverPublicKey);
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
            // do nothing
        }

        public virtual void ProcessServerKeyExchange(Stream input)
        {
            throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public virtual void ValidateCertificateRequest(CertificateRequest certificateRequest)
        {
            /*
             * RFC 4492 3. [...] The ECDSA_fixed_ECDH and RSA_fixed_ECDH mechanisms are usable
             * with ECDH_ECDSA and ECDH_RSA. Their use with ECDHE_ECDSA and ECDHE_RSA is
             * prohibited because the use of a long-term ECDH client key would jeopardize the
             * forward secrecy property of these algorithms.
             */
            ClientCertificateType[] types = certificateRequest.CertificateTypes;
            foreach (ClientCertificateType type in types)
            {
                switch (type)
                {
                    case ClientCertificateType.rsa_sign:
                    case ClientCertificateType.dss_sign:
                    case ClientCertificateType.ecdsa_sign:
                    case ClientCertificateType.rsa_fixed_ecdh:
                    case ClientCertificateType.ecdsa_fixed_ecdh:
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
                // TODO Validate client cert has matching parameters (see 'AreOnSameCurve')?

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
            if (agreementCredentials == null)
            {
                GenerateEphemeralClientKeyExchange(ecAgreeServerPublicKey.Parameters, output);
            }
        }

        public virtual byte[] GeneratePremasterSecret()
        {
            if (agreementCredentials != null)
            {
                return agreementCredentials.GenerateAgreement(ecAgreeServerPublicKey);
            }

            return CalculateECDHBasicAgreement(ecAgreeServerPublicKey, ecAgreeClientPrivateKey);
        }

        protected virtual bool AreOnSameCurve(ECDomainParameters a, ECDomainParameters b)
        {
            // TODO Move to ECDomainParameters.Equals() or other utility method?
            return a.Curve.Equals(b.Curve) && a.G.Equals(b.G) && a.N.Equals(b.N) && a.H.Equals(b.H);
        }

        protected virtual byte[] ExternalizeKey(ECPublicKeyParameters keyParameters)
        {
            // TODO Add support for compressed encoding and SPF extension

            /*
             * RFC 4492 5.7. ...an elliptic curve point in uncompressed or compressed format.
             * Here, the format MUST conform to what the server has requested through a
             * Supported Point Formats Extension if this extension was used, and MUST be
             * uncompressed if this extension was not used.
             */
            return keyParameters.Q.GetEncoded();
        }

        protected virtual AsymmetricCipherKeyPair GenerateECKeyPair(ECDomainParameters ecParams)
        {
            ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();
            ECKeyGenerationParameters keyGenerationParameters = new ECKeyGenerationParameters(ecParams,
                context.SecureRandom);
            keyPairGenerator.Init(keyGenerationParameters);
            return keyPairGenerator.GenerateKeyPair();
        }

        protected virtual void GenerateEphemeralClientKeyExchange(ECDomainParameters ecParams, Stream output)
        {
            AsymmetricCipherKeyPair ecAgreeClientKeyPair = GenerateECKeyPair(ecParams);
            this.ecAgreeClientPrivateKey = (ECPrivateKeyParameters)ecAgreeClientKeyPair.Private;

            byte[] keData = ExternalizeKey((ECPublicKeyParameters)ecAgreeClientKeyPair.Public);
            TlsUtilities.WriteOpaque8(keData, output);
        }

        protected virtual byte[] CalculateECDHBasicAgreement(ECPublicKeyParameters publicKey,
            ECPrivateKeyParameters privateKey)
        {
            ECDHBasicAgreement basicAgreement = new ECDHBasicAgreement();
            basicAgreement.Init(privateKey);
            BigInteger agreementValue = basicAgreement.CalculateAgreement(publicKey);

            /*
             * RFC 4492 5.10. Note that this octet string (Z in IEEE 1363 terminology) as output by
             * FE2OSP, the Field Element to Octet String Conversion Primitive, has constant length for
             * any given field; leading zeros found in this octet string MUST NOT be truncated.
             */
            return BigIntegers.AsUnsignedByteArray(basicAgreement.GetFieldSize(), agreementValue);
        }

        protected virtual ECPublicKeyParameters ValidateECPublicKey(ECPublicKeyParameters key)
        {
            // TODO Check RFC 4492 for validation
            return key;
        }
    }
}
