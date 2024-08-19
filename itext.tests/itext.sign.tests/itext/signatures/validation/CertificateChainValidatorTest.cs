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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Extensions;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateChainValidatorTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/CertificateChainValidatorTest/";

        private ValidatorChainBuilder validatorChainBuilder;

        private SignatureValidationProperties properties;

        private IssuingCertificateRetriever certificateRetriever;

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private MockRevocationDataValidator mockRevocationDataValidator;

        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            mockRevocationDataValidator = new MockRevocationDataValidator();
            properties = new SignatureValidationProperties();
            certificateRetriever = new IssuingCertificateRetriever();
            validatorChainBuilder = new ValidatorChainBuilder().WithIssuingCertificateRetrieverFactory(() => certificateRetriever
                ).WithSignatureValidationProperties(properties).WithRevocationDataValidatorFactory(() => mockRevocationDataValidator
                );
        }

        [NUnit.Framework.Test]
        public virtual void ValidChainTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => rootCert.
                GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void ValidNumericBasicConstraintsTest() {
            String chainName = CERTS_SRC + "signChainWithValidNumericBasicConstraints.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => rootCert.
                GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidNumericBasicConstraintsTest() {
            String chainName = CERTS_SRC + "signChainWithInvalidNumericBasicConstraints.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (2).HasNumberOfLogs(3).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => rootCert.
                GetSubjectDN()).WithCertificate(rootCert)).HasLogItem((la) => la.WithCheckName(CertificateChainValidator
                .EXTENSIONS_CHECK).WithMessage(CertificateChainValidator.EXTENSION_MISSING, (l) => "2.5.29.19").WithCertificate
                (rootCert)).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.EXTENSIONS_CHECK).WithMessage
                (CertificateChainValidator.EXTENSION_MISSING, (l) => "2.5.29.19").WithCertificate(intermediateCert)));
        }

        [NUnit.Framework.Test]
        public virtual void RevocationValidationCallTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(2, mockRevocationDataValidator.calls.Count);
            MockRevocationDataValidator.RevocationDataValidatorCall call1 = mockRevocationDataValidator.calls[0];
            NUnit.Framework.Assert.AreEqual(signingCert, call1.certificate);
            NUnit.Framework.Assert.AreEqual(CertificateSource.SIGNER_CERT, call1.context.GetCertificateSource());
            NUnit.Framework.Assert.AreEqual(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, call1.context.GetValidatorContext
                ());
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.TEST_DATE_TIME, call1.validationDate);
            MockRevocationDataValidator.RevocationDataValidatorCall call2 = mockRevocationDataValidator.calls[1];
            NUnit.Framework.Assert.AreEqual(intermediateCert, call2.certificate);
            NUnit.Framework.Assert.AreEqual(CertificateSource.CERT_ISSUER, call2.context.GetCertificateSource());
            NUnit.Framework.Assert.AreEqual(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR, call2.context.GetValidatorContext
                ());
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.TEST_DATE_TIME, call2.validationDate);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralFailuresWithProceedAfterFailTest() {
            String chainName = CERTS_SRC + "invalidCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            properties.SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), true);
            // Set random extension as a required one to force the test to fail.
            properties.SetRequiredExtensions(CertificateSources.Of(CertificateSource.CERT_ISSUER), JavaCollectionsUtil
                .SingletonList<CertificateExtension>(new KeyUsageExtension(KeyUsage.DECIPHER_ONLY)));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(3, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(4, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[0]);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[1], report.GetLogs()[1]);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[2], report.GetLogs()[2]);
            CertificateReportItem failure1 = report.GetCertificateFailures()[0];
            NUnit.Framework.Assert.AreEqual(signingCert, failure1.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Required certificate extensions check.", failure1.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Required extension {0} is missing or incorrect."
                , OID.X509Extensions.KEY_USAGE), failure1.GetMessage());
            CertificateReportItem failure2 = report.GetCertificateFailures()[1];
            NUnit.Framework.Assert.AreEqual(intermediateCert, failure2.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Required certificate extensions check.", failure2.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Required extension {0} is missing or incorrect."
                , OID.X509Extensions.KEY_USAGE), failure2.GetMessage());
            CertificateReportItem failure3 = report.GetCertificateFailures()[2];
            NUnit.Framework.Assert.AreEqual(rootCert, failure3.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Required certificate extensions check.", failure3.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Required extension {0} is missing or incorrect."
                , OID.X509Extensions.KEY_USAGE), failure3.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void SeveralFailuresWithoutProceedAfterFailTest() {
            String chainName = CERTS_SRC + "invalidCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            properties.SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), false);
            // Set random extension as a required one to force the test to fail.
            properties.SetRequiredExtensions(CertificateSources.Of(CertificateSource.CERT_ISSUER), JavaCollectionsUtil
                .SingletonList<CertificateExtension>(new KeyUsageExtension(KeyUsage.DECIPHER_ONLY)));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[0]);
            CertificateReportItem failure1 = report.GetCertificateFailures()[0];
            NUnit.Framework.Assert.AreEqual(signingCert, failure1.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Required certificate extensions check.", failure1.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Required extension {0} is missing or incorrect."
                , OID.X509Extensions.KEY_USAGE), failure1.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void IntermediateCertTrustedTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(0).HasNumberOfLogs(1).HasLogItem((la
                ) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (l) => intermediateCert.GetSubjectDN())));
        }

        [NUnit.Framework.Test]
        public virtual void ValidChainRequiredExtensionPositiveTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (l) => rootCert.GetSubjectDN()).WithCertificate
                (rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void ValidChainRequiredExtensionNegativeTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CERT_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(2).HasNumberOfLogs(3).HasLogItem((la
                ) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (l) => rootCert.GetSubjectDN()).WithCertificate(rootCert)).HasLogItem((la) => la
                .WithCheckName(CertificateChainValidator.EXTENSIONS_CHECK).WithMessage(CertificateChainValidator.EXTENSION_MISSING
                , (l) => OID.X509Extensions.KEY_USAGE).WithCertificate(signingCert)).HasLogItem((la) => la.WithCheckName
                (CertificateChainValidator.EXTENSIONS_CHECK).WithMessage(CertificateChainValidator.EXTENSION_MISSING, 
                (l) => OID.X509Extensions.BASIC_CONSTRAINTS).WithCertificate(signingCert)));
        }

        [NUnit.Framework.Test]
        public virtual void ValidChainTrustedRootIsnSetTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator
                .CERTIFICATE_CHECK).WithMessage(CertificateChainValidator.ISSUER_MISSING, (l) => intermediateCert.GetSubjectDN
                ()).WithCertificate(intermediateCert)));
        }

        [NUnit.Framework.Test]
        public virtual void IntermediateCertIsNotYetValidTest() {
            String chainName = CERTS_SRC + "chain.pem";
            String intermediateCertName = CERTS_SRC + "not-yet-valid-intermediate.cert.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(intermediateCertName)[0
                ];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(1).HasNumberOfLogs(2).HasLogItem((la
                ) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (l) => rootCert.GetSubjectDN()).WithCertificate(rootCert)).HasLogItem((la) => la
                .WithCheckName(CertificateChainValidator.VALIDITY_CHECK).WithMessage(CertificateChainValidator.NOT_YET_VALID_CERTIFICATE
                , (l) => intermediateCert.GetSubjectDN()).WithCertificate(intermediateCert).WithExceptionCauseType(typeof(
                AbstractCertificateNotYetValidException))));
        }

        [NUnit.Framework.Test]
        public virtual void IntermediateCertIsExpiredTest() {
            String chainName = CERTS_SRC + "chain.pem";
            String intermediateCertName = CERTS_SRC + "expired-intermediate.cert.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(intermediateCertName)[0
                ];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasNumberOfLogs(2).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (l) => rootCert.GetSubjectDN()).WithCertificate
                (rootCert)).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.VALIDITY_CHECK).WithMessage(
                CertificateChainValidator.EXPIRED_CERTIFICATE, (l) => intermediateCert.GetSubjectDN()).WithCertificate
                (intermediateCert).WithExceptionCauseType(typeof(AbstractCertificateExpiredException))));
        }

        [NUnit.Framework.Test]
        public virtual void CertificateGenerallyTrustedTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.GetTrustedCertificatesStore().AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList
                (rootCert));
            // Remove required extensions to make test pass.
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .OCSP_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
            ValidationReport report3 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            AssertValidationReport.AssertThat(report3, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void RootCertificateTrustedForCATest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.GetTrustedCertificatesStore().AddCATrustedCertificates(JavaCollectionsUtil.SingletonList
                (rootCert));
            // Remove required extensions to make test pass.
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext, signingCert, DateTimeUtil.GetCurrentUtcTime
                ());
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .OCSP_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
            ValidationReport report3 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            AssertValidationReport.AssertThat(report3, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void FirstCertificateTrustedForCATest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.GetTrustedCertificatesStore().AddCATrustedCertificates(JavaCollectionsUtil.SingletonList
                (signingCert));
            // Remove required extensions to make test pass.
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CERT_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This works fine because certificate in question has CertificateSource.CERT_ISSUER context.
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => signingCert.GetSubjectDN()).WithCertificate(signingCert)));
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because certificate in question has CertificateSource.TIMESTAMP context.
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasNumberOfLogs(2).HasLogItem((al) => al.WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, (i) => signingCert.GetSubjectDN(), (i) => "certificates generation"
                )).HasLogItem((al) => al.WithMessage(CertificateChainValidator.ISSUER_MISSING, (i) => intermediateCert
                .GetSubjectDN())));
        }

        [NUnit.Framework.Test]
        public virtual void RootCertificateTrustedForOCSPTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.GetTrustedCertificatesStore().AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList
                (rootCert));
            // Remove required extensions to make test pass.
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .OCSP_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This works fine because even though root certificate has CertificateSource.CERT_ISSUER context,
            // the chain contains initial certificate with CertificateSource.OCSP_ISSUER context.
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because root certificate has CertificateSource.CERT_ISSUER context and
            // the chain doesn't contain any certificate with CertificateSource.OCSP_ISSUER context.
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasNumberOfLogs(2).HasLogItem((l) => l.WithMessage(CertificateChainValidator.
                CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, (i) => rootCert.GetSubjectDN(), (i) => "OCSP response generation"
                )).HasLogItem((l) => l.WithMessage(CertificateChainValidator.ISSUER_MISSING, (i) => rootCert.GetSubjectDN
                ())));
        }

        [NUnit.Framework.Test]
        public virtual void RootCertificateTrustedForCRLTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.GetTrustedCertificatesStore().AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList
                (rootCert));
            // Remove required extensions to make test pass.
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This works fine because even though root certificate has CertificateSource.CERT_ISSUER context,
            // the chain contains initial certificate with CertificateSource.CRL_ISSUER context.
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .OCSP_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because root certificate has CertificateSource.CERT_ISSUER context and
            // the chain doesn't contain any certificate with CertificateSource.CRL_ISSUER context.
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasNumberOfLogs(2).HasLogItem((l) => l.WithMessage(CertificateChainValidator.
                CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, (i) => rootCert.GetSubjectDN(), (i) => "CRL generation")).HasLogItem
                ((l) => l.WithMessage(CertificateChainValidator.ISSUER_MISSING, (i) => rootCert.GetSubjectDN())));
        }

        [NUnit.Framework.Test]
        public virtual void RootCertificateTrustedForTimestampTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
            certificateRetriever.GetTrustedCertificatesStore().AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList
                (rootCert));
            // Remove required extensions to make test pass.
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This works fine because even though root certificate has CertificateSource.CERT_ISSUER context,
            // the chain contains initial certificate with CertificateSource.TIMESTAMP context.
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because root certificate has CertificateSource.CERT_ISSUER context and
            // the chain doesn't contain any certificate with CertificateSource.TIMESTAMP context.
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(1).HasNumberOfLogs(2).HasLogItem((l) => l.WithMessage(CertificateChainValidator.
                CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, (i) => rootCert.GetSubjectDN(), (i) => "timestamp generation"
                )).HasLogItem((l) => l.WithMessage(CertificateChainValidator.ISSUER_MISSING, (i) => rootCert.GetSubjectDN
                ())));
        }

        [NUnit.Framework.Test]
        public virtual void TrustStoreFailureTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever(certificateRetriever
                ).OnGetTrustedCertificatesStoreDo(() => {
                throw new Exception("Test trust store failure");
            }
            );
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever);
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItems(1, 10, (la) => la.WithMessage(CertificateChainValidator.TRUSTSTORE_RETRIEVAL_FAILED)));
        }

        [NUnit.Framework.Test]
        public virtual void IssuerRetrievalFailureTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever(certificateRetriever
                ).OnRetrieveIssuerCertificateDo((c) => {
                throw new Exception("Test issuer retrieval failure");
            }
            );
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever);
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItems(1, 10, (la) => la.WithMessage(CertificateChainValidator.ISSUER_RETRIEVAL_FAILED)));
        }

        [NUnit.Framework.Test]
        public virtual void RevocationValidationFailureTest() {
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            mockRevocationDataValidator.OnValidateDo((c) => {
                throw new Exception("Test revocation validation failure");
            }
            );
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItems(1, 10, (la) => la.WithMessage(CertificateChainValidator.REVOCATION_VALIDATION_FAILED)));
        }

        [NUnit.Framework.Test]
        public virtual void TestStopOnInvalidRevocationResultTest() {
            mockRevocationDataValidator.OnValidateDo((c) => c.report.AddReportItem(new ReportItem("test", "test", ReportItem.ReportItemStatus
                .INVALID)));
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            properties.SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), false);
            MockIssuingCertificateRetriever mockCertificateRetriever = new MockIssuingCertificateRetriever(certificateRetriever
                );
            validatorChainBuilder.WithIssuingCertificateRetrieverFactory(() => mockCertificateRetriever);
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                ));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(rootCert));
            ValidationReport report = validator.ValidateCertificate(baseContext, signingCert, TimeTestUtil.TEST_DATE_TIME
                );
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID));
            NUnit.Framework.Assert.AreEqual(0, mockCertificateRetriever.getCrlIssuerCertificatesCalls.Count);
            NUnit.Framework.Assert.AreEqual(1, mockRevocationDataValidator.calls.Count);
        }
    }
}
