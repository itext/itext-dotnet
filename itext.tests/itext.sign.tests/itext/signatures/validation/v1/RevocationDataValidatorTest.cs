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
using System.Threading;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Mocks;
using iText.Signatures.Validation.V1.Report;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class RevocationDataValidatorTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/RevocationDataValidatorTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        private static IX509Certificate trustedOcspResponderCert;

        private IssuingCertificateRetriever certificateRetriever;

        private SignatureValidationProperties parameters;

        private ValidationContext baseContext = new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR, CertificateSource
            .SIGNER_CERT, TimeBasedContext.PRESENT);

        private ValidatorChainBuilder validatorChainBuilder;

        private MockCrlValidator mockCrlValidator;

        private MockOCSPValidator mockOCSPValidator;

        private MockSignatureValidationProperties mockParameters;

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

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            certificateRetriever = new IssuingCertificateRetriever();
            parameters = new SignatureValidationProperties();
            mockCrlValidator = new MockCrlValidator();
            mockOCSPValidator = new MockOCSPValidator();
            mockParameters = new MockSignatureValidationProperties(parameters);
            validatorChainBuilder = new ValidatorChainBuilder().WithIssuingCertificateRetriever(certificateRetriever).
                WithSignatureValidationProperties(mockParameters).WithCRLValidator(mockCrlValidator).WithOCSPValidator
                (mockOCSPValidator);
        }

        [NUnit.Framework.Test]
        public virtual void BasicOCSPValidatorUsageTest() {
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
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime thisUpdate1 = checkDate.AddDays(-2);
            TestCrlBuilder builder1 = new TestCrlBuilder(caCert, caPrivateKey, thisUpdate1);
            builder1.SetNextUpdate(checkDate.AddDays(-2));
            TestCrlClientWrapper crlClient1 = new TestCrlClientWrapper(new TestCrlClient().AddBuilderForCertIssuer(builder1
                ));
            DateTime thisUpdate2 = checkDate;
            TestCrlBuilder builder2 = new TestCrlBuilder(caCert, caPrivateKey, thisUpdate2);
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
                ).AddFreshnessResponse(TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddOcspClient(ocspClient1
                ).AddOcspClient(ocspClient2).AddOcspClient(ocspClient3);
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(ocspClient2.GetCalls()[0].response, mockOCSPValidator.calls[0].ocspResp);
            NUnit.Framework.Assert.AreEqual(ocspClient3.GetCalls()[0].response, mockOCSPValidator.calls[1].ocspResp);
            NUnit.Framework.Assert.AreEqual(ocspClient1.GetCalls()[0].response, mockOCSPValidator.calls[2].ocspResp);
        }

        [NUnit.Framework.Test]
        public virtual void ValidityAssuredTest() {
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
        public virtual void SelfSignedCertificateIsNotValidatedTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, caCert, checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItem
                ((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .SELF_SIGNED_CERTIFICATE).WithCertificate(caCert)));
        }

        [NUnit.Framework.Test]
        public virtual void NocheckExtensionShouldNotFurtherValdiateTest() {
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
        public virtual void TryFetchRevocationDataOnlineTest() {
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.FETCH_IF_NO_OTHER_DATA_AVAILABLE).SetFreshness(ValidatorContexts
                .All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REVOCATION_DATA)));
        }

        [NUnit.Framework.Test]
        public virtual void CrlEncodingErrorTest() {
            byte[] crl = new TestCrlBuilder(caCert, caPrivateKey).MakeCrl();
            crl[5] = 0;
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddCrlClient(new _ICrlClient_401(crl)).Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .CRL_PARSING_ERROR)).HasLogItem((la) => la.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK
                ).WithMessage(RevocationDataValidator.NO_REVOCATION_DATA)));
        }

        private sealed class _ICrlClient_401 : ICrlClient {
            public _ICrlClient_401(byte[] crl) {
                this.crl = crl;
            }

            public ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
                return JavaCollectionsUtil.SingletonList(crl);
            }

            private readonly byte[] crl;
        }

        [NUnit.Framework.Test]
        public virtual void SortResponsesTest() {
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
    }
}
