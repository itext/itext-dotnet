using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Crypto.Agreement
{
    public class ECMqvBasicAgreement
        : IBasicAgreement
    {
        protected internal MqvPrivateParameters privParams;

        public virtual void Init(
            ICipherParameters parameters)
        {
            if (parameters is ParametersWithRandom)
            {
                parameters = ((ParametersWithRandom)parameters).Parameters;
            }

            this.privParams = (MqvPrivateParameters)parameters;
        }

        public virtual int GetFieldSize()
        {
            return (privParams.StaticPrivateKey.Parameters.Curve.FieldSize + 7) / 8;
        }

        public virtual BigInteger CalculateAgreement(
            ICipherParameters pubKey)
        {
            MqvPublicParameters pubParams = (MqvPublicParameters)pubKey;

            ECPrivateKeyParameters staticPrivateKey = privParams.StaticPrivateKey;

            ECPoint agreement = calculateMqvAgreement(staticPrivateKey.Parameters, staticPrivateKey,
                privParams.EphemeralPrivateKey, privParams.EphemeralPublicKey,
                pubParams.StaticPublicKey, pubParams.EphemeralPublicKey);

            return agreement.X.ToBigInteger();
        }
        
        // The ECMQV Primitive as described in SEC-1, 3.4
        private static ECPoint calculateMqvAgreement(
            ECDomainParameters		parameters,
            ECPrivateKeyParameters	d1U,
            ECPrivateKeyParameters	d2U,
            ECPublicKeyParameters	Q2U,
            ECPublicKeyParameters	Q1V,
            ECPublicKeyParameters	Q2V)
        {
            BigInteger n = parameters.N;
            int e = (n.BitLength + 1) / 2;
            BigInteger powE = BigInteger.One.ShiftLeft(e);

            // The Q2U public key is optional
            ECPoint q;
            if (Q2U == null)
            {
                q = parameters.G.Multiply(d2U.D);
            }
            else
            {
                q = Q2U.Q;
            }

            BigInteger x = q.X.ToBigInteger();
            BigInteger xBar = x.Mod(powE);
            BigInteger Q2UBar = xBar.SetBit(e);
            BigInteger s = d1U.D.Multiply(Q2UBar).Mod(n).Add(d2U.D).Mod(n);

            BigInteger xPrime = Q2V.Q.X.ToBigInteger();
            BigInteger xPrimeBar = xPrime.Mod(powE);
            BigInteger Q2VBar = xPrimeBar.SetBit(e);

            BigInteger hs = parameters.H.Multiply(s).Mod(n);

            //ECPoint p = Q1V.Q.Multiply(Q2VBar).Add(Q2V.Q).Multiply(hs);
            ECPoint p = ECAlgorithms.SumOfTwoMultiplies(
                Q1V.Q, Q2VBar.Multiply(hs).Mod(n), Q2V.Q, hs);

            if (p.IsInfinity)
                throw new InvalidOperationException("Infinity is not a valid agreement value for MQV");

            return p;
        }
    }
}
