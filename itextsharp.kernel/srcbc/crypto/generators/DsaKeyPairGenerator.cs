using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Generators
{
    /**
     * a DSA key pair generator.
     *
     * This Generates DSA keys in line with the method described
	 * in <i>FIPS 186-3 B.1 FFC Key Pair Generation</i>.
     */
    public class DsaKeyPairGenerator
		: IAsymmetricCipherKeyPairGenerator
    {
        private DsaKeyGenerationParameters param;

		public void Init(
			KeyGenerationParameters parameters)
        {
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			// Note: If we start accepting instances of KeyGenerationParameters,
			// must apply constraint checking on strength (see DsaParametersGenerator.Init)

			this.param = (DsaKeyGenerationParameters) parameters;
        }

		public AsymmetricCipherKeyPair GenerateKeyPair()
        {
			DsaParameters dsaParams = param.Parameters;

			BigInteger x = GeneratePrivateKey(dsaParams.Q, param.Random);
			BigInteger y = CalculatePublicKey(dsaParams.P, dsaParams.G, x);

			return new AsymmetricCipherKeyPair(
				new DsaPublicKeyParameters(y, dsaParams),
				new DsaPrivateKeyParameters(x, dsaParams));
        }

		private static BigInteger GeneratePrivateKey(BigInteger q, SecureRandom random)
		{
			// TODO Prefer this method? (change test cases that used fixed random)
			// B.1.1 Key Pair Generation Using Extra Random Bits
//	        BigInteger c = new BigInteger(q.BitLength + 64, random);
//	        return c.Mod(q.Subtract(BigInteger.One)).Add(BigInteger.One);

			// B.1.2 Key Pair Generation by Testing Candidates
			return BigIntegers.CreateRandomInRange(BigInteger.One, q.Subtract(BigInteger.One), random);
		}

		private static BigInteger CalculatePublicKey(BigInteger p, BigInteger g, BigInteger x)
		{
			return g.ModPow(x, p);
		}
	}
}
