/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Signutils;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class OcspClientBouncyCastleTest : ExtendedITextTest {
        private static readonly String ocspCertsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/OcspClientBouncyCastleTest/";

        private static readonly String rootOcspCert = ocspCertsSrc + "ocspRootRsa.p12";

        private static readonly String signOcspCert = ocspCertsSrc + "ocspSignRsa.p12";

        private static readonly char[] password = "testpass".ToCharArray();

        private const String ocspServiceUrl = "http://localhost:9000/demo/ocsp/ocsp-service";

        private static X509Certificate checkCert;

        private static X509Certificate rootCert;

        private static TestOcspResponseBuilder builder;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = CreateBuilder(CertificateStatus.Good);
            checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(signOcspCert, password)[0];
            rootCert = builder.GetIssuerCert();
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenCheckCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle(null);
            NUnit.Framework.Assert.IsNull(castle.GetOcspResponse(null, rootCert, ocspServiceUrl));
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenRootCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle(null);
            NUnit.Framework.Assert.IsNull(castle.GetOcspResponse(checkCert, null, ocspServiceUrl));
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenRootAndCheckCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle(null);
            NUnit.Framework.Assert.IsNull(castle.GetOcspResponse(null, null, ocspServiceUrl));
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseWhenUrlCertIsNullTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle(null);
            NUnit.Framework.Assert.Catch(typeof(WebException), () => castle.GetOcspResponse(checkCert, rootCert, null)
                );
        }

        [NUnit.Framework.Test]
        [LogMessage("Getting OCSP from http://asd", LogLevel = LogLevelConstants.INFO)]
        public virtual void IncorrectUrlTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle(null);
            NUnit.Framework.Assert.Catch(typeof(WebException), () => castle.GetOcspResponse(checkCert, rootCert, "http://asd"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage("Getting OCSP from", LogLevel = LogLevelConstants.INFO)]
        public virtual void MalformedUrlTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle(null);
            NUnit.Framework.Assert.Catch(typeof(UriFormatException), () => castle.GetOcspResponse(checkCert, rootCert, 
                ""));
        }

        [NUnit.Framework.Test]
        [LogMessage("Getting OCSP from http://localhost:9000/demo/ocsp/ocsp-service", LogLevel = LogLevelConstants
            .INFO)]
        public virtual void ConnectionRefusedTest() {
            OcspClientBouncyCastle castle = new OcspClientBouncyCastle(null);
            NUnit.Framework.Assert.Catch(typeof(WebException), () => castle.GetOcspResponse(checkCert, rootCert, ocspServiceUrl
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetBasicOcspRespTest() {
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateOcspClient();
            BasicOcspResp basicOCSPResp = ocspClientBouncyCastle.GetBasicOCSPResp(checkCert, rootCert, ocspServiceUrl);
            NUnit.Framework.Assert.IsNotNull(basicOCSPResp);
            NUnit.Framework.Assert.IsTrue(basicOCSPResp.Responses.Length > 0);
        }

        [NUnit.Framework.Test]
        public virtual void GetBasicOcspRespNullTest() {
            OcspClientBouncyCastle ocspClientBouncyCastle = new OcspClientBouncyCastle(null);
            BasicOcspResp basicOCSPResp = ocspClientBouncyCastle.GetBasicOCSPResp(checkCert, null, ocspServiceUrl);
            NUnit.Framework.Assert.IsNull(basicOCSPResp);
        }

        [NUnit.Framework.Test]
        [LogMessage("OCSP response could not be verified")]
        public virtual void GetBasicOCSPRespLogMessageTest() {
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateOcspClient();
            BasicOcspResp basicOCSPResp = ocspClientBouncyCastle.GetBasicOCSPResp(null, null, null);
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_REVOKED)]
        public virtual void OcspStatusIsRevokedTest() {
            RevokedStatus status = new RevokedStatus(DateTimeUtil.GetCurrentUtcTime().AddDays(-20), Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful
                );
            TestOcspResponseBuilder responseBuilder = CreateBuilder(status);
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateTestOcspClient(responseBuilder);
            byte[] encoded = ocspClientBouncyCastle.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            NUnit.Framework.Assert.IsNull(encoded);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_UNKNOWN)]
        public virtual void OcspStatusIsUnknownTest() {
            UnknownStatus status = new UnknownStatus();
            TestOcspResponseBuilder responseBuilder = CreateBuilder(status);
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateTestOcspClient(responseBuilder);
            byte[] encoded = ocspClientBouncyCastle.GetEncoded(checkCert, rootCert, ocspServiceUrl);
            NUnit.Framework.Assert.IsNull(encoded);
        }

        private static OcspClientBouncyCastle CreateOcspClient() {
            return CreateOcspClient(builder);
        }

        private static OcspClientBouncyCastle CreateOcspClient(TestOcspResponseBuilder builder) {
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            return new OcspClientBouncyCastleTest.TestOcspClientBouncyCastle(ocspVerifier, builder);
        }

        private static OcspClientBouncyCastle CreateTestOcspClient(TestOcspResponseBuilder responseBuilder) {
            return CreateOcspClient(responseBuilder);
        }

        private static TestOcspResponseBuilder CreateBuilder(CertificateStatus status) {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(rootOcspCert, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(rootOcspCert, password, password);
            return new TestOcspResponseBuilder(caCert, caPrivateKey, status);
        }

        private sealed class TestOcspClientBouncyCastle : OcspClientBouncyCastle {
            private static TestOcspResponseBuilder testOcspBuilder;

            public TestOcspClientBouncyCastle(OCSPVerifier verifier, TestOcspResponseBuilder testBuilder)
                : base(verifier) {
                testOcspBuilder = testBuilder;
            }

            internal override OcspResp GetOcspResponse(X509Certificate chCert, X509Certificate rCert, String url) {
                try {
                    CertificateID id = SignTestPortUtil.GenerateCertificateId(rootCert, checkCert.SerialNumber, Org.BouncyCastle.Ocsp.CertificateID.HashSha1
                        );
                    BasicOcspResp basicOCSPResp = testOcspBuilder.MakeOcspResponseObject(SignTestPortUtil.GenerateOcspRequestWithNonce
                        (id).GetEncoded());
                    return new OCSPRespGenerator().Generate(Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful, basicOCSPResp
                        );
                }
                catch (Exception e) {
                    throw new OcspException(e.Message);
                }
            }
        }
    }
}
