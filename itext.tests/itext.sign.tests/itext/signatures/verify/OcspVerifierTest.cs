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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Cert;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OcspVerifierTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String src = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/OcspVerifierTest/";

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
            DateTime thisUpdate = DateTimeUtil.GetCalendar(caCert.GetNotBefore());
            DateTime nextUpdate = DateTimeUtil.GetCalendar(caCert.GetNotAfter().AddDays(2));
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            builder.SetProducedAt(caCert.GetNotBefore());
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder, certsSrc + "signCertRsa01.pem", caCert.GetNotAfter()));
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
        public virtual void VerifyOcspWhenCertificateWasRevokedBeforeSignDateTest() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder.SetCertificateStatus(FACTORY.CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME.AddDays(-20), FACTORY
                .CreateCRLReason().GetKeyCompromise()));
            NUnit.Framework.Assert.IsFalse(VerifyTest(builder));
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.VALID_CERTIFICATE_IS_REVOKED)]
        public virtual void VerifyOcspWhenCertificateWasRevokedAfterSignDateTest() {
            String rootCertFileName = src + "rootCert.pem";
            String checkCertFileName = src + "signCert.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder.SetCertificateStatus(FACTORY.CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME.AddDays(20), FACTORY.
                CreateCRLReason().GetKeyCompromise()));
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder, checkCertFileName, TimeTestUtil.TEST_DATE_TIME));
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
        public virtual void ValidOcspCreatedAfterSignDateTest01() {
            String rootCertFileName = src + "rootCert.pem";
            String checkCertFileName = src + "signCert.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            DateTime thisUpdate = DateTimeUtil.GetCalendar(TimeTestUtil.TEST_DATE_TIME).AddDays(15);
            DateTime nextUpdate = DateTimeUtil.GetCalendar(TimeTestUtil.TEST_DATE_TIME).AddDays(30);
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder, checkCertFileName, TimeTestUtil.TEST_DATE_TIME));
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredIssuerCertTest01_atValidPeriod() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(certsSrc + "intermediateExpiredCert.pem"
                )[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(certsSrc + "intermediateExpiredCert.pem", password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            DateTime thisUpdate = DateTimeUtil.GetCalendar(caCert.GetNotBefore());
            DateTime nextUpdate = DateTimeUtil.GetCalendar(caCert.GetNotAfter().AddDays(2));
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            builder.SetProducedAt(caCert.GetNotBefore());
            NUnit.Framework.Assert.IsTrue(VerifyTest(builder, certsSrc + "signCertRsaWithExpiredChain.pem", caCert.GetNotAfter
                ().AddDays(-1)));
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredIssuerCertTest01_afterValidPeriod() {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(certsSrc + "intermediateExpiredCert.pem"
                )[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(certsSrc + "intermediateExpiredCert.pem", password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            DateTime thisUpdate = DateTimeUtil.GetCalendar(caCert.GetNotBefore());
            DateTime nextUpdate = DateTimeUtil.GetCalendar(caCert.GetNotAfter().AddDays(2));
            builder.SetThisUpdate(thisUpdate);
            builder.SetNextUpdate(nextUpdate);
            builder.SetProducedAt(caCert.GetNotAfter().AddDays(1));
            NUnit.Framework.Assert.Catch(typeof(AbstractCertificateExpiredException), () => VerifyTest(builder, certsSrc
                 + "signCertRsaWithExpiredChain.pem", caCert.GetNotAfter().AddDays(1)));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderTest() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime ocspResponderCertEndDate = ocspResponderCertStartDate.AddDays(365 * 100);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            bool verifyRes = VerifyAuthorizedOCSPResponderWithOCSPNoCheckTest(ocspResponderCertStartDate, ocspResponderCertEndDate
                , checkDate);
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredAuthorizedOCSPResponderTest_atValidPeriod() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME.AddYears(-4);
            DateTime ocspResponderCertEndDate = TimeTestUtil.TEST_DATE_TIME.AddYears(1);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            bool verifyRes = VerifyAuthorizedOCSPResponderWithOCSPNoCheckTest(ocspResponderCertStartDate, ocspResponderCertEndDate
                , checkDate);
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredAuthorizedOCSPResponderTest_afterValidPeriod() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME.AddYears(-5);
            DateTime ocspResponderCertEndDate = TimeTestUtil.TEST_DATE_TIME.AddYears(-1);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            NUnit.Framework.Assert.Catch(typeof(AbstractCertificateExpiredException), () => VerifyAuthorizedOCSPResponderWithOCSPNoCheckTest
                (ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate));
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredResponderFromRootStoreTestAtValidPeriod() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            bool verifyRes = VerifyOcspResponseWithResponderFromRootStoreTest(checkDate);
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void ExpiredResponderFromRootStoreTestAfterValidPeriod() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME.AddDays(365 * 100);
            NUnit.Framework.Assert.Catch(typeof(AbstractCertificateExpiredException), () => VerifyOcspResponseWithResponderFromRootStoreTest
                (checkDate));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderWithOcspTest() {
            String rootCertFileName = src + "rootCertForOcspTest.pem";
            String checkCertFileName = src + "signCertForOcspTest.pem";
            String ocspResponderCertFileName = src + "ocspResponderCertForOcspTest.pem";
            bool verifyRes = VerifyOcspResponseWithRevocationCheckTest(rootCertFileName, checkCertFileName, ocspResponderCertFileName
                , true, false);
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.VALID_CERTIFICATE_IS_REVOKED)]
        public virtual void AuthorizedOCSPResponderWithOcspRevokedStatusTest() {
            String rootCertFileName = src + "rootCertForOcspTest.pem";
            String checkCertFileName = src + "signCertForOcspTest.pem";
            String ocspResponderCertFileName = src + "ocspResponderCertForOcspTest.pem";
            bool verifyRes = VerifyOcspResponseWithRevocationCheckTest(rootCertFileName, checkCertFileName, ocspResponderCertFileName
                , true, true);
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderWithCrlTest() {
            String rootCertFileName = src + "rootCertForCrlTest.pem";
            String checkCertFileName = src + "signCertForCrlTest.pem";
            String ocspResponderCertFileName = src + "ocspResponderCertForCrlTest.pem";
            bool verifyRes = VerifyOcspResponseWithRevocationCheckTest(rootCertFileName, checkCertFileName, ocspResponderCertFileName
                , false, false);
            NUnit.Framework.Assert.IsTrue(verifyRes);
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderWithoutRevocationDataTest() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime ocspResponderCertEndDate = ocspResponderCertStartDate.AddDays(365 * 100);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            NUnit.Framework.Assert.Catch(typeof(VerificationException), () => VerifyAuthorizedOCSPResponderCheckRevDataTest
                (ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderSetResponderOcspsTest() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime ocspResponderCertEndDate = ocspResponderCertStartDate.AddDays(365 * 100);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME.AddDays(365 * 20);
            NUnit.Framework.Assert.IsTrue(VerifyAuthorizedOCSPResponderWithProvidedOcspsTest(ocspResponderCertStartDate
                , ocspResponderCertEndDate, checkDate));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderSetResponderCrlsTest() {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime ocspResponderCertEndDate = ocspResponderCertStartDate.AddDays(365 * 100);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME.AddDays(365 * 20);
            NUnit.Framework.Assert.IsTrue(VerifyAuthorizedOCSPResponderWithProvidedCrlsTest(ocspResponderCertStartDate
                , ocspResponderCertEndDate, checkDate));
        }

        [NUnit.Framework.Test]
        public virtual void OcspResponseCouldNotBeVerifiedTest() {
            IX509Certificate wrongCert = (IX509Certificate)PemFileHelper.ReadFirstChain(certsSrc + "intermediateExpiredCert.pem"
                )[0];
            NUnit.Framework.Assert.Catch(typeof(VerificationException), () => VerifyOcspResponseWithoutResponderAvailableTest
                (new IX509Certificate[] { wrongCert }));
        }

        [NUnit.Framework.Test]
        public virtual void OcspResponseWithoutCertsCouldNotBeVerifiedTest() {
            NUnit.Framework.Assert.Catch(typeof(VerificationException), () => VerifyOcspResponseWithoutResponderAvailableTest
                (new IX509Certificate[0]));
        }

        [NUnit.Framework.Test]
        public virtual void GetOcspResponseNullTest() {
            OCSPVerifier verifier = new OCSPVerifier(null, null);
            NUnit.Framework.Assert.IsNull(verifier.GetOcspResponse(null, null));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateDoesNotVerifyWithSuppliedKeyTest() {
            String rootCertFileName = src + "rootCertForCrlTest.pem";
            String checkCertFileName = src + "signCertForCrlTest.pem";
            String ocspResponderCertFileName = src + "ocspResponderCertForOcspTest.pem";
            NUnit.Framework.Assert.Catch(typeof(AbstractGeneralSecurityException), () => VerifyOcspResponseWithRevocationCheckTest
                (rootCertFileName, checkCertFileName, ocspResponderCertFileName, true, false));
        }

        [NUnit.Framework.Test]
        public virtual void IssuersDoesNotMatchTest() {
            String rootCertFileName = src + "rootCert.pem";
            String wrongRootCertFileName = src + "rootCertForOcspTest.pem";
            String checkCertFileName = src + "signCert.pem";
            String ocspResponderCertFileName = src + "ocspResponderCert.pem";
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate wrongCaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(wrongRootCertFileName)[0];
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            NUnit.Framework.Assert.IsFalse(ocspVerifier.Verify(basicOCSPResp, checkCert, wrongCaCert, checkDate));
        }

        [NUnit.Framework.Test]
        public virtual void CheckBothOnlineAndProvidedOcspsTest() {
            String rootCertFileName = src + "rootCert.pem";
            String checkCertFileName = src + "signCert.pem";
            String ocspResponderCertFileName = src + "ocspResponderCert.pem";
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetOcspCertsChain(new IX509Certificate[0]);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, caCert, null);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(basicOcspRespBytes
                ));
            OCSPVerifier ocspVerifier = new OcspVerifierTest.CustomOCSPVerifier(null, JavaCollectionsUtil.SingletonList
                (basicOCSPResp));
            ocspVerifier.SetRootStore(PemFileHelper.InitStore(ocspResponderCertFileName));
            NUnit.Framework.Assert.IsTrue(ocspVerifier.Verify(checkCert, caCert, checkDate)[0].ToString().Contains("Valid OCSPs Found: 2 (1 online)"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOCSPResponderCreatedAfterSignDateTest() {
            String rootCertFileName = src + "rootCert2.pem";
            String checkCertFileName = src + "signCert2.pem";
            String ocspResponderCertFileName = src + "responderCreatedIn2001.pem";
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            // Feb 14, 2000 14:14:02 UTC
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(365));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(365)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(370)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            NUnit.Framework.Assert.IsTrue(ocspVerifier.Verify(basicOCSPResp, checkCert, caCert, checkDate));
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

        private bool VerifyAuthorizedOCSPResponderWithOCSPNoCheckTest(DateTime ocspResponderStartDate, DateTime ocspResponderEndDate
            , DateTime checkDate) {
            return VerifyAuthorizedOCSPResponderTest(ocspResponderStartDate, ocspResponderEndDate, checkDate, false, false
                , false);
        }

        private void VerifyAuthorizedOCSPResponderCheckRevDataTest(DateTime ocspResponderStartDate, DateTime ocspResponderEndDate
            , DateTime checkDate) {
            VerifyAuthorizedOCSPResponderTest(ocspResponderStartDate, ocspResponderEndDate, checkDate, true, false, false
                );
        }

        private bool VerifyAuthorizedOCSPResponderWithProvidedOcspsTest(DateTime ocspResponderCertStartDate, DateTime
             ocspResponderCertEndDate, DateTime checkDate) {
            return VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate, 
                true, true, false);
        }

        private bool VerifyAuthorizedOCSPResponderWithProvidedCrlsTest(DateTime ocspResponderCertStartDate, DateTime
             ocspResponderCertEndDate, DateTime checkDate) {
            return VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate, 
                true, false, true);
        }

        private void VerifyOcspResponseWithoutResponderAvailableTest(IX509Certificate[] ocspCertsChain) {
            DateTime ocspResponderCertStartDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime ocspResponderCertEndDate = ocspResponderCertStartDate.AddDays(365 * 100);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate, false, 
                false, false, false, ocspCertsChain);
        }

        private bool VerifyAuthorizedOCSPResponderTest(DateTime ocspResponderCertStartDate, DateTime ocspResponderCertEndDate
            , DateTime checkDate, bool checkResponderRevData, bool setResponderOcsps, bool setResponderCrls) {
            return VerifyAuthorizedOCSPResponderTest(ocspResponderCertStartDate, ocspResponderCertEndDate, checkDate, 
                true, checkResponderRevData, setResponderOcsps, setResponderCrls, null);
        }

        private bool VerifyAuthorizedOCSPResponderTest(DateTime ocspResponderCertStartDate, DateTime ocspResponderCertEndDate
            , DateTime checkDate, bool addResponder, bool checkResponderRevData, bool setResponderOcsps, bool setResponderCrls
            , IX509Certificate[] ocspCertsChain) {
            String rootCertFileName = src + "rootCert.pem";
            String checkCertFileName = src + "signCert.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, password);
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IRsaKeyPairGenerator keyGen = SignTestPortUtil.BuildRSA2048KeyPairGenerator();
            IAsymmetricCipherKeyPair key = keyGen.GenerateKeyPair();
            IPrivateKey ocspRespPrivateKey = key.GetPrivateKey();
            IPublicKey ocspRespPublicKey = key.GetPublicKey();
            TestCertificateBuilder certBuilder = new TestCertificateBuilder(ocspRespPublicKey, caCert, caPrivateKey, "CN=iTextTestOCSPResponder, OU=test, O=iText"
                );
            certBuilder.SetStartDate(ocspResponderCertStartDate);
            certBuilder.SetEndDate(ocspResponderCertEndDate);
            IX509Certificate ocspResponderCert = certBuilder.BuildAuthorizedOCSPResponderCert(checkResponderRevData);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(ocspResponderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            if (!addResponder) {
                builder.SetOcspCertsChain(ocspCertsChain);
            }
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, caCert, null);
            IAsn1Object var2 = FACTORY.CreateASN1Primitive(basicOcspRespBytes);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(var2);
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            if (setResponderOcsps) {
                TestOcspResponseBuilder builder2 = new TestOcspResponseBuilder(caCert, caPrivateKey);
                builder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-5)));
                builder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
                TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder2);
                ocspVerifier.SetOcspClient(ocspClient2);
            }
            if (setResponderCrls) {
                TestCrlBuilder testCrlBuilder = new TestCrlBuilder(caCert, caPrivateKey, checkDate.AddDays(-5));
                testCrlBuilder.SetNextUpdate(checkDate.AddDays(5));
                TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(testCrlBuilder);
                ocspVerifier.SetCrlClient(crlClient);
            }
            return ocspVerifier.Verify(basicOCSPResp, checkCert, caCert, checkDate);
        }

        private bool VerifyOcspResponseWithResponderFromRootStoreTest(DateTime checkDate) {
            String rootCertFileName = src + "rootCert.pem";
            String checkCertFileName = src + "signCert.pem";
            String ocspResponderCertFileName = src + "ocspResponderCert.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(-5));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetOcspCertsChain(new IX509Certificate[0]);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            byte[] basicOcspRespBytes = ocspClient.GetEncoded(checkCert, caCert, null);
            IAsn1Object var2 = FACTORY.CreateASN1Primitive(basicOcspRespBytes);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(var2);
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            ocspVerifier.SetRootStore(PemFileHelper.InitStore(ocspResponderCertFileName));
            return ocspVerifier.Verify(basicOCSPResp, checkCert, caCert, checkDate);
        }

        private bool VerifyOcspResponseWithRevocationCheckTest(String rootCertFileName, String checkCertFileName, 
            String ocspResponderCertFileName, bool checkOcsp, bool revokedOcsp) {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, password);
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, password);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            OCSPVerifier ocspVerifier = new OCSPVerifier(null, null);
            if (checkOcsp) {
                TestOcspResponseBuilder builder2 = revokedOcsp ? new TestOcspResponseBuilder(caCert, caPrivateKey, FACTORY
                    .CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME.AddDays(20), FACTORY.CreateCRLReason().GetKeyCompromise
                    ())) : new TestOcspResponseBuilder(caCert, caPrivateKey);
                builder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(20)));
                builder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(30)));
                TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder2);
                ocspVerifier.SetOcspClient(ocspClient2);
            }
            else {
                TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, checkDate.AddDays(-1));
                crlBuilder.SetNextUpdate(checkDate.AddDays(1));
                TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(crlBuilder);
                ocspVerifier.SetCrlClient(crlClient);
            }
            return ocspVerifier.Verify(basicOCSPResp, checkCert, caCert, checkDate);
        }

        private class CustomOCSPVerifier : OCSPVerifier {
            /// <summary>Creates an OCSPVerifier instance.</summary>
            /// <param name="verifier">the next verifier in the chain</param>
            /// <param name="ocsps">
            /// a list of
            /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
            /// OCSP response wrappers for the certificate verification
            /// </param>
            public CustomOCSPVerifier(CertificateVerifier verifier, IList<IBasicOcspResponse> ocsps)
                : base(verifier, ocsps) {
            }

            public override IBasicOcspResponse GetOcspResponse(IX509Certificate signCert, IX509Certificate issuerCert) {
                String rootCertFileName = src + "rootCert.pem";
                String checkCertFileName = src + "signCert.pem";
                String ocspResponderCertFileName = src + "ocspResponderCert.pem";
                DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
                try {
                    IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
                    IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, password);
                    IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
                    IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                        [0];
                    TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, caPrivateKey);
                    builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
                    builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(15)));
                    TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
                    byte[] basicOcspRespBytes = ocspClient2.GetEncoded(checkCert, caCert, null);
                    return FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(basicOcspRespBytes));
                }
                catch (Exception) {
                }
                return null;
            }
        }
    }
}
