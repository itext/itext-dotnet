using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
    * ECDHE key exchange (see RFC 4492)
    */
    internal class TlsECDheKeyExchange : TlsECDHKeyExchange
    {
        internal TlsECDheKeyExchange(TlsClientContext context, KeyExchangeAlgorithm keyExchange)
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

            ECCurveType curveType = (ECCurveType)TlsUtilities.ReadUint8(sigIn);
            ECDomainParameters curve_params;

            //  Currently, we only support named curves
            if (curveType == ECCurveType.named_curve)
            {
                NamedCurve namedCurve = (NamedCurve)TlsUtilities.ReadUint16(sigIn);

                // TODO Check namedCurve is one we offered?

                curve_params = NamedCurveHelper.GetECParameters(namedCurve);
            }
            else
            {
                // TODO Add support for explicit curve parameters (read from sigIn)

                throw new TlsFatalAlert(AlertDescription.handshake_failure);
            }

            byte[] publicBytes = TlsUtilities.ReadOpaque8(sigIn);

            byte[] sigByte = TlsUtilities.ReadOpaque16(input);
            if (!signer.VerifySignature(sigByte))
            {
                throw new TlsFatalAlert(AlertDescription.decrypt_error);
            }

            // TODO Check curve_params not null

            ECPoint Q = curve_params.Curve.DecodePoint(publicBytes);

            this.ecAgreeServerPublicKey = ValidateECPublicKey(new ECPublicKeyParameters(Q, curve_params));
        }
        
        public override void ValidateCertificateRequest(CertificateRequest certificateRequest)
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
                        break;
                    default:
                        throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }
            }
        }
        
        public override void ProcessClientCredentials(TlsCredentials clientCredentials)
        {
            if (clientCredentials is TlsSignerCredentials)
            {
                // OK
            }
            else
            {
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
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
