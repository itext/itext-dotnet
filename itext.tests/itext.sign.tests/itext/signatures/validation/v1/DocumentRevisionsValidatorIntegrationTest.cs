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
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    [NUnit.Framework.TestFixtureSource("CreateParametersTestFixtureData")]
    public class DocumentRevisionsValidatorIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/DocumentRevisionsValidatorIntegrationTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private ValidatorChainBuilder builder;

        private readonly ValidationContext validationContext = new ValidationContext(ValidatorContext.DOCUMENT_REVISIONS_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private readonly bool continueValidationAfterFail;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = new ValidatorChainBuilder();
            builder.GetProperties().SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), continueValidationAfterFail
                );
        }

        public DocumentRevisionsValidatorIntegrationTest(Object continueValidationAfterFail) {
            this.continueValidationAfterFail = (bool)continueValidationAfterFail;
        }

        public DocumentRevisionsValidatorIntegrationTest(Object[] array)
            : this(array[0]) {
        }

        public static IEnumerable<Object[]> CreateParameters() {
            return JavaUtil.ArraysAsList(new Object[] { false }, new Object[] { true });
        }

        public static ICollection<NUnit.Framework.TestFixtureData> CreateParametersTestFixtureData() {
            return CreateParameters().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.Test]
        public virtual void NoSignaturesDocTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noSignaturesDoc.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.DOCUMENT_WITHOUT_SIGNATURES).WithStatus(ReportItem.ReportItemStatus.INFO))
                    );
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRevisionsDocumentWithoutPermissionsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocumentWithoutPermissions.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRevisionsDocumentWithPermissionsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocumentWithPermissions.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TwoCertificationSignaturesTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "twoCertificationSignatures.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                if (continueValidationAfterFail) {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.PERMISSION_REMOVED, (i) => PdfName.DocMDP).WithStatus(ReportItem.ReportItemStatus
                        .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                        .TOO_MANY_CERTIFICATION_SIGNATURES).WithStatus(ReportItem.ReportItemStatus.INDETERMINATE)));
                }
                else {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.PERMISSION_REMOVED, (i) => PdfName.DocMDP).WithStatus(ReportItem.ReportItemStatus
                        .INVALID)));
                }
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignatureNotFoundTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureNotFound.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.SIGNATURE_REVISION_NOT_FOUND).WithStatus(ReportItem.ReportItemStatus.INVALID
                    )));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DifferentFieldLockLevelsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "differentFieldLockLevels.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.UNEXPECTED_FORM_FIELD, (i) => "Signature4").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus.INVALID)));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.NO_CHANGES_PERMITTED, validator.GetAccessPermissions());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockLevelIncreaseTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockLevelIncrease.pdf"))
                ) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.ACCESS_PERMISSIONS_ADDED, (i) => "Signature3").
                    WithStatus(ReportItem.ReportItemStatus.INDETERMINATE)));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CertificationSignatureAfterApprovalTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "certificationSignatureAfterApproval.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockChildModificationAllowedTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockChildModificationAllowed.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockChildModificationNotAllowedTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockChildModificationNotAllowed.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.LOCKED_FIELD_MODIFIED, (i) => "rootField.childTextField").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockRootModificationAllowedTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockRootModificationAllowed.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockRootModificationNotAllowedTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockRootModificationNotAllowed.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.LOCKED_FIELD_MODIFIED, (i) => "childTextField").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockSequentialExcludeValuesTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockSequentialExcludeValues.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.LOCKED_FIELD_MODIFIED, (i) => "rootField.childTextField").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockSequentialIncludeValuesTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockSequentialIncludeValues.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                if (continueValidationAfterFail) {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.LOCKED_FIELD_MODIFIED, (i) => "rootField.childTextField").WithStatus(ReportItem.ReportItemStatus
                        .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                        .LOCKED_FIELD_MODIFIED, (i) => "childTextField").WithStatus(ReportItem.ReportItemStatus.INVALID)));
                }
                else {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.LOCKED_FIELD_MODIFIED, (i) => "rootField.childTextField").WithStatus(ReportItem.ReportItemStatus
                        .INVALID)));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldLockKidsRemovedAndAddedTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockKidsRemovedAndAdded.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                if (continueValidationAfterFail) {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.LOCKED_FIELD_KIDS_REMOVED, (i) => "rootField").WithStatus(ReportItem.ReportItemStatus
                        .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                        .LOCKED_FIELD_KIDS_ADDED, (i) => "rootField").WithStatus(ReportItem.ReportItemStatus.INVALID)));
                }
                else {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.LOCKED_FIELD_KIDS_REMOVED, (i) => "rootField").WithStatus(ReportItem.ReportItemStatus
                        .INVALID)));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PageAndParentIndirectReferenceModifiedTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "pageAndParentIndirectReferenceModified.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.LOCKED_FIELD_MODIFIED, (i) => "rootField.childTextField2").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void LockedSignatureFieldModifiedTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "lockedSignatureFieldModified.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.LOCKED_FIELD_MODIFIED, (i) => "Signature2").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void LockedFieldRemoveAddKidsEntryTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "lockedFieldRemoveAddKidsEntry.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                if (continueValidationAfterFail) {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.LOCKED_FIELD_KIDS_REMOVED, (i) => "rootField").WithStatus(ReportItem.ReportItemStatus
                        .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                        .LOCKED_FIELD_KIDS_ADDED, (i) => "rootField").WithStatus(ReportItem.ReportItemStatus.INVALID)));
                }
                else {
                    AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                        (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                        (DocumentRevisionsValidator.LOCKED_FIELD_KIDS_REMOVED, (i) => "rootField").WithStatus(ReportItem.ReportItemStatus
                        .INVALID)));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemovedLockedFieldTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removedLockedField.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.LOCKED_FIELD_REMOVED, (i) => "textField").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void DanglingWidgetAnnotationTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "danglingWidgetAnnotation.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                // New widget annotation not included into the acroform was added to the 1st page.
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.PAGE_ANNOTATIONS_MODIFIED).WithStatus(ReportItem.ReportItemStatus.INVALID)
                    ));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAllThePageAnnotationsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeAllAnnots.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                // All the annotations on the 2nd page were removed.
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAllTheFieldAnnotationsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeFieldAnnots.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                // All the annotations of the text field were removed. Note that Acrobat considers it invalid.
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemoveUnnamedFieldTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeUnnamedField.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                // Child field was removed, so parent field was modified. Both fields are unnamed.
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (3).HasNumberOfLogs(3).HasLogItems(2, 2, (l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                    ).WithMessage(MessageFormatUtil.Format(DocumentRevisionsValidator.FIELD_REMOVED, "")).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus.INVALID)));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }
    }
}
