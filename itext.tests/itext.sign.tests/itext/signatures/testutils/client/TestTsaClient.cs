/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestTsaClient : ITSAClient {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private const String SIGN_ALG = "SHA256withRSA";

        private const String DIGEST_ALG = "SHA256";

        private readonly IPrivateKey tsaPrivateKey;

        private readonly IList<IX509Certificate> tsaCertificateChain;

        private int estimatedSize = 4096;

        private String signatureAlgo = SIGN_ALG;

        private String digestAlgo = DIGEST_ALG;

        public TestTsaClient(IList<IX509Certificate> tsaCertificateChain, IPrivateKey tsaPrivateKey) {
            this.tsaCertificateChain = tsaCertificateChain;
            this.tsaPrivateKey = tsaPrivateKey;
        }

        public TestTsaClient(IList<IX509Certificate> tsaCertificateChain, IPrivateKey tsaPrivateKey, int estimatedSize
            , String signatureAlgo, String digestAlgo) {
            this.tsaCertificateChain = tsaCertificateChain;
            this.tsaPrivateKey = tsaPrivateKey;
            this.estimatedSize = estimatedSize;
            this.signatureAlgo = signatureAlgo;
            this.digestAlgo = digestAlgo;
        }

        public virtual int GetTokenSizeEstimate() {
            return estimatedSize;
        }

        public virtual IMessageDigest GetMessageDigest() {
            return SignTestPortUtil.GetMessageDigest(digestAlgo);
        }

        public virtual byte[] GetTimeStampToken(byte[] imprint) {
            ITimeStampRequestGenerator tsqGenerator = BOUNCY_CASTLE_FACTORY.CreateTimeStampRequestGenerator();
            tsqGenerator.SetCertReq(true);
            IBigInteger nonce = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateBigInteger().ValueOf
                (SystemUtil.GetTimeBasedSeed());
            ITimeStampRequest request = tsqGenerator.Generate(BOUNCY_CASTLE_FACTORY.CreateASN1ObjectIdentifier(DigestAlgorithms
                .GetAllowedDigest(digestAlgo)), imprint, nonce);
            if (SIGN_ALG.Equals(signatureAlgo)) {
                return new TestTimestampTokenBuilder(tsaCertificateChain, tsaPrivateKey).CreateTimeStampToken(request);
            }
            return new TestTimestampTokenBuilder(tsaCertificateChain, tsaPrivateKey, signatureAlgo, digestAlgo).CreateTimeStampToken
                (request);
        }
    }
}
