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
using System.IO;
using System.Linq;
using iText.Commons.Actions.Contexts;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    /// <summary>Validator, which is responsible for document revisions validation according to doc-MDP rules.</summary>
    internal class DocumentRevisionsValidator {
        internal const String DOC_MDP_CHECK = "DocMDP check.";

        internal const String ACROFORM_REMOVED = "AcroForm dictionary was removed from catalog.";

        internal const String ANNOTATIONS_MODIFIED = "Field annotations were removed, added or unexpectedly modified.";

        internal const String DEVELOPER_EXTENSION_REMOVED = "Developer extension \"{0}\" dictionary was removed or unexpectedly modified.";

        internal const String DIRECT_OBJECT = "{0} must be an indirect reference.";

        internal const String DSS_REMOVED = "DSS dictionary was removed from catalog.";

        internal const String EXTENSIONS_REMOVED = "Extensions dictionary was removed from the catalog.";

        internal const String EXTENSIONS_TYPE = "Developer extensions must be a dictionary.";

        internal const String EXTENSION_LEVEL_DECREASED = "Extension level number in developer extension \"{0}\" dictionary was decreased.";

        internal const String FIELD_REMOVED = "Form field {0} was removed or unexpectedly modified.";

        internal const String NOT_ALLOWED_ACROFORM_CHANGES = "PDF document AcroForm contains changes other than " 
            + "document timestamp (docMDP level >= 1), form fill-in and digital signatures (docMDP level >= 2), " 
            + "adding or editing annotations (docMDP level 3), which are not allowed.";

        internal const String NOT_ALLOWED_CATALOG_CHANGES = "PDF document catalog contains changes other than " + 
            "DSS dictionary and DTS addition (docMDP level >= 1), " + "form fill-in and digital signatures (docMDP level >= 2), "
             + "adding or editing annotations (docMDP level 3).";

        internal const String OBJECT_REMOVED = "Object \"{0}\", which is not allowed to be removed, was removed from the document through XREF table.";

        internal const String PAGES_MODIFIED = "Pages structure was unexpectedly modified.";

        internal const String PAGE_ANNOTATIONS_MODIFIED = "Page annotations were unexpectedly modified.";

        internal const String PAGE_MODIFIED = "Page was unexpectedly modified.";

        internal const String PERMISSIONS_REMOVED = "Permissions dictionary was removed from the catalog.";

        internal const String PERMISSIONS_TYPE = "Permissions must be a dictionary.";

        internal const String PERMISSION_REMOVED = "Permission \"{0}\" dictionary was removed or unexpectedly modified.";

        internal const String REFERENCE_REMOVED = "Signature reference dictionary was removed or unexpectedly modified.";

        internal const String SIGNATURE_MODIFIED = "Signature {0} was unexpectedly modified.";

        internal const String UNEXPECTED_ENTRY_IN_XREF = "New PDF document revision contains unexpected entry \"{0}\" in XREF table.";

        internal const String REVISIONS_RETRIEVAL_FAILED = "Wasn't possible to retrieve document revisions.";

        internal const String DOCUMENT_WITHOUT_SIGNATURES = "Document doesn't contain any signatures.";

        internal const String TOO_MANY_CERTIFICATION_SIGNATURES = "Document contains more than one certification signature.";

        internal const String SIGNATURE_REVISION_NOT_FOUND = "Not possible to identify document revision corresponding to the first signature in the document.";

        internal const String ACCESS_PERMISSIONS_ADDED = "Access permissions level specified for \"{0}\" approval signature "
             + "is higher than previous one specified. These access permissions will be ignored.";

        internal const String UNKNOWN_ACCESS_PERMISSIONS = "Access permissions level number specified for \"{0}\" signature "
             + "is undefined. Default level 2 will be used instead.";

        internal const String UNEXPECTED_FORM_FIELD = "New PDF document revision contains unexpected form field \"{0}\".";

        private IMetaInfo metaInfo = new ValidationMetaInfo();

        private DocumentRevisionsValidator.AccessPermissions accessPermissions = DocumentRevisionsValidator.AccessPermissions
            .ANNOTATION_MODIFICATION;

        private DocumentRevisionsValidator.AccessPermissions requestedAccessPermissions = DocumentRevisionsValidator.AccessPermissions
            .UNSPECIFIED;

        private readonly PdfDocument document;

        internal DocumentRevisionsValidator(PdfDocument document) {
            this.document = document;
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// that will be used during
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// creation.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        /// <returns>
        /// the same
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.V1.DocumentRevisionsValidator SetEventCountingMetaInfo(IMetaInfo
             metaInfo) {
            this.metaInfo = metaInfo;
            return this;
        }

        /// <summary>Set access permissions to be used during docMDP validation.</summary>
        /// <remarks>
        /// Set access permissions to be used during docMDP validation.
        /// If value is provided, related signature fields will be ignored during the validation.
        /// </remarks>
        /// <param name="accessPermissions">
        /// 
        /// <see cref="AccessPermissions"/>
        /// docMDP validation level
        /// </param>
        /// <returns>
        /// the same
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.V1.DocumentRevisionsValidator SetAccessPermissions(DocumentRevisionsValidator.AccessPermissions
             accessPermissions) {
            this.requestedAccessPermissions = accessPermissions;
            return this;
        }

        /// <summary>Validate all document revisions according to docMDP and fieldMDP transform methods.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains detailed validation results
        /// </returns>
        public virtual ValidationReport ValidateAllDocumentRevisions() {
            ValidationReport report = new ValidationReport();
            PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
            revisionsReader.SetEventCountingMetaInfo(metaInfo);
            IList<DocumentRevision> documentRevisions;
            try {
                documentRevisions = revisionsReader.GetAllRevisions();
            }
            catch (System.IO.IOException) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REVISIONS_RETRIEVAL_FAILED, ReportItem.ReportItemStatus
                    .INVALID));
                return report;
            }
            SignatureUtil signatureUtil = new SignatureUtil(document);
            IList<String> signatures = new List<String>(signatureUtil.GetSignatureNames());
            if (signatures.IsEmpty()) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, DOCUMENT_WITHOUT_SIGNATURES, ReportItem.ReportItemStatus
                    .INFO));
                return report;
            }
            bool signatureFound = false;
            bool certificationSignatureFound = false;
            PdfSignature currentSignature = signatureUtil.GetSignature(signatures[0]);
            for (int i = 0; i < documentRevisions.Count - 1; i++) {
                if (currentSignature != null && RevisionContainsSignature(documentRevisions[i], signatures[0])) {
                    signatureFound = true;
                    if (IsCertificationSignature(currentSignature)) {
                        if (certificationSignatureFound) {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, TOO_MANY_CERTIFICATION_SIGNATURES, ReportItem.ReportItemStatus
                                .INDETERMINATE));
                        }
                        else {
                            certificationSignatureFound = true;
                            UpdateCertificationSignatureAccessPermissions(currentSignature, report);
                        }
                    }
                    UpdateApprovalSignatureAccessPermissions(signatureUtil.GetSignatureFormFieldDictionary(signatures[0]), report
                        );
                    signatures.JRemoveAt(0);
                    if (signatures.IsEmpty()) {
                        currentSignature = null;
                    }
                    else {
                        currentSignature = signatureUtil.GetSignature(signatures[0]);
                    }
                }
                if (signatureFound) {
                    ValidateRevision(documentRevisions[i], documentRevisions[i + 1], report);
                }
            }
            if (!signatureFound) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, SIGNATURE_REVISION_NOT_FOUND, ReportItem.ReportItemStatus
                    .INVALID));
            }
            return report;
        }

        internal virtual ValidationReport ValidateRevision(DocumentRevision previousRevision, DocumentRevision currentRevision
            , ValidationReport validationReport) {
            try {
                using (Stream previousInputStream = CreateInputStreamFromRevision(document, previousRevision)) {
                    using (PdfReader previousReader = new PdfReader(previousInputStream)) {
                        using (PdfDocument documentWithoutRevision = new PdfDocument(previousReader, new DocumentProperties().SetEventCountingMetaInfo
                            (metaInfo))) {
                            using (Stream currentInputStream = CreateInputStreamFromRevision(document, currentRevision)) {
                                using (PdfReader currentReader = new PdfReader(currentInputStream)) {
                                    using (PdfDocument documentWithRevision = new PdfDocument(currentReader, new DocumentProperties().SetEventCountingMetaInfo
                                        (metaInfo))) {
                                        ICollection<PdfIndirectReference> indirectReferences = currentRevision.GetModifiedObjects();
                                        if (!CompareCatalogs(documentWithoutRevision, documentWithRevision, validationReport)) {
                                            return validationReport;
                                        }
                                        IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = CreateAllowedReferences(documentWithRevision
                                            , documentWithoutRevision);
                                        foreach (PdfIndirectReference indirectReference in indirectReferences) {
                                            if (indirectReference.IsFree()) {
                                                // In this boolean flag we check that reference which is about to be removed is the one which
                                                // changed in the new revision. For instance DSS reference was 5 0 obj and changed to be 6 0 obj.
                                                // In this case and only in this case reference with obj number 5 can be safely removed.
                                                bool referenceAllowedToBeRemoved = allowedReferences.Any((reference) => reference.GetPreviousReference() !=
                                                     null && reference.GetPreviousReference().GetObjNumber() == indirectReference.GetObjNumber() && (reference
                                                    .GetCurrentReference() == null || reference.GetCurrentReference().GetObjNumber() != indirectReference.
                                                    GetObjNumber()));
                                                // If some reference wasn't in the previous document, it is safe to remove it,
                                                // since it is not possible to introduce new reference and remove it at the same revision.
                                                bool referenceWasInPrevDocument = documentWithoutRevision.GetPdfObject(indirectReference.GetObjNumber()) !=
                                                     null;
                                                if (!IsMaxGenerationObject(indirectReference) && referenceWasInPrevDocument && !referenceAllowedToBeRemoved
                                                    ) {
                                                    validationReport.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(OBJECT_REMOVED, indirectReference
                                                        .GetObjNumber()), ReportItem.ReportItemStatus.INVALID));
                                                }
                                            }
                                            else {
                                                if (!CheckAllowedReferences(allowedReferences, indirectReference, documentWithoutRevision)) {
                                                    validationReport.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(UNEXPECTED_ENTRY_IN_XREF
                                                        , indirectReference.GetObjNumber()), ReportItem.ReportItemStatus.INVALID));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.IO.IOException) {
            }
            // error
            return validationReport;
        }

        internal virtual DocumentRevisionsValidator.AccessPermissions GetAccessPermissions() {
            return requestedAccessPermissions == DocumentRevisionsValidator.AccessPermissions.UNSPECIFIED ? accessPermissions
                 : requestedAccessPermissions;
        }

        private static Stream CreateInputStreamFromRevision(PdfDocument originalDocument, DocumentRevision revision
            ) {
            RandomAccessFileOrArray raf = originalDocument.GetReader().GetSafeFile();
            WindowRandomAccessSource source = new WindowRandomAccessSource(raf.CreateSourceView(), 0, revision.GetEofOffset
                ());
            return new RASInputStream(source);
        }

        private void UpdateApprovalSignatureAccessPermissions(PdfDictionary signatureField, ValidationReport report
            ) {
            PdfDictionary fieldLock = signatureField.GetAsDictionary(PdfName.Lock);
            if (fieldLock == null || fieldLock.GetAsNumber(PdfName.P) == null) {
                return;
            }
            PdfNumber p = fieldLock.GetAsNumber(PdfName.P);
            DocumentRevisionsValidator.AccessPermissions newAccessPermissions;
            switch (p.IntValue()) {
                case 1: {
                    newAccessPermissions = DocumentRevisionsValidator.AccessPermissions.NO_CHANGES_PERMITTED;
                    break;
                }

                case 2: {
                    newAccessPermissions = DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION;
                    break;
                }

                case 3: {
                    newAccessPermissions = DocumentRevisionsValidator.AccessPermissions.ANNOTATION_MODIFICATION;
                    break;
                }

                default: {
                    // Do nothing.
                    return;
                }
            }
            if (accessPermissions.CompareTo(newAccessPermissions) < 0) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(ACCESS_PERMISSIONS_ADDED, signatureField
                    .Get(PdfName.T)), ReportItem.ReportItemStatus.INDETERMINATE));
            }
            else {
                accessPermissions = newAccessPermissions;
            }
        }

        private void UpdateCertificationSignatureAccessPermissions(PdfSignature signature, ValidationReport report
            ) {
            PdfArray references = signature.GetPdfObject().GetAsArray(PdfName.Reference);
            foreach (PdfObject reference in references) {
                PdfDictionary referenceDict = (PdfDictionary)reference;
                PdfName transformMethod = referenceDict.GetAsName(PdfName.TransformMethod);
                if (PdfName.DocMDP.Equals(transformMethod)) {
                    PdfDictionary transformParameters = referenceDict.GetAsDictionary(PdfName.TransformParams);
                    if (transformParameters == null || transformParameters.GetAsNumber(PdfName.P) == null) {
                        accessPermissions = DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION;
                        return;
                    }
                    PdfNumber p = transformParameters.GetAsNumber(PdfName.P);
                    switch (p.IntValue()) {
                        case 1: {
                            accessPermissions = DocumentRevisionsValidator.AccessPermissions.NO_CHANGES_PERMITTED;
                            break;
                        }

                        case 2: {
                            accessPermissions = DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION;
                            break;
                        }

                        case 3: {
                            accessPermissions = DocumentRevisionsValidator.AccessPermissions.ANNOTATION_MODIFICATION;
                            break;
                        }

                        default: {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(UNKNOWN_ACCESS_PERMISSIONS, signature
                                .GetName()), ReportItem.ReportItemStatus.INDETERMINATE));
                            accessPermissions = DocumentRevisionsValidator.AccessPermissions.FORM_FIELDS_MODIFICATION;
                            break;
                        }
                    }
                    return;
                }
            }
        }

        private bool IsCertificationSignature(PdfSignature signature) {
            if (PdfName.DocTimeStamp.Equals(signature.GetType()) || PdfName.ETSI_RFC3161.Equals(signature.GetSubFilter
                ())) {
                // Timestamp is never a certification signature.
                return false;
            }
            PdfArray references = signature.GetPdfObject().GetAsArray(PdfName.Reference);
            if (references != null) {
                foreach (PdfObject reference in references) {
                    if (reference is PdfDictionary) {
                        PdfDictionary referenceDict = (PdfDictionary)reference;
                        PdfName transformMethod = referenceDict.GetAsName(PdfName.TransformMethod);
                        return PdfName.DocMDP.Equals(transformMethod);
                    }
                }
            }
            return false;
        }

        private bool RevisionContainsSignature(DocumentRevision revision, String signature) {
            try {
                using (Stream inputStream = CreateInputStreamFromRevision(document, revision)) {
                    using (PdfReader reader = new PdfReader(inputStream)) {
                        using (PdfDocument documentWithRevision = new PdfDocument(reader, new DocumentProperties().SetEventCountingMetaInfo
                            (metaInfo))) {
                            SignatureUtil signatureUtil = new SignatureUtil(documentWithRevision);
                            return signatureUtil.SignatureCoversWholeDocument(signature);
                        }
                    }
                }
            }
            catch (System.IO.IOException) {
            }
            return false;
        }

        private bool CompareCatalogs(PdfDocument documentWithoutRevision, PdfDocument documentWithRevision, ValidationReport
             report) {
            PdfDictionary previousCatalog = documentWithoutRevision.GetCatalog().GetPdfObject();
            PdfDictionary currentCatalog = documentWithRevision.GetCatalog().GetPdfObject();
            PdfDictionary previousCatalogCopy = CopyCatalogEntriesToCompare(previousCatalog);
            PdfDictionary currentCatalogCopy = CopyCatalogEntriesToCompare(currentCatalog);
            if (!ComparePdfObjects(previousCatalogCopy, currentCatalogCopy)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_CATALOG_CHANGES, ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            return CompareExtensions(previousCatalog.Get(PdfName.Extensions), currentCatalog.Get(PdfName.Extensions), 
                report) && ComparePermissions(previousCatalog.Get(PdfName.Perms), currentCatalog.Get(PdfName.Perms), report
                ) && CompareDss(previousCatalog.Get(PdfName.DSS), currentCatalog.Get(PdfName.DSS), report) && ComparePages
                (previousCatalog.GetAsDictionary(PdfName.Pages), currentCatalog.GetAsDictionary(PdfName.Pages), report
                ) && CompareAcroForms(previousCatalog.GetAsDictionary(PdfName.AcroForm), currentCatalog.GetAsDictionary
                (PdfName.AcroForm), report);
        }

        private IList<DocumentRevisionsValidator.ReferencesPair> CreateAllowedReferences(PdfDocument documentWithRevision
            , PdfDocument documentWithoutRevision) {
            // First indirect reference in the pair is an allowed reference to be present in new xref table,
            // and the second indirect reference in the pair is the same entry in the previous document.
            // If any reference is null, we expect this object to be newly generated or direct reference.
            IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = new List<DocumentRevisionsValidator.ReferencesPair
                >();
            if (documentWithRevision.GetTrailer().Get(PdfName.Info) != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(documentWithRevision.GetTrailer().Get(
                    PdfName.Info).GetIndirectReference(), GetIndirectReferenceOrNull(() => documentWithoutRevision.GetTrailer
                    ().Get(PdfName.Info).GetIndirectReference())));
            }
            if (documentWithRevision.GetCatalog().GetPdfObject() == null) {
                return allowedReferences;
            }
            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(documentWithRevision.GetCatalog().GetPdfObject
                ().GetIndirectReference(), GetIndirectReferenceOrNull(() => documentWithoutRevision.GetCatalog().GetPdfObject
                ().GetIndirectReference())));
            if (documentWithRevision.GetCatalog().GetPdfObject().Get(PdfName.Metadata) != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(documentWithRevision.GetCatalog().GetPdfObject
                    ().Get(PdfName.Metadata).GetIndirectReference(), GetIndirectReferenceOrNull(() => documentWithoutRevision
                    .GetCatalog().GetPdfObject().Get(PdfName.Metadata).GetIndirectReference())));
            }
            PdfDictionary currentDssDictionary = documentWithRevision.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
                .DSS);
            if (currentDssDictionary != null) {
                PdfDictionary previousDssDictionary = documentWithoutRevision.GetCatalog().GetPdfObject().GetAsDictionary(
                    PdfName.DSS);
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentDssDictionary.GetIndirectReference
                    (), GetIndirectReferenceOrNull(() => previousDssDictionary.GetIndirectReference())));
                allowedReferences.AddAll(CreateAllowedDssEntries(documentWithRevision, documentWithoutRevision));
            }
            PdfDictionary currentAcroForm = documentWithRevision.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm
                );
            if (currentAcroForm != null) {
                PdfDictionary previousAcroForm = documentWithoutRevision.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
                    .AcroForm);
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentAcroForm.GetIndirectReference()
                    , GetIndirectReferenceOrNull(() => previousAcroForm.GetIndirectReference())));
                allowedReferences.AddAll(CreateAllowedAcroFormEntries(documentWithRevision, documentWithoutRevision));
            }
            PdfDictionary currentPagesDictionary = documentWithRevision.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
                .Pages);
            if (currentPagesDictionary != null) {
                PdfDictionary previousPagesDictionary = documentWithoutRevision.GetCatalog().GetPdfObject().GetAsDictionary
                    (PdfName.Pages);
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentPagesDictionary.GetIndirectReference
                    (), GetIndirectReferenceOrNull(() => previousPagesDictionary.GetIndirectReference())));
                allowedReferences.AddAll(CreateAllowedPagesEntries(currentPagesDictionary, previousPagesDictionary));
            }
            return allowedReferences;
        }

        private bool CheckAllowedReferences(IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences, PdfIndirectReference
             indirectReference, PdfDocument documentWithoutRevision) {
            foreach (DocumentRevisionsValidator.ReferencesPair allowedReference in allowedReferences) {
                if (IsSameReference(allowedReference.GetCurrentReference(), indirectReference)) {
                    return documentWithoutRevision.GetPdfObject(indirectReference.GetObjNumber()) == null || allowedReferences
                        .Any((reference) => IsSameReference(reference.GetPreviousReference(), indirectReference));
                }
            }
            return false;
        }

        // Compare catalogs nested methods section:
        private bool CompareExtensions(PdfObject previousExtensions, PdfObject currentExtensions, ValidationReport
             report) {
            if (previousExtensions == null || ComparePdfObjects(previousExtensions, currentExtensions)) {
                return true;
            }
            if (currentExtensions == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, EXTENSIONS_REMOVED, ReportItem.ReportItemStatus.INVALID
                    ));
                return false;
            }
            if (!(previousExtensions is PdfDictionary) || !(currentExtensions is PdfDictionary)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, EXTENSIONS_TYPE, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfDictionary previousExtensionsDictionary = (PdfDictionary)previousExtensions;
            PdfDictionary currentExtensionsDictionary = (PdfDictionary)currentExtensions;
            foreach (KeyValuePair<PdfName, PdfObject> previousExtension in previousExtensionsDictionary.EntrySet()) {
                PdfDictionary currentExtension = currentExtensionsDictionary.GetAsDictionary(previousExtension.Key);
                if (currentExtension == null) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DEVELOPER_EXTENSION_REMOVED, previousExtension
                        .Key), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
                else {
                    PdfDictionary currentExtensionCopy = new PdfDictionary(currentExtension);
                    currentExtensionCopy.Remove(PdfName.ExtensionLevel);
                    PdfDictionary previousExtensionCopy = new PdfDictionary((PdfDictionary)previousExtension.Value);
                    previousExtensionCopy.Remove(PdfName.ExtensionLevel);
                    // Apart from extension level dictionaries are expected to be equal.
                    if (!ComparePdfObjects(previousExtensionCopy, currentExtensionCopy)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DEVELOPER_EXTENSION_REMOVED, previousExtension
                            .Key), ReportItem.ReportItemStatus.INVALID));
                        return false;
                    }
                    PdfNumber previousExtensionLevel = ((PdfDictionary)previousExtension.Value).GetAsNumber(PdfName.ExtensionLevel
                        );
                    PdfNumber currentExtensionLevel = currentExtension.GetAsNumber(PdfName.ExtensionLevel);
                    if (previousExtensionLevel != null) {
                        if (currentExtensionLevel == null || previousExtensionLevel.IntValue() > currentExtensionLevel.IntValue()) {
                            report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(EXTENSION_LEVEL_DECREASED, previousExtension
                                .Key), ReportItem.ReportItemStatus.INVALID));
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool ComparePermissions(PdfObject previousPerms, PdfObject currentPerms, ValidationReport report) {
            if (previousPerms == null || ComparePdfObjects(previousPerms, currentPerms)) {
                return true;
            }
            if (currentPerms == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PERMISSIONS_REMOVED, ReportItem.ReportItemStatus.INVALID
                    ));
                return false;
            }
            if (!(previousPerms is PdfDictionary) || !(currentPerms is PdfDictionary)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PERMISSIONS_TYPE, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfDictionary previousPermsDictionary = (PdfDictionary)previousPerms;
            PdfDictionary currentPermsDictionary = (PdfDictionary)currentPerms;
            foreach (KeyValuePair<PdfName, PdfObject> previousPermission in previousPermsDictionary.EntrySet()) {
                PdfDictionary currentPermission = currentPermsDictionary.GetAsDictionary(previousPermission.Key);
                if (currentPermission == null) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(PERMISSION_REMOVED, previousPermission
                        .Key), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
                else {
                    // Perms dictionary is the signature dictionary.
                    if (!CompareSignatureDictionaries(previousPermission.Value, currentPermission, report)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(PERMISSION_REMOVED, previousPermission
                            .Key), ReportItem.ReportItemStatus.INVALID));
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CompareDss(PdfObject previousDss, PdfObject currentDss, ValidationReport report) {
            if (previousDss == null) {
                return true;
            }
            if (currentDss == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, DSS_REMOVED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            return true;
        }

        private bool ComparePages(PdfDictionary prevPages, PdfDictionary currPages, ValidationReport report) {
            if (prevPages == null ^ currPages == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGES_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            if (prevPages == null) {
                return true;
            }
            PdfDictionary previousPagesCopy = new PdfDictionary(prevPages);
            previousPagesCopy.Remove(PdfName.Kids);
            previousPagesCopy.Remove(PdfName.Parent);
            PdfDictionary currentPagesCopy = new PdfDictionary(currPages);
            currentPagesCopy.Remove(PdfName.Kids);
            currentPagesCopy.Remove(PdfName.Parent);
            if (!ComparePdfObjects(previousPagesCopy, currentPagesCopy) || !CompareIndirectReferencesObjNums(prevPages
                .Get(PdfName.Parent), currPages.Get(PdfName.Parent), report, "Page tree node parent")) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGES_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfArray prevKids = prevPages.GetAsArray(PdfName.Kids);
            PdfArray currKids = currPages.GetAsArray(PdfName.Kids);
            if (prevKids.Size() != currKids.Size()) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGES_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            for (int i = 0; i < currKids.Size(); ++i) {
                PdfDictionary previousKid = prevKids.GetAsDictionary(i);
                PdfDictionary currentKid = currKids.GetAsDictionary(i);
                if (PdfName.Pages.Equals(previousKid.GetAsName(PdfName.Type))) {
                    // Compare page tree nodes.
                    if (!ComparePages(previousKid, currentKid, report)) {
                        return false;
                    }
                }
                else {
                    // Compare page objects (leaf node in the page tree).
                    PdfDictionary previousPageCopy = new PdfDictionary(previousKid);
                    previousPageCopy.Remove(PdfName.Annots);
                    previousPageCopy.Remove(PdfName.Parent);
                    PdfDictionary currentPageCopy = new PdfDictionary(currentKid);
                    currentPageCopy.Remove(PdfName.Annots);
                    currentPageCopy.Remove(PdfName.Parent);
                    if (!ComparePdfObjects(previousPageCopy, currentPageCopy) || !CompareIndirectReferencesObjNums(previousKid
                        .Get(PdfName.Parent), currentKid.Get(PdfName.Parent), report, "Page parent")) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGE_MODIFIED, ReportItem.ReportItemStatus.INVALID));
                        return false;
                    }
                    PdfArray prevAnnots = GetAnnotsNotAllowedToBeModified(previousKid);
                    PdfArray currAnnots = GetAnnotsNotAllowedToBeModified(currentKid);
                    if (!ComparePdfObjects(prevAnnots, currAnnots)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, PAGE_ANNOTATIONS_MODIFIED, ReportItem.ReportItemStatus.
                            INVALID));
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CompareAcroForms(PdfDictionary prevAcroForm, PdfDictionary currAcroForm, ValidationReport report
            ) {
            if (prevAcroForm == null) {
                if (currAcroForm == null) {
                    return true;
                }
                PdfArray fields = currAcroForm.GetAsArray(PdfName.Fields);
                foreach (PdfObject field in fields) {
                    PdfDictionary fieldDict = (PdfDictionary)field;
                    if (!IsAllowedSignatureField(fieldDict, report)) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_ACROFORM_CHANGES, ReportItem.ReportItemStatus
                            .INVALID));
                        return false;
                    }
                }
                return true;
            }
            if (currAcroForm == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, ACROFORM_REMOVED, ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            PdfDictionary previousAcroFormCopy = CopyAcroformDictionary(prevAcroForm);
            PdfDictionary currentAcroFormCopy = CopyAcroformDictionary(currAcroForm);
            PdfArray prevFields = prevAcroForm.GetAsArray(PdfName.Fields);
            PdfArray currFields = currAcroForm.GetAsArray(PdfName.Fields);
            if (!ComparePdfObjects(previousAcroFormCopy, currentAcroFormCopy) || (prevFields.Size() > currFields.Size(
                )) || !CompareFormFields(prevFields, currFields, report)) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, NOT_ALLOWED_ACROFORM_CHANGES, ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            return true;
        }

        private bool CompareFormFields(PdfArray prevFields, PdfArray currFields, ValidationReport report) {
            IDictionary<String, PdfDictionary> prevFieldsMap = PopulateFormFieldsMap(prevFields);
            IDictionary<String, PdfDictionary> currFieldsMap = PopulateFormFieldsMap(currFields);
            foreach (KeyValuePair<String, PdfDictionary> fieldEntry in prevFieldsMap) {
                PdfDictionary previousField = fieldEntry.Value;
                PdfDictionary currentField = currFieldsMap.Get(fieldEntry.Key);
                if (currentField == null || !CompareFields(previousField, currentField, report)) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(FIELD_REMOVED, fieldEntry.Key)
                        , ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
                currFieldsMap.JRemove(fieldEntry.Key);
            }
            foreach (KeyValuePair<String, PdfDictionary> fieldEntry in currFieldsMap) {
                if (!IsAllowedSignatureField(fieldEntry.Value, report)) {
                    return false;
                }
            }
            return CompareAnnotations(prevFields, currFields, report);
        }

        /// <summary>DocMDP level &gt;= 2 allows setting values of the fields and accordingly update the widget appearances of them.
        ///     </summary>
        /// <remarks>
        /// DocMDP level &gt;= 2 allows setting values of the fields and accordingly update the widget appearances of them. But
        /// you cannot change the form structure, so it is not allowed to add, remove or rename fields, change most of their
        /// properties.
        /// </remarks>
        /// <param name="previousField">field from the previous revision to check</param>
        /// <param name="currentField">field from the current revision to check</param>
        /// <param name="report">validation report</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the changes of the field are allowed,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        private bool CompareFields(PdfDictionary previousField, PdfDictionary currentField, ValidationReport report
            ) {
            PdfDictionary prevFormDict = CopyFieldDictionary(previousField);
            PdfDictionary currFormDict = CopyFieldDictionary(currentField);
            if (!ComparePdfObjects(prevFormDict, currFormDict) || !CompareIndirectReferencesObjNums(prevFormDict.Get(PdfName
                .Parent), currFormDict.Get(PdfName.Parent), report, "Form field parent") || !CompareIndirectReferencesObjNums
                (prevFormDict.Get(PdfName.P), currFormDict.Get(PdfName.P), report, "Page object with which field annotation is associated"
                )) {
                return false;
            }
            PdfObject prevValue = previousField.Get(PdfName.V);
            PdfObject currValue = currentField.Get(PdfName.V);
            if (prevValue == null && currValue == null && PdfName.Ch.Equals(currentField.GetAsName(PdfName.FT))) {
                // Choice field: if the items in the I entry differ from those in the V entry, the V entry shall be used.
                prevValue = previousField.Get(PdfName.I);
                currValue = currentField.Get(PdfName.I);
            }
            if (PdfName.Sig.Equals(currentField.GetAsName(PdfName.FT))) {
                if (!CompareSignatureDictionaries(prevValue, currValue, report)) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(SIGNATURE_MODIFIED, currentField
                        .GetAsString(PdfName.T).GetValue()), ReportItem.ReportItemStatus.INVALID));
                    return false;
                }
            }
            else {
                if (GetAccessPermissions() == DocumentRevisionsValidator.AccessPermissions.NO_CHANGES_PERMITTED && !ComparePdfObjects
                    (prevValue, currValue)) {
                    return false;
                }
            }
            return CompareFormFields(previousField.GetAsArray(PdfName.Kids), currentField.GetAsArray(PdfName.Kids), report
                );
        }

        private bool CompareAnnotations(PdfArray prevFields, PdfArray currFields, ValidationReport report) {
            IList<PdfDictionary> prevAnnots = PopulateAnnotations(prevFields);
            IList<PdfDictionary> currAnnots = PopulateAnnotations(currFields);
            if (prevAnnots.Count != currAnnots.Count) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, ANNOTATIONS_MODIFIED, ReportItem.ReportItemStatus.INVALID
                    ));
                return false;
            }
            for (int i = 0; i < prevAnnots.Count; i++) {
                PdfDictionary prevAnnot = new PdfDictionary(prevAnnots[i]);
                RemoveAppearanceRelatedProperties(prevAnnot);
                PdfDictionary currAnnot = new PdfDictionary(currAnnots[i]);
                RemoveAppearanceRelatedProperties(currAnnot);
                if (!ComparePdfObjects(prevAnnot, currAnnot) || !CompareIndirectReferencesObjNums(prevAnnots[i].Get(PdfName
                    .P), currAnnots[i].Get(PdfName.P), report, "Page object with which annotation is associated")) {
                    report.AddReportItem(new ReportItem(DOC_MDP_CHECK, ANNOTATIONS_MODIFIED, ReportItem.ReportItemStatus.INVALID
                        ));
                    return false;
                }
            }
            return true;
        }

        private bool CompareSignatureDictionaries(PdfObject prevSigDict, PdfObject curSigDict, ValidationReport report
            ) {
            if (prevSigDict == null) {
                return true;
            }
            if (curSigDict == null) {
                return false;
            }
            if (!(prevSigDict is PdfDictionary) || !(curSigDict is PdfDictionary)) {
                return false;
            }
            PdfDictionary currentSigDictCopy = new PdfDictionary((PdfDictionary)curSigDict);
            currentSigDictCopy.Remove(PdfName.Reference);
            PdfDictionary previousSigDictCopy = new PdfDictionary((PdfDictionary)prevSigDict);
            previousSigDictCopy.Remove(PdfName.Reference);
            // Apart from the reference, dictionaries are expected to be equal.
            if (!ComparePdfObjects(previousSigDictCopy, currentSigDictCopy)) {
                return false;
            }
            PdfArray previousReference = ((PdfDictionary)prevSigDict).GetAsArray(PdfName.Reference);
            PdfArray currentReference = ((PdfDictionary)curSigDict).GetAsArray(PdfName.Reference);
            return CompareSignatureReferenceDictionaries(previousReference, currentReference, report);
        }

        private bool CompareSignatureReferenceDictionaries(PdfArray previousReferences, PdfArray currentReferences
            , ValidationReport report) {
            if (previousReferences == null || ComparePdfObjects(previousReferences, currentReferences)) {
                return true;
            }
            if (currentReferences == null || previousReferences.Size() != currentReferences.Size()) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REFERENCE_REMOVED, ReportItem.ReportItemStatus.INVALID)
                    );
                return false;
            }
            else {
                for (int i = 0; i < previousReferences.Size(); ++i) {
                    PdfDictionary currentReferenceCopy = new PdfDictionary(currentReferences.GetAsDictionary(i));
                    currentReferenceCopy.Remove(PdfName.Data);
                    PdfDictionary previousReferenceCopy = new PdfDictionary(previousReferences.GetAsDictionary(i));
                    previousReferenceCopy.Remove(PdfName.Data);
                    // Apart from the data, dictionaries are expected to be equal. Data is an indirect reference
                    // to the object in the document upon which the object modification analysis should be performed.
                    if (!ComparePdfObjects(previousReferenceCopy, currentReferenceCopy) || !CompareIndirectReferencesObjNums(previousReferences
                        .GetAsDictionary(i).Get(PdfName.Data), currentReferences.GetAsDictionary(i).Get(PdfName.Data), report, 
                        "Data entry in the signature reference dictionary")) {
                        report.AddReportItem(new ReportItem(DOC_MDP_CHECK, REFERENCE_REMOVED, ReportItem.ReportItemStatus.INVALID)
                            );
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CompareIndirectReferencesObjNums(PdfObject prevObj, PdfObject currObj, ValidationReport report
            , String type) {
            if (prevObj == null ^ currObj == null) {
                return false;
            }
            if (prevObj == null) {
                return true;
            }
            PdfIndirectReference prevObjRef = prevObj.GetIndirectReference();
            PdfIndirectReference currObjRef = currObj.GetIndirectReference();
            if (prevObjRef == null || currObjRef == null) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(DIRECT_OBJECT, type), ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            return IsSameReference(prevObjRef, currObjRef);
        }

        /// <summary>
        /// DocMDP level &lt;=2 allows adding new fields in the following cases:
        /// docMDP level 1: allows adding only DocTimeStamp signature fields;
        /// docMDP level 2: same as level 1 and also adding and then signing signature fields,
        /// so signature dictionary shouldn't be null.
        /// </summary>
        /// <param name="field">newly added field entry</param>
        /// <param name="report">validation report</param>
        /// <returns>true if newly added field is allowed to be added, false otherwise.</returns>
        private bool IsAllowedSignatureField(PdfDictionary field, ValidationReport report) {
            PdfDictionary value = field.GetAsDictionary(PdfName.V);
            if (!PdfName.Sig.Equals(field.GetAsName(PdfName.FT)) || value == null || (GetAccessPermissions() == DocumentRevisionsValidator.AccessPermissions
                .NO_CHANGES_PERMITTED && !PdfName.DocTimeStamp.Equals(value.GetAsName(PdfName.Type)))) {
                report.AddReportItem(new ReportItem(DOC_MDP_CHECK, MessageFormatUtil.Format(UNEXPECTED_FORM_FIELD, field.GetAsString
                    (PdfName.T).GetValue()), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            return true;
        }

        private IDictionary<String, PdfDictionary> PopulateFormFieldsMap(PdfArray fieldsArray) {
            IDictionary<String, PdfDictionary> fields = new Dictionary<String, PdfDictionary>();
            if (fieldsArray != null) {
                for (int i = 0; i < fieldsArray.Size(); ++i) {
                    PdfDictionary fieldDict = (PdfDictionary)fieldsArray.Get(i);
                    if (PdfFormField.IsFormField(fieldDict)) {
                        String fieldName = fieldDict.GetAsString(PdfName.T).GetValue();
                        fields.Put(fieldName, fieldDict);
                    }
                }
            }
            return fields;
        }

        private IList<PdfDictionary> PopulateAnnotations(PdfArray fieldsArray) {
            IList<PdfDictionary> annotations = new List<PdfDictionary>();
            if (fieldsArray != null) {
                for (int i = 0; i < fieldsArray.Size(); ++i) {
                    PdfDictionary annotDict = (PdfDictionary)fieldsArray.Get(i);
                    if (PdfFormAnnotationUtil.IsPureWidget(annotDict)) {
                        annotations.Add(annotDict);
                    }
                }
            }
            return annotations;
        }

        private PdfArray GetAnnotsNotAllowedToBeModified(PdfDictionary page) {
            PdfArray annots = page.GetAsArray(PdfName.Annots);
            if (annots == null) {
                return null;
            }
            PdfArray annotsCopy = new PdfArray(annots);
            foreach (PdfObject annot in annots) {
                PdfDictionary annotDict = (PdfDictionary)annot;
                if (PdfFormAnnotationUtil.IsPureWidgetOrMergedField(annotDict)) {
                    // Ideally we should also distinguish between docMDP level 1 (DTS) or 2 allowed annotations
                    // (we check them only on the acroform level, but they could be added to the page)
                    annotsCopy.Remove(annot);
                }
            }
            return annotsCopy;
        }

        private PdfDictionary CopyCatalogEntriesToCompare(PdfDictionary catalog) {
            PdfDictionary catalogCopy = new PdfDictionary(catalog);
            catalogCopy.Remove(PdfName.Metadata);
            catalogCopy.Remove(PdfName.Extensions);
            catalogCopy.Remove(PdfName.Perms);
            catalogCopy.Remove(PdfName.DSS);
            catalogCopy.Remove(PdfName.AcroForm);
            catalogCopy.Remove(PdfName.Pages);
            return catalogCopy;
        }

        private PdfDictionary CopyAcroformDictionary(PdfDictionary acroForm) {
            PdfDictionary acroFormCopy = new PdfDictionary(acroForm);
            acroFormCopy.Remove(PdfName.Fields);
            acroFormCopy.Remove(PdfName.DR);
            acroFormCopy.Remove(PdfName.DA);
            return acroFormCopy;
        }

        private PdfDictionary CopyFieldDictionary(PdfDictionary field) {
            PdfDictionary formDict = new PdfDictionary(field);
            formDict.Remove(PdfName.V);
            // Value for the choice fields could be specified by the /I key.
            formDict.Remove(PdfName.I);
            formDict.Remove(PdfName.Parent);
            formDict.Remove(PdfName.Kids);
            // Remove also annotation related properties (e.g. in case of the merged field).
            RemoveAppearanceRelatedProperties(formDict);
            return formDict;
        }

        private void RemoveAppearanceRelatedProperties(PdfDictionary annotDict) {
            annotDict.Remove(PdfName.P);
            if (GetAccessPermissions() != DocumentRevisionsValidator.AccessPermissions.NO_CHANGES_PERMITTED) {
                annotDict.Remove(PdfName.AP);
                annotDict.Remove(PdfName.AS);
                annotDict.Remove(PdfName.M);
                annotDict.Remove(PdfName.F);
            }
        }

        // Allowed references creation nested methods section:
        private IList<DocumentRevisionsValidator.ReferencesPair> CreateAllowedDssEntries(PdfDocument documentWithRevision
            , PdfDocument documentWithoutRevision) {
            IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = new List<DocumentRevisionsValidator.ReferencesPair
                >();
            PdfDictionary currentDssDictionary = documentWithRevision.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
                .DSS);
            PdfDictionary previousDssDictionary = documentWithoutRevision.GetCatalog().GetPdfObject().GetAsDictionary(
                PdfName.DSS);
            PdfArray certs = currentDssDictionary.GetAsArray(PdfName.Certs);
            if (certs != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(certs.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.Certs).GetIndirectReference())));
                for (int i = 0; i < certs.Size(); ++i) {
                    int finalI = i;
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(certs.Get(i).GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsArray(PdfName.Certs).Get(finalI).GetIndirectReference())));
                }
            }
            PdfArray ocsps = currentDssDictionary.GetAsArray(PdfName.OCSPs);
            if (ocsps != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(ocsps.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.OCSPs).GetIndirectReference())));
                for (int i = 0; i < ocsps.Size(); ++i) {
                    int finalI = i;
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(ocsps.Get(i).GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsArray(PdfName.OCSPs).Get(finalI).GetIndirectReference())));
                }
            }
            PdfArray crls = currentDssDictionary.GetAsArray(PdfName.CRLs);
            if (crls != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(crls.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.CRLs).GetIndirectReference())));
                for (int i = 0; i < crls.Size(); ++i) {
                    int finalI = i;
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(crls.Get(i).GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsArray(PdfName.CRLs).Get(finalI).GetIndirectReference())));
                }
            }
            PdfDictionary vris = currentDssDictionary.GetAsDictionary(PdfName.VRI);
            if (vris != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vris.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousDssDictionary.Get(PdfName.VRI).GetIndirectReference())));
                foreach (KeyValuePair<PdfName, PdfObject> vri in vris.EntrySet()) {
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vri.Value.GetIndirectReference(), GetIndirectReferenceOrNull
                        (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).Get(vri.Key).GetIndirectReference())));
                    if (vri.Value is PdfDictionary) {
                        PdfDictionary vriDictionary = (PdfDictionary)vri.Value;
                        PdfArray vriCerts = vriDictionary.GetAsArray(PdfName.Cert);
                        if (vriCerts != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCerts.GetIndirectReference(), GetIndirectReferenceOrNull
                                (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri.Key).Get(PdfName.Cert).GetIndirectReference
                                ())));
                            for (int i = 0; i < vriCerts.Size(); ++i) {
                                int finalI = i;
                                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCerts.Get(i).GetIndirectReference()
                                    , GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(
                                    vri.Key).GetAsArray(PdfName.Cert).Get(finalI).GetIndirectReference())));
                            }
                        }
                        PdfArray vriOcsps = vriDictionary.GetAsArray(PdfName.OCSP);
                        if (vriOcsps != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriOcsps.GetIndirectReference(), GetIndirectReferenceOrNull
                                (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri.Key).Get(PdfName.OCSP).GetIndirectReference
                                ())));
                            for (int i = 0; i < vriOcsps.Size(); ++i) {
                                int finalI = i;
                                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriOcsps.Get(i).GetIndirectReference()
                                    , GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(
                                    vri.Key).GetAsArray(PdfName.OCSP).Get(finalI).GetIndirectReference())));
                            }
                        }
                        PdfArray vriCrls = vriDictionary.GetAsArray(PdfName.CRL);
                        if (vriCrls != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCrls.GetIndirectReference(), GetIndirectReferenceOrNull
                                (() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri.Key).Get(PdfName.CRL).GetIndirectReference
                                ())));
                            for (int i = 0; i < vriCrls.Size(); ++i) {
                                int finalI = i;
                                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriCrls.Get(i).GetIndirectReference(), 
                                    GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary(vri
                                    .Key).GetAsArray(PdfName.CRL).Get(finalI).GetIndirectReference())));
                            }
                        }
                        if (vriDictionary.Get(new PdfName("TS")) != null) {
                            allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(vriDictionary.Get(new PdfName("TS")).GetIndirectReference
                                (), GetIndirectReferenceOrNull(() => previousDssDictionary.GetAsDictionary(PdfName.VRI).GetAsDictionary
                                (vri.Key).Get(new PdfName("TS")).GetIndirectReference())));
                        }
                    }
                }
            }
            return allowedReferences;
        }

        private ICollection<DocumentRevisionsValidator.ReferencesPair> CreateAllowedPagesEntries(PdfDictionary currentPagesDictionary
            , PdfDictionary previousPagesDictionary) {
            IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = new List<DocumentRevisionsValidator.ReferencesPair
                >();
            PdfArray currentKids = currentPagesDictionary.GetAsArray(PdfName.Kids);
            if (currentKids != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentKids.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousPagesDictionary.Get(PdfName.Kids).GetIndirectReference())));
                for (int i = 0; i < currentKids.Size(); ++i) {
                    int finalI = i;
                    PdfDictionary currentPageNode = currentKids.GetAsDictionary(i);
                    PdfDictionary previousPageNode = null;
                    try {
                        previousPageNode = previousPagesDictionary.GetAsArray(PdfName.Kids).GetAsDictionary(i);
                    }
                    catch (NullReferenceException) {
                    }
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentKids.Get(i).GetIndirectReference
                        (), GetIndirectReferenceOrNull(() => previousPagesDictionary.GetAsArray(PdfName.Kids).Get(finalI).GetIndirectReference
                        ())));
                    if (currentPageNode != null) {
                        if (PdfName.Pages.Equals(currentPageNode.GetAsName(PdfName.Type))) {
                            allowedReferences.AddAll(CreateAllowedPagesEntries(currentPageNode, previousPageNode));
                        }
                        else {
                            PdfObject currentAnnots = currentPageNode.Get(PdfName.Annots);
                            if (currentAnnots != null) {
                                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentAnnots.GetIndirectReference(), 
                                    GetIndirectReferenceOrNull(() => previousPagesDictionary.GetAsArray(PdfName.Kids).GetAsDictionary(finalI
                                    ).Get(PdfName.Annots).GetIndirectReference())));
                            }
                        }
                    }
                }
            }
            // We don't need to add annotations because all the allowed ones are already added during acroform processing.
            return allowedReferences;
        }

        private ICollection<DocumentRevisionsValidator.ReferencesPair> CreateAllowedAcroFormEntries(PdfDocument documentWithRevision
            , PdfDocument documentWithoutRevision) {
            IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = new List<DocumentRevisionsValidator.ReferencesPair
                >();
            PdfAcroForm prevAcroForm = PdfFormCreator.GetAcroForm(documentWithoutRevision, false);
            PdfAcroForm currAcroForm = PdfFormCreator.GetAcroForm(documentWithRevision, false);
            IDictionary<String, PdfFormField> prevFields = prevAcroForm == null ? new Dictionary<String, PdfFormField>
                () : prevAcroForm.GetAllFormFields();
            foreach (KeyValuePair<String, PdfFormField> fieldEntry in currAcroForm.GetAllFormFields()) {
                PdfFormField previousField = prevFields.Get(fieldEntry.Key);
                PdfFormField currentField = fieldEntry.Value;
                PdfObject value = currentField.GetValue();
                if (GetAccessPermissions() != DocumentRevisionsValidator.AccessPermissions.NO_CHANGES_PERMITTED || (value 
                    is PdfDictionary && PdfName.DocTimeStamp.Equals(((PdfDictionary)value).GetAsName(PdfName.Type)))) {
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentField.GetPdfObject().GetIndirectReference
                        (), GetIndirectReferenceOrNull(() => previousField.GetPdfObject().GetIndirectReference())));
                    if (previousField == null) {
                        // For newly generated form field all references are allowed to be added.
                        AddAllNestedDictionaryEntries(allowedReferences, currentField.GetPdfObject(), null);
                    }
                    else {
                        // For already existing form field only several entries are allowed to be updated.
                        allowedReferences.AddAll(CreateAllowedExistingFormFieldEntries(currentField, previousField));
                    }
                }
            }
            PdfDictionary currentResources = currAcroForm.GetDefaultResources();
            if (currentResources != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentResources.GetIndirectReference(
                    ), GetIndirectReferenceOrNull(() => prevAcroForm.GetDefaultResources().GetIndirectReference())));
                AddAllNestedDictionaryEntries(allowedReferences, currentResources, prevAcroForm == null ? null : prevAcroForm
                    .GetDefaultResources());
            }
            return allowedReferences;
        }

        private ICollection<DocumentRevisionsValidator.ReferencesPair> CreateAllowedExistingFormFieldEntries(PdfFormField
             currentField, PdfFormField previousField) {
            IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences = new List<DocumentRevisionsValidator.ReferencesPair
                >();
            PdfObject currentValue = currentField.GetValue();
            if (currentValue != null) {
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentValue.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => previousField.GetValue().GetIndirectReference())));
            }
            IList<PdfFormAnnotation> currAnnots = currentField.GetChildFormAnnotations();
            if (!currAnnots.IsEmpty()) {
                IList<PdfFormAnnotation> prevAnnots = previousField == null ? null : previousField.GetChildFormAnnotations
                    ();
                for (int i = 0; i < currAnnots.Count; i++) {
                    int finalI = i;
                    allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currAnnots[i].GetPdfObject().GetIndirectReference
                        (), GetIndirectReferenceOrNull(() => prevAnnots[finalI].GetPdfObject().GetIndirectReference())));
                    PdfObject currentAppearance = currAnnots[i].GetPdfObject().Get(PdfName.AP);
                    if (currentAppearance != null) {
                        allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentAppearance.GetIndirectReference
                            (), GetIndirectReferenceOrNull(() => prevAnnots[finalI].GetPdfObject().Get(PdfName.AP).GetIndirectReference
                            ())));
                        if (currentAppearance is PdfDictionary) {
                            PdfObject previousAppearance;
                            try {
                                previousAppearance = prevAnnots[finalI].GetPdfObject().Get(PdfName.AP);
                            }
                            catch (NullReferenceException) {
                                previousAppearance = null;
                            }
                            AddAllNestedDictionaryEntries(allowedReferences, (PdfDictionary)currentAppearance, previousAppearance);
                        }
                    }
                    PdfObject currentAppearanceState = currAnnots[i].GetPdfObject().Get(PdfName.AS);
                    if (currentAppearanceState != null) {
                        allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentAppearanceState.GetIndirectReference
                            (), GetIndirectReferenceOrNull(() => prevAnnots[finalI].GetPdfObject().Get(PdfName.AS).GetIndirectReference
                            ())));
                    }
                    PdfObject currentTimeStamp = currAnnots[i].GetPdfObject().Get(PdfName.M);
                    if (currentTimeStamp != null) {
                        allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentTimeStamp.GetIndirectReference(
                            ), GetIndirectReferenceOrNull(() => prevAnnots[finalI].GetPdfObject().Get(PdfName.M).GetIndirectReference
                            ())));
                    }
                }
            }
            return allowedReferences;
        }

        private void AddAllNestedDictionaryEntries(IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences
            , PdfDictionary currentDictionary, PdfObject previousDictionary) {
            foreach (KeyValuePair<PdfName, PdfObject> entry in currentDictionary.EntrySet()) {
                PdfObject currValue = entry.Value;
                if (currValue.GetIndirectReference() != null && allowedReferences.Any((pair) => IsSameReference(pair.GetCurrentReference
                    (), currValue.GetIndirectReference()))) {
                    // Required to not end up in an infinite loop.
                    continue;
                }
                PdfObject prevValue = previousDictionary is PdfDictionary ? ((PdfDictionary)previousDictionary).Get(entry.
                    Key) : null;
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currValue.GetIndirectReference(), GetIndirectReferenceOrNull
                    (() => prevValue.GetIndirectReference())));
                if (currValue is PdfDictionary) {
                    AddAllNestedDictionaryEntries(allowedReferences, (PdfDictionary)currValue, prevValue);
                }
                if (currValue is PdfArray) {
                    AddAllNestedArrayEntries(allowedReferences, (PdfArray)currValue, prevValue);
                }
            }
        }

        private void AddAllNestedArrayEntries(IList<DocumentRevisionsValidator.ReferencesPair> allowedReferences, 
            PdfArray currentArray, PdfObject previousArray) {
            for (int i = 0; i < currentArray.Size(); ++i) {
                PdfObject currentArrayEntry = currentArray.Get(i);
                if (currentArrayEntry.GetIndirectReference() != null && allowedReferences.Any((pair) => IsSameReference(pair
                    .GetCurrentReference(), currentArrayEntry.GetIndirectReference()))) {
                    // Required to not end up in an infinite loop.
                    continue;
                }
                PdfObject previousArrayEntry = previousArray is PdfArray ? ((PdfArray)previousArray).Get(i) : null;
                allowedReferences.Add(new DocumentRevisionsValidator.ReferencesPair(currentArrayEntry.GetIndirectReference
                    (), GetIndirectReferenceOrNull(() => previousArrayEntry.GetIndirectReference())));
                if (currentArrayEntry is PdfDictionary) {
                    AddAllNestedDictionaryEntries(allowedReferences, currentArray.GetAsDictionary(i), previousArrayEntry);
                }
                if (currentArrayEntry is PdfArray) {
                    AddAllNestedArrayEntries(allowedReferences, currentArray.GetAsArray(i), previousArrayEntry);
                }
            }
        }

        // Compare PDF objects util section:
        private static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2) {
            return ComparePdfObjects(pdfObject1, pdfObject2, new HashSet<PdfObject>());
        }

        private static bool ComparePdfObjects(PdfObject pdfObject1, PdfObject pdfObject2, ICollection<PdfObject> visitedObjects
            ) {
            if (visitedObjects.Contains(pdfObject1)) {
                return true;
            }
            visitedObjects.Add(pdfObject1);
            if (Object.Equals(pdfObject1, pdfObject2)) {
                return true;
            }
            if (pdfObject1 == null || pdfObject2 == null) {
                return false;
            }
            if (pdfObject1.GetType() != pdfObject2.GetType()) {
                return false;
            }
            // We don't allow objects to be direct and indirect.
            // Acrobat however allows it, but such change can invalidate the document.
            if (pdfObject1.GetIndirectReference() == null ^ pdfObject2.GetIndirectReference() == null) {
                return false;
            }
            switch (pdfObject1.GetObjectType()) {
                case PdfObject.BOOLEAN:
                case PdfObject.NAME:
                case PdfObject.NULL:
                case PdfObject.LITERAL:
                case PdfObject.NUMBER:
                case PdfObject.STRING: {
                    return pdfObject1.Equals(pdfObject2);
                }

                case PdfObject.INDIRECT_REFERENCE: {
                    return ComparePdfObjects(((PdfIndirectReference)pdfObject1).GetRefersTo(), ((PdfIndirectReference)pdfObject2
                        ).GetRefersTo(), visitedObjects);
                }

                case PdfObject.ARRAY: {
                    return ComparePdfArrays((PdfArray)pdfObject1, (PdfArray)pdfObject2, visitedObjects);
                }

                case PdfObject.DICTIONARY: {
                    return ComparePdfDictionaries((PdfDictionary)pdfObject1, (PdfDictionary)pdfObject2, visitedObjects);
                }

                case PdfObject.STREAM: {
                    return ComparePdfStreams((PdfStream)pdfObject1, (PdfStream)pdfObject2, visitedObjects);
                }

                default: {
                    return false;
                }
            }
        }

        private static bool ComparePdfArrays(PdfArray array1, PdfArray array2, ICollection<PdfObject> visitedObjects
            ) {
            if (array1.Size() != array2.Size()) {
                return false;
            }
            for (int i = 0; i < array1.Size(); i++) {
                if (!ComparePdfObjects(array1.Get(i), array2.Get(i), visitedObjects)) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfDictionaries(PdfDictionary dictionary1, PdfDictionary dictionary2, ICollection
            <PdfObject> visitedObjects) {
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet1 = dictionary1.EntrySet();
            ICollection<KeyValuePair<PdfName, PdfObject>> entrySet2 = dictionary2.EntrySet();
            if (entrySet1.Count != entrySet2.Count) {
                return false;
            }
            foreach (KeyValuePair<PdfName, PdfObject> entry1 in entrySet1) {
                if (!entrySet2.Any((entry2) => entry2.Key.Equals(entry1.Key) && ComparePdfObjects(entry2.Value, entry1.Value
                    , visitedObjects))) {
                    return false;
                }
            }
            return true;
        }

        private static bool ComparePdfStreams(PdfStream stream1, PdfStream stream2, ICollection<PdfObject> visitedObjects
            ) {
            return JavaUtil.ArraysEquals(stream1.GetBytes(), stream2.GetBytes()) && ComparePdfDictionaries(stream1, stream2
                , visitedObjects);
        }

        private static bool IsSameReference(PdfIndirectReference indirectReference1, PdfIndirectReference indirectReference2
            ) {
            if (indirectReference1 == indirectReference2) {
                return true;
            }
            if (indirectReference1 == null || indirectReference2 == null) {
                return false;
            }
            return indirectReference1.GetObjNumber() == indirectReference2.GetObjNumber() && indirectReference1.GetGenNumber
                () == indirectReference2.GetGenNumber();
        }

        private static bool IsMaxGenerationObject(PdfIndirectReference indirectReference) {
            return indirectReference.GetObjNumber() == 0 && indirectReference.GetGenNumber() == 65535;
        }

        private static PdfIndirectReference GetIndirectReferenceOrNull(Func<PdfIndirectReference> referenceGetter) {
            try {
                return referenceGetter();
            }
            catch (Exception) {
                return null;
            }
        }

        internal enum AccessPermissions {
            UNSPECIFIED,
            NO_CHANGES_PERMITTED,
            FORM_FIELDS_MODIFICATION,
            ANNOTATION_MODIFICATION
        }

        private class ReferencesPair {
            private readonly PdfIndirectReference currentReference;

            private readonly PdfIndirectReference previousReference;

            internal ReferencesPair(PdfIndirectReference currentReference, PdfIndirectReference previousReference) {
                this.currentReference = currentReference;
                this.previousReference = previousReference;
            }

            public virtual PdfIndirectReference GetCurrentReference() {
                return currentReference;
            }

            public virtual PdfIndirectReference GetPreviousReference() {
                return previousReference;
            }
        }
    }
}
