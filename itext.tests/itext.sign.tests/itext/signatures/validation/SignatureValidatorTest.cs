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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignatureValidatorTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/SignatureValidatorTest/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/SignatureValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void ValidLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = report.GetCertificateLogs()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureIsTimestampTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timestampSignatureDoc.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = report.GetCertificateLogs()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void ValidLatestSignatureWithTimestampTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDocWithTimestamp.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem item1 = report.GetCertificateLogs()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item1.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item1.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item1.GetMessage());
            CertificateReportItem item2 = report.GetCertificateLogs()[1];
            NUnit.Framework.Assert.AreEqual(rootCert, item2.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item2.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item2.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureWithBrokenTimestampTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithBrokenTimestamp.pdf"))
                ) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(3, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[0]);
            ReportItem failure = report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(SignatureValidator.TIMESTAMP_VERIFICATION, failure.GetCheckName());
            NUnit.Framework.Assert.AreEqual(SignatureValidator.CANNOT_VERIFY_TIMESTAMP, failure.GetMessage());
            CertificateReportItem item1 = report.GetCertificateLogs()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item1.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item1.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item1.GetMessage());
            CertificateReportItem item2 = report.GetCertificateLogs()[1];
            NUnit.Framework.Assert.AreEqual(rootCert, item2.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item2.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item2.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentModifiedLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "modifiedDoc.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(2, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(3, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[0]);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[1], report.GetLogs()[1]);
            ReportItem item1 = report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(SignatureValidator.SIGNATURE_VERIFICATION, item1.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignatureValidator.DOCUMENT_IS_NOT_COVERED, "Signature1"
                ), item1.GetMessage());
            ReportItem item2 = report.GetFailures()[1];
            NUnit.Framework.Assert.AreEqual(SignatureValidator.SIGNATURE_VERIFICATION, item2.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignatureValidator.CANNOT_VERIFY_SIGNATURE, "Signature1"
                ), item2.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void LatestSignatureInvalidStopValidationTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "modifiedDoc.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                signatureValidator.ProceedValidationAfterFail(false);
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(2, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[0]);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[1], report.GetLogs()[1]);
            ReportItem item1 = report.GetFailures()[0];
            NUnit.Framework.Assert.AreEqual(SignatureValidator.SIGNATURE_VERIFICATION, item1.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignatureValidator.DOCUMENT_IS_NOT_COVERED, "Signature1"
                ), item1.GetMessage());
            ReportItem item2 = report.GetFailures()[1];
            NUnit.Framework.Assert.AreEqual(SignatureValidator.SIGNATURE_VERIFICATION, item2.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignatureValidator.CANNOT_VERIFY_SIGNATURE, "Signature1"
                ), item2.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDocWithoutChain.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[0]);
            CertificateReportItem item = report.GetCertificateFailures()[0];
            NUnit.Framework.Assert.AreEqual(signingCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} isn't trusted and issuer certificate isn't provided."
                , signingCert.GetSubjectDN()), item.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureButSetAsKnownTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDocWithoutChain.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                signatureValidator.SetKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = report.GetCertificateLogs()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureButTakenFromDSSTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithDss.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            CertificateReportItem item = report.GetCertificateLogs()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void CertificatesNotInLatestSignatureButTakenFromDSSOneCertIsBrokenTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithBrokenDss.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                signatureValidator.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            ReportItem reportItem = report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(SignatureValidator.CERTS_FROM_DSS, reportItem.GetCheckName());
            NUnit.Framework.Assert.IsTrue(reportItem.GetExceptionCause() is AbstractGeneralSecurityException);
            CertificateReportItem item = report.GetCertificateLogs()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} is trusted, revocation data checks are not required."
                , rootCert.GetSubjectDN()), item.GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void RootIsNotTrustedInLatestSignatureTest() {
            String chainName = CERTS_SRC + "validCertsChain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "validDoc.pdf"))) {
                SignatureValidator signatureValidator = new SignatureValidator(document);
                report = signatureValidator.ValidateLatestSignature();
            }
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[0]);
            CertificateReportItem item = report.GetCertificateFailures()[0];
            NUnit.Framework.Assert.AreEqual(rootCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual("Certificate check.", item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format("Certificate {0} isn't trusted and issuer certificate isn't provided."
                , rootCert.GetSubjectDN()), item.GetMessage());
        }
    }
}
