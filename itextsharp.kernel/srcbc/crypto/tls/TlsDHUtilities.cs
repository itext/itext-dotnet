using System;
using System.IO;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class TlsDHUtilities
    {
        public static byte[] CalculateDHBasicAgreement(DHPublicKeyParameters publicKey,
            DHPrivateKeyParameters privateKey)
        {
            DHBasicAgreement basicAgreement = new DHBasicAgreement();
            basicAgreement.Init(privateKey);
            BigInteger agreementValue = basicAgreement.CalculateAgreement(publicKey);

            /*
             * RFC 5246 8.1.2. Leading bytes of Z that contain all zero bits are stripped before it is
             * used as the pre_master_secret.
             */
            return BigIntegers.AsUnsignedByteArray(agreementValue);
        }

        public static AsymmetricCipherKeyPair GenerateDHKeyPair(SecureRandom random, DHParameters dhParams)
        {
            DHBasicKeyPairGenerator dhGen = new DHBasicKeyPairGenerator();
            dhGen.Init(new DHKeyGenerationParameters(random, dhParams));
            return dhGen.GenerateKeyPair();
        }

        public static DHPrivateKeyParameters GenerateEphemeralClientKeyExchange(SecureRandom random,
            DHParameters dhParams, Stream output)
        {
            AsymmetricCipherKeyPair dhAgreeClientKeyPair = GenerateDHKeyPair(random, dhParams);
            DHPrivateKeyParameters dhAgreeClientPrivateKey =
                (DHPrivateKeyParameters)dhAgreeClientKeyPair.Private;

            BigInteger Yc = ((DHPublicKeyParameters)dhAgreeClientKeyPair.Public).Y;
            byte[] keData = BigIntegers.AsUnsignedByteArray(Yc);
            TlsUtilities.WriteOpaque16(keData, output);

            return dhAgreeClientPrivateKey;
        }
        
        public static DHPublicKeyParameters ValidateDHPublicKey(DHPublicKeyParameters key)
        {
            BigInteger Y = key.Y;
            DHParameters parameters = key.Parameters;
            BigInteger p = parameters.P;
            BigInteger g = parameters.G;

            if (!p.IsProbablePrime(2))
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
            if (g.CompareTo(BigInteger.Two) < 0 || g.CompareTo(p.Subtract(BigInteger.Two)) > 0)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
            if (Y.CompareTo(BigInteger.Two) < 0 || Y.CompareTo(p.Subtract(BigInteger.One)) > 0)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            // TODO See RFC 2631 for more discussion of Diffie-Hellman validation

            return key;
        }
    }
}