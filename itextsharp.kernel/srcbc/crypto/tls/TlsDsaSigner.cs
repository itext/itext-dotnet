using System;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal abstract class TlsDsaSigner
        :	TlsSigner
    {
        public virtual byte[] GenerateRawSignature(SecureRandom random,
            AsymmetricKeyParameter privateKey, byte[] md5andsha1)
        {
            ISigner s = MakeSigner(new NullDigest(), true, new ParametersWithRandom(privateKey, random));
            // Note: Only use the SHA1 part of the hash
            s.BlockUpdate(md5andsha1, 16, 20);
            return s.GenerateSignature();
        }

        public bool VerifyRawSignature(byte[] sigBytes, AsymmetricKeyParameter publicKey, byte[] md5andsha1)
        {
            ISigner s = MakeSigner(new NullDigest(), false, publicKey);
            // Note: Only use the SHA1 part of the hash
            s.BlockUpdate(md5andsha1, 16, 20);
            return s.VerifySignature(sigBytes);
        }

        public virtual ISigner CreateSigner(SecureRandom random, AsymmetricKeyParameter privateKey)
        {
            return MakeSigner(new Sha1Digest(), true, new ParametersWithRandom(privateKey, random));
        }

        public virtual ISigner CreateVerifyer(AsymmetricKeyParameter publicKey)
        {
            return MakeSigner(new Sha1Digest(), false, publicKey);
        }

        public abstract bool IsValidPublicKey(AsymmetricKeyParameter publicKey);

        protected virtual ISigner MakeSigner(IDigest d, bool forSigning, ICipherParameters cp)
        {
            ISigner s = new DsaDigestSigner(CreateDsaImpl(), d);
            s.Init(forSigning, cp);
            return s;
        }

        protected abstract IDsa CreateDsaImpl();
    }
}
