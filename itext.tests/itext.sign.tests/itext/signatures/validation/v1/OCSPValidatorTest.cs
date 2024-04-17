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
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OCSPValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/OCSPValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        private IssuingCertificateRetriever certificateRetriever;

        private SignatureValidationProperties parameters;

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.REVOCATION_DATA_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private ValidatorChainBuilder validatorChainBuilder;

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

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            certificateRetriever = new IssuingCertificateRetriever();
            parameters = new SignatureValidationProperties();
            validatorChainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties(parameters).WithIssuingCertificateRetriever
                (certificateRetriever);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateResponderOcspNoCheckTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate);
            new AssertValidationReport(report).HasNumberOfFailures(0).HasNumberOfLogs(2).HasLogItem((l) => l.GetCheckName
                ().Equals(RevocationDataValidator.REVOCATION_DATA_CHECK) && l.GetMessage().Equals(RevocationDataValidator
                .TRUSTED_OCSP_RESPONDER), "Revocation data check with trusted responder").HasLogItem((l) => l.GetCheckName
                ().Equals(CertificateChainValidator.CERTIFICATE_CHECK) && l.GetMessage().Equals(MessageFormatUtil.Format
                (CertificateChainValidator.CERTIFICATE_TRUSTED, ((CertificateReportItem)l).GetCertificate().GetSubjectDN
                ())), "ChainValidator certificate trusted").HasStatus(ValidationReport.ValidationResult.VALID).DoAssert
                ();
        }

        [NUnit.Framework.Test]
        public virtual void ValidateAuthorizedOCSPResponderWithOcspTest() {
            ValidationReport report = VerifyResponderWithOcsp(false);
            new AssertValidationReport(report).HasNumberOfFailures(0).HasNumberOfLogs(2).HasLogItems((l) => l.GetCheckName
                ().Equals(CertificateChainValidator.CERTIFICATE_CHECK) && l.GetMessage().Equals(MessageFormatUtil.Format
                (CertificateChainValidator.CERTIFICATE_TRUSTED, ((CertificateReportItem)l).GetCertificate().GetSubjectDN
                ())), 2, "Certificate check with trusted certificate").HasStatus(ValidationReport.ValidationResult.VALID
                ).DoAssert();
        }

        [NUnit.Framework.Test]
        public virtual void ValidateAuthorizedOCSPResponderWithOcspRevokedTest() {
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertForOcspTest.pem";
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.Singleton(responderCert));
            ValidationReport report = VerifyResponderWithOcsp(true);
            new AssertValidationReport(report).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName
                ().Equals(OCSPValidator.OCSP_CHECK) && l.GetMessage().Equals(OCSPValidator.CERT_IS_REVOKED) && l.GetStatus
                ().Equals(ReportItem.ReportItemStatus.INDETERMINATE), "Certificate revoked").DoAssert();
        }

        [NUnit.Framework.Test]
        public virtual void ValidateAuthorizedOCSPResponderFromTheTrustedStoreTest() {
            ValidationReport report = ValidateOcspWithoutCertsTest(true);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
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
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME);
            new AssertValidationReport(report).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName
                ().Equals(OCSPValidator.OCSP_CHECK) && l.GetMessage().Equals(OCSPValidator.OCSP_COULD_NOT_BE_VERIFIED)
                , "OCSP responder not found").HasStatus(ValidationReport.ValidationResult.INDETERMINATE).DoAssert();
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
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(OCSPValidator.OCSP_IS_NO_LONGER_VALID, checkDate, 
                nextUpdate), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateWasRevokedAfterCheckDateTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(10);
            ValidationReport report = ValidateRevokedTest(checkDate, revocationDate);
            new AssertValidationReport(report).HasNumberOfFailures(0).HasNumberOfLogs(3).HasLogItem((l) => l.GetCheckName
                ().Equals(OCSPValidator.OCSP_CHECK) && l.GetMessage().Equals(MessageFormatUtil.Format(SignLogMessageConstant
                .VALID_CERTIFICATE_IS_REVOKED, revocationDate)), "valid certificate is revoked").HasStatus(ValidationReport.ValidationResult
                .VALID).DoAssert();
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
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                );
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
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, caBasicOCSPResp.GetResponses()[0], caBasicOCSPResp, checkDate
                );
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.SERIAL_NUMBERS_DO_NOT_MATCH, item.GetMessage());
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
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, caCert, caBasicOCSPResp.GetResponses()[0], caBasicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.SELF_SIGNED_CERTIFICATE, item.GetMessage());
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
            validatorChainBuilder.WithIssuingCertificateRetriever(new OCSPValidatorTest.TestIssuingCertificateRetriever
                (wrongRootCertFileName));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME);
            new AssertValidationReport(report).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName
                ().Equals(OCSPValidator.OCSP_CHECK) && l.GetMessage().Equals(OCSPValidator.ISSUERS_DO_NOT_MATCH) && l.
                GetStatus().Equals(ReportItem.ReportItemStatus.INDETERMINATE), OCSPValidator.ISSUERS_DO_NOT_MATCH).DoAssert
                ();
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
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem ocspCheckItem = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, ocspCheckItem.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.INVALID_OCSP, ocspCheckItem.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void TrustedOcspResponderDoesNotHaveOcspSigningExtensionTest() {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse caBasicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient
                .GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            // Configure OCSP signing authority for the certificate in question
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, caBasicOCSPResp.GetResponses()[0], caBasicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME);
            new AssertValidationReport(report).HasNumberOfFailures(0).HasStatus(ValidationReport.ValidationResult.VALID
                ).DoAssert();
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOcspResponderDoesNotHaveOcspSigningExtensionTest() {
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertWithoutOcspSigning.pem";
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(TimeTestUtil.TEST_DATE_TIME.AddDays(1)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME);
            new AssertValidationReport(report).HasNumberOfFailures(1).HasLogItem((l) => l.GetCheckName().Equals(CertificateChainValidator
                .EXTENSIONS_CHECK) && l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator.EXTENSION_MISSING
                , OID.X509Extensions.EXTENDED_KEY_USAGE)), "OCSP_SIGNING extended key usage is missing").HasStatus(ValidationReport.ValidationResult
                .INDETERMINATE).DoAssert();
        }

        [NUnit.Framework.Test]
        public virtual void PositiveFreshnessPositiveTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate, checkDate.AddDays(-3), 5);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void PositiveFreshnessNegativeTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime thisUpdate = checkDate.AddDays(-3);
            ValidationReport report = ValidateTest(checkDate, thisUpdate, 2);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(OCSPValidator.FRESHNESS_CHECK, thisUpdate, checkDate
                , TimeSpan.FromDays(2)), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void NegativeFreshnessPositiveTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate, checkDate.AddDays(5), -3);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void NegativeFreshnessNegativeTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime thisUpdate = checkDate.AddDays(2);
            ValidationReport report = ValidateTest(checkDate, thisUpdate, -3);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(OCSPValidator.FRESHNESS_CHECK, thisUpdate, checkDate
                , TimeSpan.FromDays(-3)), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        private ValidationReport ValidateTest(DateTime checkDate) {
            return ValidateTest(checkDate, checkDate.AddDays(1), 0);
        }

        private ValidationReport ValidateTest(DateTime checkDate, DateTime thisUpdate, long freshness) {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(thisUpdate));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (freshness));
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                );
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
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                );
            return report;
        }

        private ValidationReport ValidateOcspWithoutCertsTest(bool addResponderToTrusted) {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetOcspCertsChain(new IX509Certificate[0]);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            if (addResponderToTrusted) {
                certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(responderCert));
            }
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME);
            return report;
        }

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
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            TestOcspResponseBuilder builder2 = revokedOcsp ? new TestOcspResponseBuilder(caCert, caPrivateKey, FACTORY
                .CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME.AddDays(-5), FACTORY.CreateCRLReason().GetKeyCompromise
                ())) : new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(20)));
            builder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(30)));
            TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder2);
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(5));
            if (revokedOcsp) {
                parameters.SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), false);
            }
            validatorChainBuilder.GetRevocationDataValidator().AddOcspClient(ocspClient);
            validatorChainBuilder.GetRevocationDataValidator().AddOcspClient(ocspClient2);
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                );
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
