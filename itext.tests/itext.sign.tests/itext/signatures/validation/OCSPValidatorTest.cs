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
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
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

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.REVOCATION_DATA_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private IssuingCertificateRetriever certificateRetriever;

        private SignatureValidationProperties parameters;

        private ValidatorChainBuilder validatorChainBuilder;

        private MockChainValidator mockCertificateChainValidator;

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
            mockCertificateChainValidator = new MockChainValidator();
            validatorChainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties(parameters).WithIssuingCertificateRetrieverFactory
                (() => certificateRetriever).WithCertificateChainValidatorFactory(() => mockCertificateChainValidator);
        }

        [NUnit.Framework.Test]
        public virtual void HappyPathTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
        }

        [NUnit.Framework.Test]
        public virtual void OcpsIssuerChainValidationsUsesCorrectParametersTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidateTest(checkDate);
            NUnit.Framework.Assert.AreEqual(1, mockCertificateChainValidator.verificationCalls.Count);
            NUnit.Framework.Assert.AreEqual(responderCert, mockCertificateChainValidator.verificationCalls[0].certificate
                );
            NUnit.Framework.Assert.AreEqual(ValidatorContext.OCSP_VALIDATOR, mockCertificateChainValidator.verificationCalls
                [0].context.GetValidatorContext());
            NUnit.Framework.Assert.AreEqual(CertificateSource.OCSP_ISSUER, mockCertificateChainValidator.verificationCalls
                [0].context.GetCertificateSource());
            NUnit.Framework.Assert.AreEqual(checkDate, mockCertificateChainValidator.verificationCalls[0].checkDate);
        }

        [NUnit.Framework.Test]
        public virtual void OcspForSelfSignedCertShouldNotValdateFurtherTest() {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse caBasicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient
                .GetEncoded(caCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, caCert, caBasicOCSPResp.GetResponses()[0], caBasicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                (1).HasLogItem((al) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(RevocationDataValidator.
                SELF_SIGNED_CERTIFICATE).WithCertificate(caCert)));
            NUnit.Framework.Assert.AreEqual(0, mockCertificateChainValidator.verificationCalls.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ValidationDateAfterNextUpdateTest() {
            // Same next update is set in the test OCSP builder.
            DateTime nextUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(30);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME.AddDays(45);
            ValidationReport report = ValidateTest(checkDate, TimeTestUtil.TEST_DATE_TIME, 50);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((al) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.OCSP_IS_NO_LONGER_VALID
                , (l) => checkDate, (l) => nextUpdate)));
        }

        [NUnit.Framework.Test]
        public virtual void SerialNumbersDoNotMatchTest() {
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
                , checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfLogs(1).HasStatus(ValidationReport.ValidationResult
                .INDETERMINATE).HasLogItem((al) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator
                .SERIAL_NUMBERS_DO_NOT_MATCH).WithCertificate(checkCert)));
            NUnit.Framework.Assert.AreEqual(0, mockCertificateChainValidator.verificationCalls.Count);
        }

        [NUnit.Framework.Test]
        public virtual void IssuersDoNotMatchTest() {
            String wrongRootCertFileName = SOURCE_FOLDER + "rootCertForOcspTest.pem";
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            OCSPValidatorTest.TestIssuingCertificateRetriever wrongRootCertificateRetriever = new OCSPValidatorTest.TestIssuingCertificateRetriever
                (wrongRootCertFileName);
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => wrongRootCertificateRetriever);
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((la
                ) => la.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.ISSUERS_DO_NOT_MATCH).WithStatus
                (ReportItem.ReportItemStatus.INDETERMINATE)));
        }

        [NUnit.Framework.Test]
        public virtual void PositiveFreshnessNegativeTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime thisUpdate = checkDate.AddDays(-3);
            ValidationReport report = ValidateTest(checkDate, thisUpdate, 2);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasLogItem((al) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator
                .FRESHNESS_CHECK, (l) => thisUpdate, (l) => checkDate, (l) => TimeSpan.FromDays(2))));
        }

        [NUnit.Framework.Test]
        public virtual void NextUpdateNotSetResultsInValidStatusTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-20)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar((DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE));
            builder.SetProducedAt(TimeTestUtil.TEST_DATE_TIME.AddDays(-20));
            TestOcspClient client = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(client.GetEncoded
                (checkCert, caCert, "")));
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.Singleton(caCert));
            ValidationReport report = new ValidationReport();
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                , checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateWasRevokedBeforeCheckDateShouldFailTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(-1);
            ValidationReport report = ValidateRevokedTestMocked(checkDate, revocationDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasLogItem
                ((al) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.CERT_IS_REVOKED).WithCertificate
                (checkCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateWasRevokedAfterCheckDateShouldSucceedTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(10);
            ValidationReport report = ValidateRevokedTestMocked(checkDate, revocationDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasLogItem((la) => la.WithCheckName(OCSPValidator.OCSP_CHECK
                ).WithMessage(SignLogMessageConstant.VALID_CERTIFICATE_IS_REVOKED, (l) => revocationDate)).HasStatus(ValidationReport.ValidationResult
                .VALID));
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
                , checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((al) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.CERT_STATUS_IS_UNKNOWN
                ).WithCertificate(checkCert)));
            NUnit.Framework.Assert.AreEqual(0, mockCertificateChainValidator.verificationCalls.Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcspIssuerCertificateDoesNotVerifyWithCaPKTest() {
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
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(1).HasStatus(ValidationReport.ValidationResult
                .INVALID).HasLogItem((al) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.INVALID_OCSP
                )
                        // This should be the checked certificate, not the ocsp responder
                        
                        //.withCertificate(checkCert)
                        .WithCertificate(responderCert)));
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
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasLogItem((la) => la.WithCheckName(OCSPValidator.OCSP_CHECK
                ).WithMessage(OCSPValidator.OCSP_COULD_NOT_BE_VERIFIED)).HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ChainValidatorReportWrappingTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            mockCertificateChainValidator.OnCallDo((c) => {
                c.report.AddReportItem(new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO));
                c.report.AddReportItem(new ReportItem("test2", "test2", ReportItem.ReportItemStatus.INDETERMINATE));
                c.report.AddReportItem(new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INVALID));
            }
            );
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItems(0, 0, (la) => la.WithStatus(ReportItem.ReportItemStatus.INVALID)).HasLogItems(2, 2, (la) =>
                 la.WithStatus(ReportItem.ReportItemStatus.INDETERMINATE)).HasLogItem((la) => la.WithStatus(ReportItem.ReportItemStatus
                .INFO)));
        }

        [NUnit.Framework.Test]
        public virtual void NoArchiveCutoffExtensionTest() {
            DateTime producedAt = checkCert.GetNotAfter().AddDays(5);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(producedAt);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(OCSPValidator.OCSP_CHECK
                ).WithMessage(OCSPValidator.CERT_IS_EXPIRED, (i) => checkCert.GetNotAfter()).WithCertificate(checkCert
                )));
        }

        [NUnit.Framework.Test]
        public virtual void NoArchiveCutoffExtensionButRevokedStatusTest() {
            DateTime producedAt = checkCert.GetNotAfter().AddDays(5);
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(5);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(producedAt);
            builder.SetCertificateStatus(FACTORY.CreateRevokedStatus(revocationDate, FACTORY.CreateCRLReason().GetKeyCompromise
                ()));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(SignLogMessageConstant
                .VALID_CERTIFICATE_IS_REVOKED, (i) => revocationDate).WithCertificate(checkCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CertExpiredBeforeArchiveCutoffDateTest() {
            DateTime producedAt = checkCert.GetNotAfter().AddDays(5);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(producedAt);
            builder.AddResponseExtension(FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspArchiveCutoff(), FACTORY.CreateDEROctetString
                (FACTORY.CreateASN1GeneralizedTime(producedAt.AddDays(-3)).GetEncoded()));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(OCSPValidator.OCSP_CHECK
                ).WithMessage(OCSPValidator.CERT_IS_EXPIRED, (i) => checkCert.GetNotAfter()).WithCertificate(checkCert
                )));
        }

        [NUnit.Framework.Test]
        public virtual void CertExpiredAfterArchiveCutoffDateTest() {
            DateTime producedAt = checkCert.GetNotAfter().AddDays(5);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(producedAt);
            builder.AddResponseExtension(FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspArchiveCutoff(), FACTORY.CreateDEROctetString
                (FACTORY.CreateASN1GeneralizedTime(producedAt.AddDays(-10)).GetEncoded()));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(0));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverRetrieveIssuerCertificateFailureTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever();
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever);
            mockCertificateRetriever.OnRetrieveIssuerCertificateDo((c) => {
                throw new Exception("Test retrieveMissingCertificates failure");
            }
            );
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(OCSPValidator.UNABLE_TO_RETRIEVE_ISSUER)));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverRetrieveOCSPResponderCertificateFailureTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever(certificateRetriever
                );
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever);
            mockCertificateRetriever.OnRetrieveOCSPResponderCertificateDo((c) => {
                throw new Exception("Test retrieveMissingCertificates failure");
            }
            );
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(OCSPValidator.OCSP_RESPONDER_NOT_RETRIEVED)));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverIsCertificateTrustedFailureTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever(certificateRetriever
                );
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever);
            mockCertificateRetriever.OnIsCertificateTrustedDo((c) => {
                throw new Exception("Test isCertificateTrusted failure");
            }
            );
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(OCSPValidator.OCSP_RESPONDER_TRUST_NOT_RETRIEVED)));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverIsCertificateTrustedForOcspFailureTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever(certificateRetriever
                );
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever);
            mockCertificateRetriever.OnIsCertificateTrustedDo((c) => false);
            MockTrustedCertificatesStore mockTrustedStore = new MockTrustedCertificatesStore(certificateRetriever.GetTrustedCertificatesStore
                ());
            mockCertificateRetriever.OnGetTrustedCertificatesStoreDo(() => mockTrustedStore);
            mockTrustedStore.OnIsCertificateTrustedForOcspDo((c) => {
                throw new Exception("Test isCertificateTrustedForOcsp failure");
            }
            );
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(OCSPValidator.OCSP_RESPONDER_TRUST_NOT_RETRIEVED)));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateChainValidationFailureTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            mockCertificateChainValidator.OnCallDo((c) => {
                throw new Exception("Test chain validation failure");
            }
            );
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(OCSPValidator.OCSP_RESPONDER_NOT_VERIFIED)));
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
                , checkDate);
            return report;
        }

        private ValidationReport ValidateRevokedTestMocked(DateTime checkDate, DateTime revocationDate) {
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
                , checkDate);
            return report;
        }

        private class TestIssuingCertificateRetriever : IssuingCertificateRetriever {
//\cond DO_NOT_DOCUMENT
            internal IX509Certificate issuerCertificate;
//\endcond

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
