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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class DocumentRevisionsValidatorIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/DocumentRevisionsValidatorIntegrationTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private ValidatorChainBuilder builder;

        private readonly ValidationContext validationContext = new ValidationContext(ValidatorContext.DOCUMENT_REVISIONS_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        public virtual void SetUp(bool continueValidationAfterFail) {
            builder = new ValidatorChainBuilder();
            builder.GetProperties().SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), continueValidationAfterFail
                );
        }

        public static IEnumerable<Object[]> CreateParameters() {
            return JavaUtil.ArraysAsList(new Object[] { false }, new Object[] { true });
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void NoSignaturesDocTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void LinearizedDocTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "linearizedDoc.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(0));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void MultipleRevisionsDocumentWithoutPermissionsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocumentWithoutPermissions.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void MultipleRevisionsDocumentWithPermissionsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocumentWithPermissions.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void EolNotIncludedIntoByteRangeTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "eolNotIncludedIntoByteRange.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TwoCertificationSignaturesTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void SignatureNotFoundTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void DifferentFieldLockLevelsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockLevelIncreaseTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void CertificationSignatureAfterApprovalTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "certificationSignatureAfterApproval.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                    ).HasNumberOfFailures(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.NOT_ALLOWED_CERTIFICATION_SIGNATURE).WithStatus(ReportItem.ReportItemStatus
                    .INDETERMINATE)));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockChildModificationAllowedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockChildModificationAllowed.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockChildModificationNotAllowedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockRootModificationAllowedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fieldLockRootModificationAllowed.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockRootModificationNotAllowedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockSequentialExcludeValuesTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockSequentialIncludeValuesTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FieldLockKidsRemovedAndAddedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void PageAndParentIndirectReferenceModifiedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void LockedSignatureFieldModifiedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void LockedFieldRemoveAddKidsEntryTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemovedLockedFieldTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removedLockedField.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.FIELD_MDP_CHECK).WithMessage
                    (DocumentRevisionsValidator.LOCKED_FIELD_REMOVED, (i) => "textField").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void DanglingWidgetAnnotationTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemoveAllThePageAnnotationsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeAllAnnots.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                // All the annotations on the 2nd page were removed.
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemoveAllTheFieldAnnotationsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeFieldAnnots.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                // All the annotations of the text field were removed. Note that Acrobat considers it invalid.
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void SimpleTaggedDocTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "simpleTaggedDoc.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                    (0));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TaggedDocAddAndRemoveAnnotationsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "taggedDocAddAndRemoveAnnotations.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                // Annotations were removed, but were also considered modified objects and therefore are added to xref table.
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                    (2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .UNEXPECTED_ENTRY_IN_XREF, (m) => "18").WithStatus(ReportItem.ReportItemStatus.INFO)).HasLogItem((l) =>
                     l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.UNEXPECTED_ENTRY_IN_XREF
                    , (m) => "50").WithStatus(ReportItem.ReportItemStatus.INFO)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TaggedDocRemoveStructTreeElementTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "taggedDocRemoveStructTreeElement.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .STRUCT_TREE_ROOT_MODIFIED).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void OutlinesNotModifiedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "outlinesNotModified.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void OutlinesModifiedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "outlinesModified.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .NOT_ALLOWED_CATALOG_CHANGES).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TaggedDocRemoveStructTreeAnnotationTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "taggedDocRemoveStructTreeAnnotation.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .STRUCT_TREE_ROOT_MODIFIED).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TaggedDocModifyAnnotationTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "taggedDocModifyAnnotation.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                    (0));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TaggedDocModifyAnnotationAndStructElementTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "taggedDocModifyAnnotationAndStructElement.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .STRUCT_TREE_ROOT_MODIFIED).WithStatus(ReportItem.ReportItemStatus.INVALID)).HasLogItem((l) => l.WithCheckName
                    (DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.STRUCT_TREE_ELEMENT_MODIFIED
                    ).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TaggedDocModifyAnnotationAndStructContentTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "taggedDocModifyAnnotationAndStructContent.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfLogs
                    (0));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void TaggedDocModifyStructElementTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "taggedDocModifyStructElement.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .STRUCT_TREE_ROOT_MODIFIED).WithStatus(ReportItem.ReportItemStatus.INVALID)).HasLogItem((l) => l.WithCheckName
                    (DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.STRUCT_TREE_ELEMENT_MODIFIED
                    ).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemoveUnnamedFieldTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
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

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FullCompressionModeLevel1Test(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fullCompressionModeLevel1.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                NUnit.Framework.Assert.AreEqual(AccessPermissions.NO_CHANGES_PERMITTED, validator.GetAccessPermissions());
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(0));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FullCompressionModeLevel2Test(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fullCompressionModeLevel2.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                NUnit.Framework.Assert.AreEqual(AccessPermissions.FORM_FIELDS_MODIFICATION, validator.GetAccessPermissions
                    ());
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(0));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FullCompressionModeLevel3Test(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fullCompressionModeLevel3.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                NUnit.Framework.Assert.AreEqual(AccessPermissions.ANNOTATION_MODIFICATION, validator.GetAccessPermissions(
                    ));
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(0));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void PdfVersionAddedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "pdfVersionAdded.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                ValidationReport report = validator.ValidateAllDocumentRevisions(validationContext, document);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID));
            }
        }
    }
}
