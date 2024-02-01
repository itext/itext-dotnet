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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Cert;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OcspVerifierTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        private static readonly String caCertFileName = certsSrc + "rootRsa.pem";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void ValidOcspTest01() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void ValidOcspWithoutOcspResponseBuilderTest() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(certsSrc + "signCertRsa01.pem")[0
                ];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            NUnit.Framework.Assert.IsTrue(ocspVerifier.Verify(caCert, rootCert, checkDate).IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRevokedOcspTest01() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder.SetCertificateStatus(FACTORY.CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME.AddDays(-20), FACTORY
                .CreateCRLReason().GetKeyCompromise()));
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidUnknownOcspTest01() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder.SetCertificateStatus(FACTORY.CreateUnknownStatus());
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOutdatedOcspTest01() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            DateTime thisUpdate = DateTimeUtil.GetCalendar(TimeTestUtil.TEST_DATE_TIME).AddDays(-30);
            DateTime nextUpdate = DateTimeUtil.GetCalendar(TimeTestUtil.TEST_DATE_TIME).AddDays(-15);
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredIssuerCertTest01() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(certsSrc + "intermediateExpiredCert.pem"
                )[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(certsSrc + "intermediateExpiredCert.pem", password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            DateTime thisUpdate = DateTimeUtil.GetCurrentTime().AddDays(30);
            DateTime nextUpdate = DateTimeUtil.GetCurrentTime();
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder, certsSrc + "signCertRsaWithExpiredChain.pem", caCert.GetNotBefore
                ()));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderTest() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime ocspResponderCertEndDate = ocspResponderCertStartDate.AddDays(365 * 100);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            bool verifyRes = VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate
                );
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredAuthorizedOCSPResponderTest_atValidPeriod() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME.AddYears(-4);
            DateTime ocspResponderCertEndDate = TimeTestUtil.TEST_DATE_TIME.AddYears(1);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            bool verifyRes = VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate
                );
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredAuthorizedOCSPResponderTest_now() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME.AddYears(-5);
            DateTime ocspResponderCertEndDate = TimeTestUtil.TEST_DATE_TIME.AddYears(-1);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            NUnit.Framework.Assert.Catch(typeof(AbstractCertificateExpiredException), () => VerifyAuthorizedOCSPResponderTest
                (ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate));
        }

        // Not getting here because of exception
        //Assert.assertFalse(verifyRes);
        [NUnit.Framework.Test]
        public virtual void GetOcspResponseNullTest() {
            OCSPVerifier verifier = new OCSPVerifier(null, null);
            NUnit.Framework.Assert.IsNull(verifier.GetOcspResponse(null, null));
        }

        private bool VerifyTest(TestOcspResponseBuilder rootRsaOcspBuilder) {
            return VerifyTest(rootRsaOcspBuilder, certsSrc + "signCertRsa01.pem", TimeTestUtil.TEST_DATE_TIME);
        }

        private bool VerifyTest(TestOcspResponseBuilder rootRsaOcspBuilder, String checkCertFileName, DateTime checkDate
            ) {
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate rootCert = rootRsaOcspBuilder.GetIssuerCert();
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, rootRsaOcspBuilder);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, rootCert, null);
            IAsn1Object var2 = FACTORY.CreateASN1Primitive(basicOcspRespBytes);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(var2);
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            return ocspVerifier.Verify(basicOCSPResp, checkCert, rootCert, checkDate);
        }

        public virtual bool VerifyAuthorizedOCSPResponderTest(DateTime ocspResponderCertStartDate, DateTime ocspResponderCertEndDate
            , DateTime checkDate) {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(certsSrc + "intermediateRsa.pem")
                [0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(certsSrc + "intermediateRsa.pem", password);
            String checkCertFileName = certsSrc + "signCertRsaWithChain.pem";
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IRsaKeyPairGenerator keyGen = SignTestPortUtil.BuildRSA2048KeyPairGenerator();
            IAsymmetricCipherKeyPair key = keyGen.GenerateKeyPair();
            IPrivateKey ocspRespPrivateKey = key.GetPrivateKey();
            IPublicKey ocspRespPublicKey = key.GetPublicKey();
            TestCertificateBuilder certBuilder = new TestCertificateBuilder(ocspRespPublicKey, caCert, caPrivateKey, "CN=iTextTestOCSPResponder, OU=test, O=iText"
                );
            certBuilder.SetStartDate(ocspResponderCertStartDate);
            certBuilder.SetEndDate(ocspResponderCertEndDate);
            IX509Certificate ocspResponderCert = certBuilder.BuildAuthorizedOCSPResponderCert();
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(ocspResponderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, caCert, null);
            IAsn1Object var2 = FACTORY.CreateASN1Primitive(basicOcspRespBytes);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(var2);
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            return ocspVerifier.Verify(basicOCSPResp, checkCert, caCert, checkDate);
        }
    }
}
