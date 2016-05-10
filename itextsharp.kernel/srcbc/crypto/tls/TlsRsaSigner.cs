using System;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal class TlsRsaSigner
        : TlsSigner
    {
        public virtual byte[] GenerateRawSignature(SecureRandom random,
            AsymmetricKeyParameter privateKey, byte[] md5AndSha1)
        {
            IAsymmetricBlockCipher engine = CreateRsaImpl();
            engine.Init(true, new ParametersWithRandom(privateKey, random));
            return engine.ProcessBlock(md5AndSha1, 0, md5AndSha1.Length);
        }

        public virtual bool VerifyRawSignature(byte[] sigBytes, AsymmetricKeyParameter publicKey,
            byte[] md5AndSha1)
        {
            IAsymmetricBlockCipher engine = CreateRsaImpl();
            engine.Init(false, publicKey);
            byte[] signed = engine.ProcessBlock(sigBytes, 0, sigBytes.Length);
            return Arrays.ConstantTimeAreEqual(signed, md5AndSha1);
        }

        public virtual ISigner CreateSigner(SecureRandom random, AsymmetricKeyParameter privateKey)
        {
            return MakeSigner(new CombinedHash(), true, new ParametersWithRandom(privateKey, random));
        }

        public virtual ISigner CreateVerifyer(AsymmetricKeyParameter publicKey)
        {
            return MakeSigner(new CombinedHash(), false, publicKey);
        }

        public virtual bool IsValidPublicKey(AsymmetricKeyParameter publicKey)
        {
            return publicKey is RsaKeyParameters && !publicKey.IsPrivate;
        }

        protected virtual ISigner MakeSigner(IDigest d, bool forSigning, ICipherParameters cp)
        {
            ISigner s = new GenericSigner(CreateRsaImpl(), d);
            s.Init(forSigning, cp);
            return s;
        }

        protected virtual IAsymmetricBlockCipher CreateRsaImpl()
        {
            return new Pkcs1Encoding(new RsaBlindedEngine());
        }
    }
}
