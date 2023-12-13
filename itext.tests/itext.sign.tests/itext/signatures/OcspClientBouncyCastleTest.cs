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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Signutils;

namespace iText.Signatures {
    public class OcspClientBouncyCastleTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpass".ToCharArray();

        private static readonly String caCertFileName = certsSrc + "rootRsa.p12";

        private static X509Certificate checkCert;

        private static X509Certificate rootCert;

        private static TestOcspResponseBuilder builder;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "signCertRsa01.p12", password)[0];
            rootCert = builder.GetIssuerCert();
        }

        [NUnit.Framework.Test]
        public virtual void GetBasicOCSPRespTest() {
            OcspClientBouncyCastle ocspClientBouncyCastle = CreateOcspClient();
            BasicOcspResp basicOCSPResp = ocspClientBouncyCastle.GetBasicOCSPResp(checkCert, rootCert, null);
            NUnit.Framework.Assert.IsNotNull(basicOCSPResp);
            NUnit.Framework.Assert.IsTrue(basicOCSPResp.Responses.Length > 0);
        }

        [NUnit.Framework.Test]
        public virtual void GetBasicOCSPRespNullTest() {
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            OcspClientBouncyCastle ocspClientBouncyCastle = new OcspClientBouncyCastle(ocspVerifier);
            BasicOcspResp basicOCSPResp = ocspClientBouncyCastle.GetBasicOCSPResp(checkCert, null, null);
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
            byte[] encoded = ocspClientBouncyCastle.GetEncoded(checkCert, rootCert, null);
            NUnit.Framework.Assert.IsNotNull(encoded);
            NUnit.Framework.Assert.IsTrue(encoded.Length > 0);
        }

        private static OcspClientBouncyCastle CreateOcspClient() {
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            return new OcspClientBouncyCastleTest.TestOcspClientBouncyCastle(ocspVerifier);
        }

        private sealed class TestOcspClientBouncyCastle : OcspClientBouncyCastle {
            public TestOcspClientBouncyCastle(OCSPVerifier verifier)
                : base(verifier) {
            }

            internal override OcspResp GetOcspResponse(X509Certificate chCert, X509Certificate rCert, String url) {
                try {
                    CertificateID id = SignTestPortUtil.GenerateCertificateId(rootCert, checkCert.SerialNumber, Org.BouncyCastle.Ocsp.CertificateID.HashSha1
                        );
                    BasicOcspResp basicOCSPResp = builder.MakeOcspResponseObject(SignTestPortUtil.GenerateOcspRequestWithNonce
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
