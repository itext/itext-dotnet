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
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class SignatureValidatorTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/SignatureValidatorTest/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/SignatureValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private SignatureValidationProperties parameters;

        private MockIssuingCertificateRetriever mockCertificateRetriever;

        private ValidatorChainBuilder builder;

        private MockChainValidator mockCertificateChainValidator;

        private MockDocumentRevisionsValidator mockDocumentRevisionsValidator;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            mockCertificateChainValidator = new MockChainValidator();
            parameters = new SignatureValidationProperties();
            mockCertificateRetriever = new MockIssuingCertificateRetriever();
            mockDocumentRevisionsValidator = new MockDocumentRevisionsValidator();
            builder = new ValidatorChainBuilder().WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever
                ).WithSignatureValidationProperties(parameters).WithCertificateChainValidatorFactory(() => mockCertificateChainValidator
                ).WithRevocationDataValidatorFactory(() => new MockRevocationDataValidator()).WithDocumentRevisionsValidatorFactory
                (() => mockDocumentRevisionsValidator);
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureIsTimestampTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            String privateKeyName = CERTS_SRC + "rootCertKey.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(privateKeyName, PASSWORD);
            IX509Certificate timeStampCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "timestamp.pem"
                )[0];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timestampSignatureDoc.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
                DateTime currentDate = DateTimeUtil.GetCurrentUtcTime();
                ocspBuilder.SetProducedAt(currentDate);
                ocspBuilder.SetThisUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(3)));
                ocspBuilder.SetNextUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(30)));
                TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, ocspBuilder);
                builder.GetRevocationDataValidator().AddOcspClient(ocspClient);
                parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                    .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                    , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                (1).HasNumberOfFailures(0));
            NUnit.Framework.Assert.AreEqual(1, mockCertificateChainValidator.verificationCalls.Count);
            MockChainValidator.ValidationCallBack call = mockCertificateChainValidator.verificationCalls[0];
            NUnit.Framework.Assert.AreEqual(CertificateSource.TIMESTAMP, call.context.GetCertificateSource());
            NUnit.Framework.Assert.AreEqual(ValidatorContext.SIGNATURE_VALIDATOR, call.context.GetValidatorContext());
            NUnit.Framework.Assert.AreEqual(timeStampCert.GetSubjectDN(), call.certificate.GetSubjectDN());
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureIsDocTimestampWithModifiedDateTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            String privateKeyName = CERTS_SRC + "rootCertKey.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(privateKeyName, PASSWORD);
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "modifiedDocTimestampDate.pdf"
                ))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
                DateTime currentDate = DateTimeUtil.GetCurrentUtcTime();
                ocspBuilder.SetProducedAt(currentDate);
                ocspBuilder.SetThisUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(3)));
                ocspBuilder.SetNextUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(30)));
                TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, ocspBuilder);
                builder.GetRevocationDataValidator().AddOcspClient(ocspClient);
                parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                    .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                    , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfLogs(2).HasNumberOfFailures(1).HasStatus(ValidationReport.ValidationResult
                .INVALID).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator
                .VALIDATING_SIGNATURE_NAME, (p) => "timestampSignature1")).HasLogItem((al) => al.WithCheckName(SignatureValidator
                .SIGNATURE_VERIFICATION).WithMessage(MessageFormatUtil.Format(SignatureValidator.CANNOT_VERIFY_SIGNATURE
                , "timestampSignature1")).WithStatus(ReportItem.ReportItemStatus.INVALID)));
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureWithModifiedTimestampDateTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            String privateKeyName = CERTS_SRC + "rootCertKey.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(privateKeyName, PASSWORD);
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureWithModifiedTimestampDate.pdf"
                ))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
                DateTime currentDate = DateTimeUtil.GetCurrentUtcTime();
                ocspBuilder.SetProducedAt(currentDate);
                ocspBuilder.SetThisUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(3)));
                ocspBuilder.SetNextUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(30)));
                TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, ocspBuilder);
                builder.GetRevocationDataValidator().AddOcspClient(ocspClient);
                parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                    .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                    , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2)).SetContinueAfterFailure(ValidatorContexts
                    .All(), CertificateSources.All(), false);
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfLogs(2).HasNumberOfFailures(1).HasStatus(ValidationReport.ValidationResult
                .INVALID).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator
                .VALIDATING_SIGNATURE_NAME, (p) => "Signature1")).HasLogItem((al) => al.WithCheckName(SignatureValidator
                .TIMESTAMP_VERIFICATION).WithMessage(SignatureValidator.CANNOT_VERIFY_TIMESTAMP).WithStatus(ReportItem.ReportItemStatus
                .INVALID)));
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureWithBrokenTimestampTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithBrokenTimestamp.pdf"))
                ) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasLogItems
                (2, 2, (al) => al.WithCheckName(SignatureValidator.TIMESTAMP_VERIFICATION).WithMessage(SignatureValidator
                .CANNOT_VERIFY_TIMESTAMP).WithStatus(ReportItem.ReportItemStatus.INVALID)));
        }

        [NUnit.Framework.Test]
        public virtual void DocumentModifiedLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "modifiedDoc.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasLogItem
                ((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.DOCUMENT_IS_NOT_COVERED
                , (i) => "Signature1")).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION)
                .WithMessage(SignatureValidator.CANNOT_VERIFY_SIGNATURE, (i) => "Signature1")));
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureInvalidStopValidationTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            parameters.SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), false);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "modifiedDoc.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasLogItem
                ((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.DOCUMENT_IS_NOT_COVERED
                , (i) => "Signature1").WithStatus(ReportItem.ReportItemStatus.INVALID)).HasLogItem((al) => al.WithCheckName
                (SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.CANNOT_VERIFY_SIGNATURE, (i
                ) => "Signature1").WithStatus(ReportItem.ReportItemStatus.INVALID)));
            // check that no requests are made after failure
            NUnit.Framework.Assert.AreEqual(0, mockCertificateChainValidator.verificationCalls.Count);
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureButTakenFromDSSTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate signCert = (IX509Certificate)certificateChain[0];
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithDss.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                signatureValidator.ValidateLatestSignature(document);
            }
            NUnit.Framework.Assert.AreEqual(2, mockCertificateRetriever.addKnownCertificatesCalls.Count);
            ICollection<IX509Certificate> dssCall = mockCertificateRetriever.addKnownCertificatesCalls[0];
            NUnit.Framework.Assert.AreEqual(3, dssCall.Count);
            NUnit.Framework.Assert.AreEqual(1, dssCall.Where((c) => ((IX509Certificate)c).Equals(rootCert)).Count());
            NUnit.Framework.Assert.AreEqual(1, dssCall.Where((c) => ((IX509Certificate)c).Equals(intermediateCert)).Count
                ());
            NUnit.Framework.Assert.AreEqual(1, dssCall.Where((c) => ((IX509Certificate)c).Equals(signCert)).Count());
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureButTakenFromDSSOneCertIsBrokenTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithBrokenDss.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItem
                ((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithExceptionCauseType(typeof(AbstractGeneralSecurityException
                ))));
        }

        [NUnit.Framework.Test]
        public virtual void IndeterminateChainValidationLeadsToIndeterminateResultTest() {
            mockCertificateChainValidator.OnCallDo((c) => c.report.AddReportItem(new ReportItem("test", "test", ReportItem.ReportItemStatus
                .INDETERMINATE)));
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasLogItem((al) => al.WithCheckName("test").WithMessage("test")));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidChainValidationLeadsToInvalidResultTest() {
            mockCertificateChainValidator.OnCallDo((c) => c.report.AddReportItem(new ReportItem("test", "test", ReportItem.ReportItemStatus
                .INVALID)));
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasLogItem((al) => al.WithCheckName("test").WithMessage("test")));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRevisionsValidationLeadsToInvalidResultTest() {
            mockDocumentRevisionsValidator.SetReportItemStatus(ReportItem.ReportItemStatus.INVALID);
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignatures();
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasLogItem((al) => al.WithCheckName("test").WithMessage("test")));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateMultipleSignatures() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithMultipleSignaturesAndTimeStamp.pdf"
                ))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                ValidationReport report = signatureValidator.ValidateSignatures();
                AssertValidationReport.AssertThat(report, (r) => r.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                    (5).HasNumberOfFailures(0).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                    ).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (p) => "Signature1")).HasLogItem((l) => l.
                    WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME
                    , (p) => "Signature2")).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage
                    (SignatureValidator.VALIDATING_SIGNATURE_NAME, (p) => "Signature3")).HasLogItem((l) => l.WithCheckName
                    (SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, 
                    (p) => "signer1")).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage
                    (SignatureValidator.VALIDATING_SIGNATURE_NAME, (p) => "signer2")));
                DateTime date1 = TimeTestUtil.TEST_DATE_TIME.AddDays(1);
                DateTime date2 = TimeTestUtil.TEST_DATE_TIME.AddDays(10);
                DateTime date3 = TimeTestUtil.TEST_DATE_TIME.AddDays(20);
                // 2 signatures with timestamp
                // 3 document timestamps
                IList<MockChainValidator.ValidationCallBack> verificationCalls = mockCertificateChainValidator.verificationCalls;
                NUnit.Framework.Assert.AreEqual(7, verificationCalls.Count);
                NUnit.Framework.Assert.AreEqual(TimeBasedContext.PRESENT, verificationCalls[0].context.GetTimeBasedContext
                    ());
                for (int i = 1; i < verificationCalls.Count; ++i) {
                    NUnit.Framework.Assert.AreEqual(TimeBasedContext.HISTORICAL, verificationCalls[i].context.GetTimeBasedContext
                        ());
                }
                NUnit.Framework.Assert.IsTrue(verificationCalls.Any((c) => c.certificate.GetSerialNumber().ToString().Equals
                    ("1491571297") && c.checkDate.Equals(date3)));
                NUnit.Framework.Assert.IsTrue(verificationCalls.Any((c) => c.certificate.GetSerialNumber().ToString().Equals
                    ("1491571297") && c.checkDate.Equals(date2)));
                NUnit.Framework.Assert.IsTrue(verificationCalls.Any((c) => c.certificate.GetSerialNumber().ToString().Equals
                    ("1491571297") && c.checkDate.Equals(date1)));
                NUnit.Framework.Assert.IsTrue(verificationCalls.Any((c) => c.certificate.GetSerialNumber().ToString().Equals
                    ("1550593058") && c.checkDate.Equals(date2)));
                NUnit.Framework.Assert.IsTrue(verificationCalls.Any((c) => c.certificate.GetSerialNumber().ToString().Equals
                    ("1701704311986") && c.checkDate.Equals(date1)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignatureChainValidatorFailureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                mockCertificateChainValidator.OnCallDo((c) => {
                    throw new Exception("Test chain validation failure");
                }
                );
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                ValidationReport report = signatureValidator.ValidateLatestSignature(document);
                AssertValidationReport.AssertThat(report, (r) => r.HasLogItem((l) => l.WithMessage(SignatureValidator.CHAIN_VALIDATION_FAILED
                    )));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TimeStampChainValidatorFailureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timestampSignatureDoc.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                mockCertificateChainValidator.OnCallDo((c) => {
                    throw new Exception("Test chain validation failure");
                }
                );
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                ValidationReport report = signatureValidator.ValidateLatestSignature(document);
                AssertValidationReport.AssertThat(report, (r) => r.HasLogItem((l) => l.WithMessage(SignatureValidator.CHAIN_VALIDATION_FAILED
                    )));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverAddKnownCertificatesFromDSSFailureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithDss.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                mockCertificateRetriever.OnAddKnownCertificatesDo((c) => {
                    throw new Exception("Test add know certificates failure");
                }
                );
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                ValidationReport report = signatureValidator.ValidateLatestSignature(document);
                AssertValidationReport.AssertThat(report, (r) => r.HasLogItems(1, int.MaxValue, (l) => l.WithMessage(SignatureValidator
                    .ADD_KNOWN_CERTIFICATES_FAILED)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverAddKnownCertificatesFromSignatureFailureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                mockCertificateRetriever.OnAddKnownCertificatesDo((c) => {
                    throw new Exception("Test add know certificates failure");
                }
                );
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                ValidationReport report = signatureValidator.ValidateLatestSignature(document);
                AssertValidationReport.AssertThat(report, (r) => r.HasLogItems(1, int.MaxValue, (l) => l.WithMessage(SignatureValidator
                    .ADD_KNOWN_CERTIFICATES_FAILED)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CertificateRetrieverAddKnownCertificatesFromTimestampFailureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timestampSignatureDoc.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                mockCertificateRetriever.OnAddKnownCertificatesDo((c) => {
                    throw new Exception("Test add know certificates failure");
                }
                );
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                ValidationReport report = signatureValidator.ValidateLatestSignature(document);
                AssertValidationReport.AssertThat(report, (r) => r.HasLogItems(1, int.MaxValue, (l) => l.WithMessage(SignatureValidator
                    .ADD_KNOWN_CERTIFICATES_FAILED)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocumentRevisionValidatorFailureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                mockCertificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                mockDocumentRevisionsValidator.OnCallDo((c) => {
                    throw new Exception("Test add know certificates failure");
                }
                );
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                ValidationReport report = signatureValidator.ValidateSignatures();
                AssertValidationReport.AssertThat(report, (r) => r.HasLogItem((l) => l.WithMessage(SignatureValidator.REVISIONS_VALIDATION_FAILED
                    )));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ThrowExceptionOnTheSecondValidationAttempt() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timestampSignatureDoc.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                signatureValidator.ValidateSignatures();
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => signatureValidator.ValidateSignatures
                    ());
                NUnit.Framework.Assert.AreEqual(SignatureValidator.VALIDATION_PERFORMED, exception.Message);
                exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => signatureValidator.ValidateSignature(
                    "Signature1"));
                NUnit.Framework.Assert.AreEqual(SignatureValidator.VALIDATION_PERFORMED, exception.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignatureWithSpecifiedNameNotFound() {
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timestampSignatureDoc.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignature("Invalid signature name");
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfLogs(1).HasNumberOfFailures(1).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                ).WithMessage(SignatureValidator.SIGNATURE_NOT_FOUND, (p) => "Invalid signature name").WithStatus(ReportItem.ReportItemStatus
                .INDETERMINATE)));
        }
    }
}
