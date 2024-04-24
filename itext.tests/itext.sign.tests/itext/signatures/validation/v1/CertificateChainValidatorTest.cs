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
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Extensions;
using iText.Signatures.Validation.V1.Report;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateChainValidatorTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/CertificateChainValidatorTest/";

        private ValidatorChainBuilder validatorChainBuilder;

        private SignatureValidationProperties properties;

        private IssuingCertificateRetriever certificateRetriever;

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            properties = new SignatureValidationProperties();
            certificateRetriever = new IssuingCertificateRetriever();
            validatorChainBuilder = new ValidatorChainBuilder().WithIssuingCertificateRetriever(certificateRetriever).
                WithSignatureValidationProperties(properties);
            validatorChainBuilder.WithRevocationDataValidator(new CertificateChainValidatorTest.MockRevocationDataValidator
                (validatorChainBuilder));
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
            new AssertValidationReport(report).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures(
                0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage(
                ).Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), CertificateChainValidator.CERTIFICATE_TRUSTED
                ).DoAssert();
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
            new AssertValidationReport(report).HasNumberOfFailures(0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName
                ().Equals("Certificate check.") && l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator
                .CERTIFICATE_TRUSTED, intermediateCert.GetSubjectDN())), CertificateChainValidator.CERTIFICATE_TRUSTED
                ).DoAssert();
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
            new AssertValidationReport(report).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures(
                0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage(
                ).Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), CertificateChainValidator.CERTIFICATE_TRUSTED
                ).DoAssert();
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
            new AssertValidationReport(report).HasNumberOfFailures(2).HasNumberOfLogs(3).HasLogItem((l) => l.GetCheckName
                ().Equals("Certificate check.") && l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator
                .CERTIFICATE_TRUSTED, rootCert.GetSubjectDN())) && ((CertificateReportItem)l).GetCertificate().Equals(
                rootCert), CertificateChainValidator.CERTIFICATE_TRUSTED).HasLogItem((l) => l.GetCheckName().Equals("Required certificate extensions check."
                ) && l.GetMessage().Equals(MessageFormatUtil.Format("Required extension {0} is missing or incorrect.", 
                OID.X509Extensions.KEY_USAGE)) && ((CertificateReportItem)l).GetCertificate().Equals(signingCert), "Required extension {0} is missing or incorrect."
                ).HasLogItem((l) => l.GetCheckName().Equals("Required certificate extensions check.") && l.GetMessage(
                ).Equals(MessageFormatUtil.Format("Required extension {0} is missing or incorrect.", OID.X509Extensions
                .BASIC_CONSTRAINTS)) && ((CertificateReportItem)l).GetCertificate().Equals(signingCert), "Required extension {0} is missing or incorrect."
                ).DoAssert();
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
            new AssertValidationReport(report).HasStatus(ValidationReport.ValidationResult.INDETERMINATE).HasNumberOfFailures
                (1).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.ISSUER_MISSING, intermediateCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(intermediateCert), CertificateChainValidator
                .ISSUER_MISSING).DoAssert();
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
            new AssertValidationReport(report).HasNumberOfFailures(1).HasNumberOfLogs(2).HasLogItem((l) => l.GetCheckName
                ().Equals("Certificate check.") && l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator
                .CERTIFICATE_TRUSTED, rootCert.GetSubjectDN())) && ((CertificateReportItem)l).GetCertificate().Equals(
                rootCert), CertificateChainValidator.CERTIFICATE_TRUSTED).HasLogItem((l) => l.GetCheckName().Equals("Certificate validity period check."
                ) && l.GetMessage().Equals(MessageFormatUtil.Format("Certificate {0} is not yet valid.", intermediateCert
                .GetSubjectDN())) && ((CertificateReportItem)l).GetCertificate().Equals(intermediateCert) && l.GetExceptionCause
                () is AbstractCertificateNotYetValidException, "Certificate {0} is not yet valid.").DoAssert();
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
            new AssertValidationReport(report).HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasNumberOfLogs(2).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), CertificateChainValidator.ISSUER_MISSING
                ).HasLogItem((l) => l.GetCheckName().Equals("Certificate validity period check.") && l.GetMessage().Equals
                (MessageFormatUtil.Format("Certificate {0} is expired.", intermediateCert.GetSubjectDN())) && ((CertificateReportItem
                )l).GetCertificate().Equals(intermediateCert) && l.GetExceptionCause() is AbstractCertificateExpiredException
                , CertificateChainValidator.ISSUER_MISSING).DoAssert();
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
            new AssertValidationReport(report1).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .OCSP_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            new AssertValidationReport(report2).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
            ValidationReport report3 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            new AssertValidationReport(report3).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
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
            new AssertValidationReport(report1).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .OCSP_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            new AssertValidationReport(report2).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
            ValidationReport report3 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            new AssertValidationReport(report3).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
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
            new AssertValidationReport(report1).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, signingCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(signingCert), "Certificate {0} is trusted."
                ).DoAssert();
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because certificate in question has CertificateSource.TIMESTAMP context.
            new AssertValidationReport(report2).HasStatus(ValidationReport.ValidationResult.INDETERMINATE).HasNumberOfFailures
                (1).HasNumberOfLogs(2).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, signingCert.GetSubjectDN(), "certificates generation")), CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.
                Format(CertificateChainValidator.ISSUER_MISSING, intermediateCert.GetSubjectDN())), CertificateChainValidator
                .ISSUER_MISSING).DoAssert();
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
            new AssertValidationReport(report1).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because root certificate has CertificateSource.CERT_ISSUER context and
            // the chain doesn't contain any certificate with CertificateSource.OCSP_ISSUER context.
            new AssertValidationReport(report2).HasStatus(ValidationReport.ValidationResult.INDETERMINATE).HasNumberOfFailures
                (1).HasNumberOfLogs(2).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, rootCert.GetSubjectDN(), "OCSP response generation")), CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.
                Format(CertificateChainValidator.ISSUER_MISSING, rootCert.GetSubjectDN())), CertificateChainValidator.
                ISSUER_MISSING).DoAssert();
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
            new AssertValidationReport(report1).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .OCSP_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because root certificate has CertificateSource.CERT_ISSUER context and
            // the chain doesn't contain any certificate with CertificateSource.CRL_ISSUER context.
            new AssertValidationReport(report2).HasStatus(ValidationReport.ValidationResult.INDETERMINATE).HasNumberOfFailures
                (1).HasNumberOfLogs(2).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, rootCert.GetSubjectDN(), "CRL generation")), CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.
                Format(CertificateChainValidator.ISSUER_MISSING, rootCert.GetSubjectDN())), CertificateChainValidator.
                ISSUER_MISSING).DoAssert();
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
            new AssertValidationReport(report1).HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.GetCheckName().Equals("Certificate check.") && l.GetMessage
                ().Equals(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, rootCert.GetSubjectDN
                ())) && ((CertificateReportItem)l).GetCertificate().Equals(rootCert), "Certificate {0} is trusted.").DoAssert
                ();
            ValidationReport report2 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), signingCert, DateTimeUtil.GetCurrentUtcTime());
            // This doesn't work because root certificate has CertificateSource.CERT_ISSUER context and
            // the chain doesn't contain any certificate with CertificateSource.TIMESTAMP context.
            new AssertValidationReport(report2).HasStatus(ValidationReport.ValidationResult.INDETERMINATE).HasNumberOfFailures
                (1).HasNumberOfLogs(2).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.Format(CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, rootCert.GetSubjectDN(), "timestamp generation")), CertificateChainValidator
                .CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT).HasLogItem((l) => l.GetMessage().Equals(MessageFormatUtil.
                Format(CertificateChainValidator.ISSUER_MISSING, rootCert.GetSubjectDN())), CertificateChainValidator.
                ISSUER_MISSING).DoAssert();
        }

        private class MockRevocationDataValidator : RevocationDataValidator {
            public MockRevocationDataValidator(ValidatorChainBuilder builder)
                : base(builder) {
            }

            public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
                , DateTime validationDate) {
            }
        }
    }
}
