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
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OCSPValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/OCSPValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            String rootCertFileName = SOURCE_FOLDER + "rootCert.pem";
            String checkCertFileName = SOURCE_FOLDER + "signCert.pem";
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCert.pem";
            caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)[0];
            ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, PASSWORD);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateResponderOcspNoCheckTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8176: Implement RevocationDataValidator class")]
        public virtual void ValidateAuthorizedOCSPResponderWithOcspTest() {
            ValidationReport report = VerifyResponderWithOcsp(false);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8176: Implement RevocationDataValidator class")]
        public virtual void ValidateAuthorizedOCSPResponderWithOcspRevokedTest() {
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertForOcspTest.pem";
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            ValidationReport report = VerifyResponderWithOcsp(true);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.CERT_IS_REVOKED, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(responderCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void ValidateAuthorizedOCSPResponderFromTheTrustedStoreTest() {
            ValidationReport report = ValidateOcspWithoutCertsTest(true);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void NoResponderFoundInCertsTest() {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetOcspCertsChain(new IX509Certificate[] { caCert });
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, checkCert, basicOCSPResp, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_COULD_NOT_BE_VERIFIED, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void NoResponderFoundTest() {
            ValidationReport report = ValidateOcspWithoutCertsTest(false);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_COULD_NOT_BE_VERIFIED, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void ValidationDateAfterNextUpdateTest() {
            // Same next update is set in the test OCSP builder.
            DateTime nextUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(30);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME.AddDays(45);
            ValidationReport report = ValidateTest(checkDate, TimeTestUtil.TEST_DATE_TIME, 50);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem logItem = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, logItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(OCSPValidator.OCSP_IS_NO_LONGER_VALID, checkDate, 
                nextUpdate), logItem.GetMessage());
            CertificateReportItem failureItem = (CertificateReportItem)report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, failureItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.NO_USABLE_OCSP_WAS_FOUND, failureItem.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateWasRevokedAfterCheckDateTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(10);
            ValidationReport report = ValidateRevokedTest(checkDate, revocationDate);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem certificateCheckItem = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, certificateCheckItem.GetCheckName
                ());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, certificateCheckItem
                .GetCertificate().GetSubjectDN()), certificateCheckItem.GetMessage());
            CertificateReportItem ocspCheckItem = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, ocspCheckItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignLogMessageConstant.VALID_CERTIFICATE_IS_REVOKED
                , revocationDate), ocspCheckItem.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateWasRevokedBeforeCheckDateTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(-1);
            ValidationReport report = ValidateRevokedTest(checkDate, revocationDate);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem ocspCheckItem = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, ocspCheckItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.CERT_IS_REVOKED, ocspCheckItem.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateStatusIsUnknownTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetCertificateStatus(FACTORY.CreateUnknownStatus());
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, checkCert, basicOCSPResp, checkDate);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem ocspCheckItem = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, ocspCheckItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.CERT_STATUS_IS_UNKNOWN, ocspCheckItem.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void SerialNumbersDoesNotMatchTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(1)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse caBasicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient
                .GetEncoded(caCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, checkCert, caBasicOCSPResp, checkDate);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.NO_USABLE_OCSP_WAS_FOUND, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void OcspForSelfSignedCertTest() {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse caBasicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient
                .GetEncoded(caCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, caCert, caBasicOCSPResp, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void IssuersDoesNotMatchTest() {
            String wrongRootCertFileName = SOURCE_FOLDER + "rootCertForOcspTest.pem";
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(new OCSPValidatorTest.TestIssuingCertificateRetriever
                (wrongRootCertFileName));
            validator.Validate(report, checkCert, basicOCSPResp, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem logItem = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, logItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.ISSUERS_DOES_NOT_MATCH, logItem.GetMessage());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.NO_USABLE_OCSP_WAS_FOUND, report.GetFailures()[0].GetMessage
                ());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateDoesNotVerifyWithSuppliedKeyTest() {
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertForOcspTest.pem";
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, PASSWORD);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, checkCert, basicOCSPResp, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem ocspCheckItem = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, ocspCheckItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.INVALID_OCSP, ocspCheckItem.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void OcspResponderDoesNotHaveOcspSigningExtensionTest() {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse caBasicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient
                .GetEncoded(caCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, caCert, caBasicOCSPResp, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.EXTENSIONS_CHECK, item.GetCheckName());
            // ExtendedKeyUsageExtension.OCSP_SIGNING is missing.
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.EXTENSION_MISSING, OID.X509Extensions
                .EXTENDED_KEY_USAGE), item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void ValidateNullTest() {
            IX509Certificate certificate = null;
            ValidationReport report = new ValidationReport();
            new OCSPValidator().Validate(report, certificate, null, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(0, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void PositiveFreshnessPositiveTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate, checkDate.AddDays(-3), 5);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void PositiveFreshnessNegativeTest() {
            // TODO DEVSIX-8176 Implement RevocationDataValidator class: fix asserts
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate, checkDate.AddDays(-3), 2);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void NegativeFreshnessPositiveTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate, checkDate.AddDays(5), -3);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void NegativeFreshnessNegativeTest() {
            // TODO DEVSIX-8176 Implement RevocationDataValidator class: fix asserts
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate, checkDate.AddDays(2), -3);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        private ValidationReport ValidateTest(DateTime checkDate) {
            return ValidateTest(checkDate, checkDate.AddDays(1), 0);
        }

        private ValidationReport ValidateTest(DateTime checkDate, DateTime thisUpdate, int freshness) {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(thisUpdate));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            // TODO DEVSIX-8176 Implement RevocationDataValidator class: set up freshness
            validator.Validate(report, checkCert, basicOCSPResp, checkDate);
            return report;
        }

        private ValidationReport ValidateRevokedTest(DateTime checkDate, DateTime revocationDate) {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetCertificateStatus(FACTORY.CreateRevokedStatus(revocationDate, FACTORY.CreateCRLReason().GetKeyCompromise
                ()));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, checkCert, basicOCSPResp, checkDate);
            return report;
        }

        private ValidationReport ValidateOcspWithoutCertsTest(bool addResponderToTrusted) {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetOcspCertsChain(new IX509Certificate[0]);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            if (addResponderToTrusted) {
                certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(responderCert));
            }
            OCSPValidator validator = new OCSPValidator().SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, checkCert, basicOCSPResp, TimeTestUtil.TEST_DATE_TIME);
            return report;
        }

        // TODO DEVSIX-8176 Implement RevocationDataValidator class: make sure that any tests for any
        //  certificates with CRLs (valid result) and without revocation data (indeterminate result) are added
        private ValidationReport VerifyResponderWithOcsp(bool revokedOcsp) {
            String rootCertFileName = SOURCE_FOLDER + "rootCertForOcspTest.pem";
            String checkCertFileName = SOURCE_FOLDER + "signCertForOcspTest.pem";
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertForOcspTest.pem";
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, PASSWORD);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            OCSPValidator validator = new OCSPValidator();
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            TestOcspResponseBuilder builder2 = revokedOcsp ? new TestOcspResponseBuilder(caCert, caPrivateKey, FACTORY
                .CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME.AddDays(-5), FACTORY.CreateCRLReason().GetKeyCompromise
                ())) : new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(20)));
            builder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(30)));
            TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder2);
            CertificateChainValidator certificateChainValidator = new CertificateChainValidator().SetOcspClient(ocspClient2
                );
            if (revokedOcsp) {
                certificateChainValidator.ProceedValidationAfterFail(false);
            }
            validator.SetCertificateChainValidator(certificateChainValidator);
            validator.SetIssuingCertificateRetriever(certificateRetriever);
            validator.Validate(report, checkCert, basicOCSPResp, checkDate);
            return report;
        }

        private class TestIssuingCertificateRetriever : IssuingCertificateRetriever {
            internal IX509Certificate issuerCertificate;

            public TestIssuingCertificateRetriever(String issuerPath)
                : base() {
                this.issuerCertificate = PemFileHelper.ReadFirstChain(issuerPath)[0];
            }

            public override IX509Certificate RetrieveIssuerCertificate(IX509Certificate certificate) {
                return issuerCertificate;
            }
        }
    }
}
