/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class TSAClientBouncyCastleTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        [NUnit.Framework.Test]
        public virtual void SetTSAInfoTest() {
            TSAClientBouncyCastle clientBouncyCastle = new TSAClientBouncyCastle("url");
            TSAClientBouncyCastleTest.CustomItsaInfoBouncyCastle infoBouncyCastle = new TSAClientBouncyCastleTest.CustomItsaInfoBouncyCastle
                ();
            clientBouncyCastle.SetTSAInfo(infoBouncyCastle);
            NUnit.Framework.Assert.AreEqual(infoBouncyCastle, clientBouncyCastle.tsaInfo);
        }

        [NUnit.Framework.Test]
        public virtual void TestTsaClientBouncyCastleConstructor3Args() {
            String userName = "user";
            String password = "password";
            String url = "url";
            TSAClientBouncyCastle tsaClientBouncyCastle = new TSAClientBouncyCastle(url, userName, password);
            NUnit.Framework.Assert.AreEqual(url, tsaClientBouncyCastle.tsaURL);
            NUnit.Framework.Assert.AreEqual(userName, tsaClientBouncyCastle.tsaUsername);
            NUnit.Framework.Assert.AreEqual(password, tsaClientBouncyCastle.tsaPassword);
            NUnit.Framework.Assert.AreEqual(TSAClientBouncyCastle.DEFAULTTOKENSIZE, tsaClientBouncyCastle.GetTokenSizeEstimate
                ());
            NUnit.Framework.Assert.AreEqual(TSAClientBouncyCastle.DEFAULTHASHALGORITHM, tsaClientBouncyCastle.digestAlgorithm
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestTsaClientBouncyCastleConstructorAllArgs() {
            String userName = "user";
            String password = "password";
            String url = "url";
            int tokenSize = 1024;
            String digestAlgorithm = "SHA-1";
            TSAClientBouncyCastle tsaClientBouncyCastle = new TSAClientBouncyCastle(url, userName, password, tokenSize
                , digestAlgorithm);
            NUnit.Framework.Assert.AreEqual(url, tsaClientBouncyCastle.tsaURL);
            NUnit.Framework.Assert.AreEqual(userName, tsaClientBouncyCastle.tsaUsername);
            NUnit.Framework.Assert.AreEqual(password, tsaClientBouncyCastle.tsaPassword);
            NUnit.Framework.Assert.AreEqual(TSAClientBouncyCastle.DEFAULTTOKENSIZE, tsaClientBouncyCastle.tokenSizeEstimate
                );
            NUnit.Framework.Assert.AreEqual(tokenSize, tsaClientBouncyCastle.GetTokenSizeEstimate());
            NUnit.Framework.Assert.AreEqual(digestAlgorithm, tsaClientBouncyCastle.digestAlgorithm);
        }

        [NUnit.Framework.Test]
        public virtual void TestTsaClientBouncyCastleConstructor1Arg() {
            String url = "url";
            TSAClientBouncyCastle tsaClientBouncyCastle = new TSAClientBouncyCastle(url);
            NUnit.Framework.Assert.AreEqual(url, tsaClientBouncyCastle.tsaURL);
            NUnit.Framework.Assert.IsNull(tsaClientBouncyCastle.tsaUsername);
            NUnit.Framework.Assert.IsNull(tsaClientBouncyCastle.tsaPassword);
            NUnit.Framework.Assert.AreEqual(TSAClientBouncyCastle.DEFAULTTOKENSIZE, tsaClientBouncyCastle.tokenSizeEstimate
                );
            NUnit.Framework.Assert.AreEqual(TSAClientBouncyCastle.DEFAULTHASHALGORITHM, tsaClientBouncyCastle.digestAlgorithm
                );
        }

        [NUnit.Framework.Test]
        public virtual void GetTokenSizeEstimateTest() {
            String userName = "user";
            String password = "password";
            String url = "url";
            String digestAlgorithm = "SHA-256";
            int tokenSizeEstimate = 4096;
            TSAClientBouncyCastle tsaClientBouncyCastle = new TSAClientBouncyCastle(url, userName, password, tokenSizeEstimate
                , digestAlgorithm);
            NUnit.Framework.Assert.AreEqual(tokenSizeEstimate, tsaClientBouncyCastle.GetTokenSizeEstimate());
        }

        [NUnit.Framework.Test]
        public virtual void SetGetTsaReqPolicyTest() {
            String regPolicy = "regPolicy";
            TSAClientBouncyCastle clientBouncyCastle = new TSAClientBouncyCastle("url");
            clientBouncyCastle.SetTSAReqPolicy(regPolicy);
            NUnit.Framework.Assert.AreEqual(regPolicy, clientBouncyCastle.GetTSAReqPolicy());
        }

        [NUnit.Framework.Test]
        public virtual void GetMessageDigestTest() {
            String userName = "user";
            String password = "password";
            String url = "url";
            String digestAlgorithm = "SHA-256";
            int tokenSizeEstimate = 4096;
            TSAClientBouncyCastle tsaClientBouncyCastle = new TSAClientBouncyCastle(url, userName, password, tokenSizeEstimate
                , digestAlgorithm);
            IMessageDigest digest = tsaClientBouncyCastle.GetMessageDigest();
            NUnit.Framework.Assert.IsNotNull(digest);
            NUnit.Framework.Assert.AreEqual(digestAlgorithm, digest.GetAlgorithmName());
        }

        [NUnit.Framework.Test]
        public virtual void GetTimeStampTokenTest() {
            String allowedDigest = "SHA256";
            String signatureAlgorithm = "SHA256withRSA";
            String policyOid = "1.3.6.1.4.1.45794.1.1";
            TSAClientBouncyCastleTest.CustomTsaClientBouncyCastle tsaClientBouncyCastle = new TSAClientBouncyCastleTest.CustomTsaClientBouncyCastle
                ("", signatureAlgorithm, allowedDigest);
            tsaClientBouncyCastle.SetTSAReqPolicy(policyOid);
            TSAClientBouncyCastleTest.CustomItsaInfoBouncyCastle itsaInfoBouncyCastle = new TSAClientBouncyCastleTest.CustomItsaInfoBouncyCastle
                ();
            tsaClientBouncyCastle.SetTSAInfo(itsaInfoBouncyCastle);
            byte[] timestampTokenArray = tsaClientBouncyCastle.GetTimeStampToken(tsaClientBouncyCastle.GetMessageDigest
                ().Digest());
            ITimeStampToken expectedToken = BOUNCY_CASTLE_FACTORY.CreateTimeStampResponse(tsaClientBouncyCastle.GetExpectedTsaResponseBytes
                ()).GetTimeStampToken();
            ITimeStampTokenInfo expectedTsTokenInfo = expectedToken.GetTimeStampInfo();
            ITimeStampTokenInfo resultTsTokenInfo = itsaInfoBouncyCastle.GetTimeStampTokenInfo();
            NUnit.Framework.Assert.IsNotNull(timestampTokenArray);
            NUnit.Framework.Assert.IsNotNull(resultTsTokenInfo);
            NUnit.Framework.Assert.AreEqual(expectedTsTokenInfo.GetEncoded(), resultTsTokenInfo.GetEncoded());
            NUnit.Framework.Assert.AreEqual(expectedToken.GetEncoded(), timestampTokenArray);
        }

        [NUnit.Framework.Test]
        public virtual void GetTimeStampTokenFailureExceptionTest() {
            String allowedDigest = "SHA1";
            String signatureAlgorithm = "SHA256withRSA";
            String url = "url";
            TSAClientBouncyCastleTest.CustomTsaClientBouncyCastle tsaClientBouncyCastle = new TSAClientBouncyCastleTest.CustomTsaClientBouncyCastle
                (url, signatureAlgorithm, allowedDigest);
            tsaClientBouncyCastle.SetTSAInfo(new TSAClientBouncyCastleTest.CustomItsaInfoBouncyCastle());
            byte[] digest = tsaClientBouncyCastle.GetMessageDigest().Digest();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tsaClientBouncyCastle.GetTimeStampToken
                (digest));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.INVALID_TSA_RESPONSE
                , url, "128: request contains unknown algorithm"), e.Message);
        }

        private sealed class CustomTsaClientBouncyCastle : TSAClientBouncyCastle {
            private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

            private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

            private readonly IPrivateKey tsaPrivateKey;

            private readonly IList<IX509Certificate> tsaCertificateChain;

            private readonly String signatureAlgorithm;

            private readonly String allowedDigest;

            private byte[] expectedTsaResponseBytes;

            public CustomTsaClientBouncyCastle(String url, String signatureAlgorithm, String allowedDigest)
                : base(url) {
                this.signatureAlgorithm = signatureAlgorithm;
                this.allowedDigest = allowedDigest;
                tsaPrivateKey = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.pem", PASSWORD);
                String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
                tsaCertificateChain = JavaUtil.ArraysAsList(PemFileHelper.ReadFirstChain(tsaCertFileName));
            }

            public byte[] GetExpectedTsaResponseBytes() {
                return expectedTsaResponseBytes;
            }

            protected internal override byte[] GetTSAResponse(byte[] requestBytes) {
                TestTimestampTokenBuilder builder = new TestTimestampTokenBuilder(tsaCertificateChain, tsaPrivateKey);
                expectedTsaResponseBytes = builder.CreateTSAResponse(requestBytes, signatureAlgorithm, allowedDigest);
                return expectedTsaResponseBytes;
            }
        }

        private sealed class CustomItsaInfoBouncyCastle : ITSAInfoBouncyCastle {
            private ITimeStampTokenInfo timeStampTokenInfo;

            public void InspectTimeStampTokenInfo(ITimeStampTokenInfo info) {
                this.timeStampTokenInfo = info;
            }

            public ITimeStampTokenInfo GetTimeStampTokenInfo() {
                return timeStampTokenInfo;
            }
        }
    }
}
