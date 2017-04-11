using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestTsaClient : ITSAClient {
        private const String DIGEST_ALG = "SHA256";

        private readonly ICipherParameters tsaPrivateKey;

        private IList<X509Certificate> tsaCertificateChain;

        public TestTsaClient(IList<X509Certificate> tsaCertificateChain, ICipherParameters tsaPrivateKey) {
            this.tsaCertificateChain = tsaCertificateChain;
            this.tsaPrivateKey = tsaPrivateKey;
        }

        public virtual int GetTokenSizeEstimate() {
            return 4096;
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        public virtual IDigest GetMessageDigest() {
            return SignTestPortUtil.GetMessageDigest(DIGEST_ALG);
        }

        /// <exception cref="System.Exception"/>
        public virtual byte[] GetTimeStampToken(byte[] imprint) {
            TimeStampRequestGenerator tsqGenerator = new TimeStampRequestGenerator();
            tsqGenerator.SetCertReq(true);
            BigInteger nonce = BigInteger.ValueOf(SystemUtil.GetSystemTimeTicks());
            TimeStampRequest request = tsqGenerator.Generate(new DerObjectIdentifier(DigestAlgorithms.GetAllowedDigest
                (DIGEST_ALG)), imprint, nonce);
            return new TestTimestampTokenBuilder(tsaCertificateChain, tsaPrivateKey).CreateTimeStampToken(request);
        }
    }
}
