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
using System.Net;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OcspClientBouncyCastleTest : ExtendedITextTest {
        private static readonly String ocspCertsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/OcspClientBouncyCastleTest/";

        private static readonly String rootOcspCert = ocspCertsSrc + "ocspRootRsa.pem";

        private static readonly String signOcspCert = ocspCertsSrc + "ocspSignRsa.pem";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        private const String ocspServiceUrl = "http://localhost:9000/demo/ocsp/ocsp-service";

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static IX509Certificate checkCert;

        private static IX509Certificate rootCert;

        private static TestOcspResponseBuilder builder;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = CreateBuilder(BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood());
            checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signOcspCert)[0];
            rootCert = builder.GetIssuerCert();
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenCheckCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle();
            NUnit.Framework.Assert.IsNull(castle.GetOcspResponse(null, rootCert, ocspServiceUrl));
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenRootCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle();
            NUnit.Framework.Assert.IsNull(castle.GetOcspResponse(checkCert, null, ocspServiceUrl));
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenRootAndCheckCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle();
            NUnit.Framework.Assert.IsNull(castle.GetOcspResponse(null, null, ocspServiceUrl));
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenUrlCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle();
            NUnit.Framework.Assert.Catch(typeof(WebException), () => castle.GetOcspResponse(checkCert, rootCert, null)
                );
        }

        [NUnit.Framework.Test]
        [LogMessage("Getting OCSP from http://asd", LogLevel = LogLevelConstants.INFO)]
        public virtual void IncorrectUrlTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle();
            NUnit.Framework.Assert.Catch(typeof(WebException), () => castle.GetOcspResponse(checkCert, rootCert, "http://asd"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage("Getting OCSP from", LogLevel = LogLevelConstants.INFO)]
        public virtual void MalformedUrlTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle();
            NUnit.Framework.Assert.Catch(typeof(UriFormatException), () => castle.GetOcspResponse(checkCert, rootCert, 
                ""));
        }

        [NUnit.Framework.Test]
        [LogMessage("Getting OCSP from http://localhost:9000/demo/ocsp/ocsp-service", LogLevel = LogLevelConstants
            .INFO)]
        public virtual void ConnectionRefusedTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle();
            NUnit.Framework.Assert.Catch(typeof(WebException), () => castle.GetOcspResponse(checkCert, rootCert, ocspServiceUrl
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetBasicOcspRespTest() {
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateOcspClient();
            IBasicOcspResponse basicOCSPResp = ocspClientBouncyCastle.GetBasicOCSPResp(checkCert, rootCert, ocspServiceUrl
                );
            NUnit.Framework.Assert.IsNotNull(basicOCSPResp);
            NUnit.Framework.Assert.IsTrue(basicOCSPResp.GetResponses().Length > 0);
        }

        [NUnit.Framework.Test]
        public virtual void GetBasicOcspRespNullTest() {
            OcspClientBouncyCastle ocspClientBouncyCastle = new OcspClientBouncyCastle();
            IBasicOcspResponse basicOCSPResp = ocspClientBouncyCastle.GetBasicOCSPResp(checkCert, null, ocspServiceUrl
                );
            NUnit.Framework.Assert.IsNull(basicOCSPResp);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedTest() {
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateOcspClient();
            byte[] encoded = ocspClientBouncyCastle.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            NUnit.Framework.Assert.IsNotNull(encoded);
            NUnit.Framework.Assert.IsTrue(encoded.Length > 0);
        }

        [NUnit.Framework.Test]
        public virtual void OcspStatusIsRevokedTest() {
            IRevokedCertStatus status = BOUNCY_CASTLE_FACTORY.CreateRevokedStatus(DateTimeUtil.GetCurrentUtcTime().AddDays
                (-20), BOUNCY_CASTLE_FACTORY.CreateOCSPResponse().GetSuccessful());
            TestOcspResponseBuilder responseBuilder = CreateBuilder(status);
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateTestOcspClient(responseBuilder);
            byte[] encoded = ocspClientBouncyCastle.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            NUnit.Framework.Assert.IsNotNull(BOUNCY_CASTLE_FACTORY.CreateRevokedStatus(OcspClientBouncyCastle.GetCertificateStatus
                (encoded)));
        }

        [NUnit.Framework.Test]
        public virtual void OcspStatusIsUnknownTest() {
            IUnknownCertStatus status = BOUNCY_CASTLE_FACTORY.CreateUnknownStatus();
            TestOcspResponseBuilder responseBuilder = CreateBuilder(status);
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateTestOcspClient(responseBuilder);
            byte[] encoded = ocspClientBouncyCastle.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            NUnit.Framework.Assert.AreNotEqual(BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood(), OcspClientBouncyCastle
                .GetCertificateStatus(encoded));
            NUnit.Framework.Assert.IsNull(BOUNCY_CASTLE_FACTORY.CreateRevokedStatus(OcspClientBouncyCastle.GetCertificateStatus
                (encoded)));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOcspStatusIsNullTest() {
            byte[] encoded = new byte[0];
            NUnit.Framework.Assert.IsNull(OcspClientBouncyCastle.GetCertificateStatus(encoded));
        }

        private static OcspClientBouncyCastle CreateOcspClient() {
            return CreateOcspClient(builder);
        }

        private static OcspClientBouncyCastle CreateOcspClient(TestOcspResponseBuilder builder) {
            return new OcspClientBouncyCastleTest.TestOcspClientBouncyCastle(builder);
        }

        private static OcspClientBouncyCastle CreateTestOcspClient(TestOcspResponseBuilder responseBuilder) {
            return CreateOcspClient(responseBuilder);
        }

        private static TestOcspResponseBuilder CreateBuilder(ICertStatus status) {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootOcspCert)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootOcspCert, password);
            return new TestOcspResponseBuilder(caCert, caPrivateKey, status);
        }

        private sealed class TestOcspClientBouncyCastle : OcspClientBouncyCastle {
            private static TestOcspResponseBuilder testOcspBuilder;

            public TestOcspClientBouncyCastle(TestOcspResponseBuilder testBuilder)
                : base() {
                testOcspBuilder = testBuilder;
            }

//\cond DO_NOT_DOCUMENT
            internal override IOcspResponse GetOcspResponse(IX509Certificate chCert, IX509Certificate rCert, String url
                ) {
                try {
                    ICertID id = SignTestPortUtil.GenerateCertificateId(rootCert, checkCert.GetSerialNumber(), BOUNCY_CASTLE_FACTORY
                        .CreateCertificateID().GetHashSha1());
                    IBasicOcspResponse basicOCSPResp = testOcspBuilder.MakeOcspResponseObject(SignTestPortUtil.GenerateOcspRequestWithNonce
                        (id).GetEncoded());
                    return BOUNCY_CASTLE_FACTORY.CreateOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateOCSPResponseStatus().GetSuccessful
                        (), basicOCSPResp);
                }
                catch (Exception e) {
                    throw BOUNCY_CASTLE_FACTORY.CreateAbstractOCSPException(e);
                }
            }
//\endcond
        }
    }
}
