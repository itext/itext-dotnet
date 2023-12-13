/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

        public virtual IDigest GetMessageDigest() {
            return SignTestPortUtil.GetMessageDigest(DIGEST_ALG);
        }

        public virtual byte[] GetTimeStampToken(byte[] imprint) {
            TimeStampRequestGenerator tsqGenerator = new TimeStampRequestGenerator();
            tsqGenerator.SetCertReq(true);
            BigInteger nonce = BigInteger.ValueOf(SystemUtil.GetTimeBasedSeed());
            TimeStampRequest request = tsqGenerator.Generate(new DerObjectIdentifier(DigestAlgorithms.GetAllowedDigest
                (DIGEST_ALG)), imprint, nonce);
            return new TestTimestampTokenBuilder(tsaCertificateChain, tsaPrivateKey).CreateTimeStampToken(request);
        }
    }
}
