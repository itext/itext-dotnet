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
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class DocumentRevisionsValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/DocumentRevisionsValidatorTest/";

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
        public virtual void MultipleRevisionsDocumentLevel1Test(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Between these two revisions DSS and timestamp are added, which is allowed,
                // but there is unused entry in the xref table, which is an itext signature generation artifact.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ).HasNumberOfFailures(0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.UNEXPECTED_ENTRY_IN_XREF, (i) => 27).WithStatus
                    (ReportItem.ReportItemStatus.INFO)));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[1], documentRevisions[2], document, validationReport, validationContext
                    );
                // Between these two revisions only DSS is updated, which is allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void HugeDocumentTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hugeDocument.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void ExtensionsModificationsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "extensionsModifications.pdf")
                )) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[1], documentRevisions[2], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.BASE_VERSION_DECREASED, (i) => PdfName.ESIC).WithStatus
                    (ReportItem.ReportItemStatus.INVALID)));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[2], documentRevisions[3], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.EXTENSION_LEVEL_DECREASED, (i) => PdfName.ESIC)
                    .WithStatus(ReportItem.ReportItemStatus.INVALID)));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[3], documentRevisions[4], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.DEVELOPER_EXTENSION_REMOVED, (i) => PdfName.ESIC
                    ).WithStatus(ReportItem.ReportItemStatus.INVALID)));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[4], documentRevisions[5], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.EXTENSIONS_REMOVED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void CompletelyInvalidDocumentTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "completelyInvalidDocument.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.PAGES_MODIFIED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void MakeFontDirectAndIndirectTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "makeFontDirectAndIndirect.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.FIELD_REMOVED, (i) => "Signature1").WithStatus(
                    ReportItem.ReportItemStatus.INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                    ).WithMessage(DocumentRevisionsValidator.NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[1], documentRevisions[2], document, validationReport, validationContext
                    );
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.FIELD_REMOVED, (i) => "Signature1").WithStatus(
                    ReportItem.ReportItemStatus.INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                    ).WithMessage(DocumentRevisionsValidator.NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RandomEntryAddedTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "randomEntryAdded.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RandomEntryWithoutUsageTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "randomEntryWithoutUsage.pdf")
                )) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator().SetAccessPermissions(AccessPermissions
                    .NO_CHANGES_PERMITTED).SetUnexpectedXrefChangesStatus(ReportItem.ReportItemStatus.INVALID);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.UNEXPECTED_ENTRY_IN_XREF, (i) => 16).WithStatus
                    (ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void ChangeExistingFontTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "changeExistingFont.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.PAGE_MODIFIED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void ChangeExistingFontAndAddAsDssTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "changeExistingFontAndAddAsDss.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.PAGE_MODIFIED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void FillInFieldAtLevel1Test(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fillInField.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.NO_CHANGES_PERMITTED);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Between these two revisions forms were filled in, it is not allowed at docMDP level 1.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.FIELD_REMOVED, (i) => "input").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void MultipleRevisionsDocumentLevel2Test(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument2.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Between these two revisions forms were filled in, it is allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ).HasNumberOfFailures(0).HasNumberOfLogs(0));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[1], documentRevisions[2], document, validationReport, validationContext
                    );
                // Between these two revisions existing signature field was signed, it is allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ).HasNumberOfFailures(0).HasNumberOfLogs(0));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[2], documentRevisions[3], document, validationReport, validationContext
                    );
                // Between these two revisions newly added signature field was signed, it is allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ).HasNumberOfFailures(0).HasNumberOfLogs(0));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemovePermissionsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removePermissions.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions /Perms key was removed, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.PERMISSIONS_REMOVED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemoveDSSTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeDSS.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions /DSS key was removed, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.DSS_REMOVED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemoveAcroformTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeAcroform.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions /Acroform key was removed, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.ACROFORM_REMOVED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RemoveFieldTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "removeField.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions field was removed, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void RenameFieldTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "renameField.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions field was renamed, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.FIELD_REMOVED, (i) => "input").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void AddTextFieldTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "addTextField.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions new field was added, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.UNEXPECTED_FORM_FIELD, (i) => "text").WithStatus
                    (ReportItem.ReportItemStatus.INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                    ).WithMessage(DocumentRevisionsValidator.NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void AddUnsignedSignatureFieldTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "addUnsignedSignatureField.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions new unsigned signature field was added, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.UNEXPECTED_FORM_FIELD, (i) => "signature").WithStatus
                    (ReportItem.ReportItemStatus.INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                    ).WithMessage(DocumentRevisionsValidator.NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void BrokenSignatureFieldDictionaryTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "brokenSignatureFieldDictionary.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions signature value was replaced by text, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(3).HasNumberOfLogs(3).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.SIGNATURE_MODIFIED, (i) => "Signature1").WithStatus
                    (ReportItem.ReportItemStatus.INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK
                    ).WithMessage(DocumentRevisionsValidator.FIELD_REMOVED, (i) => "Signature1").WithStatus(ReportItem.ReportItemStatus
                    .INVALID)).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator.DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator
                    .NOT_ALLOWED_ACROFORM_CHANGES).WithStatus(ReportItem.ReportItemStatus.INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void ModifyPageAnnotsTest(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "modifyPageAnnots.pdf"))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.FORM_FIELDS_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[documentRevisions.Count - 2], documentRevisions[documentRevisions
                    .Count - 1], document, validationReport, validationContext);
                // Between these two revisions circle annotation was added to the first page, it is not allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID
                    ).HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName(DocumentRevisionsValidator
                    .DOC_MDP_CHECK).WithMessage(DocumentRevisionsValidator.PAGE_ANNOTATIONS_MODIFIED).WithStatus(ReportItem.ReportItemStatus
                    .INVALID)));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        public virtual void MultipleRevisionsDocumentLevel3Test(bool continueValidationAfterFail) {
            SetUp(continueValidationAfterFail);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument3.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetAccessPermissions(AccessPermissions.ANNOTATION_MODIFICATION);
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[0], documentRevisions[1], document, validationReport, validationContext
                    );
                // Between these two revisions annotations were added and deleted, text field was filled-in.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ).HasNumberOfFailures(0).HasNumberOfLogs(0));
                validationReport = new ValidationReport();
                validator.ValidateRevision(documentRevisions[1], documentRevisions[2], document, validationReport, validationContext
                    );
                // Between these two revisions existed annotations were modified, it is allowed.
                AssertValidationReport.AssertThat(validationReport, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID
                    ).HasNumberOfFailures(0).HasNumberOfLogs(0));
            }
        }
    }
}
