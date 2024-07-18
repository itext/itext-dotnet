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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Mocks;
using iText.Signatures.Validation.V1.Report;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignatureValidatorIntegrationTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/SignatureValidatorIntegrationTest/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/SignatureValidatorIntegrationTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private SignatureValidationProperties parameters;

        private IssuingCertificateRetriever certificateRetriever;

        private ValidatorChainBuilder builder;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            parameters = new SignatureValidationProperties();
            certificateRetriever = new IssuingCertificateRetriever();
            builder = new ValidatorChainBuilder().WithIssuingCertificateRetriever(certificateRetriever).WithSignatureValidationProperties
                (parameters);
        }

        [NUnit.Framework.Test]
        public virtual void ValidLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                AddRevDataClients();
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignatures();
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItems
                (3, (al) => al.WithCertificate(rootCert).WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage
                (CertificateChainValidator.CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN())));
        }

        [NUnit.Framework.Test]
        public virtual void ShortValidityCertsWithOcspTest() {
            String rootCertName = CERTS_SRC + "short_validity_root_cert.pem";
            String tsRootCertName = CERTS_SRC + "ts_root_cert.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertName)[0];
            IX509Certificate tsRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(tsRootCertName)[0];
            // We need to set infinite freshness for first timestamp validation. Otherwise, test will fail.
            builder.GetProperties().SetFreshness(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR), CertificateSources
                .Of(CertificateSource.TIMESTAMP), TimeBasedContexts.Of(TimeBasedContext.PRESENT), TimeSpan.FromDays(999999
                ));
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "shortValidityCertsWithOcsp.pdf"
                ))) {
                certificateRetriever.SetTrustedCertificates(JavaUtil.ArraysAsList(rootCert, tsRootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignatures();
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItem
                ((al) => al.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                .UNEXPECTED_ENTRY_IN_XREF, (i) => 30)).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                ).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (i) => "timestampSig1")).HasLogItem((al) =>
                 al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME
                , (i) => "Signature1")).HasLogItems(2, (al) => al.WithCertificate(rootCert).WithCheckName(CertificateChainValidator
                .CERTIFICATE_CHECK).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN
                ())).HasLogItems(4, (al) => al.WithCertificate(tsRootCert).WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (i) => tsRootCert.GetSubjectDN())));
        }

        [NUnit.Framework.Test]
        public virtual void ShortValidityCertsWithCrlTest() {
            String rootCertName = CERTS_SRC + "short_validity_root_cert.pem";
            String tsRootCertName = CERTS_SRC + "ts_root_cert.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertName)[0];
            IX509Certificate tsRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(tsRootCertName)[0];
            // We need to set infinite freshness for first timestamp validation. Otherwise, test will fail.
            builder.GetProperties().SetFreshness(ValidatorContexts.Of(ValidatorContext.CRL_VALIDATOR), CertificateSources
                .Of(CertificateSource.TIMESTAMP), TimeBasedContexts.Of(TimeBasedContext.PRESENT), TimeSpan.FromDays(999999
                ));
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "shortValidityCertsWithCrl.pdf"
                ))) {
                certificateRetriever.SetTrustedCertificates(JavaUtil.ArraysAsList(rootCert, tsRootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignatures();
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItem
                ((al) => al.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                .UNEXPECTED_ENTRY_IN_XREF, (i) => 32)).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                ).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (i) => "timestampSig1")).HasLogItem((al) =>
                 al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME
                , (i) => "Signature1")).HasLogItems(2, (al) => al.WithCertificate(rootCert).WithCheckName(CertificateChainValidator
                .CERTIFICATE_CHECK).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN
                ())).HasLogItems(4, (al) => al.WithCertificate(tsRootCert).WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (i) => tsRootCert.GetSubjectDN())));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateMultipleDocumentsTest() {
            String rootCertName = CERTS_SRC + "short_validity_root_cert.pem";
            String tsRootCertName = CERTS_SRC + "ts_root_cert.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertName)[0];
            IX509Certificate tsRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(tsRootCertName)[0];
            // We need to set infinite freshness for first timestamp validation. Otherwise, test will fail.
            builder.GetProperties().SetFreshness(ValidatorContexts.Of(ValidatorContext.CRL_VALIDATOR), CertificateSources
                .Of(CertificateSource.TIMESTAMP), TimeBasedContexts.Of(TimeBasedContext.PRESENT), TimeSpan.FromDays(999999
                ));
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "shortValidityCertsWithCrl.pdf"
                ))) {
                certificateRetriever.SetTrustedCertificates(JavaUtil.ArraysAsList(rootCert, tsRootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                signatureValidator.ValidateSignatures();
            }
            using (PdfDocument document_1 = new PdfDocument(new PdfReader(SOURCE_FOLDER + "shortValidityCertsWithOcsp.pdf"
                ))) {
                certificateRetriever.SetTrustedCertificates(JavaUtil.ArraysAsList(rootCert, tsRootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document_1);
                signatureValidator.ValidateSignatures();
            }
            IList<ICrlClient> crlClients = builder.GetProperties().GetCrlClients();
            IList<IOcspClient> ocspClients = builder.GetProperties().GetOcspClients();
            NUnit.Framework.Assert.AreEqual(1, crlClients.Count);
            NUnit.Framework.Assert.AreEqual(1, ocspClients.Count);
            NUnit.Framework.Assert.IsTrue(crlClients[0] is ValidationCrlClient);
            NUnit.Framework.Assert.IsTrue(ocspClients[0] is ValidationOcspClient);
            ValidationCrlClient validationCrlClient = (ValidationCrlClient)crlClients[0];
            ValidationOcspClient validationOcspClient = (ValidationOcspClient)ocspClients[0];
            NUnit.Framework.Assert.AreEqual(2, validationCrlClient.GetCrls().Count);
            NUnit.Framework.Assert.AreEqual(2, validationOcspClient.GetResponses().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSingleSignatureTest1() {
            String rootCertName = CERTS_SRC + "root_cert.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertName)[0];
            ValidationReport report1;
            ValidationReport report2;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validateSingleSignature1.pdf"
                ))) {
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                AddRevDataClients();
                SignatureValidator signatureValidator1 = builder.BuildSignatureValidator(document);
                report1 = signatureValidator1.ValidateSignature("Signature1");
                SignatureValidator signatureValidator2 = builder.BuildSignatureValidator(document);
                report2 = signatureValidator2.ValidateSignature("Signature2");
            }
            // Signature1 set access permissions to level 3, Signature2 - to level 1, after that annotation was added.
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                (4).HasNumberOfFailures(0).HasLogItem((al) => al.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                ).WithMessage(DocumentRevisionsValidator.UNEXPECTED_ENTRY_IN_XREF, (i) => 17).WithStatus(ReportItem.ReportItemStatus
                .INFO)).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator
                .VALIDATING_SIGNATURE_NAME, (p) => "Signature1")).HasLogItems(2, (al) => al.WithCertificate(rootCert).
                WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED
                , (i) => rootCert.GetSubjectDN())));
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfLogs
                (4).HasNumberOfFailures(1).HasLogItem((al) => al.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                ).WithMessage(DocumentRevisionsValidator.PAGE_ANNOTATIONS_MODIFIED).WithStatus(ReportItem.ReportItemStatus
                .INVALID)).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(
                SignatureValidator.VALIDATING_SIGNATURE_NAME, (p) => "Signature2")).HasLogItems(2, (al) => al.WithCertificate
                (rootCert).WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN())));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSingleSignatureTest2() {
            String rootCertName = CERTS_SRC + "root_cert.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertName)[0];
            ValidationReport report1;
            ValidationReport report2;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validateSingleSignature2.pdf"
                ))) {
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                AddRevDataClients();
                SignatureValidator signatureValidator1 = builder.BuildSignatureValidator(document);
                report1 = signatureValidator1.ValidateSignature("Signature1");
                SignatureValidator signatureValidator2 = builder.BuildSignatureValidator(document);
                report2 = signatureValidator2.ValidateSignature("Signature2");
            }
            // Signature1 set access permissions to level 1, after that annotation was added and then Signature2 applied.
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (3).HasLogItem((al) => al.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                .PAGE_ANNOTATIONS_MODIFIED).WithStatus(ReportItem.ReportItemStatus.INVALID)).HasLogItem((al) => al.WithCheckName
                (DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.UNEXPECTED_FORM_FIELD
                , (p) => "Signature2").WithStatus(ReportItem.ReportItemStatus.INVALID)).HasLogItem((al) => al.WithCheckName
                (DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.NOT_ALLOWED_ACROFORM_CHANGES
                ).WithStatus(ReportItem.ReportItemStatus.INVALID)).HasLogItem((al) => al.WithCheckName(SignatureValidator
                .SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (p) => "Signature1"
                )));
            AssertValidationReport.AssertThat(report2, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator
                .VALIDATING_SIGNATURE_NAME, (p) => "Signature2")));
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveRevocationDataFromTheSignatureContainerTest() {
            String rootCertName = CERTS_SRC + "rootRsa.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertName)[0];
            // We need to set infinite freshness for the signature validation. Otherwise, test will fail.
            builder.GetProperties().SetFreshness(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR, ValidatorContext
                .CRL_VALIDATOR), CertificateSources.Of(CertificateSource.SIGNER_CERT), TimeBasedContexts.Of(TimeBasedContext
                .PRESENT), TimeSpan.FromDays(999999));
            ValidationReport report;
            // Signature container stores OCSP response with indeterminate status and less fresh but valid CRL response.
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "revDataInTheSignatureContainer.pdf"
                ))) {
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignatures();
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                (4).HasNumberOfFailures(0).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                ).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (i) => "Signature1")).HasLogItem((al) => al
                .WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.CERT_STATUS_IS_UNKNOWN).WithStatus(
                ReportItem.ReportItemStatus.INFO)).HasLogItems(2, (al) => al.WithCertificate(rootCert).WithCheckName(CertificateChainValidator
                .CERTIFICATE_CHECK).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN
                ())));
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveRevocationDataStoredInTheSignerInfoTest() {
            String rootCertName = CERTS_SRC + "rootRsa.pem";
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertName)[0];
            // We need to set infinite freshness for the embedded timestamp validation. Otherwise, test will fail.
            builder.GetProperties().SetFreshness(ValidatorContexts.Of(ValidatorContext.OCSP_VALIDATOR, ValidatorContext
                .CRL_VALIDATOR), CertificateSources.Of(CertificateSource.TIMESTAMP), TimeBasedContexts.Of(TimeBasedContext
                .PRESENT), TimeSpan.FromDays(999999)).SetFreshness(ValidatorContexts.Of(ValidatorContext.CRL_VALIDATOR
                ), CertificateSources.Of(CertificateSource.SIGNER_CERT), TimeBasedContexts.Of(TimeBasedContext.HISTORICAL
                ), TimeSpan.FromDays(2));
            ValidationReport report;
            // Signer info authenticated attributes store OCSP response with indeterminate status and valid CRL response.
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "revDataInTheSignerInfo.pdf"))
                ) {
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignatures();
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                (6).HasNumberOfFailures(0).HasLogItem((al) => al.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                ).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (i) => "Signature1")).HasLogItem((al) => al
                .WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.CERT_STATUS_IS_UNKNOWN).WithStatus(
                ReportItem.ReportItemStatus.INFO)).HasLogItems(4, (al) => al.WithCertificate(rootCert).WithCheckName(CertificateChainValidator
                .CERTIFICATE_CHECK).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN
                ())));
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureIsTimestampTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            String privateKeyName = CERTS_SRC + "rootCertKey.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(privateKeyName, PASSWORD);
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timestampSignatureDoc.pdf"))) {
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
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
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(0).HasNumberOfLogs(3).HasLogItems(2
                , (la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (l) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDocWithoutChain.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                    .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                    , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasLogItem((al) => al.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REVOCATION_DATA).WithCertificate(signingCert).WithStatus(ReportItem.ReportItemStatus.INDETERMINATE
                )).HasLogItem((al) => al.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .ISSUER_MISSING, (i) => signingCert.GetSubjectDN()).WithStatus(ReportItem.ReportItemStatus.INDETERMINATE
                ).WithCertificate(signingCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureButSetAsKnownTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDocWithoutChain.pdf"))) {
                certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
                AddRevDataClients();
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasLogItems
                (3, (al) => al.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void RootIsNotTrustedInLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                SignatureValidator signatureValidator = builder.BuildSignatureValidator(document);
                parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                    .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                    , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
                report = signatureValidator.ValidateLatestSignature(document);
            }
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ).HasNumberOfFailures(3).HasLogItem((al) => al.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK
                ).WithMessage(RevocationDataValidator.NO_REVOCATION_DATA).WithCertificate((IX509Certificate)certificateChain
                [0])).HasLogItem((al) => al.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .NO_REVOCATION_DATA).WithCertificate((IX509Certificate)certificateChain[1])).HasLogItem((al) => al.WithCheckName
                (CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator.ISSUER_MISSING, (i
                ) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateMultipleSignaturesUsingLastKnownPoETest() {
            String trustedCertsFileName = CERTS_SRC + "trustedCerts.pem";
            IX509Certificate[] trustedCerts = PemFileHelper.ReadFirstChain(trustedCertsFileName);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureSigningCertExpired.pdf"
                ))) {
                SignatureValidator signatureValidator = new ValidatorChainBuilder().WithTrustedCertificates(JavaUtil.ArraysAsList
                    (trustedCerts)).WithRevocationDataValidator(new MockRevocationDataValidator()).BuildSignatureValidator
                    (document);
                ValidationReport report = signatureValidator.ValidateSignatures();
                AssertValidationReport.AssertThat(report, (r) => r.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                    (5).HasNumberOfFailures(0).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                    ).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (p) => "timestampSig1")).HasLogItem((l) =>
                     l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME
                    , (p) => "Signature1"))
                                // Document contains unused unexpected entry.
                                .HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .UNEXPECTED_ENTRY_IN_XREF, (p) => "28").WithStatus(ReportItem.ReportItemStatus.INFO)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void StopAfterTimestampChainValidationFailureTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDocWithTimestamp.pdf"))) {
                SignatureValidator signatureValidator = new ValidatorChainBuilder().WithSignatureValidationProperties(new 
                    SignatureValidationProperties().SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All
                    (), false)).WithRevocationDataValidator(new MockRevocationDataValidator()).BuildSignatureValidator(document
                    );
                ValidationReport report = signatureValidator.ValidateSignatures();
                AssertValidationReport.AssertThat(report, (r) => r.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                    ).HasNumberOfLogs(3).HasNumberOfFailures(2).HasLogItem((l) => l.WithCheckName(SignatureValidator.SIGNATURE_VERIFICATION
                    ).WithMessage(SignatureValidator.VALIDATING_SIGNATURE_NAME, (p) => "Signature1")).HasLogItems(2, (l) =>
                     l.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithStatus(ReportItem.ReportItemStatus.INDETERMINATE
                    )));
            }
        }

        private void AddRevDataClients() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            String privateKeyName = CERTS_SRC + "rootCertKey.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(privateKeyName, PASSWORD);
            DateTime currentDate = DateTimeUtil.GetCurrentUtcTime();
            TestOcspResponseBuilder builder1 = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
            builder1.SetProducedAt(currentDate);
            builder1.SetThisUpdate(DateTimeUtil.GetCalendar(currentDate));
            builder1.SetNextUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(30)));
            TestOcspResponseBuilder builder2 = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
            builder2.SetProducedAt(currentDate);
            builder2.SetThisUpdate(DateTimeUtil.GetCalendar(currentDate));
            builder2.SetNextUpdate(DateTimeUtil.GetCalendar(currentDate.AddDays(30)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, builder1).AddBuilderForCertIssuer
                (intermediateCert, builder2);
            builder.GetRevocationDataValidator().AddOcspClient(ocspClient);
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH);
        }
    }
}
