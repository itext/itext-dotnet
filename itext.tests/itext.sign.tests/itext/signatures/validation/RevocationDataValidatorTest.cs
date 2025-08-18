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
using System.Threading;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class RevocationDataValidatorTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/RevocationDataValidatorTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        private static IX509Certificate trustedOcspResponderCert;

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

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
            trustedOcspResponderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)[0];
        }

        private ValidatorChainBuilder CreateValidatorChainBuilder(IssuingCertificateRetriever certificateRetriever
            , MockSignatureValidationProperties mockParameters, MockCrlValidator mockCrlValidator, MockOCSPValidator
             mockOCSPValidator) {
            return new ValidatorChainBuilder().WithIssuingCertificateRetrieverFactory(() => certificateRetriever).WithSignatureValidationProperties
                (mockParameters).WithCRLValidatorFactory(() => mockCrlValidator).WithOCSPValidatorFactory(() => mockOCSPValidator
                );
        }

        [NUnit.Framework.Test]
        public virtual void BasicOCSPValidatorUsageTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(5));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClientWrapper ocspClient = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer(
                caCert, builder));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddFreshnessResponse(TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddOcspClient(ocspClient);
            ReportItem reportItem = new ReportItem("validator", "message", ReportItem.ReportItemStatus.INFO);
            mockOCSPValidator.OnCallDo((c) => c.report.AddReportItem(reportItem));
            validator.Validate(report, baseContext, checkCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID)
                        // the logitem from the OCSP valdiation should be copied to the final report
                        .HasNumberOfLogs(1).HasLogItem(reportItem));
            // there should be one call per ocspClient
            NUnit.Framework.Assert.AreEqual(1, ocspClient.GetCalls().Count);
            // There was only one ocsp response so we expect 1 call to the ocsp validator
            NUnit.Framework.Assert.AreEqual(1, mockOCSPValidator.calls.Count);
            // the validationDate should be passed as is
            NUnit.Framework.Assert.AreEqual(checkDate, mockOCSPValidator.calls[0].validationDate);
            // the response should be passed as is
            NUnit.Framework.Assert.AreEqual(ocspClient.GetCalls()[0].response, mockOCSPValidator.calls[0].ocspResp);
            // There should be a new report generated and any logs must be copied the actual report.
            NUnit.Framework.Assert.AreNotEqual(report, mockOCSPValidator.calls[0].report);
        }

        [NUnit.Framework.Test]
        public virtual void BasicCrlValidatorUsageTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = checkDate.AddDays(-1);
            TestCrlBuilder builder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            builder.SetNextUpdate(checkDate.AddDays(10));
            builder.AddCrlEntry(checkCert, revocationDate, FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlClientWrapper crlClient = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder
                ));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddFreshnessResponse(TimeSpan.FromDays(0));
            ReportItem reportItem = new ReportItem("validator", "message", ReportItem.ReportItemStatus.INFO);
            mockCrlValidator.OnCallDo((c) => c.report.AddReportItem(reportItem));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient
                );
            validator.Validate(report, baseContext, checkCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(0)
                        // the logitem from the CRL valdiation should be copied to the final report
                        .HasNumberOfLogs(1).HasLogItem(reportItem));
            // there should be one call per CrlClient
            NUnit.Framework.Assert.AreEqual(1, crlClient.GetCalls().Count);
            // since there was one response there should be one validator call
            NUnit.Framework.Assert.AreEqual(1, mockCrlValidator.calls.Count);
            NUnit.Framework.Assert.AreEqual(checkCert, mockCrlValidator.calls[0].certificate);
            NUnit.Framework.Assert.AreEqual(checkDate, mockCrlValidator.calls[0].validationDate);
            // There should be a new report generated and any logs must be copied the actual report.
            NUnit.Framework.Assert.AreNotEqual(report, mockCrlValidator.calls[0].report);
            NUnit.Framework.Assert.AreEqual(crlClient.GetCalls()[0].responses[0], mockCrlValidator.calls[0].crl);
        }

        [NUnit.Framework.Test]
        public virtual void CrlResponseOrderingTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime thisUpdate1 = checkDate.AddDays(-2);
            TestCrlBuilder builder1 = new TestCrlBuilder(caCert, caPrivateKey, thisUpdate1);
            builder1.SetNextUpdate(checkDate.AddDays(-2));
            TestCrlClientWrapper crlClient1 = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder1
                ));
            TestCrlBuilder builder2 = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            builder2.SetNextUpdate(checkDate);
            TestCrlClientWrapper crlClient2 = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder2
                ));
            DateTime thisUpdate3 = checkDate.AddDays(+2);
            TestCrlBuilder builder3 = new TestCrlBuilder(caCert, caPrivateKey, thisUpdate3);
            builder3.SetNextUpdate(checkDate.AddDays(-2));
            TestCrlClientWrapper crlClient3 = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder3
                ));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient1
                ).AddCrlClient(crlClient2).AddCrlClient(crlClient3);
            mockCrlValidator.OnCallDo((c) => c.report.AddReportItem(new ReportItem("test", "test", ReportItem.ReportItemStatus
                .INDETERMINATE)));
            ValidationReport report = new ValidationReport();
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(crlClient3.GetCalls()[0].responses[0], mockCrlValidator.calls[0].crl);
            NUnit.Framework.Assert.AreEqual(crlClient2.GetCalls()[0].responses[0], mockCrlValidator.calls[1].crl);
            NUnit.Framework.Assert.AreEqual(crlClient1.GetCalls()[0].responses[0], mockCrlValidator.calls[2].crl);
        }

        [NUnit.Framework.Test]
        public virtual void OcspResponseOrderingTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder1 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder1.SetProducedAt(checkDate);
            builder1.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate));
            builder1.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            TestOcspClientWrapper ocspClient1 = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer
                (caCert, builder1));
            TestOcspResponseBuilder builder2 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder2.SetProducedAt(checkDate.AddDays(5));
            builder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClientWrapper ocspClient2 = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer
                (caCert, builder2));
            TestOcspResponseBuilder builder3 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder3.SetProducedAt(checkDate.AddDays(2));
            builder3.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(2)));
            builder3.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(8)));
            TestOcspClientWrapper ocspClient3 = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer
                (caCert, builder3));
            mockOCSPValidator.OnCallDo((c) => c.report.AddReportItem(new ReportItem("", "", ReportItem.ReportItemStatus
                .INDETERMINATE)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                ).AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH).AddRevocationOnlineFetchingResponse
                (SignatureValidationProperties.OnlineFetching.NEVER_FETCH).AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching
                .NEVER_FETCH).AddFreshnessResponse(TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddOcspClient(ocspClient1
                ).AddOcspClient(ocspClient2).AddOcspClient(ocspClient3);
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(ocspClient2.GetCalls()[0].response, mockOCSPValidator.calls[0].ocspResp);
            NUnit.Framework.Assert.AreEqual(ocspClient3.GetCalls()[0].response, mockOCSPValidator.calls[1].ocspResp);
            NUnit.Framework.Assert.AreEqual(ocspClient1.GetCalls()[0].response, mockOCSPValidator.calls[2].ocspResp);
        }

        [NUnit.Framework.Test]
        public virtual void ValidityAssuredTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            String checkCertFileName = SOURCE_FOLDER + "validityAssuredSigningCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, certificate, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItem
                ((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .VALIDITY_ASSURED).WithCertificate(certificate)));
        }

        [NUnit.Framework.Test]
        public virtual void NoRevAvailTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            String checkCertFileName = SOURCE_FOLDER + "noRevAvailCertWithoutCA.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, certificate, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItem
                ((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REV_AVAILABLE, (m) => certificate.GetSubjectDN()).WithCertificate(certificate)));
        }

        [NUnit.Framework.Test]
        public virtual void NoRevAvailWithCATest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            String checkCertFileName = SOURCE_FOLDER + "noRevAvailCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, certificate, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REV_AVAILABLE_CA, (m) => certificate.GetSubjectDN()).WithCertificate(certificate)));
        }

        [NUnit.Framework.Test]
        public virtual void SelfSignedCertificateIsNotValidatedTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, caCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItem
                ((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .SELF_SIGNED_CERTIFICATE).WithCertificate(caCert)));
        }

        [NUnit.Framework.Test]
        public virtual void NocheckExtensionShouldNotFurtherValidateTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH);
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext.SetCertificateSource(CertificateSource.OCSP_ISSUER), trustedOcspResponderCert
                , TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasLogItem((la) => la.WithStatus(ReportItem.ReportItemStatus
                .INFO).WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .TRUSTED_OCSP_RESPONDER)));
        }

        [NUnit.Framework.Test]
        public virtual void NoRevocationDataTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasLogItem((la) => la.WithStatus(ReportItem.ReportItemStatus
                .INDETERMINATE).WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REVOCATION_DATA)));
        }

        [NUnit.Framework.Test]
        public virtual void DoNotFetchOcspOnlineIfCrlAvailableTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime thisUpdate = checkDate.AddDays(-2);
            TestCrlBuilder builder = new TestCrlBuilder(caCert, caPrivateKey, thisUpdate);
            builder.SetNextUpdate(checkDate.AddDays(2));
            TestCrlClientWrapper crlClient = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder
                ));
            mockOCSPValidator.OnCallDo((c) => c.report.AddReportItem(new ReportItem("", "", ReportItem.ReportItemStatus
                .INDETERMINATE)));
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.FETCH_IF_NO_OTHER_DATA_AVAILABLE).SetFreshness(ValidatorContexts
                .All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient
                );
            ValidationReport report = new ValidationReport();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(0));
        }

        [NUnit.Framework.Test]
        public virtual void DoNotFetchCrlOnlineIfOcspAvailableTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            TestOcspClientWrapper ocspClient = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer(
                caCert, builder));
            mockOCSPValidator.OnCallDo((c) => c.report.AddReportItem(new ReportItem("", "", ReportItem.ReportItemStatus
                .INFO)));
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.FETCH_IF_NO_OTHER_DATA_AVAILABLE).SetFreshness(ValidatorContexts
                .All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddOcspClient(ocspClient
                );
            ValidationReport report = new ValidationReport();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1));
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,O=iText,CN=iTextTestSignRsa", LogLevel = LogLevelConstants
            .INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO)]
        public virtual void TryToFetchCrlOnlineIfOnlyIndeterminateOcspAvailableTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            TestOcspClientWrapper ocspClient = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer(
                caCert, builder));
            mockOCSPValidator.OnCallDo((c) => c.report.AddReportItem(new ReportItem("", "", ReportItem.ReportItemStatus
                .INDETERMINATE)));
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.Of(ValidatorContext.CRL_VALIDATOR), CertificateSources
                .All(), TimeBasedContexts.All(), SignatureValidationProperties.OnlineFetching.FETCH_IF_NO_OTHER_DATA_AVAILABLE
                ).SetRevocationOnlineFetching(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR), CertificateSources
                .All(), TimeBasedContexts.All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness
                (ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddOcspClient(ocspClient
                );
            ValidationReport report = new ValidationReport();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasLogItem((la) => la.WithStatus(ReportItem.ReportItemStatus
                .INDETERMINATE).WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REVOCATION_DATA)));
        }

        [NUnit.Framework.Test]
        public virtual void TryFetchRevocationDataOnlineTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.ALWAYS_FETCH).SetFreshness(ValidatorContexts.All(
                ), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REVOCATION_DATA)));
        }

        [NUnit.Framework.Test]
        public virtual void CrlEncodingErrorTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            byte[] crl = new TestCrlBuilder(caCert, caPrivateKey).MakeCrl();
            crl[5] = 0;
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddCrlClient(new _ICrlClient_674(crl)).Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(MessageFormatUtil
                .Format(RevocationDataValidator.CANNOT_PARSE_CRL, "Test crl client."))).HasLogItem((la) => la.WithCheckName
                (RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator.NO_REVOCATION_DATA
                )));
        }

        private sealed class _ICrlClient_674 : ICrlClient {
            public _ICrlClient_674(byte[] crl) {
                this.crl = crl;
            }

            public ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
                return JavaCollectionsUtil.SingletonList(crl);
            }

            public override String ToString() {
                return "Test crl client.";
            }

            private readonly byte[] crl;
        }

        [NUnit.Framework.Test]
        public virtual void SortResponsesTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            // The oldest one, but the only one valid.
            TestOcspResponseBuilder ocspBuilder1 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            ocspBuilder1.SetProducedAt(checkDate);
            ocspBuilder1.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate));
            ocspBuilder1.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(3)));
            TestOcspClientWrapper ocspClient1 = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer
                (caCert, ocspBuilder1));
            TestOcspResponseBuilder ocspBuilder2 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            ocspBuilder2.SetProducedAt(checkDate.AddDays(3));
            ocspBuilder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(3)));
            ocspBuilder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            ocspBuilder2.SetCertificateStatus(FACTORY.CreateUnknownStatus());
            TestOcspClientWrapper ocspClient2 = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer
                (caCert, ocspBuilder2));
            TestOcspResponseBuilder ocspBuilder3 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            ocspBuilder3.SetProducedAt(checkDate.AddDays(5));
            ocspBuilder3.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            ocspBuilder3.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            ocspBuilder3.SetCertificateStatus(FACTORY.CreateUnknownStatus());
            TestOcspClientWrapper ocspClient3 = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer
                (caCert, ocspBuilder3));
            TestCrlBuilder crlBuilder1 = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            crlBuilder1.SetNextUpdate(checkDate.AddDays(2));
            TestCrlBuilder crlBuilder2 = new TestCrlBuilder(caCert, caPrivateKey, checkDate.AddDays(2));
            crlBuilder2.SetNextUpdate(checkDate.AddDays(5));
            TestCrlClientWrapper crlClient = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(crlBuilder1
                ).AddBuilderForCertIssuer(crlBuilder2));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.Of(ValidatorContext
                .CRL_VALIDATOR), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-5));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient
                ).AddOcspClient(ocspClient1).AddOcspClient(ocspClient2).AddOcspClient(ocspClient3);
            mockCrlValidator.OnCallDo((c) => {
                c.report.AddReportItem(new ReportItem("1", "2", ReportItem.ReportItemStatus.INDETERMINATE));
                try {
                    Thread.Sleep(10);
                }
                catch (ThreadInterruptedException) {
                }
            }
            );
            mockOCSPValidator.OnCallDo((c) => {
                c.report.AddReportItem(new ReportItem("1", "2", ReportItem.ReportItemStatus.INDETERMINATE));
                try {
                    Thread.Sleep(10);
                }
                catch (ThreadInterruptedException) {
                }
            }
            );
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.IsTrue(mockOCSPValidator.calls[0].timeStamp.Before(mockOCSPValidator.calls[1].timeStamp
                ));
            NUnit.Framework.Assert.IsTrue(mockOCSPValidator.calls[1].timeStamp.Before(mockCrlValidator.calls[0].timeStamp
                ));
            NUnit.Framework.Assert.IsTrue(mockCrlValidator.calls[0].timeStamp.Before(mockCrlValidator.calls[1].timeStamp
                ));
            NUnit.Framework.Assert.IsTrue(mockCrlValidator.calls[1].timeStamp.Before(mockOCSPValidator.calls[2].timeStamp
                ));
            NUnit.Framework.Assert.AreEqual(ocspClient1.GetCalls()[0].response, mockOCSPValidator.calls[2].ocspResp);
            NUnit.Framework.Assert.AreEqual(ocspClient2.GetCalls()[0].response, mockOCSPValidator.calls[1].ocspResp);
            NUnit.Framework.Assert.AreEqual(ocspClient3.GetCalls()[0].response, mockOCSPValidator.calls[0].ocspResp);
            NUnit.Framework.Assert.AreEqual(crlClient.GetCalls()[0].responses[0], mockCrlValidator.calls[1].crl);
            NUnit.Framework.Assert.AreEqual(crlClient.GetCalls()[0].responses[1], mockCrlValidator.calls[0].crl);
        }

        [NUnit.Framework.Test]
        public virtual void ResponsesFromValidationClientArePassedTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime ocspGeneration = checkDate.AddDays(2);
            // Here we check that proper generation time was set.
            mockOCSPValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(ocspGeneration, c.responseGenerationDate
                ));
            DateTime crlGeneration = checkDate.AddDays(3);
            // Here we check that proper generation time was set.
            mockCrlValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(crlGeneration, c.responseGenerationDate));
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.GetRevocationDataValidator();
            ValidationOcspClient ocspClient = new _ValidationOcspClient_809();
            TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            byte[] ocspResponseBytes = new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder).GetEncoded(checkCert
                , caCert, null);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspResponseBytes
                ));
            ocspClient.AddResponse(basicOCSPResp, ocspGeneration, TimeBasedContext.HISTORICAL);
            validator.AddOcspClient(ocspClient);
            ValidationCrlClient crlClient = new _ValidationCrlClient_824();
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            byte[] crlResponseBytes = new List<byte[]>(new TestCrlClient().AddBuilderForCertIssuer(crlBuilder).GetEncoded
                (checkCert, null))[0];
            crlClient.AddCrl((IX509Crl)CertificateUtil.ParseCrlFromBytes(crlResponseBytes), crlGeneration, TimeBasedContext
                .HISTORICAL);
            validator.AddCrlClient(crlClient);
            validator.Validate(report, baseContext, checkCert, checkDate);
        }

        private sealed class _ValidationOcspClient_809 : ValidationOcspClient {
            public _ValidationOcspClient_809() {
            }

            public override byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate issuerCert, String url) {
                NUnit.Framework.Assert.Fail("This method shall not be called");
                return null;
            }
        }

        private sealed class _ValidationCrlClient_824 : ValidationCrlClient {
            public _ValidationCrlClient_824() {
            }

            public override ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
                NUnit.Framework.Assert.Fail("This method shall not be called");
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TimeBasedContextProperlySetValidationClientsTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            mockOCSPValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(TimeBasedContext.HISTORICAL, c.context.GetTimeBasedContext
                ()));
            mockCrlValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(TimeBasedContext.HISTORICAL, c.context.GetTimeBasedContext
                ()));
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.GetRevocationDataValidator();
            ValidationOcspClient ocspClient = new ValidationOcspClient();
            TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            byte[] ocspResponseBytes = new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder).GetEncoded(checkCert
                , caCert, null);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspResponseBytes
                ));
            ocspClient.AddResponse(basicOCSPResp, checkDate, TimeBasedContext.HISTORICAL);
            validator.AddOcspClient(ocspClient);
            ValidationCrlClient crlClient = new ValidationCrlClient();
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            byte[] crlResponseBytes = new List<byte[]>(new TestCrlClient().AddBuilderForCertIssuer(crlBuilder).GetEncoded
                (checkCert, null))[0];
            crlClient.AddCrl((IX509Crl)CertificateUtil.ParseCrlFromBytes(crlResponseBytes), checkDate, TimeBasedContext
                .HISTORICAL);
            validator.AddCrlClient(crlClient);
            validator.Validate(report, baseContext, checkCert, checkDate);
        }

        [NUnit.Framework.Test]
        public virtual void TimeBasedContextProperlySetRandomClientsTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockOCSPValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(TimeBasedContext.PRESENT, c.context.GetTimeBasedContext
                ()));
            mockCrlValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(TimeBasedContext.PRESENT, c.context.GetTimeBasedContext
                ()));
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.GetRevocationDataValidator();
            TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            validator.AddOcspClient(new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder));
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            validator.AddCrlClient(new TestCrlClient().AddBuilderForCertIssuer(crlBuilder));
            validator.Validate(report, baseContext.SetTimeBasedContext(TimeBasedContext.HISTORICAL), checkCert, checkDate
                );
        }

        [NUnit.Framework.Test]
        public virtual void TimeBasedContextProperlySetOnlineClientsTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockOCSPValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(TimeBasedContext.PRESENT, c.context.GetTimeBasedContext
                ()));
            mockCrlValidator.OnCallDo((c) => NUnit.Framework.Assert.AreEqual(TimeBasedContext.PRESENT, c.context.GetTimeBasedContext
                ()));
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.GetRevocationDataValidator();
            TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            TestOcspClient testOcspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder);
            OcspClientBouncyCastle ocspClient = new _OcspClientBouncyCastle_927(testOcspClient);
            validator.AddOcspClient(ocspClient);
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            TestCrlClient testCrlClient = new TestCrlClient().AddBuilderForCertIssuer(crlBuilder);
            CrlClientOnline crlClient = new _CrlClientOnline_937(testCrlClient);
            validator.AddCrlClient(crlClient);
            validator.Validate(report, baseContext.SetTimeBasedContext(TimeBasedContext.HISTORICAL), checkCert, checkDate
                );
        }

        private sealed class _OcspClientBouncyCastle_927 : OcspClientBouncyCastle {
            public _OcspClientBouncyCastle_927(TestOcspClient testOcspClient) {
                this.testOcspClient = testOcspClient;
            }

            public override byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate rootCert, String url) {
                return testOcspClient.GetEncoded(checkCert, rootCert, url);
            }

            private readonly TestOcspClient testOcspClient;
        }

        private sealed class _CrlClientOnline_937 : CrlClientOnline {
            public _CrlClientOnline_937(TestCrlClient testCrlClient) {
                this.testCrlClient = testCrlClient;
            }

            public override ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
                return testCrlClient.GetEncoded(checkCert, url);
            }

            private readonly TestCrlClient testCrlClient;
        }

        [NUnit.Framework.Test]
        public virtual void BasicOCSPValidatorFailureTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(5));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClientWrapper ocspClient = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer(
                caCert, builder));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddFreshnessResponse(TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddOcspClient(ocspClient);
            mockOCSPValidator.OnCallDo((c) => {
                throw new Exception("Test OCSP client failure");
            }
            );
            validator.Validate(report, baseContext, checkCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID)
                        // the logitem from the OCSP valdiation should be copied to the final report
                        .HasLogItem((l) => l.WithMessage(RevocationDataValidator.OCSP_VALIDATOR_FAILURE)));
        }

        [NUnit.Framework.Test]
        public virtual void OCSPValidatorFailureTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = checkDate.AddDays(-1);
            TestCrlBuilder builder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            builder.SetNextUpdate(checkDate.AddDays(10));
            builder.AddCrlEntry(checkCert, revocationDate, FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlClientWrapper crlClient = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder
                ));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddFreshnessResponse(TimeSpan.FromDays(0));
            mockCrlValidator.OnCallDo((c) => {
                throw new Exception("Test OCSP client failure");
            }
            );
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient
                );
            validator.Validate(report, baseContext, checkCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID)
                        // the logitem from the OCSP valdiation should be copied to the final report
                        .HasLogItem((l) => l.WithMessage(RevocationDataValidator.CRL_VALIDATOR_FAILURE)));
        }

        //certificateRetriever.retrieveIssuerCertificate
        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverRetrieveIssuerCertificateFailureTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(5));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClientWrapper ocspClient = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer(
                caCert, builder));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddFreshnessResponse(TimeSpan.FromDays(-2));
            MockIssuingCertificateRetriever mockCertificateRetreiver = new MockIssuingCertificateRetriever(certificateRetriever
                ).OnRetrieveIssuerCertificateDo((c) => {
                throw new Exception("Test retrieveIssuerCertificate failure");
            }
            );
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetreiver);
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddOcspClient(ocspClient);
            ReportItem reportItem = new ReportItem("validator", "message", ReportItem.ReportItemStatus.INFO);
            mockOCSPValidator.OnCallDo((c) => c.report.AddReportItem(reportItem));
            validator.Validate(report, baseContext, checkCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(RevocationDataValidator.UNABLE_TO_RETRIEVE_REV_DATA_ONLINE)));
        }

        [NUnit.Framework.Test]
        public virtual void OcspClientGetEncodedFailureTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(5));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClientWrapper ocspClient = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer(
                caCert, builder));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddFreshnessResponse(TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddOcspClient(ocspClient);
            ReportItem reportItem = new ReportItem("validator", "message", ReportItem.ReportItemStatus.INFO);
            mockOCSPValidator.OnCallDo((c) => c.report.AddReportItem(reportItem));
            ocspClient.OnGetEncodedDo((c) => {
                throw new Exception("Test onGetEncoded failure");
            }
            );
            validator.Validate(report, baseContext, checkCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(RevocationDataValidator.OCSP_CLIENT_FAILURE, (p) => ocspClient)));
        }

        [NUnit.Framework.Test]
        public virtual void CrlClientGetEncodedFailureTest() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = checkDate.AddDays(-1);
            TestCrlBuilder builder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            builder.SetNextUpdate(checkDate.AddDays(10));
            builder.AddCrlEntry(checkCert, revocationDate, FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlClientWrapper crlClient = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder
                ));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.NEVER_FETCH
                );
            mockParameters.AddFreshnessResponse(TimeSpan.FromDays(0));
            ReportItem reportItem = new ReportItem("validator", "message", ReportItem.ReportItemStatus.INFO);
            mockCrlValidator.OnCallDo((c) => c.report.AddReportItem(reportItem));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient
                );
            crlClient.OnGetEncodedDo((c) => {
                throw new Exception("Test getEncoded failure");
            }
            );
            validator.Validate(report, baseContext, checkCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((l) => l.WithMessage(RevocationDataValidator.CRL_CLIENT_FAILURE, (p) => crlClient.ToString
                ())));
        }

        [NUnit.Framework.Test]
        public virtual void TestCrlClientInjection() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            TestCrlClient testCrlClient = new TestCrlClient();
            TestCrlClientWrapper mockCrlClient = new TestCrlClientWrapper(testCrlClient);
            validatorChainBuilder.WithCrlClient(() => mockCrlClient);
            testCrlClient.AddBuilderForCertIssuer(caCert, caPrivateKey);
            ValidationReport report = new ValidationReport();
            ValidationContext context = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource
                .SIGNER_CERT, TimeBasedContext.HISTORICAL);
            validatorChainBuilder.BuildRevocationDataValidator().Validate(report, context, checkCert, TimeTestUtil.TEST_DATE_TIME
                );
            NUnit.Framework.Assert.AreEqual(1, mockCrlClient.GetCalls().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestOcspClientInjection() {
            IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            MockCrlValidator mockCrlValidator = new MockCrlValidator();
            MockOCSPValidator mockOCSPValidator = new MockOCSPValidator();
            MockSignatureValidationProperties mockParameters = new MockSignatureValidationProperties(parameters);
            ValidatorChainBuilder validatorChainBuilder = CreateValidatorChainBuilder(certificateRetriever, mockParameters
                , mockCrlValidator, mockOCSPValidator);
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(5));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClientWrapper mockOcspClient = new TestOcspClientWrapper(new TestOcspClient().AddBuilderForCertIssuer
                (caCert, builder));
            validatorChainBuilder.WithOcspClient(() => mockOcspClient);
            mockParameters.AddRevocationOnlineFetchingResponse(SignatureValidationProperties.OnlineFetching.ALWAYS_FETCH
                );
            certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(caCert, trustedOcspResponderCert));
            ValidationReport report = new ValidationReport();
            ValidationContext context = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, CertificateSource
                .SIGNER_CERT, TimeBasedContext.HISTORICAL);
            validatorChainBuilder.BuildRevocationDataValidator().Validate(report, context, checkCert, TimeTestUtil.TEST_DATE_TIME
                );
            NUnit.Framework.Assert.AreEqual(2, mockOcspClient.GetBasicResponceCalls().Count);
        }
    }
}
