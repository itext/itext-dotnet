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
using iText.Kernel.Pdf;
using iText.Signatures.Validation.V1.Report;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class DocumentRevisionsValidatorIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/DocumentRevisionsValidatorIntegrationTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void NoSignaturesDocTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noSignaturesDoc.pdf"))) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.DOCUMENT_WITHOUT_SIGNATURES).WithStatus(ReportItem.ReportItemStatus.INFO))
                    );
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.ANNOTATION_MODIFICATION, validator
                    .GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRevisionsDocumentWithoutPermissionsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocumentWithoutPermissions.pdf"
                ))) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.ANNOTATION_MODIFICATION, validator
                    .GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRevisionsDocumentWithPermissionsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocumentWithPermissions.pdf"
                ))) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION, validator
                    .GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TwoCertificationSignaturesTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "twoCertificationSignatures.pdf"
                ))) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.PERMISSION_REMOVED, (i) => PdfName.DocMDP).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .TOO_MANY_CERTIFICATION_SIGNATURES).WithStatus(ReportItem.ReportItemStatus.INDETERMINATE)));
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION, validator
                    .GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignatureNotFoundTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureNotFound.pdf"))) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.SIGNATURE_REVISION_NOT_FOUND).WithStatus(ReportItem.ReportItemStatus.INVALID
                    )));
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.ANNOTATION_MODIFICATION, validator
                    .GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DifferentFieldLockLevelsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "differentFieldLockLevels.pdf"
                ))) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.UNEXPECTED_FORM_FIELD, (i) => "Signature4").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus.INVALID)));
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.NO_CHANGES_PERMITTED, validator
                    .GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockLevelIncreaseTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockLevelIncrease.pdf"))
                ) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.ACCESS_PERMISSIONS_ADDED, (i) => "Signature3").
                    WithStatus(ReportItem.ReportItemStatus.INDETERMINATE)));
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION, validator
                    .GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CertificationSignatureAfterApprovalTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "certificationSignatureAfterApproval.pdf"
                ))) {
                DocumentRevisionsValidator validator = new DocumentRevisionsValidator(document);
                ValidationReport report = validator.ValidateAllDocumentRevisions();
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION, validator
                    .GetAccessPermissions());
            }
        }
    }
}
